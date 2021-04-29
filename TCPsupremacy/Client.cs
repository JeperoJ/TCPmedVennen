using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Threading;
using System.Net.Sockets;

namespace TCPsupremacy
{
    class Client
    {        
        public string name { get; set; }
        public RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
        public Thread receiver { get; set; }
        public TcpClient client = new TcpClient();
    }

        
}
