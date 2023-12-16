using System;
using MessagePipe;
using MocapSignalTransmission.Infrastructure.Constants;
using MocapSignalTransmission.Transmitter;
using UniRx;
using VContainer.Unity;

namespace MocastStudio.Universal.UIView.Transmitter
{
    public sealed class TransmitterPresenter : IInitializable, IDisposable
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

        void IDisposable.Dispose()
        {
            _compositeDisposable.Dispose();
        }

        void IInitializable.Initialize()
        {
            UnityEngine.Debug.Log($"<color=lime>[{nameof(TransmitterPresenter)}] Initialize</color>");

            _transmitterLoaderView.UpdateTransmitterTypeDropdown(new TransmitterType[]
            {
                TransmitterType.HumanPose_OSC_Local,
            });

            _transmitterLoaderView.OnAdditionRequested
                .Subscribe(settingsRequest =>
                {
                    var serializerType = settingsRequest.TransmitterType switch 
                    {
                        TransmitterType.HumanPose_OSC_Local => (int)SerializerType.HumanPose_OSC,
                        _ => throw new ArgumentException($"Unknown TransmitterType: {settingsRequest.TransmitterType}"),
                    };

                    var transportType = settingsRequest.TransmitterType switch 
                    {
                        TransmitterType.HumanPose_OSC_Local => (int)TransportType.Udp,
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
                    _transmitterListView.UpdateStatus(status.TransmitterId, status.StatusType);
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
