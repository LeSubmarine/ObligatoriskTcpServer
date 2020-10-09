using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FanLibrary;
using Newtonsoft.Json;

namespace ObligatoriskTcpServer
{
    class TcpServer
    {
        private static int totaltConnectionNo = 0;

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
                    Console.WriteLine(tcpMessage[0]);
                    Console.WriteLine(tcpMessage[1]);
                    if (tcpMessage[0].ToLower() == "hent alle" || tcpMessage[0].ToLower() == "hentalle" || (tcpMessage[0].ToLower() == "get" && tcpMessage[1].ToLower() == ""))
                    {
                        sw.WriteLine(JsonConvert.SerializeObject(APIHandler.Get()));
                        sw.WriteLine("Done");
                    }
                    else if (tcpMessage[0].ToLower() == "hent" || tcpMessage[0].ToLower() == "get")
                    {
                        try
                        {
                            Convert.ToInt32(tcpMessage[1]);
                            sw.WriteLine(JsonConvert.SerializeObject(APIHandler.Get(Convert.ToInt32(tcpMessage[1]))));
                        }
                        catch (FormatException e)
                        {
                            Console.WriteLine(e);
                            sw.WriteLine("Wrong formating on that message pal");
                            sw.WriteLine("Not done");
                        }
                    }
                    else if (tcpMessage[0].ToLower() == "gem" || tcpMessage[0].ToLower() == "post")
                    {
                        try
                        {
                            Console.WriteLine(tcpMessage[1]);
                            FanOutput fanOutput = (FanOutput)JsonConvert.DeserializeObject(tcpMessage[1],typeof(FanOutput));
                            APIHandler.Post(fanOutput);
                            sw.WriteLine("Object posted");
                            sw.WriteLine("Done");
                        }
                        catch (FormatException e)
                        {
                            Console.WriteLine(e);
                            //Kan det passe at der ikke skal være en respons her?
                            sw.WriteLine("Wrong formating on that message pal");
                            sw.WriteLine("Not done");
                        }
                    }
                    else
                    {
                        sw.WriteLine("Wrong formating on that message pal");
                        sw.WriteLine("Not done");
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
