using System;
using System.Collections.Generic;
using MessagePipe;

namespace MocapSignalTransmission.Transmitter
{
    public sealed class TransmitterServiceContext : IDisposable
    {
        internal readonly IDisposablePublisher<TransmitterSettings> _transmitterAddedEventPublisher;
        internal readonly IDisposablePublisher<TransmitterStatus> _transmitterStatusUpdatedEventPublisher;
        internal readonly IDisposablePublisher<(int TransmitterId, IReadOnlyCollection<int> ActorIds)> _transmitterActorIdUpdatedEventPublisher;

        internal readonly List<TransmitterSettings> _transmitterSettingsList = new();
        internal readonly Dictionary<int, ITransmitter> _transmitters = new();

        public ISubscriber<TransmitterSettings> OnTransmitterAdded;
        public ISubscriber<TransmitterStatus> OnTransmitterStatusUpdated;
        public ISubscriber<(int TransmitterId, IReadOnlyCollection<int> ActorIds)> OnTransmitterActorIdUpdated;

        public IReadOnlyList<TransmitterSettings> TransmitterSettingsList => _transmitterSettingsList;

        public TransmitterServiceContext(EventFactory eventFactory)
        {
            (_transmitterAddedEventPublisher, OnTransmitterAdded) = eventFactory.CreateEvent<TransmitterSettings>();
            (_transmitterStatusUpdatedEventPublisher, OnTransmitterStatusUpdated) = eventFactory.CreateEvent<TransmitterStatus>();
            (_transmitterActorIdUpdatedEventPublisher, OnTransmitterActorIdUpdated) = eventFactory.CreateEvent<(int TransmitterId, IReadOnlyCollection<int> ActorIds)>();
        }

        public void Dispose()
        {
            _transmitterAddedEventPublisher.Dispose();
            _transmitterStatusUpdatedEventPublisher.Dispose();
            _transmitterActorIdUpdatedEventPublisher.Dispose();
            _transmitterSettingsList.Clear();
            _transmitters.Clear();
        }
    }
}
