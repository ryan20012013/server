using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace HttpListenerExample {
    class ShutdownModel {
        public Boolean ServerRunning {get; set;} = true;

        public ShutdownModel(Boolean runningStatus) {
            this.ServerRunning = runningStatus;
        }
    }
    class ShutdownServlet: BaseServlet {
        public byte[] handleRequest(HttpListenerRequest request, HttpListenerResponse response){
            switch (request.HttpMethod) {
                case METHOD_POST:
                    HttpServer.runServer = false;
                    byte[] data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new ShutdownModel(false)));
                    setResponse(response, MediaTypeNames.Application.Json, Encoding.UTF8, data.LongLength);
                    Console.WriteLine(JsonSerializer.Serialize(new ShutdownModel(false)));
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