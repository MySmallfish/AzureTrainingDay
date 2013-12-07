using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using Contracts;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;

namespace ImagesProcessor
{
    public class WorkerRole : RoleEntryPoint
    {

        // QueueClient is thread-safe. Recommended that you cache 
        // rather than recreating it on every request
        QueueClient Client;
        ManualResetEvent CompletedEvent = new ManualResetEvent(false);

        
        public override void Run()
        {
            Trace.WriteLine("Starting processing of messages");

            // Initiates the message pump and callback is invoked for each message that is received, calling close on the client will stop the pump.
            Client.OnMessage((receivedMessage) =>
                {
                    try
                    {
                        var contactImage = receivedMessage.GetBody<ContactImageInfo>();
                        var localPath = DownloadImage(contactImage.ImageUrl);
                        var uri = UploadToCloudinary(localPath);
                        
                        var client = GetContactsManagerClient();
                        client.AssignThumbnail(contactImage.UniqueId, uri);

                        // Process the message
                        Trace.WriteLine("Processing Service Bus message: " + receivedMessage.SequenceNumber.ToString());
                    }
                    catch (Exception anyException)
                    {
                        var x = anyException;
                        // Handle any message processing specific exceptions here
                    }
                });

            CompletedEvent.WaitOne();
        }

        private IContactsManager GetContactsManagerClient()
        {
            var client = new ContactsManagerClient(CloudConfigurationManager.GetSetting("ContactsManagerUrl"));
            return client;
        }


        private string DownloadImage(string imageUrl)
        {
            var connectionString =
                 CloudConfigurationManager.GetSetting("StorageConnectionString");
            
            var fileName = (new Uri(imageUrl)).Segments.Last();

            // create storage client
            var account = CloudStorageAccount.Parse(connectionString);

            var client = account.CreateCloudBlobClient();
            var container = client.GetContainerReference("images");
            var blob = container.GetBlockBlobReference(fileName);

            var resource = RoleEnvironment.GetLocalResource("Images");
            var filePath = Path.Combine(resource.RootPath, fileName);
            using (var file = File.OpenWrite(filePath))
            {
                blob.DownloadToStream(file);    
            }

            return filePath;
        }


        private string UploadToCloudinary(string localPath)
        {
            var account =
                new CloudinaryDotNet.Account("hkdflrml9", "146185261819812", "yre-vZNVENvgU4zZnloGCnWjL8c");
            var cloudinary = new CloudinaryDotNet.Cloudinary(account);
            CloudinaryDotNet.Actions.ImageUploadParams uploadParams = new CloudinaryDotNet.Actions.ImageUploadParams()
            {
                File = new CloudinaryDotNet.Actions.FileDescription(localPath),
                PublicId = Path.GetFileNameWithoutExtension(localPath)
            };

            var uploadResult = cloudinary.Upload(uploadParams);

            var url = cloudinary.Api.UrlImgUp.Transform(new CloudinaryDotNet.Transformation().Width(100).Height(150).Crop("fill")).BuildUrl(Path.GetFileName(localPath));
            
            return url;
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // Create the queue if it does not exist already
            var connectionString = CloudConfigurationManager.GetSetting("ServiceBusConnectionString");
            var queueName = CloudConfigurationManager.GetSetting("ImagesQueueName");
            // Initialize the connection to Service Bus Queue
            Client = QueueClient.CreateFromConnectionString(connectionString, queueName);
            return base.OnStart();
        }

        public override void OnStop()
        {
            // Close the connection to Service Bus Queue
            Client.Close();
            CompletedEvent.Set();
            base.OnStop();
        }
    }
}
