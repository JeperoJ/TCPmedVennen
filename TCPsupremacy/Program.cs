using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Security.Cryptography;
using System.IO;

namespace TCPsupremacy
{
       
    class Program
    {
        //private static List<TcpClient> clients = new List<TcpClient>();

        private static Dictionary<TcpClient, Thread> connections = new Dictionary<TcpClient, Thread>();
        private static Dictionary<TcpClient, string> names = new Dictionary<TcpClient, string>();
        static void Main(string[] args)
        {
            Console.WriteLine("Enter IP of server, or enter 0 for Mads or 1 for loopback");
            string serverIP = Console.ReadLine();
            if (serverIP == "0")
            {
                serverIP = "176.23.96.141";
            }
            else if (serverIP == "1")
            {
                serverIP = "127.0.0.1";
            }

            string user = "anon";
            Console.Write("Room Name: ");
            string rum = Console.ReadLine();
            Console.Write("Room Password: ");
            string pass = Console.ReadLine();
            Console.Write("Username: ");
            user = Console.ReadLine();

            //Skab forbindelse til serveren, skriv rum og pass til serveren.
            TcpClient roomConnector = new TcpClient();
            roomConnector.Connect(serverIP, 5050);
            string hash = MakeHash(rum+pass);
            Send(roomConnector, hash);
            //Send(roomConnector, rum + pass);
            roomConnector.Close();



            Console.WriteLine("Connected - Waiting for friends...");

            Thread sender = new Thread(new ThreadStart(Sender));
            sender.Start();
            Thread killer = new Thread(new ThreadStart(ThreadKiller));
            killer.Start();

            while (true)
            {
                try
                {
                    TcpClient tcp = new TcpClient();
                    tcp.Connect(serverIP, 5050);
                    Send(tcp, "!RECONNECT" + hash);
                    //Send(tcp, rum + pass);

                    while (true)
                    {
                        if (Read(tcp) == "!GO")
                        {
                            break;
                        }
                    }
                    Console.WriteLine("Friend found, establish connection");
                    tcp.Close();
                    TcpClient tcp2 = new TcpClient();
                    tcp2.Connect(serverIP, 5050 + 1);
                    string peerIP = Read(tcp2);
                    int port = Convert.ToInt32(Read(tcp2));
                    TcpClient tcp3 = new TcpClient();
                    Console.WriteLine("Attempting Holepunch {0} {1}", peerIP, port);
                    tcp3.Connect(peerIP, port + 1);
                    Console.WriteLine("Success");
                    //clients.Add(tcp3);
                    Send(tcp3, user);
                    Console.WriteLine("Success username send");
                    names.Add(tcp3, Read(tcp3));
                    Console.WriteLine("Added to the dick");
                    Thread receiver = new Thread(() => Receive(tcp3));
                    tcp3.ReceiveTimeout = 1;
                    Console.WriteLine("Receive made");
                    receiver.Start();
                    Console.WriteLine("Receive started");
                    connections.Add(tcp3, receiver);
                    Console.WriteLine("Connection added");
                    Console.WriteLine("Connected to {0}:{1} with name {2}", peerIP, port + 1, names[tcp3]);

                }
                catch { }
            }
        }

        static void Send(TcpClient client, string msg)
        {
            byte[] data = Encoding.UTF8.GetBytes(msg);
            client.GetStream().Write(data, 0, data.Length);
        }

        static void Sender()
        {
            while (true)
            {
                string msg = Console.ReadLine();
                foreach (var client in connections.Keys)
                {
                    Send(client, msg);
                }
            }
        }
        
        static string Read(TcpClient tcp)
        {
            Byte[] data = new Byte[256];
            String responseData = String.Empty;
            int bytes = tcp.GetStream().Read(data, 0, data.Length);
            return(Encoding.UTF8.GetString(data, 0, bytes));
        }

        static void Receive(TcpClient client)
        {
            while (client.Connected) {
                Byte[] data = new Byte[256];
                String responseData = String.Empty;
                int bytes = 0;
                try
                {
                    bytes = client.GetStream().Read(data, 0, data.Length);
                    Console.WriteLine("{0}: {1}", names[client], Encoding.UTF8.GetString(data, 0, bytes));
                }
                catch { }
            }
        }

        static string MakeHash(string input)
        {
            //Initialiser stream
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8); //Skift encoding hvis det behøves
            //Skriv string til streamen
            streamWriter.Write(input);
            streamWriter.Flush();
            memoryStream.Position = 0;

            //Lav hashet
            string output = Encoding.UTF8.GetString(SHA256.Create().ComputeHash(memoryStream));
            return output;
        }

        static void ThreadKiller()
        {
            while (true)
            {
                if (connections.Count != 0)
                {
                    foreach (var kvp in connections)
                    {
                        if (!kvp.Value.IsAlive)
                        {
                            kvp.Value.Join();
                            Console.WriteLine("{0} has disconnected", kvp.Key.Client.RemoteEndPoint.ToString());
                            kvp.Key.Close();
                            connections.Remove(kvp.Key);
                        }
                    }
                }
                Thread.Sleep(10);
            }
        }
    }
}
