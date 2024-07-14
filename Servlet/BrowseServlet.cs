using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace HttpListenerExample {

    class BrowseServlet: BaseServlet {

        private BrowseModel browseInfo = new BrowseModel();

        private const String START_BROWSER_CMD = "start ";
        private const String KEY_URL = "url";

        class BrowseModel {
            public String url {get; set;}

            public BrowseModel(String url) {
                this.url = url;
            }

            public BrowseModel() : this("") {
            }
        }

        public byte[] handleRequest(HttpListenerRequest request, HttpListenerResponse response){
            switch (request.HttpMethod) {
                case METHOD_POST:
                    StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding);
                    String url = extractJson(System.Web.HttpUtility.UrlDecode(reader.ReadToEnd()))[KEY_URL];
                    browseInfo.url = url;
                    byte[] data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(browseInfo));
                    Console.WriteLine(JsonSerializer.Serialize(browseInfo));
                    setResponse(response, MediaTypeNames.Application.Json, Encoding.UTF8, data.LongLength);
                    ExecuteCmd.executePowerShellCmd(START_BROWSER_CMD + url);
                    return data;
                case METHOD_GET:
                case METHOD_PUT:
                case METHOD_DELETE:
                default:
                break;
            }
            return new byte[0];
        }
    }
}