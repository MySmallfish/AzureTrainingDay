using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public class StorageEngineClient : ClientBase<IStorageEngine>, IStorageEngine
    {
        public StorageEngineClient(string url)
            : base(new WSHttpBinding()
            {
                Security = new WSHttpSecurity() { Mode = SecurityMode.None }
            }, new EndpointAddress(new Uri(url), new DnsEndpointIdentity("localhost")))
        {

        }

        public string CreateSharedAccessUrl(string containerName, string policyName)
        {
            return Channel.CreateSharedAccessUrl(containerName, policyName);
        }
    }
}
