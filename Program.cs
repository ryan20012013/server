// See https://aka.ms/new-console-template for more information
using System.Text;
using System.Net;
using CrawlerExample;


namespace HttpListenerExample
{
    
    class HttpServer
    {
        public static HttpListener listener;
        public static string url = "http://localhost:8080/";
        public static string httpProtocol = "http://";
        public static string portNumber = ":20250";
        public const string JSON_FILE = ".json";
        public const string HTTP_JSON_FORMAT = "application/json";
        public const string HTTP_TEXT_FORMAT = "text/html";
        public static int pageViews = 0;
        public static int requestCount = 0;
        public static bool runServer = true;

        private static string resourceDir = Environment.CurrentDirectory + "/root";
        public static string PAGEDATA = File.ReadAllText(resourceDir + "/web/default.html");

        private static string dataDir = Environment.CurrentDirectory + "/root/data";
        private static string bandaiInfo = "/bandaiInfo.json";


        private static readonly HttpClient client = new HttpClient();


        public static async Task HandleIncomingConnections() {

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
                    case "/api/message": {
                        data = new DisplayMessageServlet().handleRequest(req, resp);
                        break;
                    }
                    default:
                        String pageData = "";
                        if (req.Url.AbsolutePath == "/" ) {
                            pageData = PAGEDATA;
                        }
                        else if (File.Exists(absolutePath)) {
                            Console.WriteLine("pageData from " + absolutePath);
                            pageData = File.ReadAllText(absolutePath);
                        } else {
                            pageData = File.ReadAllText(resourceDir + "/web/404.html");
                        }
                        data = Encoding.UTF8.GetBytes(pageData);
                        resp.ContentType = absolutePath.EndsWith(JSON_FILE) ? HTTP_JSON_FORMAT : HTTP_TEXT_FORMAT;
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

        public static void Main(string[] args) {
            // Create a Http server and start listening for incoming connections
            url = httpProtocol + "*" + portNumber + '/';
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            Console.WriteLine("Listening for connections on {0}", url);
            Crawler crawler = new Crawler("https://p-bandai.com/hk/search?sort=relevance&sellDate=0&sellDate=1&shop=05-001", dataDir + bandaiInfo);
            crawler.startCrawlerTask(); 
            // Handle requests
            Task listenTask = HandleIncomingConnections();

            listenTask.GetAwaiter().GetResult();

            // Close the listener
            listener.Close();
        }
    }
    
}
