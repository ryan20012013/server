using System.Net;
using System.Text.RegularExpressions;

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
            if (jsonString == "") {
                return new Dictionary<string, string>();
            }
            Dictionary<String, String> result = new Dictionary<String, String>();
            String[] jsonObjects = jsonString.Split("&");
            foreach (String jsonObject in jsonObjects){
                Console.WriteLine("jsonObject: " + jsonObject);
                String[] keyValue = jsonObject.Split("=", 2); 
                if (keyValue.Length == 1) {
                    keyValue = jsonObject.Split(":", 2);
                    keyValue[0] = removeSymbol(keyValue[0]);
                    keyValue[1] = removeSymbol(keyValue[1]);
                }
                result.Add(keyValue[0], keyValue[1]);
            }
            return result;
        }

        private string removeSymbol(String str) {
            return Regex.Replace(str, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
        }
    }
}