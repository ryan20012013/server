// See https://aka.ms/new-console-template for more information
using System;
using System.IO;
using System.Windows;
using System.Diagnostics;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace HttpListenerExample
{
    
    class HttpServer
    {
        public static HttpListener listener;
        public static string url = "http://localhost:7878/";
        public static string httpProtocol = "http://";
        public static string portNumber = ":7878";
        public static int pageViews = 0;
        public static int requestCount = 0;
        public static bool runServer = true;

        private static string resourceDir = Environment.CurrentDirectory + "/root/web";
        public static string PAGEDATA = System.IO.File.ReadAllText(resourceDir + "/default.html");


        public static async Task HandleIncomingConnections()
        {

            // While a user hasn't visited the `shutdown` url, keep on handling requests
            while (runServer)
            {
                // Will wait here until we hear from a connection
                HttpListenerContext ctx = await listener.GetContextAsync();

                // Peel out the requests and response objects
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;

                // Print out some info about the request
                Console.WriteLine("Request #: {0}", ++requestCount);
                Console.WriteLine(req.Url.ToString());
                Console.WriteLine(req.HttpMethod);
                Console.WriteLine(req.UserHostName);
                Console.WriteLine(req.UserAgent);
                Console.WriteLine();

                // String pageData = String.Format(PAGEDATA, pageViews);
                String pageData = "";
                String absolutePath = resourceDir + req.Url.AbsolutePath;
                byte[] data;
                switch (req.Url.AbsolutePath) {
                    case "/api/shutdown": {
                        data = new ShutdownServlet().handleRequest(req, resp);
                        break;
                    }
                    case "/api/browse": {
                        data = new BrowseServlet().handleRequest(req, resp);
                        break;
                    }
                    case "/api/audio": {
                        data = new AudioServlet().handleRequest(req, resp);
                        break;
                    }
                    default:
                        if (req.Url.AbsolutePath == "/" ) {
                            pageData = PAGEDATA;
                        }
                        else if (System.IO.File.Exists(absolutePath)) {
                            Console.WriteLine("pageData from " + absolutePath);
                            pageData = System.IO.File.ReadAllText(absolutePath);
                        } else {
                            pageData = System.IO.File.ReadAllText(resourceDir + "/404.html");
                        }
                        data = Encoding.UTF8.GetBytes(pageData);
                        resp.ContentType = absolutePath.EndsWith(".json") ? "application/json" : "text/html";
                        resp.ContentEncoding = Encoding.UTF8;
                        resp.ContentLength64 = data.LongLength;
                        break;
                }

                if (!req.Url.AbsolutePath.Contains(".")) {
                    pageViews++;
                }
                

                // Write out to the response stream (asynchronously), then close it
                await resp.OutputStream.WriteAsync(data, 0, data.Length);
                resp.Close();
            }
        }

        public static void Main(string[] args)
        {
            // Create a Http server and start listening for incoming connections
            url = httpProtocol + "*" + portNumber + '/';
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            Console.WriteLine("Listening for connections on {0}", url);

            // Handle requests
            Task listenTask = HandleIncomingConnections();
            listenTask.GetAwaiter().GetResult();

            // Close the listener
            listener.Close();
        }
    }
    
}