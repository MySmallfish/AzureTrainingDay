using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    [ServiceContract]
    public interface IContactsManager
    {
        [OperationContract(IsOneWay = true)]
        void AddContact(string uniqueId, string name, string email, int age);

        [OperationContract]
        StorageInfo GetImagesUploadUrl();

        [OperationContract]
        void AssignPicture(string contactUniqueId, string url);

        [OperationContract]
        Contact[] GetContacts();

        [OperationContract]
        void AssignThumbnail(string uniqueId, string uri);
    }
}
