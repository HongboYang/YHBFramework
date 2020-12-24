using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace YHBServer
{
    public class Server
    {

        private Socket m_listenfd;
        private Dictionary<Socket, ClientState> m_clients = new Dictionary<Socket, ClientState>();
        private int m_port = 8888;
        private string m_ipString= "127.0.0.1";
        private int m_clientId = 0;


        public void Begin()
        {
            m_listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipAdr = IPAddress.Parse(m_ipString);
            IPEndPoint ipEp = new IPEndPoint(ipAdr, m_port);
            m_listenfd.Bind(ipEp);
            m_listenfd.Listen(0);
            //m_listenfd.BeginAccept(AcceptCallback, m_listenfd);
            Console.WriteLine("服务器启动成功......");
            
            List<Socket> checkRead = new List<Socket>();
            while (true)
            {
                checkRead.Clear();
                checkRead.Add(m_listenfd);
                foreach (var item in m_clients.Values)
                {
                    checkRead.Add(item.Socket);
                }
                
                Socket.Select(checkRead, null, null, 1000);

                foreach (Socket item in checkRead)
                {
                    if (item == m_listenfd)
                    {
                        ReadListenfd(item);
                    }
                    else
                    {
                        ReadClitentfd(item);
                    }
                    
                }
            }
            
            
        }

        private void ReadListenfd(Socket socket)
        {
            Socket clitentfd = socket.Accept();
            ClientState state = new ClientState();
            state.id = GenerateId();
            state.Socket = clitentfd;
            m_clients.Add(clitentfd, state);
        }

        private void ReadClitentfd(Socket socket)
        {
            System.Text.StringBuilder sb = new StringBuilder();
            byte[] readBytes = new byte[1024];
            int count = socket.Receive(readBytes, 0, readBytes.Length, SocketFlags.None);
            if (count == 0)
            {
                m_clients.Remove(socket);
                socket.Close();
                sb.Append("编号为");
                sb.Append(m_clients[socket].id);
                sb.Append("的朋友已下线");
            }
            else
            {
               sb.Append(m_clients[socket].id);
               sb.Append(":");
               sb.Append(System.Text.Encoding.UTF8.GetString(readBytes, 0, count));
            }

            byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString()); 
            foreach (ClientState state in m_clients.Values)
            {
                state.Socket.Send(sendBytes, 0, sendBytes.Length, SocketFlags.None);
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                Console.WriteLine("[服务器]Accept");
                Socket listenfd =(Socket)ar.AsyncState;
                Socket clientfd = listenfd.EndAccept(ar);
                
                //clients列表
                ClientState state = new ClientState();
                state.Socket = clientfd;
                m_clients.Add(clientfd, state);

                clientfd.BeginReceive(state.ReadBuff, 0, 1024, 0, ReceiveCallback
                    , state);
                listenfd.BeginAccept(AcceptCallback, listenfd);
            }
            catch (SocketException e)
            {
                Console.WriteLine(e);
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                ClientState state = (ClientState) ar.AsyncState;
                Socket clientfd = state.Socket;
                int count = clientfd.EndReceive(ar);
    
                if (count == 0)
                {
                    clientfd.Close();
                    m_clients.Remove(clientfd);
                    Console.WriteLine("Socket Close");
                    return;
                }
    
                string recvStr = System.Text.Encoding.UTF8.GetString(state.ReadBuff, 0, count);
                byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes("echo"+recvStr);
                //clientfd.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallback, clientfd);

                foreach (var item in m_clients.Values)
                {
                    item.Socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallback, item.Socket);
                }
                
                
                clientfd.BeginReceive(state.ReadBuff, 0, 1024, 0, ReceiveCallback, state);
            }
            catch (SocketException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket clientfd = (Socket) ar.AsyncState;
                int count = clientfd.EndSend(ar);
            }
            catch (SocketException e)
            {
                Console.WriteLine(e);
            }
            

        }

        public void Close()
        {
            foreach (var item in m_clients.Values)
            {
                item.Socket.Close();
            }
            m_clients.Clear();
            
            m_listenfd.Close();
        }

        private int GenerateId()
        {
            return ++m_clientId;
        }
    }
}