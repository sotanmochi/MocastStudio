namespace MocapSignalTransmission.MotionDataSource
{
    public readonly struct MotionDataSourceStatus
    {
        public readonly int DataSourceId;
        public readonly DataSourceStatusType StatusType;

        public MotionDataSourceStatus(int dataSourceId, DataSourceStatusType statusType)
        {
            DataSourceId = dataSourceId;
            StatusType = statusType;
        }
    }

    public enum DataSourceStatusType
    {
        Disconnected = -1,
        Connecting = 0,
        Connected = 1,
    }
}
