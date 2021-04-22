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
        static string  MakeHash(string input)
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

        private static List<TcpClient> clients = new List<TcpClient>();
        private static string user;
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
            using (SHA256 sHA256 = SHA256.Create())
                Console.Write("Room Name: ");
            //Lav string om til datastream
            string rum = MakeHash(Console.ReadLine());
            Console.Write("Room Password: ");
            string pass = MakeHash(Console.ReadLine());
            Console.Write("Username: ");
            user = Console.ReadLine();

            //Skab forbindelse til serveren, skriv rum og pass til serveren.
            TcpClient roomConnector = new TcpClient();
            roomConnector.Connect(serverIP, 5050);
            NetworkStream stream = roomConnector.GetStream();
            byte[] data = Encoding.UTF8.GetBytes(rum);
            Console.WriteLine(rum + ": " + pass);
            stream.Write(data, 0, data.Length);
            data = Encoding.UTF8.GetBytes(pass);
            stream.Write(data, 0, data.Length);  
            roomConnector.Close();
            stream.Close();

            while (true)
            {
                TcpClient tcp = new TcpClient();
                tcp.Connect(serverIP, 5050);
                Console.WriteLine("Connected - Waiting for friends...");

                if (Read(tcp) == "!GO") {
                    Console.WriteLine("Friend found, establish connection");
                    tcp.Close();
                    TcpClient tcp2 = new TcpClient();
                    tcp2.Connect(serverIP, 5050);
                    string peerIP = Read(tcp2);
                    int port = Convert.ToInt32(Read(tcp2));
                    Console.WriteLine("Connected to: {0}:{1]", peerIP, port);
                    TcpClient tcp3 = new TcpClient();
                    tcp3.Connect(peerIP, port+1);
                    clients.Add(tcp3);

                    /*while (true)
                    {
                        Byte[] data2 = Encoding.UTF8.GetBytes(Console.ReadLine());
                        tcp3.GetStream().Write(data2, 0, data2.Length);
                        Console.WriteLine(Read(tcp3));
                    }*/
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

        void Receive()
        {

        }
    }
}
