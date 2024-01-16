using System;
using MessagePipe;
using MocapSignalTransmission.Infrastructure.Constants;
using MocapSignalTransmission.Transmitter;
using UniRx;

namespace MocastStudio.Presentation.UIView.Transmitter
{
    public sealed class TransmitterPresenter : IDisposable
    {
        private readonly TransmitterService _transmitterService;
        private readonly TransmitterServiceContext _transmitterServiceContext;
        private readonly TransmitterListView _transmitterListView;
        private readonly TransmitterLoaderView _transmitterLoaderView;
        private readonly CompositeDisposable _compositeDisposable = new();

        public TransmitterPresenter(
            TransmitterService transmitterService,
            TransmitterServiceContext transmitterServiceContext,
            TransmitterListView transmitterListView,
            TransmitterLoaderView transmitterLoaderView)
        {
            _transmitterService = transmitterService;
            _transmitterServiceContext = transmitterServiceContext;
            _transmitterListView = transmitterListView;
            _transmitterLoaderView = transmitterLoaderView;
        }

        public void Dispose()
        {
            _compositeDisposable.Dispose();
        }

        public void Initialize()
        {
            UnityEngine.Debug.Log($"<color=lime>[{nameof(TransmitterPresenter)}] Initialize</color>");

            _transmitterLoaderView.UpdateTransmitterTypeDropdown(new TransmitterType[]
            {
                TransmitterType.MotionActor_VMCProtocol_Local,
                TransmitterType.HumanPose_MocastStudio_Online,
                TransmitterType.HumanPose_OSC_Local,
            });

            _transmitterLoaderView.OnAdditionRequested
                .Subscribe(settingsRequest =>
                {
                    var serializerType = settingsRequest.TransmitterType switch 
                    {
                        TransmitterType.HumanPose_OSC_Local => (int)SerializerType.HumanPose_OSC,
                        TransmitterType.HumanPose_MocastStudio_Online => (int)SerializerType.HumanPose_MessagePack,
                        TransmitterType.MotionActor_VMCProtocol_Local => (int)SerializerType.MotionActor_VMCProtocol,
                        _ => throw new ArgumentException($"Unknown TransmitterType: {settingsRequest.TransmitterType}"),
                    };

                    var transportType = settingsRequest.TransmitterType switch 
                    {
                        TransmitterType.HumanPose_OSC_Local => (int)TransportType.Udp,
                        TransmitterType.HumanPose_MocastStudio_Online => (int)TransportType.SignalStreaming_ENet,
                        TransmitterType.MotionActor_VMCProtocol_Local => (int)TransportType.Udp,
                        _ => throw new ArgumentException($"Unknown TransmitterType: {settingsRequest.TransmitterType}"),
                    };

                    var transmitterId = _transmitterService.AddTransmitter(new TransmitterSettings(serializerType, transportType, settingsRequest.ServerAddress, settingsRequest.Port));
                    _transmitterService.AddActorIds(transmitterId, settingsRequest.ActorIds);
                })
                .AddTo(_compositeDisposable);

            _transmitterListView.OnConnectionRequested
                .Subscribe(transmitterId =>
                {
                    _transmitterService.ConnectAsync(transmitterId);
                })
                .AddTo(_compositeDisposable);

            _transmitterListView.OnDisconnectionRequested
                .Subscribe(transmitterId =>
                {
                    _transmitterService.DisconnectAsync(transmitterId);
                })
                .AddTo(_compositeDisposable);

            _transmitterServiceContext.OnTransmitterAdded 
                .Subscribe(value =>
                {
                    var transmitterType = TransmitterType.Unknown;

                    if (value.SerializerType == (int)SerializerType.HumanPose_OSC &&
                        value.TransportType == (int)TransportType.Udp)
                    {
                        transmitterType = TransmitterType.HumanPose_OSC_Local;
                    }

                    _transmitterListView.UpdateItemView(value.TransmitterId,
                        new TransmitterSettingsRequest(transmitterType, null, value.ServerAddress, value.Port));
                })
                .AddTo(_compositeDisposable);

            _transmitterServiceContext.OnTransmitterStatusUpdated
                .Subscribe(status =>
                {
                    var transmitterId = status.TransmitterId;
                    var statusType = status.StatusType;

                    _transmitterListView.UpdateStatus(transmitterId, statusType);

                    if (statusType == TransmitterStatusType.Connected)
                    {
                        if (_transmitterService.TryGetStreamingActorIds(transmitterId, out var streamingActorIds))
                        {
                            _transmitterListView.UpdateActorIdList(transmitterId, streamingActorIds);
                        }
                    }
                    else if (statusType == TransmitterStatusType.Disconnected)
                    {
                        if (_transmitterService.TryGetLocalActorIds(transmitterId, out var localActorIds))
                        {
                            _transmitterListView.UpdateActorIdList(transmitterId, localActorIds);
                        }
                    }
                })
                .AddTo(_compositeDisposable);

            _transmitterServiceContext.OnTransmitterActorIdUpdated
                .Subscribe(value =>
                {
                    _transmitterListView.UpdateActorIdList(value.TransmitterId, value.ActorIds);
                })
                .AddTo(_compositeDisposable);
        }
    }
}
