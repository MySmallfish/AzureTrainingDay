using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    [DataContract]
    public class StorageInfo
    {
        [DataMember]
        public string Url { get; set; }

        [DataMember]
        public string SharedAccessToken { get; set; }
    }
}
