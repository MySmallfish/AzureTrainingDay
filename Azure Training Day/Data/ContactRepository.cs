using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Contracts;

namespace Data
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class ContactRepository : IContactRepository
    {
        public void Add(Contracts.Contact contact)
        {
            using (var dataContext = new ContactsDataContext())
            {
                dataContext.Contacts.InsertOnSubmit(new Contact()
                {
                    Name = contact.Name,
                    Email = contact.Email,
                    Age = contact.Age
                });

                dataContext.SubmitChanges();
            }
        }

        public Contracts.Contact[] Query()
        {
            var result = new List<Contracts.Contact>();

            using (var dataContext = new ContactsDataContext())
            {
                result.AddRange(dataContext.Contacts.Select(c=>new Contracts.Contact()
                {
                    Id = c.Id,
                    Name = c.Name,
                    Age = c.Age.GetValueOrDefault(),
                    Email = c.Email
                }));
            }

            return result.ToArray();
        }
    }
}
