using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MocapSignalTransmission.Infrastructure.Constants;
using MocapSignalTransmission.MotionDataSource;
using Debug = UnityEngine.Debug;

namespace MocapSignalTransmission.Infrastructure.MotionDataSource
{
    public sealed class VMCProtocolDataSourceManager : IMotionDataSourceManager
    {
        private readonly Dictionary<int, VMCProtocolStreamingReceiver> _streamingReceivers = new();

        public void Dispose()
        {
            foreach (var streamingReceiver in _streamingReceivers.Values)
            {
                streamingReceiver.Dispose();
            }
            _streamingReceivers.Clear();
        }

        public bool Contains(int dataSourceId)
        {
            return _streamingReceivers.ContainsKey(dataSourceId);
        }

        public async Task<IMotionDataSource> CreateAsync(int dataSourceId, MotionDataSourceSettings dataSourceSettings)
        {
            if (dataSourceId < 0)
            {
                throw new ArgumentException($"{nameof(dataSourceId)} must be greater than or equal to 0.");
            }

            if (dataSourceSettings.DataSourceType != (int)MotionDataSourceType.VMCProtocol_TypeA &&
                dataSourceSettings.DataSourceType != (int)MotionDataSourceType.VMCProtocol_TypeB)
            {
                throw new ArgumentException($"{nameof(dataSourceSettings.DataSourceType)} must be " +
                                            $"{nameof(MotionDataSourceType.VMCProtocol_TypeA)} or {nameof(MotionDataSourceType.VMCProtocol_TypeB)}.");
            }

            if (_streamingReceivers.ContainsKey(dataSourceId))
            {
                Debug.Log($"<color=orange>[{nameof(VMCProtocolDataSourceManager)}] DataSource[{dataSourceId}] is already created.</color>");
                return null;
            }

            var streamingReceiverId = -1;
            foreach (var registeredData in _streamingReceivers)
            {
                if (dataSourceSettings.Port == registeredData.Value.Port)
                {
                    streamingReceiverId = registeredData.Key;
                    break;
                }
            }

            if (streamingReceiverId < 0)
            {
                var rootTransformType = dataSourceSettings.DataSourceType switch
                {
                    (int)MotionDataSourceType.VMCProtocol_TypeA => VMCProtocolStreamingReceiver.RootTransformType.TypeA,
                    (int)MotionDataSourceType.VMCProtocol_TypeB => VMCProtocolStreamingReceiver.RootTransformType.TypeB,
                    _ => throw new ArgumentException($"{nameof(dataSourceSettings.DataSourceType)} must be " +
                                                     $"{nameof(MotionDataSourceType.VMCProtocol_TypeA)} or {nameof(MotionDataSourceType.VMCProtocol_TypeB)}.")
                };
                _streamingReceivers.Add(dataSourceId, new VMCProtocolStreamingReceiver(dataSourceSettings.Port, rootTransformType));
            }
            else
            {
                _streamingReceivers.Add(dataSourceId, _streamingReceivers[streamingReceiverId]);
            }

            var dataBuffer = new VMCProtocolDataBuffer(dataSourceId);
            _streamingReceivers[dataSourceId].OnDataReceived += dataBuffer.Enqueue;

            return dataBuffer;
        }

        public async Task<bool> ConnectAsync(int dataSourceId, CancellationToken cancellationToken = default)
        {
            if (!_streamingReceivers.TryGetValue(dataSourceId, out var streamingReceiver))
            {
                return false;
            }

            if (streamingReceiver.IsRunning)
            {
                return true;
            }

            streamingReceiver.Start();
            return true;
        }

        public async Task DisconnectAsync(int dataSourceId, CancellationToken cancellationToken = default)
        {
            if (_streamingReceivers.TryGetValue(dataSourceId, out var streamingReceiver))
            {
                streamingReceiver.Stop();
            }
        }
    }
}
