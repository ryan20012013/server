using System.Net;

namespace HttpListenerExample {
    class BaseServlet {

        public const String METHOD_GET = "GET";
        public const String METHOD_PUT = "PUT";
        public const String METHOD_POST = "POST";
        public const String METHOD_DELETE = "DELETE";
        
        public byte[] handleRequest(HttpListenerRequest request, HttpListenerResponse response) {
            byte[] data = new byte[0];
            response.StatusCode = (int) HttpStatusCode.MethodNotAllowed;
            response.ContentLength64 = data.Length;
            return data;
        }

        public void setResponse(HttpListenerResponse response, String contentType, System.Text.Encoding contentEncoding, long dataLen){
            response.ContentType = contentType;
            response.ContentEncoding = contentEncoding;
            response.ContentLength64 = dataLen;
        }

        public Dictionary<String, String> extractJson(String jsonString) {
            Dictionary<String, String> result = new Dictionary<String, String>();
            String[] jsonObjects = jsonString.Split("&");
            foreach (String jsonObject in jsonObjects){
                String[] keyValue = jsonObject.Split("=", 2); 
                result.Add(keyValue[0], keyValue[1]);
            }
            return result;
        }
    }
}