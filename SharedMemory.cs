using System;
using System.Runtime.InteropServices;
using System.IO.MemoryMappedFiles;

public class SharedMemory
{
    private int _Size = Marshal.SizeOf(typeof(ServerTaskSharedInfo));
    private bool _Task = false;
    private ServerTaskSharedInfo _info = new ServerTaskSharedInfo();

    private string _SharedMemoryName = "TSTask_Server_SharedMemory_";

    // tstaskは最大30だけど
    private const int SharedMemoryChkCount = 15;

    public SharedMemory(uint ProcessID)
    {
        int i;
        for (i = 1; i <= SharedMemoryChkCount; i++)
        {
            try
            {
                using (MemoryMappedFile mmf = MemoryMappedFile.OpenExisting(_SharedMemoryName + i.ToString()))
                {
                    MemoryMappedViewAccessor mma = mmf.CreateViewAccessor(0, _Size);
                    mma.Read(0, out this._info);
                }
                if (this._info.Task.ProcessID == ProcessID)
                {
                    this._Task = true;
                    break;
                }
            }
            catch (Exception ex)
            {
            }
        }
    }

    public uint TaskID
    {
        get
        {
            if (this._Task == false)
                return 0;
            else
                return this._info.Task.TaskID;
        }
    }

    public struct ServerTaskSharedInfo
    {
        public TaskSharedInfoHeader Header;
        public TaskInfo Task;
        public ulong StatisticsUpdateCount;
        public StreamStatistics Statistics;
        public ulong TotTime;
    }

    public struct TaskSharedInfoHeader
    {
        public uint Size;
        public uint Version;
    }

    public struct TaskInfo
    {
        public uint TaskID;
        public TaskType Type;
        public uint ProcessID;
        public uint Version;
        public TaskState State;
    }

    public enum TaskType
    {
        TASK_TYPE_SERVER = 0,
        TASK_TYPE_CLIENT
    }

    public enum TaskState
    {
        TASK_STATE_STARTING = 0,
        TASK_STATE_RUNNING,
        TASK_STATE_ENDING
    }

    public struct StreamStatistics
    {
        public float SignalLevel;
        public uint BitRate;
        public ulong InputPacketCount;
        public ulong ErrorPacketCount;
        public ulong DiscontinuityCount;
        public ulong ScramblePacketCount;
    }
}
