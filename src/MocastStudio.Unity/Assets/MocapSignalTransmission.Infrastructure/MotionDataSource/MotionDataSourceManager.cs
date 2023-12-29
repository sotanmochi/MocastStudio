using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MocapSignalTransmission.Infrastructure.Constants;
using MocapSignalTransmission.MotionDataSource;

namespace MocapSignalTransmission.Infrastructure.MotionDataSource
{
    public sealed class MotionDataSourceManager: IMotionDataSourceManager
    {
        private readonly List<IMotionDataSourceManager> _dataSourceManagers = new();

        private MocastStudioDataSourceManager _mocastStudioDataSourceManager;
        private VMCProtocolDataSourceManager _vmcProtocolDataSourceManager;
#if MOCOPI_RECEIVER_PLUGIN
        private MocopiDataSourceManager _mocopiDataSourceManager;
#endif
#if MOTION_BUILDER_RECEIVER_PLUGIN
        private MotionBuilderDataSourceManager _motionBuilderDataSourceManager;
#endif

        public MotionDataSourceManager
        (
            MocastStudioDataSourceManager mocastStudioDataSourceManager,
            VMCProtocolDataSourceManager vmcProtocolDataSourceManager
#if MOCOPI_RECEIVER_PLUGIN
            ,MocopiDataSourceManager mocopiDataSourceManager
#endif
#if MOTION_BUILDER_RECEIVER_PLUGIN
            ,MotionBuilderDataSourceManager motionBuilderDataSourceManager
#endif
        )
        {
            // Mocast Studio
            _mocastStudioDataSourceManager = mocastStudioDataSourceManager;
            _dataSourceManagers.Add(mocastStudioDataSourceManager);

            // VMC Protocol
            _vmcProtocolDataSourceManager = vmcProtocolDataSourceManager;
            _dataSourceManagers.Add(vmcProtocolDataSourceManager);

#if MOCOPI_RECEIVER_PLUGIN
            _mocopiDataSourceManager = mocopiDataSourceManager;
            _dataSourceManagers.Add(mocopiDataSourceManager);
#endif
#if MOTION_BUILDER_RECEIVER_PLUGIN
            _motionBuilderDataSourceManager = motionBuilderDataSourceManager;
            _dataSourceManagers.Add(motionBuilderDataSourceManager);
#endif
        }

        public void Dispose()
        {
            _vmcProtocolDataSourceManager = null;
#if MOCOPI_RECEIVER_PLUGIN
            _mocopiDataSourceManager = null;
#endif
#if MOTION_BUILDER_RECEIVER_PLUGIN
            _motionBuilderDataSourceManager = null;
#endif
            foreach (var dataSourceManager in _dataSourceManagers)
            {
                dataSourceManager.Dispose();
            }
            _dataSourceManagers.Clear();
        }

        public bool Contains(int dataSourceId)
        {
            foreach (var dataSourceManager in _dataSourceManagers)
            {
                if (dataSourceManager.Contains(dataSourceId))
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<IMotionDataSource> CreateAsync(int dataSourceId, MotionDataSourceSettings settings)
        {
            return settings.DataSourceType switch
            {
                (int)MotionDataSourceType.MocastStudio_Remote => await _mocastStudioDataSourceManager.CreateAsync(dataSourceId, settings),
                (int)MotionDataSourceType.VMCProtocol_TypeA => await _vmcProtocolDataSourceManager.CreateAsync(dataSourceId, settings),
                (int)MotionDataSourceType.VMCProtocol_TypeB => await _vmcProtocolDataSourceManager.CreateAsync(dataSourceId, settings),
#if MOCOPI_RECEIVER_PLUGIN
                (int)MotionDataSourceType.Mocopi => await _mocopiDataSourceManager.CreateAsync(dataSourceId, settings),
#endif
#if MOTION_BUILDER_RECEIVER_PLUGIN
                (int)MotionDataSourceType.MotionBuilder => await _motionBuilderDataSourceManager.CreateAsync(dataSourceId, settings),
#endif
                _ => throw new ArgumentException($"Invalid DataSourceType: {settings.DataSourceType}"),
            };
        }

        public async Task<bool> ConnectAsync(int dataSourceId, CancellationToken cancellationToken = default)
        {
            foreach (var dataSourceManager in _dataSourceManagers)
            {
                if (dataSourceManager.Contains(dataSourceId))
                {
                    return await dataSourceManager.ConnectAsync(dataSourceId, cancellationToken);
                }
            }
            return false;
        }

        public async Task DisconnectAsync(int dataSourceId, CancellationToken cancellationToken = default)
        {
            foreach (var dataSourceManager in _dataSourceManagers)
            {
                if (dataSourceManager.Contains(dataSourceId))
                {
                    await dataSourceManager.DisconnectAsync(dataSourceId, cancellationToken);
                    return;
                }
            }
        }
    }
}
