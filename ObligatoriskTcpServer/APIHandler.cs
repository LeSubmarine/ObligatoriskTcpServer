using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using FanLibrary;
using Newtonsoft.Json;

namespace ObligatoriskTcpServer
{
    static class APIHandler
    {
        private const string serverUrl = "https://localhost:5001/";

        public static List<FanOutput> Get()
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.UseDefaultCredentials = true;
            
            List<FanOutput> fanoutputs = new List<FanOutput>();
            using (var client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri(serverUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                try
                {
                    var responce = client.GetAsync("api/fanoutput").Result;
                    if (responce.IsSuccessStatusCode)
                    {
                        var fanoutputsString = responce.Content.ReadAsStringAsync().Result;
                        string jsonObject = "";
                        foreach (var VARIABLE in fanoutputsString)
                        {
                            if (VARIABLE == '[' || VARIABLE == '{')
                            {
                                jsonObject = "";
                            }
                            else if (VARIABLE == '}')
                            {
                                fanoutputs.Add((FanOutput)JsonConvert.DeserializeObject("{" + jsonObject + "}", typeof(FanOutput)));

                            }
                            else
                            {
                                jsonObject += VARIABLE;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            return fanoutputs;
        }

        public static FanOutput Get(int index)
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.UseDefaultCredentials = true;

            FanOutput fanoutput = null;
            using (var client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri(serverUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                try
                {
                    var responce = client.GetAsync($"api/fanoutput/{index}").Result;
                    if (responce.IsSuccessStatusCode)
                    {
                        var fanoutputsString = responce.Content.ReadAsStringAsync().Result;
                        string jsonObject = "";
                        foreach (var VARIABLE in fanoutputsString)
                        {
                            if (VARIABLE == '[' || VARIABLE == '{')
                            {
                                jsonObject = "";
                            }
                            else if (VARIABLE == '}')
                            {
                                fanoutput = ((FanOutput)JsonConvert.DeserializeObject("{" + jsonObject + "}", typeof(FanOutput)));

                            }
                            else
                            {
                                jsonObject += VARIABLE;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            return fanoutput;
        }

        public static void Post(FanOutput fanOutput)
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.UseDefaultCredentials = true;

            FanOutput fanoutput = null;
            using (var client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri(serverUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                try
                {
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(fanOutput), Encoding.UTF8, "application/json");
                    var responce = client.PostAsync($"api/fanoutput/",httpContent).Result;
                    Console.WriteLine(responce);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
    }
}
