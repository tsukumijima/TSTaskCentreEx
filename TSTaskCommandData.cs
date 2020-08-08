public class TSTaskCommandData
{
    // "TaskStarted",
    // "TaskEnded",
    // "Response",
    // "Event",
    // "RegisterClient",
    // "UnregisterClient",
    public string[] _tstaskcommand = new string[] {
        "Hello",
        "EndTask",
        "LoadBonDriver",
        "UnloadBonDriver",
        "GetBonDriver",
        "OpenTuner",
        "CloseTuner",
        "SetChannel",
        "GetChannel",
        "SetService",
        "GetService",
        "GetServiceList",
        "OpenCasCard",
        "CloseCasCard",
        "GetStreamStatistics",
        "ResetErrorStatistics",
        "StartRecording",
        "StopRecording",
        "ChangeRecordingFile",
        "GetRecordingInfo",
        "GetEventInfo",
        "GetLog",
        "GetChannelList",
        "GetBonDriverChannelList",
        "GetScannedChannelList",
        "CreateStreamPool",
        "CloseStreamPool",
        "GetTvRockInfo",
        "StartStreaming",
        "StopStreaming",
        "GetStreamingInfo",
        "GetSetting"
    };

    private string[] _tstaskproperty = new string[] {
        "Result",
        "Message",
        "ErrorCode",
        "TaskID",
        "FilePath",
        "Directory",
        "FileName",
        "TuningSpace",
        "Channel",
        "ChannelName",
        "TuningSpaceName",
        "NetworkID",
        "TransportStreamID",
        "ServiceName",
        "ServiceID",
        "ServiceType",
        "ScannedChannel",
        "ScannedServiceID",
        "RemoteControlKeyID",
        "Name",
        "Value",
        "Status",
        "ServiceSelect",
        "Descramble",
        "Streams",
        "StartTime",
        "StartTickCount"
    };
}
