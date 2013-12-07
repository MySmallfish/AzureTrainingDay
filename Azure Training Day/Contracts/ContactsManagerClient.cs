using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public class ContactsManagerClient : ClientBase<IContactsManager>, IContactsManager
    {
        public ContactsManagerClient(string url)
            :base(new WSHttpBinding()
            {
                Security = new WSHttpSecurity() { Mode = SecurityMode.None }
            },new EndpointAddress(new Uri(url), new DnsEndpointIdentity("localhost")))
        {
            
        }
        public void AddContact(string uniqueId, string name, string email, int age)
        {
            Channel.AddContact(uniqueId, name, email, age);
        }


        public Contact[] GetContacts()
        {
            return Channel.GetContacts();
        }


        public StorageInfo GetImagesUploadUrl()
        {
            return Channel.GetImagesUploadUrl();
        }

        public void AssignPicture(string contactUniqueId, string url)
        {
            Channel.AssignPicture(contactUniqueId, url);
        }


        public void AssignThumbnail(string uniqueId, string uri)
        {
            Channel.AssignThumbnail(uniqueId, uri);
        }
    }
}
