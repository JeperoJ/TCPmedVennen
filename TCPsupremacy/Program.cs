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
            //while(true)
            //{
                TcpClient tcp = new TcpClient();
                tcp.Connect("176.23.96.141", 5050);

                if (Read(tcp) == "!GO") {
                    tcp.Close();
                    TcpClient tcp2 = new TcpClient();
                    tcp2.Connect("176.23.96.141", 5050);
                    string IP = Read(tcp2);
                    int port = Convert.ToInt32(Read(tcp2));
                    TcpClient tcp3 = new TcpClient();
                    tcp3.Connect(IP, port+1);
                    while (true)
                    {
                        Byte[] data2 = Encoding.UTF8.GetBytes(Console.ReadLine());
                        tcp.GetStream().Write(data2, 0, data2.Length);
                        Console.WriteLine(Read(tcp3));
                    }
                }
            //}
        }

        static string Read(TcpClient tcp)
        {
            Byte[] data = new Byte[256];
            String responseData = String.Empty;
            int bytes = tcp.GetStream().Read(data, 0, data.Length);
            return(Encoding.UTF8.GetString(data, 0, bytes));
        }
    }
}
