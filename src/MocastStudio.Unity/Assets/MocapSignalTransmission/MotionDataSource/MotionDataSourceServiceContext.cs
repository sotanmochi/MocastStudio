using System;
using System.Collections.Generic;
using MessagePipe;

namespace MocapSignalTransmission.MotionDataSource
{
    public sealed class MotionDataSourceServiceContext : IDisposable
    {
        internal readonly IDisposablePublisher<MotionDataSourceSettings> _dataSourceAddedEventPublisher;
        internal readonly IDisposablePublisher<MotionDataSourceStatus> _dataSourceStatusUpdatedEventPublisher;

        internal readonly List<MotionDataSourceSettings> _dataSourceSettingsList = new();
        internal readonly List<IBodyTrackingDataSource> _bodyTrackingDataSources = new();
        internal readonly List<IFingerTrackingDataSource> _fingerTrackingDataSources = new();
        internal readonly List<IHumanPoseTrackingDataSource> _humanPoseTrackingDataSources = new();

        public ISubscriber<MotionDataSourceSettings> OnDataSourceAdded { get; }
        public ISubscriber<MotionDataSourceStatus> OnDataSourceStatusUpdated { get; }

        public IReadOnlyList<MotionDataSourceSettings> DataSourceSettingsList => _dataSourceSettingsList;
        public IReadOnlyList<IBodyTrackingDataSource> BodyTrackingDataSources => _bodyTrackingDataSources;
        public IReadOnlyList<IFingerTrackingDataSource> FingerTrackingDataSources => _fingerTrackingDataSources;
        public IReadOnlyList<IHumanPoseTrackingDataSource> HumanPoseTrackingDataSources => _humanPoseTrackingDataSources;

        public MotionDataSourceServiceContext(EventFactory eventFactory)
        {
            (_dataSourceAddedEventPublisher, OnDataSourceAdded) = eventFactory.CreateEvent<MotionDataSourceSettings>();
            (_dataSourceStatusUpdatedEventPublisher, OnDataSourceStatusUpdated) = eventFactory.CreateEvent<MotionDataSourceStatus>();
        }

        public void Dispose()
        {
            _dataSourceAddedEventPublisher.Dispose();
            _dataSourceStatusUpdatedEventPublisher.Dispose();
            _dataSourceSettingsList.Clear();
            _bodyTrackingDataSources.Clear();
            _fingerTrackingDataSources.Clear();
        }
    }
}
