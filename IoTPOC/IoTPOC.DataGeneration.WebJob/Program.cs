using IoTPOC.Model;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IoTPOC.DataGeneration.WebJob
{
    public class Program
    {
        static readonly string eventHubName = ConfigurationManager.AppSettings["eventHub:Name"]; 
        static readonly string eventHubSAPName = ConfigurationManager.AppSettings["eventHub:SAPName"]; 
        static readonly string connectionString = ConfigurationManager.AppSettings["eventHub:ConnectionString"];
        static readonly int sleepInterval = Convert.ToInt32(ConfigurationManager.AppSettings["interval"]);
        static void Main(string[] args)
        {
            if (string.IsNullOrEmpty(connectionString) || string.IsNullOrWhiteSpace(eventHubName))
            {
                Console.WriteLine("EventHUb configuration not present");
            }
            else
            {
                SendRandomMessages();
            }
        }

        static void SendRandomMessages()
        {
            var eventHubClient = EventHubClient.CreateFromConnectionString(connectionString, eventHubName);
            var eventHubSAPClient = EventHubClient.CreateFromConnectionString(connectionString, eventHubSAPName ?? eventHubName);
            var run = true;
            while (run)
            {
                try
                {
                    var message = JsonConvert.SerializeObject(GenerateMessage());
                    eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(message)));
                    
                }
                catch (Exception exception)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("{0} > Exception: {1}", DateTime.Now, exception.Message);
                    Console.ResetColor();
                }

                Thread.Sleep(sleepInterval);
            }
        }

        public static GenericMessage GenerateMessage()
        {
            var genericMessage = new GenericMessage();
            Random random = new Random();
            int randomNumber = random.Next(5000,300000);
            genericMessage.MachineNumber = randomNumber;
            genericMessage.Name = String.Format("Machine {0}", randomNumber);
            genericMessage.Production = random.Next(1, 99);
            genericMessage.Text = String.Format("Machine {0} has produced {1}", randomNumber, genericMessage.Production);
            return genericMessage;
        } 
    }
}
