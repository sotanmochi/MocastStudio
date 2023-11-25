#if MOCOPI_RECEIVER_PLUGIN

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MocapSignalTransmission.Infrastructure.Constants;
using MocapSignalTransmission.MotionDataSource;
using Mocopi.Receiver.Core;
using Debug = UnityEngine.Debug;

namespace MocapSignalTransmission.Infrastructure.MotionDataSource
{
    public sealed class MocopiDataSourceManager : IMotionDataSourceManager
    {
        private readonly Dictionary<int, MocopiUdpReceiver> _mocopiUdpReceivers = new();

        public void Dispose()
        {
            foreach (var mocopiUdpReceiver in _mocopiUdpReceivers.Values)
            {
                mocopiUdpReceiver.UdpStop();
            }
            _mocopiUdpReceivers.Clear();
        }

        public bool Contains(int dataSourceId)
        {
            return _mocopiUdpReceivers.ContainsKey(dataSourceId);
        }

        public async Task<IMotionDataSource> CreateAsync(int dataSourceId, MotionDataSourceSettings dataSourceSettings)
        {
            if (dataSourceId < 0)
            {
                throw new ArgumentException($"{nameof(dataSourceId)} must be greater than or equal to 0.");
            }

            if (dataSourceSettings.DataSourceType != (int)MotionDataSourceType.Mocopi)
            {
                throw new ArgumentException($"{nameof(dataSourceSettings.DataSourceType)} must be {nameof(MotionDataSourceType.Mocopi)}.");
            }

            if (_mocopiUdpReceivers.ContainsKey(dataSourceId))
            {
                Debug.Log($"<color=orange>[{nameof(MocopiDataSourceManager)}] DataSource[{dataSourceId}] is already created.</color>");
                return null;
            }

            var streamingReceiverId = -1;
            foreach (var registeredData in _mocopiUdpReceivers)
            {
                if (dataSourceSettings.Port == registeredData.Value.Port)
                {
                    streamingReceiverId = registeredData.Key;
                    break;
                }
            }

            if (streamingReceiverId < 0)
            {
                _mocopiUdpReceivers.Add(dataSourceId, new MocopiUdpReceiver(dataSourceSettings.Port));
            }
            else
            {
                _mocopiUdpReceivers.Add(dataSourceId, _mocopiUdpReceivers[streamingReceiverId]);
            }

            var dataBuffer = new MocopiDataBuffer(dataSourceId);

            var mocopiUdpReceiver = _mocopiUdpReceivers[dataSourceId];
            mocopiUdpReceiver.OnReceiveSkeletonDefinition += dataBuffer.InitializeSkeleton;
            mocopiUdpReceiver.OnReceiveFrameData += dataBuffer.UpdateSkeleton;

            return dataBuffer;
        }

        public async Task<bool> ConnectAsync(int dataSourceId, CancellationToken cancellationToken = default)
        {
            if (!_mocopiUdpReceivers.TryGetValue(dataSourceId, out var mocopiUdpReceiver))
            {
                return false;
            }

            if (mocopiUdpReceiver.IsRuning)
            {
                return true;
            }

            mocopiUdpReceiver.UdpStart();
            return true;
        }

        public async Task DisconnectAsync(int dataSourceId, CancellationToken cancellationToken = default)
        {
            if (_mocopiUdpReceivers.TryGetValue(dataSourceId, out var mocopiUdpReceiver))
            {
                mocopiUdpReceiver.UdpStop();
            }
        }
    }
}
#endif
