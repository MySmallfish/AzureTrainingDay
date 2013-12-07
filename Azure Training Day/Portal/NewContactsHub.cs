using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Contracts;
using Microsoft.AspNet.SignalR;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;

namespace Portal
{
    public class NewContactsHub : Hub
    {


        public void Notify(Contact contact)
        {
            Clients.All.Notify(contact.Name);
        }
    }
}