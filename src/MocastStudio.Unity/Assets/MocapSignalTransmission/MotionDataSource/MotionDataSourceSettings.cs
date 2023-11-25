namespace MocapSignalTransmission.MotionDataSource
{
    public sealed class MotionDataSourceSettings
    {
        public int DataSourceId { get; internal set; }
        public int DataSourceType { get; }
        public int StreamingDataId { get; }
        public string ServerAddress { get; }
        public int Port { get; }

        public MotionDataSourceSettings(int dataSourceType, int streamingDataId, string serverAddress, int port)
        {
            DataSourceId = -1;
            DataSourceType = dataSourceType;
            StreamingDataId = streamingDataId;
            ServerAddress = serverAddress;
            Port = port;
        }
    }
}
