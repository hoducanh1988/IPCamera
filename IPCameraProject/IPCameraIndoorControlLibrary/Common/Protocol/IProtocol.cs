using System.IO.Ports;

namespace IPCameraIndoorControlLibrary.Common.Protocol {

    public interface IProtocol {

        bool IsConnected();
        bool Open();
        bool Close();
        
        bool Write(string cmd);
        bool WriteLine(string cmd);
        bool WriteCtrlBreak();
        string Read();
        string Query(string cmd);

    }
}
