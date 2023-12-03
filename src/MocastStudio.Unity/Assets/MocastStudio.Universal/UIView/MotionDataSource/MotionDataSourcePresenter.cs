using System;
using MessagePipe;
using MocapSignalTransmission.Infrastructure.Constants;
using MocapSignalTransmission.MotionDataSource;
using UniRx;
using VContainer.Unity;

namespace MocastStudio.Universal.UIView.MotionDataSource
{
    public sealed class MotionDataSourcePresenter : IInitializable, IDisposable
    {
        readonly MotionDataSourceService _motionDataSourceService;
        readonly MotionDataSourceServiceContext _motionDataSourceContext;
        readonly MotionDataSourceListView _motionDataSourceListView;
        readonly MotionDataSourceLoaderView _motionDataSourceLoaderView;
        readonly CompositeDisposable _compositeDisposable = new();

        public MotionDataSourcePresenter(
            MotionDataSourceService motionDataSourceService,
            MotionDataSourceServiceContext motionDataSourceContext,
            MotionDataSourceListView motionDataSourceListView,
            MotionDataSourceLoaderView motionDataSourceLoaderView)
        {
            _motionDataSourceService = motionDataSourceService;
            _motionDataSourceContext = motionDataSourceContext;
            _motionDataSourceListView = motionDataSourceListView;
            _motionDataSourceLoaderView = motionDataSourceLoaderView;
        }

        void IDisposable.Dispose()
        {
            _compositeDisposable.Dispose();
        }

        void IInitializable.Initialize()
        {
            UnityEngine.Debug.Log($"<color=lime>[{nameof(MotionDataSourcePresenter)}] Initialize</color>");

            _motionDataSourceLoaderView.UpdateDataSourceTypeDropdown(new MotionDataSourceType[]
            {
                MotionDataSourceType.VMCProtocol_TypeA,
                MotionDataSourceType.VMCProtocol_TypeB,
                MotionDataSourceType.Mocopi,
                MotionDataSourceType.MotionBuilder,
            });

            _motionDataSourceLoaderView.OnDataSourceAdditionRequested
                .Subscribe(dataSourceSettings =>
                {
                    _motionDataSourceService.AddMotionDataSourceAsync(dataSourceSettings);
                })
                .AddTo(_compositeDisposable);

            _motionDataSourceListView.OnConnectionRequested
                .Subscribe(dataSourceId =>
                {
                    _motionDataSourceService.ConnectAsync(dataSourceId);
                })
                .AddTo(_compositeDisposable);

            _motionDataSourceListView.OnDisconnectionRequested
                .Subscribe(dataSourceId =>
                {
                    _motionDataSourceService.DisconnectAsync(dataSourceId);
                })
                .AddTo(_compositeDisposable);

            _motionDataSourceContext.OnDataSourceAdded
                .Subscribe(dataSource =>
                {
                    var dataSourceType = dataSource.DataSourceType switch
                    {
                        (int)MotionDataSourceType.VMCProtocol_TypeA => "VMCProtocol_TypeA",
                        (int)MotionDataSourceType.VMCProtocol_TypeB => "VMCProtocol_TypeB",
                        (int)MotionDataSourceType.Mocopi => "mocopi",
                        (int)MotionDataSourceType.MotionBuilder => "MotionBuilder",
                        _ => "Unknown",
                    };
                    _motionDataSourceListView.UpdateView(dataSource.DataSourceId, dataSourceType, dataSource.ServerAddress, dataSource.Port, dataSource.StreamingDataId);
                })
                .AddTo(_compositeDisposable);

            _motionDataSourceContext.OnDataSourceStatusUpdated
                .Subscribe(status =>
                {
                    var color = status.StatusType switch
                    {
                        DataSourceStatusType.Connected => UnityEngine.Color.green,
                        DataSourceStatusType.Connecting => UnityEngine.Color.yellow,
                        DataSourceStatusType.Disconnected => UnityEngine.Color.red,
                        _ => UnityEngine.Color.gray,
                    };
                    _motionDataSourceListView.UpdateStatusColor(status.DataSourceId, color);
                })
                .AddTo(_compositeDisposable);
        }
    }
}
