using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Xml;

namespace YHBServer
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Server server = new Server();
            while (true)
            {
                string str = Console.ReadLine();
                if (str.Equals("q"))
                {
                    server.Close();
                    Console.WriteLine("已成功退出服务......");
                    return;
                }

                if (str.Equals("begin"))
                {
                    server.Begin();
                }
                else
                {
                    Console.WriteLine("无法识别的命令:" + str);
                }
            }
        }
    }
}