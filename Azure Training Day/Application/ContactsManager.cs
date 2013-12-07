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

        public void AddContact(string name, string email, int age)
        {
            var repository = GetRepository();
            var contact = new Contact()
            {
                Name = name,
                Email = email,
                Age = age
            };
            repository.Add(contact);

            NotifyContactAdded(contact);
        }

        private void NotifyContactAdded(Contact contact)
        {
            var connectionString =
                CloudConfigurationManager.GetSetting("ServiceBusConnectionString");

            var client =
                    TopicClient.CreateFromConnectionString(connectionString, CloudConfigurationManager.GetSetting("NewContactsTopic"));

            client.Send(new BrokeredMessage(contact));
        }

        public Contact[] GetContacts()
        {
            var repository = GetRepository();
            return repository.Query();
        }
    }
}
