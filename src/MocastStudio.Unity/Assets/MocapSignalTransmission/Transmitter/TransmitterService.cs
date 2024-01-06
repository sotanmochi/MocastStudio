using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MocapSignalTransmission.MotionActor;
using Debug = UnityEngine.Debug;

namespace MocapSignalTransmission.Transmitter
{
    public sealed class TransmitterService : IDisposable
    {
        private readonly TransmitterServiceContext _context;
        private readonly ITransmitterFactory _transmitterFactory;
        private readonly ConcurrentDictionary<int, CancellationTokenSource> _cancellationTokenSources = new();

        private int _transmitterCount;

        public TransmitterService(TransmitterServiceContext context, ITransmitterFactory transmitterFactory)
        {
            _context = context;
            _transmitterFactory = transmitterFactory;
        }

        public void Dispose()
        {
            foreach (var cts in _cancellationTokenSources.Values)
            {
                cts.Cancel();
            }
            _cancellationTokenSources.Clear();
        }

        public void SendMotionActorPose(IReadOnlyList<IMotionActor> motionActors)
        {
            var transmitters = _context._transmitters.Values;

            // NOTE: Avoid boxing allocation that occur when using foreach.
            for (var index = 0; index < motionActors.Count; index++)
            {
                foreach (var transmitter in transmitters)
                {
                    transmitter.Send(motionActors[index]);
                }
            }
        }

        public int AddTransmitter(TransmitterSettings settings)
        {
            var id = Interlocked.Increment(ref _transmitterCount) - 1;
            settings.TransmitterId = id;

            var transmitter = _transmitterFactory.Create(id, settings);

            _context._transmitters.Add(id, transmitter);
            _context._transmitterSettingsList.Add(settings);

            _context._transmitterAddedEventPublisher.Publish(settings);

            return id;
        }

        public void AddActorIds(int transmitterId, IReadOnlyList<int> actorIds)
        {
            if (_context._transmitters.TryGetValue(transmitterId, out var transmitter))
            { 
                // NOTE: Avoid boxing allocation that occur when using foreach.
                for (var i = 0; i < actorIds.Count; i++)
                {
                    transmitter.AddActorId(actorIds[i]);
                }

                _context._transmitterActorIdUpdatedEventPublisher.Publish((transmitterId, transmitter.ActorIds));
            }
        }

        public async Task<bool> ConnectAsync(int transmitterId)
        {
            var connected = false;

            if (_context._transmitters.TryGetValue(transmitterId, out var transmitter))
            {
                _context._transmitterStatusUpdatedEventPublisher.Publish(new TransmitterStatus(transmitterId, TransmitterStatusType.Connecting));

                if (_cancellationTokenSources.TryRemove(transmitterId, out var cts))
                {
                    Debug.Log($"[{nameof(TransmitterService)}] Cancel connection task of transmitter[{transmitterId}]");
                    cts.Cancel();
                }

                var cancellationTokenSource = new CancellationTokenSource();
                _cancellationTokenSources.TryAdd(transmitterId, cancellationTokenSource);

                connected = await transmitter.ConnectAsync(cancellationTokenSource.Token);
                _cancellationTokenSources.TryRemove(transmitterId, out var _);

                var statusType = connected ? TransmitterStatusType.Connected : TransmitterStatusType.Disconnected;
                _context._transmitterStatusUpdatedEventPublisher.Publish(new TransmitterStatus(transmitterId, statusType));
            }

            return connected;
        }

        public async Task DisconnectAsync(int transmitterId)
        {
            if (_context._transmitters.TryGetValue(transmitterId, out var transmitter))
            {
                if (_cancellationTokenSources.TryRemove(transmitterId, out var cts))
                {
                    Debug.Log($"[{nameof(TransmitterService)}] Cancel connection task of transmitter [{transmitterId}]");
                    cts.Cancel();
                }

                await transmitter.DisconnectAsync();

                _context._transmitterStatusUpdatedEventPublisher.Publish(new TransmitterStatus(transmitterId, TransmitterStatusType.Disconnected));
            }
        }

        public static uint GetStreamingActorId(uint transmitterId, byte localActorId)
        {
            var maxLocalActors = MotionActorService.MaxLocalActors;
            return (uint)(localActorId % maxLocalActors + transmitterId * maxLocalActors);
        }
    }
}
