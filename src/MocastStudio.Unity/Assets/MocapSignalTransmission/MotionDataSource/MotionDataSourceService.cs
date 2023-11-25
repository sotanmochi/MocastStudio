using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Debug = UnityEngine.Debug;

namespace MocapSignalTransmission.MotionDataSource
{
    public sealed class MotionDataSourceService : IDisposable
    {
        private readonly MotionDataSourceServiceContext _context;
        private readonly IMotionDataSourceManager _motionDataSourceManager;
        private readonly ConcurrentDictionary<int, CancellationTokenSource> _cancellationTokenSources = new();

        private int _dataSourceCount;

        public MotionDataSourceService(MotionDataSourceServiceContext context, IMotionDataSourceManager motionDataSourceManager)
        {
            _context = context;
            _motionDataSourceManager = motionDataSourceManager;
        }

        public void Dispose()
        {
            foreach (var cts in _cancellationTokenSources.Values)
            {
                cts.Cancel();
            }
            _cancellationTokenSources.Clear();
        }

        public async Task<int> AddMotionDataSourceAsync(MotionDataSourceSettings dataSourceSettings)
        {
            var dataSourceId = Interlocked.Increment(ref _dataSourceCount) - 1;
            dataSourceSettings.DataSourceId = dataSourceId;

            var dataSource = await _motionDataSourceManager.CreateAsync(dataSourceId, dataSourceSettings);
            if (dataSource != null)
            {
                // Body tracking data source
                if (dataSource is IBodyTrackingDataSource bodyTrackingDataSource)
                {
                    _context._bodyTrackingDataSources.Add(bodyTrackingDataSource);
                }
                else
                {
                    _context._bodyTrackingDataSources.Add(null); // TODO: Add comment
                }

                // Finger trakcing data source
                if (dataSource is IFingerTrackingDataSource fingerTrackingDataSource)
                {
                    _context._fingerTrackingDataSources.Add(fingerTrackingDataSource);
                }
                else
                {
                    _context._fingerTrackingDataSources.Add(null); // TODO: Add comment
                }

                // Data source settings
                _context._dataSourceSettingsList.Add(dataSourceSettings);

                _context._dataSourceAddedEventPublisher.Publish(dataSourceSettings);
                _context._dataSourceStatusUpdatedEventPublisher.Publish(new MotionDataSourceStatus(dataSourceId, DataSourceStatusType.Disconnected));
            }
            else
            {
                dataSourceId = -1;
                dataSourceSettings.DataSourceId = -1;

                 // TODO: Add comment
                _context._bodyTrackingDataSources.Add(null);
                _context._fingerTrackingDataSources.Add(null);
                _context._dataSourceSettingsList.Add(null);
            }

            return dataSourceId;
        }

        public async Task<bool> ConnectAsync(int dataSourceId)
        {
            _context._dataSourceStatusUpdatedEventPublisher.Publish(new MotionDataSourceStatus(dataSourceId, DataSourceStatusType.Connecting));

            if (_cancellationTokenSources.TryRemove(dataSourceId, out var cts))
            {
                Debug.Log($"[{nameof(MotionDataSourceService)}] Cancel connection task of data source [{dataSourceId}]");
                cts.Cancel();
            }

            var cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokenSources.TryAdd(dataSourceId, cancellationTokenSource);

            var connected = await _motionDataSourceManager.ConnectAsync(dataSourceId, cancellationTokenSource.Token);
            _cancellationTokenSources.TryRemove(dataSourceId, out var _);

            var statusType = connected ? DataSourceStatusType.Connected : DataSourceStatusType.Disconnected;
            _context._dataSourceStatusUpdatedEventPublisher.Publish(new MotionDataSourceStatus(dataSourceId, statusType));

            return connected;
        }

        public async Task DisconnectAsync(int dataSourceId)
        {
            if (_cancellationTokenSources.TryRemove(dataSourceId, out var cts))
            {
                Debug.Log($"[{nameof(MotionDataSourceService)}] Cancel connection task of data source [{dataSourceId}]");
                cts.Cancel();
            }

            await _motionDataSourceManager.DisconnectAsync(dataSourceId);

            _context._dataSourceStatusUpdatedEventPublisher.Publish(new MotionDataSourceStatus(dataSourceId, DataSourceStatusType.Disconnected));
        }
    }
}
