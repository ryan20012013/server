using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace HttpListenerExample {
    class DisplayMessageServlet: BaseServlet {


        private const String IMPORT_MESSAGEBOX_CMD = "Add-Type -AssemblyName System.Windows.Forms";
        private const String CREATE_MESSAGEBOX_CMD = "[System.Windows.Forms.MessageBox]::Show('{0}','{1}'," +
                                                    "[System.Windows.Forms.MessageBoxButtons]::OK," +
                                                    "[System.Windows.Forms.MessageBoxIcon]::Information," +
                                                    "[System.Windows.Forms.MessageBoxDefaultButton]::Button1," +
                                                    "[System.Windows.Forms.MessageBoxOptions]::ServiceNotification)";
        public const String KEY_MESSAGE = "message";
        public const String KEY_TITLE = "title";
        public const String DEFAULT_TITLE = "default title";
        public const String DEFAULT_MESSAGE = "default message";

        public class DisplayMessageModel {
            public String message;
            public String title;

            public DisplayMessageModel(String message, String title) {
                this.message = message;
                this.title = title;
            }
            
            public DisplayMessageModel(String message): this(message, DEFAULT_MESSAGE) {
            }

        }
        
        public byte[] handleRequest(HttpListenerRequest request, HttpListenerResponse response) {
            switch (request.HttpMethod) {
                case METHOD_POST:
                    StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding);
                    
                    Dictionary<String, String> messageInfo = extractJson(System.Web.HttpUtility.UrlDecode(reader.ReadToEnd()));
                    String message = messageInfo.GetValueOrDefault(KEY_MESSAGE, DEFAULT_MESSAGE);
                    String title = messageInfo.GetValueOrDefault(KEY_TITLE, DEFAULT_TITLE);
                    
                    ExecuteCmd.executePowerShellCmdNoResult(IMPORT_MESSAGEBOX_CMD + ";" + String.Format(CREATE_MESSAGEBOX_CMD, message, title));
                    byte[] data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new DisplayMessageModel(message, title)));
                    setResponse(response, MediaTypeNames.Application.Json, Encoding.UTF8, data.LongLength);
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