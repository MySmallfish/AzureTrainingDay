using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    [DataContract]
    public class ContactImageInfo
    {
        [DataMember]
        public string UniqueId { get; set; }
        [DataMember]
        public string ImageUrl { get; set; }
    }
}
