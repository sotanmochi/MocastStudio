using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MocastStudio.Samples.Receiver.Domain.MotionActor
{
    public sealed class MotionActorService : IDisposable
    {
        private readonly IMotionActorFactory _motionActorFactory;
        private readonly List<HumanoidMotionActor> _humanoidMotionActors = new();

        public MotionActorService(IMotionActorFactory motionActorFactory)
        {
            _motionActorFactory = motionActorFactory;
        }

        public void Dispose()
        {
            foreach (var actor in _humanoidMotionActors)
            {
                actor.Dispose();
            }
            _humanoidMotionActors.Clear();
        }

        public async Task AddHumanoidMotionActorAsync(string resourcePath, CancellationToken cancellationToken = default)
        {
            var motionActor = await _motionActorFactory.CreateAsync(resourcePath, cancellationToken);
            _humanoidMotionActors.Add(motionActor);
        }
    }
}
