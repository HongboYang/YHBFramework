using System;
using System.Net.Sockets;

namespace YHBServer
{
   class ClientState
   {
       public Socket Socket;
       public byte[] ReadBuff = new byte[1024];
   }  
}
