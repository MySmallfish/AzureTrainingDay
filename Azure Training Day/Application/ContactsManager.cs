using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace Application
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class ContactsManager : IContactsManager
    {
        private IContactRepository GetRepository()
        {
            var client = new ContactRepositoryClient(RoleEnvironment.IsAvailable ? RoleEnvironment.GetConfigurationSettingValue("RepositoryUrl") : ConfigurationManager.AppSettings["RepositoryUrl"]);
            return client;
        }

        private IStorageEngine CreateStorageEngineClient()
        {
            var client = new StorageEngineClient(RoleEnvironment.IsAvailable ? RoleEnvironment.GetConfigurationSettingValue("StorageEngineUrl") : ConfigurationManager.AppSettings["StorageEngineUrl"]);
            return client;
        }

        public StorageInfo GetImagesUploadUrl()
        {
            var client = CreateStorageEngineClient();
            var url = "https://e4dazureday.blob.core.windows.net/images";
            return new StorageInfo()
            {
                Url = url,
                SharedAccessToken = client.CreateSharedAccessUrl("images", "upload")
            };
        }

        public void AddContact(string uniqueId, string name, string email, int age)
        {
            var repository = GetRepository();
            var contact = new Contact()
            {
                Name = name,
                Email = email,
                Age = age,
                UniqueId = uniqueId
            };
            repository.Add(contact);

            NotifyContactAdded(contact);
        }

        private void NotifyContactAdded(Contact contact)
        {
            Notify("NewContactsTopic", contact);
        }

        private void Notify(string topic, object info)
        {
            var connectionString = GetServiceBusConnectionString();

            var client =
                TopicClient.CreateFromConnectionString(connectionString,
                    CloudConfigurationManager.GetSetting(topic));

            client.Send(new BrokeredMessage(info));
        }

        private static string GetServiceBusConnectionString()
        {
            var connectionString =
                CloudConfigurationManager.GetSetting("ServiceBusConnectionString");
            return connectionString;
        }

        public Contact[] GetContacts()
        {
            var repository = GetRepository();
            return repository.Query();
        }


        public void AssignPicture(string contactUniqueId, string url)
        {
            var repository = GetRepository();
            repository.UpdatePicture(contactUniqueId, url);

            var connectionString = GetServiceBusConnectionString();

            var client = QueueClient.CreateFromConnectionString(connectionString, CloudConfigurationManager.GetSetting("ImagesQueueName"));
            var message = new BrokeredMessage(new ContactImageInfo {ImageUrl = url, UniqueId = contactUniqueId});
            client.Send(message);
        }


        public void AssignThumbnail(string uniqueId, string uri)
        {
            var repository = GetRepository();
            repository.UpdateThumbnail(uniqueId, uri);
        }
    }
}
