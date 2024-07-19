
using System.Diagnostics;

namespace HttpListenerExample {
    class ExecuteCmd {

        public const String EXECUTE_FILE_LINUX = "/bin/bash";
        public const String EXECUTE_FILE_WINDOW = "powershell.exe";
        public const String EXECUTE_CMD_PREFIX_LINUX = "-c \"{0}\"";
        public const String EXECUTE_CMD_PREFIX_WINDOW = "/C ";

        public static string executeLinuxCmd(String cmd) {
            return executeCmd(EXECUTE_FILE_LINUX, cmd);
        }

        public static string executePowerShellCmd(String cmd) {
            return executeCmd(EXECUTE_FILE_WINDOW, cmd);
        }

        public static void executePowerShellCmdNoResult(String cmd) {
            executeCmdNoResult(EXECUTE_FILE_WINDOW, cmd);
        }

        private static void executeCmdNoResult(String fileName, String cmd) {
            var psi = new ProcessStartInfo();
            psi.FileName = fileName;
            psi.Arguments = fileName == EXECUTE_FILE_LINUX ? String.Format(EXECUTE_CMD_PREFIX_LINUX, cmd) : EXECUTE_CMD_PREFIX_WINDOW + cmd;
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            using var process = Process.Start(psi);
        }

        private static string executeCmd(String fileName, String cmd) {
            var psi = new ProcessStartInfo();
            psi.FileName = fileName;
            psi.Arguments = fileName == EXECUTE_FILE_LINUX ? String.Format(EXECUTE_CMD_PREFIX_LINUX, cmd) : EXECUTE_CMD_PREFIX_WINDOW + cmd;
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            using var process = Process.Start(psi);

            process.WaitForExit();

            var output = process.StandardOutput.ReadToEnd();
            
            return output;
        }
        
    }
}