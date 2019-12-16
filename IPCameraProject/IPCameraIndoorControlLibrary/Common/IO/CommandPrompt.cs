using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraIndoorControlLibrary.Common.IO {

    public class CommandPrompt {

        public static bool Write(string cmd) {
            try {
                var p = new System.Diagnostics.Process();
                p.StartInfo.FileName = @"c:\Windows\Sysnative\cmd.exe";
                p.StartInfo.Arguments = @"/c " + cmd;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardInput = false;
                p.Start();
                p.WaitForExit();
                return true;

            }
            catch { return false; }
        }


        public static string Query(string cmd) {
            try {
                string strOutput = "";
                var p = new System.Diagnostics.Process();
                p.StartInfo.FileName = @"c:\Windows\Sysnative\cmd.exe";
                p.StartInfo.Arguments = @"/c " + cmd;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardInput = false;
                p.OutputDataReceived += (a, b) => strOutput += b.Data + "\n";
                p.ErrorDataReceived += (a, b) => strOutput += b.Data + "\n";
                p.Start();
                p.BeginErrorReadLine();
                p.BeginOutputReadLine();
                p.WaitForExit();
                return strOutput;
            }
            catch { return null; }
        }

    }
}
