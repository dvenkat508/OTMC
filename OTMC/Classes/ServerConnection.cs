using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace OTMC.Classes
{
    class ServerConnection
    {
        public static readonly Socket ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public const int PORT = 975;        
        public static bool ConnectToServer()
        {
            int count = 0;
            while (!ClientSocket.Connected && count<=1)
            {                
                try
                {
                    count++;
                    // Change IPAddress.Loopback to a remote IP to connect to a remote host.
                    ClientSocket.Connect("192.168.0.50", PORT);
                }
                catch (SocketException)
                {
                }               
            }
            if(count>2)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
