using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Contracts;

namespace Web.Controllers
{
    public class ContactsController : ApiController
    {
        public Contact[] Get()
        {
            var client = new ContactsManagerClient(ConfigurationManager.AppSettings["ContactsRepositoryUrl"]);
            return client.GetContacts();
        }
    }
}
