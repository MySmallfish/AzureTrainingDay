using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    [ServiceContract]
    public interface IContactRepository
    {
        [OperationContract]
        void UpdatePicture(string uniqueId, string url);

        [OperationContract]
        void Add(Contact contact);

        [OperationContract]
        Contact[] Query();

        [OperationContract]
        void UpdateThumbnail(string uniqueId, string url);
    }
}
