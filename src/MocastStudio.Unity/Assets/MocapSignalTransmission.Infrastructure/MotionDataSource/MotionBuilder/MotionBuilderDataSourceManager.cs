#if MOTION_BUILDER_RECEIVER_PLUGIN

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MocapSignalTransmission.Infrastructure.Constants;
using MocapSignalTransmission.MotionDataSource;
using MocapStreamingReceiver.MotionBuilder;
using UnityEngine;

namespace MocapSignalTransmission.Infrastructure.MotionDataSource
{
    public sealed class MotionBuilderDataSourceManager : IMotionDataSourceManager
    {
        private readonly Dictionary<int, MotionDataSourceSettings> _dataSourceSettings = new();
        private readonly Dictionary<int, MotionBuilderStreamingReceiver> _streamingReceivers = new();

        private readonly int _targetFrameRate = 60;
        private readonly bool _isBackground = true;
        
        public MotionBuilderDataSourceManager()
        {
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
            if (dataSourceId < 0)
            {
                throw new ArgumentException($"{nameof(dataSourceId)} must be greater than or equal to 0.");
            }
            
            if (dataSourceSettings.DataSourceType != (int)MotionDataSourceType.MotionBuilder)
            {
                throw new ArgumentException($"{nameof(dataSourceSettings.DataSourceType)} must be {nameof(MotionDataSourceType.MotionBuilder)}.");
            }
            
            if (_dataSourceSettings.ContainsKey(dataSourceId))
            {
                Debug.Log($"<color=orange>[{nameof(MotionBuilderDataSourceManager)}] DataSource[{dataSourceId}] is already created.</color>");
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
                _streamingReceivers.Add(dataSourceId, new MotionBuilderStreamingReceiver(_targetFrameRate, _isBackground));
            }
            else
            {
                _streamingReceivers.Add(dataSourceId, _streamingReceivers[streamingReceiverId]);
            }
            
            var dataBuffer = new MotionBuilderDataBuffer(dataSourceId, dataSourceSettings.StreamingDataId);
            
            _streamingReceivers[dataSourceId].OnDataReceived += dataBuffer.Enqueue;
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
            
            var serverAddress = dataSourceSettings.ServerAddress;
            var port = dataSourceSettings.Port;

            return await streamingReceiver.ConnectAsync(serverAddress, port, cancellationToken);
        }
        
        public async Task DisconnectAsync(int dataSourceId, CancellationToken cancellationToken = default)
        {
            if (_streamingReceivers.TryGetValue(dataSourceId, out var streamingReceiver))
            {
                streamingReceiver.Disconnect();
            }
        }
    }
}
#endif