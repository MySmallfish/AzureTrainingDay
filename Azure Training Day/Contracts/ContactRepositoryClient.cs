using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public class ContactRepositoryClient : ClientBase<IContactRepository>, IContactRepository
    {
        public ContactRepositoryClient(string url)
            : base(new WSHttpBinding()
            {
                Security = new WSHttpSecurity() { Mode = SecurityMode.None }
            }, new EndpointAddress(new Uri(url), new DnsEndpointIdentity("localhost")))
        {
            
        }

        public void Add(Contact contact)
        {
            Channel.Add(contact);
        }

        public Contact[] Query()
        {
            return Channel.Query();
        }

        public void UpdatePicture(string uniqueId, string url)
        {
            Channel.UpdatePicture(uniqueId, url);
        }


        public void UpdateThumbnail(string uniqueId, string url)
        {
            Channel.UpdateThumbnail(uniqueId, url);
        }
    }
}
