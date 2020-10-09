using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ObligatoriskTcpServer
{
    class TcpServer
    {
        private static int totaltConnectionNo = 0;
        const string serverUrl = "https://localhost:5001/api/fanoutput";

        public TcpServer()
        {
            int clientNo = 0;
            TcpClient connectionSocket;
            Thread.CurrentThread.Name = "Main";
            TcpListener serverSocket = new TcpListener(4646);
            serverSocket.Start();
            TaskFactory taskFactory = new TaskFactory();
            while (true)
            {
                connectionSocket = serverSocket.AcceptTcpClient();
                Console.WriteLine("server activated");
                clientNo++;
                totaltConnectionNo++;
                taskFactory.StartNew(() => DoClient(connectionSocket, clientNo));
            }

            connectionSocket.Close();
            Console.WriteLine("connection socket closed");
            serverSocket.Stop();
            Console.WriteLine("server stopped");
        }
        private void DoClient(TcpClient connection, int clientNo)
        {
            try
            {
                Stream ns = connection.GetStream();
                StreamReader sr = new StreamReader(ns);
                StreamWriter sw = new StreamWriter(ns);
                sw.AutoFlush = true; // enable automatic flushing
                int emptyMessages = 0;
                while (true)
                {
                    string[] tcpMessage = { sr.ReadLine(), sr.ReadLine()};
                    HttpClientHandler handler = new HttpClientHandler();
                    handler.UseDefaultCredentials = true;
                    if (tcpMessage[0].ToLower() == "HentAlle")
                    {
                        using (var client = new HttpClient(handler))
                        {
                            client.BaseAddress = new Uri(serverUrl);
                            client.DefaultRequestHeaders.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            try
                            {
                                var responce = client.GetAsync("api/hotels").Result;
                                if (responce.IsSuccessStatusCode)
                                {
                                    var hotels = responce.Content.ReadAsAsync<IEnumerable<Hotel>>().Result;
                                    foreach (var hotel in hotels)
                                    {
                                        Console.WriteLine(hotel);
                                    }
                                }
                                responce = client.GetAsync("api/hotels").Result;
                                if (responce.IsSuccessStatusCode)
                                {
                                    var hotels = responce.Content.ReadAsAsync<IEnumerable<Hotel>>().Result;
                                    foreach (var hotel in hotels)
                                    {
                                        Console.WriteLine(hotel);
                                    }
                                }

                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                                throw;
                            }
                        }
                        sw.Write("");
                    }
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                Console.WriteLine($"Connection ended with clientNo{clientNo}");
                connection.Close();
                Console.WriteLine("Net stream closed");
                totaltConnectionNo--;
            }
        }

    }
}
