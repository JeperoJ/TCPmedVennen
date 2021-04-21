using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace TCPsupremacy
{
    class Program
    {
        static void Main(string[] args)
        {
            while(true)
            {
                TcpClient tcp = new TcpClient();
                tcp.Connect("176.23.96.141", 5050);
                Console.WriteLine("Connected");
                tcp.Close();
                Console.WriteLine("Dis");
                Thread.Sleep(500);
            }
        }
    }
}
