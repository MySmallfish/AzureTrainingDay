using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Application
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class AzureStorageEngine : IStorageEngine
    {
        public string CreateSharedAccessUrl(string containerName, string policyName)
        {
            var connectionString =
                 CloudConfigurationManager.GetSetting("StorageConnectionString");

            // create storage client
            var account = CloudStorageAccount.Parse(connectionString);
            var client = account.CreateCloudBlobClient();

            // get / create the images (full) container (where images will be stored)
            var container = client.GetContainerReference(containerName);
            container.CreateIfNotExists();

            // set the Shared Access policy permissions 
            var permissions = new BlobContainerPermissions();
            permissions.SharedAccessPolicies.Add(policyName, new SharedAccessBlobPolicy()
            {
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(15),
                Permissions = SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.Read
            });
            permissions.PublicAccess = BlobContainerPublicAccessType.Off;

            container.SetPermissions(permissions);

            var sharedAccessToken = container.GetSharedAccessSignature(new SharedAccessBlobPolicy(), policyName);

            return sharedAccessToken;
        }
    }
}
