using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Contracts;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace SQLAzure
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            
            InitializeComponent();

            Subscribe();
        }

        public const string DebugTopic = "new-contacts-debug";
        public const string Topic = "new-contacts";

        private string m_topic = DebugTopic;
        private void Subscribe()
        {

            var connectionString =
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

        private async Task UploadImage(string path, string url, string sasToken)
        {
            var blobUri = new Uri(url);

            // Create credentials with the SAS token. The SAS token was created in previous example.
            var credentials = new StorageCredentials(sasToken);

            // Create a new blob.
            var blob = new CloudBlockBlob(blobUri, credentials);

            // Upload the blob. 
            // If the blob does not yet exist, it will be created. 
            // If the blob does exist, its existing content will be overwritten.
            using (var fileStream = System.IO.File.OpenRead(path))
            {
                await Task.Run(() =>
                {
                    blob.UploadFromStream(fileStream);
                });
            }        
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var uniqueId = Guid.NewGuid().ToString();
            var contactsClient = new ContactsManagerClient(AddressText.Text);
            contactsClient.AddContact(uniqueId, NameText.Text, Email.Text, (int)Age.Value);

            var path = openFileDialog1.FileName;
            var fileName = Path.GetFileNameWithoutExtension(path);
            fileName = fileName + "-" + uniqueId + Path.GetExtension(path);

            if (!string.IsNullOrEmpty(path))
            {
                var info = contactsClient.GetImagesUploadUrl();
                var url = info.Url + "/" + fileName;
                UploadImage(path, url, info.SharedAccessToken)
                    .ContinueWith(t => contactsClient.AssignPicture(uniqueId, url));
            }
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

        private void button3_Click(object sender, EventArgs e)
        {
            AddressText.Text = "http://127.0.0.1:81/ContactsManager.svc";
            m_topic = DebugTopic;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            AddressText.Text = "http://e4d-azure-day.cloudapp.net/ContactsManager.svc";
            m_topic = Topic;
        }
        
        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            var path = openFileDialog1.FileName;
            pictureBox1.Image = Image.FromFile(path);

        }

        private void button5_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }
    }
}
