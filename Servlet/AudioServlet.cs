using System.ComponentModel;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace HttpListenerExample {
    class AudioServlet: BaseServlet {

        public const String KEY_VOLUME = "volume";
        public const String KEY_MUTE = "mute";
        public const String AUDIO_SET_COMMAND = "Set-AudioDevice";
        public const String AUDIO_GET_COMMAND = "Get-AudioDevice";
        public const String AUDIO_MUTE_OPTION = "-PlaybackMute";
        public const String AUDIO_VOLUME_OPTION = "-PlaybackVolume";
        public const String AUDIO_CONTROL_MUTE = "1";
        public const String AUDIO_CONTROL_UNMUTE = "0";
        public const String SET_MUTE_CMD = AUDIO_SET_COMMAND + " " + AUDIO_MUTE_OPTION + " " + AUDIO_CONTROL_MUTE;
        public const String SET_UNMUTE_CMD = AUDIO_SET_COMMAND + " " + AUDIO_MUTE_OPTION + " " + AUDIO_CONTROL_UNMUTE;
        public const String GET_MUTE_CMD = AUDIO_GET_COMMAND + " " + AUDIO_MUTE_OPTION;
        public const String SET_VOLUME_CMD = AUDIO_SET_COMMAND + " " + AUDIO_VOLUME_OPTION;
        public const String GET_VOLUME_CMD = AUDIO_GET_COMMAND + " " + AUDIO_VOLUME_OPTION;
        private AudioModel audioInfo = new AudioModel();


        class AudioModel {
            public int volume {get; set;}
            public Boolean mute {get; set;}

            public AudioModel(int volume, Boolean mute){
                this.volume = volume;
                this.mute = mute;
            }

            public AudioModel(): this(100, false) {
            }
        }

        public byte[] handleRequest(HttpListenerRequest request, HttpListenerResponse response) {
            byte[] data;
            switch (request.HttpMethod) {
                case METHOD_POST:
                    StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding);
                    Dictionary<String, String> keyValuePairs = extractJson(System.Web.HttpUtility.UrlDecode(reader.ReadToEnd()));
                    
                    int volume = Int32.Parse(keyValuePairs.GetValueOrDefault(KEY_VOLUME, "0"));
                    Boolean mute = Boolean.Parse(keyValuePairs.GetValueOrDefault(KEY_MUTE, "False"));

                    audioInfo.volume = volume > 100 ? 100 : (volume < 0 ? 0 : volume);
                    audioInfo.mute = mute;

                    if (mute) {
                        ExecuteCmd.executePowerShellCmd(SET_MUTE_CMD);
                    } else {
                        ExecuteCmd.executePowerShellCmd(SET_UNMUTE_CMD);
                        ExecuteCmd.executePowerShellCmd(SET_VOLUME_CMD + " " + audioInfo.volume);
                    }
                    data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(audioInfo));
                    setResponse(response, MediaTypeNames.Application.Json, Encoding.UTF8, data.LongLength);
                    return data;
                case METHOD_GET:
                    String isMute = Regex.Replace(ExecuteCmd.executePowerShellCmd(GET_MUTE_CMD), @"\t|\n|\r", "");
                    int currentVolume = Int32.Parse(ExecuteCmd.executePowerShellCmd(GET_VOLUME_CMD).Split("%")[0]);
                    audioInfo = new AudioModel(currentVolume, isMute == "True");
                    data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(audioInfo));
                    Console.WriteLine("Mute: " + audioInfo.mute + " Volume: " + audioInfo.volume + " data: " + data);
                    setResponse(response, MediaTypeNames.Application.Json, Encoding.UTF8, data.LongLength);
                    return data;
                case METHOD_PUT:
                case METHOD_DELETE:
                default:
                break;
            }
            return new byte[0];
        }

        public void setResponse(HttpListenerResponse response, String contentType, System.Text.Encoding contentEncoding, long dataLen){
            response.ContentType = contentType;
            response.ContentEncoding = contentEncoding;
            response.ContentLength64 = dataLen;
        }
    }
}