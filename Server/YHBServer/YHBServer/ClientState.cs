using System;
using System.Net.Sockets;

namespace YHBServer
{
   class ClientState
   {
       public int id = 0;
       public Socket Socket = null;
       public byte[] ReadBuff = new byte[1024];
   }  
}
