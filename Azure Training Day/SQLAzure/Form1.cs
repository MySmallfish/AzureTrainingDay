using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Contracts;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;

namespace SQLAzure
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            
            InitializeComponent();

            Subscribe();
        }

        private void Subscribe()
        {

            string connectionString =
    CloudConfigurationManager.GetSetting("ServiceBusConnectionString");

            var namespaceManager =
                NamespaceManager.CreateFromConnectionString(connectionString);
            var topic = ConfigurationManager.AppSettings["NewContactsTopic"];
            var subscription = "WinClient";
            if (!namespaceManager.SubscriptionExists(topic, subscription))
            {
                namespaceManager.CreateSubscription(topic, subscription);
            }

            var client =
                SubscriptionClient.CreateFromConnectionString
                        (connectionString, topic, subscription, ReceiveMode.ReceiveAndDelete);

            client.OnMessageAsync(Process, new OnMessageOptions(){AutoComplete = true });
        }

        private async Task Process(BrokeredMessage message)
        {
            var contact = message.GetBody<Contact>();
            Invoke(new EventHandler((sender, args) =>
            {
                NewContactAdded.Text = contact.Name + " added successfully.";
                ReloadContacts();

            }));

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var client = new ContactsManagerClient(AddressText.Text);
            client.AddContact(NameText.Text, Email.Text, (int)Age.Value);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ReloadContacts();
        }

        private void ReloadContacts()
        {
            var client = new ContactsManagerClient(AddressText.Text);
            Contacts.DataSource = client.GetContacts();
        }
    }
}
