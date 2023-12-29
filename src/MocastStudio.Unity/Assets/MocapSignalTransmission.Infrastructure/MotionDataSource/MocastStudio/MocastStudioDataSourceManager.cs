using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MessagePack;
using MocapSignalTransmission.Infrastructure.Constants;
using MocapSignalTransmission.MotionDataSource;
using SignalStreaming;
using SignalStreaming.Infrastructure;
using Debug = UnityEngine.Debug;
using HumanPose = UnityEngine.HumanPose;

namespace MocapSignalTransmission.Infrastructure.MotionDataSource
{
    public sealed class MocastStudioDataSourceManager : IMotionDataSourceManager
    {
        readonly Dictionary<uint, MocastStudioDataBuffer> _dataBuffers = new();
        readonly int _streamingMessageId = (int)SignalType.MotionCaptureData;

        ISignalStreamingClientFactory _clientFactory;
        ISignalStreamingClient _client;
        IConnectParameters _connectParameters;
        int _dataSourceId = -1;

        public MocastStudioDataSourceManager(ISignalStreamingClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public void Dispose()
        {
            _dataSourceId = -1;

            if (_client != null)
            {
                _client.OnDataReceived -= OnDataReceivedInternal;
                _client = null;
            }
        }

        public bool Contains(int dataSourceId)
        {
            return _dataSourceId == dataSourceId;
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

            if (_dataSourceId == dataSourceId)
            {
                Debug.Log($"<color=orange>[{nameof(MocastStudioDataSourceManager)}] DataSource[{dataSourceId}] is already created.</color>");
                return null;
            }

            var streamingDataId = dataSourceSettings.StreamingDataId;
            var dataBuffer = new MocastStudioDataBuffer(streamingDataId);

            _dataSourceId = dataSourceId;
            _dataBuffers.Add((uint)streamingDataId, dataBuffer);

            _client = _clientFactory.Create((int)SignalTransportType.ENet);
            _client.OnDataReceived += OnDataReceivedInternal;
            _connectParameters = _clientFactory.CreateConnectParameters(
                (int)SignalTransportType.ENet, dataSourceSettings.ServerAddress, (ushort)dataSourceSettings.Port);

            return dataBuffer;
        }

        public async Task<bool> ConnectAsync(int dataSourceId, CancellationToken cancellationToken = default)
        {
            if (_dataSourceId != dataSourceId) return false;
            return await _client.ConnectAsync(_connectParameters, cancellationToken);
        }

        public async Task DisconnectAsync(int dataSourceId, CancellationToken cancellationToken = default)
        {
            if (_dataSourceId != dataSourceId) return;
            await _client.DisconnectAsync(cancellationToken);
        }

        void OnDataReceivedInternal(int messageId, uint senderClientId, long originTimestamp, long transmitTimestamp, ReadOnlyMemory<byte> payload)
        {
            if (messageId != _streamingMessageId) return;

            if (_dataBuffers.TryGetValue(senderClientId, out var dataBuffer))
            {
                dataBuffer.Enqueue(MessagePackSerializer.Deserialize<HumanPose>(payload));
            }
        }
    }
}
