using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace YHBServer
{
    public class Server
    {

        private Socket m_listenfd;
        private Dictionary<Socket, ClientState> m_clients = new Dictionary<Socket, ClientState>();
        private int m_port = 8888;
        private string m_ipString= "127.0.0.1";


        public void Begin()
        {
            m_listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipAdr = IPAddress.Parse(m_ipString);
            IPEndPoint ipEp = new IPEndPoint(ipAdr, m_port);
            m_listenfd.Bind(ipEp);
            m_listenfd.Listen(0);
            m_listenfd.BeginAccept(AcceptCallback, m_listenfd);
            Console.WriteLine("服务器启动成功......");
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
    }
}