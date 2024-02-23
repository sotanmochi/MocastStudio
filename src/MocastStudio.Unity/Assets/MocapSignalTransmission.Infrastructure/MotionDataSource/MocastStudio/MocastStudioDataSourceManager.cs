using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MocapSignalTransmission.Infrastructure.Constants;
using MocapSignalTransmission.MotionData;
using MocapSignalTransmission.MotionDataSource;
using SignalStreaming;
using Debug = UnityEngine.Debug;

namespace MocapSignalTransmission.Infrastructure.MotionDataSource
{
    public sealed class MocastStudioDataSourceManager : IMotionDataSourceManager
    {
        readonly Dictionary<int, MotionDataSourceSettings> _dataSourceSettings = new();
        readonly Dictionary<int, ISignalStreamingClient> _streamingReceivers = new();

        ISignalStreamingClientFactory _streamingClientFactory;
        int _signalTransportType;

        public MocastStudioDataSourceManager(ISignalStreamingClientFactory streamingClientFactory, int signalTransportType)
        {
            _streamingClientFactory = streamingClientFactory;
            _signalTransportType = signalTransportType;
        }

        public void Dispose()
        {
            foreach (var streamingReceiver in _streamingReceivers.Values)
            {
                streamingReceiver.Dispose();
            }
            _streamingReceivers.Clear();
            _dataSourceSettings.Clear();
        }

        public bool Contains(int dataSourceId)
        {
            return _dataSourceSettings.ContainsKey(dataSourceId) && _streamingReceivers.ContainsKey(dataSourceId);
        }

        public async Task<IMotionDataSource> CreateAsync(int dataSourceId, MotionDataSourceSettings dataSourceSettings)
        {
            if (dataSourceSettings.DataSourceType != (int)MotionDataSourceType.MocastStudio_Remote)
            {
                throw new ArgumentException($"{nameof(dataSourceSettings.DataSourceType)} must be {nameof(MotionDataSourceType.MocastStudio_Remote)}.");
            }

            if (dataSourceId < 0)
            {
                throw new ArgumentException($"{nameof(dataSourceId)} must be greater than or equal to 0.");
            }

            if (_dataSourceSettings.ContainsKey(dataSourceId))
            {
                Debug.Log($"<color=orange>[{nameof(MocastStudioDataSourceManager)}] DataSource[{dataSourceId}] is already created.</color>");
                return null;
            }

            var streamingReceiverId = -1;
            foreach (var registeredData in _dataSourceSettings)
            {
                if (dataSourceSettings.ServerAddress == registeredData.Value.ServerAddress
                && dataSourceSettings.Port == registeredData.Value.Port)
                {
                    streamingReceiverId = registeredData.Key;
                    break;
                }
            }

            if (streamingReceiverId < 0)
            {
                var streamingClient = _streamingClientFactory.Create(_signalTransportType);
                _streamingReceivers.Add(dataSourceId, streamingClient);
            }
            else
            {
                _streamingReceivers.Add(dataSourceId, _streamingReceivers[streamingReceiverId]);
            }

            var dataBuffer = new MocastStudioDataBuffer(dataSourceId, (uint)dataSourceSettings.StreamingDataId);
            _streamingReceivers[dataSourceId].OnIncomingSignalDequeued += dataBuffer.Enqueue;
            _dataSourceSettings.Add(dataSourceId, dataSourceSettings);

            return dataBuffer;
        }

        public async Task<bool> ConnectAsync(int dataSourceId, CancellationToken cancellationToken = default)
        {
            if (!_streamingReceivers.TryGetValue(dataSourceId, out var streamingReceiver))
            {
                return false;
            }

            if (!_dataSourceSettings.TryGetValue(dataSourceId, out var dataSourceSettings))
            {
                return false;
            }

            if (streamingReceiver.IsConnected)
            {
                return true;
            }

            var settings = _dataSourceSettings[dataSourceId];
            var connectParameters = _streamingClientFactory.CreateConnectParameters(
                _signalTransportType, settings.ServerAddress, (ushort)settings.Port);

            return await streamingReceiver.ConnectAsync(connectParameters, cancellationToken);
        }

        public async Task DisconnectAsync(int dataSourceId, CancellationToken cancellationToken = default)
        {
            if (_streamingReceivers.TryGetValue(dataSourceId, out var streamingReceiver))
            {
                await streamingReceiver.DisconnectAsync(cancellationToken);
            }
        }
    }
}
