﻿using System;
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

            /*Console.Write("Room Name: ");
            string rum = Console.ReadLine();
            Console.Write("Room Password: ");
            string pass = Console.ReadLine();
            Console.Write("Username: ");
            user = Console.ReadLine();

            //Skab forbindelse til serveren, skriv rum og pass til serveren.
            TcpClient roomConnector = new TcpClient();
            roomConnector.Connect(serverIP, 5050);
            byte[] data = MakeHash(rum+pass);
            roomConnector.GetStream().Write(data, 0, data.Length); 
            roomConnector.Close();*/

            Thread sender = new Thread(new ThreadStart(Sender));
            sender.Start();

            //while (true)
            //{
                try
                {
                    TcpClient tcp = new TcpClient();
                    tcp.Connect(serverIP, 5050);
                    Console.WriteLine("Connected - Waiting for friends...");

                    if (Read(tcp) == "!GO")
                    {
                        Console.WriteLine("Friend found, establish connection");
                        tcp.Close();
                        TcpClient tcp2 = new TcpClient();
                        tcp2.Connect(serverIP, 5050+1);
                        string peerIP = Read(tcp2);
                        int port = Convert.ToInt32(Read(tcp2));
                        TcpClient tcp3 = new TcpClient();
                        tcp3.Connect(peerIP, port + 1);
                        tcp3.ReceiveTimeout = 1;
                        clients.Add(tcp3);
                        Thread receiver = new Thread(() => Receive(tcp3));
                        receiver.Start();
                        Console.WriteLine("Connected to: {0}:{1}", peerIP, port + 1);
                    }
                }
                catch { }
            //}
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
                foreach (var client in clients)
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
                Console.WriteLine("{0}: {1}", client.Client.RemoteEndPoint.ToString(), Read(client));
            }
        }

        static byte[] MakeHash(string input)
        {
            //Initialiser stream
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8); //Skift encoding hvis det behøves
            //Skriv string til streamen
            streamWriter.Write(input);
            streamWriter.Flush();
            memoryStream.Position = 0;

            //Lav hashet
            byte[] output = SHA256.Create().ComputeHash(memoryStream);
            return output;
        }
    }
}
