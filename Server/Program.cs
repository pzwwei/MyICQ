using System;
using System.ServiceModel;
using MyLibrary;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new ServiceHost(typeof(Server));
            var address = new Uri("net.tcp://xx.xx.xx.xx:8000/Server");
            var binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.None;  //important
            var contract = typeof(IServer);             
            host.AddServiceEndpoint(contract, binding, address);
            
            using (host)
            {
                host.Open();
                Console.WriteLine("Server started");

                LoadDatabase();
                
                Console.ReadLine();
                Console.WriteLine("Quitting...");
                host.Close();
            }
        }

        private static void LoadDatabase()
        {
            Server.database = new Database();   //probably not here
            Server.database.Load();
            Console.WriteLine("Database loaded");
        }
    }
}
