using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Contracts;
using Microsoft.AspNet.SignalR;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;

namespace Portal
{
    public class NewContactsListener
    {
        public void Notify(Contact contact)
        {
            var clients = GlobalHost.ConnectionManager.GetHubContext<NewContactsHub>().Clients;
            clients.All.Notify(contact.Name);
        }

        public void Subscribe()
        {

            var connectionString =
                CloudConfigurationManager.GetSetting("ServiceBusConnectionString");

            var namespaceManager =
                NamespaceManager.CreateFromConnectionString(connectionString);
            var topic = ConfigurationManager.AppSettings["NewContactsTopic"];
            var subscription = "Portal";
            if (!namespaceManager.SubscriptionExists(topic, subscription))
            {
                namespaceManager.CreateSubscription(topic, subscription);
            }

            var client =
                SubscriptionClient.CreateFromConnectionString
                        (connectionString, topic, subscription, ReceiveMode.ReceiveAndDelete);

            client.OnMessageAsync(Process, new OnMessageOptions() { AutoComplete = true });
        }

        private async Task Process(BrokeredMessage message)
        {
            var contact = message.GetBody<Contact>();
            Notify(contact);

        }

    }
}