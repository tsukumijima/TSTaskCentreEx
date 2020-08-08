using System;
using System.Text;
using Microsoft.VisualBasic;
using System.IO.Pipes;

public class TSTaskCommandSender
{
    private string _ServerPipeName;
    private string _TaskID;

    public TSTaskCommandSender(int TaskID)
    {
        _TaskID = Convert.ToString(TaskID);
        _ServerPipeName = "TSTask_Server_Pipe_" + _TaskID.ToString();
    }

    public string Send(string Command, string ServerName)
    {
        string ret = "";
        try
        {
            using (NamedPipeClientStream Pipe = new NamedPipeClientStream(ServerName, _ServerPipeName, PipeDirection.InOut, PipeOptions.None))
            {
                Pipe.Connect(1000);
                if (Pipe.IsConnected)
                {
                    byte[] msg = Encoding.Unicode.GetBytes(Command);
                    byte[] wsize = new byte[] { System.Convert.ToByte(msg.Length), 0, 0, 0 };
                    uint sendSize = System.Convert.ToUInt32(msg.Length);
                    Pipe.Write(wsize, 0, wsize.Length);
                    Pipe.WaitForPipeDrain();
                    Pipe.Write(msg, 0, Convert.ToInt32(sendSize));

                    byte[] receiveField = new byte[3145729];
                    int reclen = Pipe.Read(receiveField, 0, receiveField.Length);
                    if (reclen == 4)
                    {
                        long datalen = 0;
                        datalen = BitConverter.ToInt32(receiveField, 0);
                        reclen = Pipe.Read(receiveField, 0, Convert.ToInt32(datalen));
                        ret = Encoding.Unicode.GetString(receiveField, 0, reclen).Replace("\n", "\r\n");
                    }
                    else
                        ret = Encoding.Unicode.GetString(receiveField, 4, reclen - 4).Replace("\n", "\r\n");
                }
            }
        }
        catch (Exception ex)
        {
            return null;
        }
        return ret;
    }
}
