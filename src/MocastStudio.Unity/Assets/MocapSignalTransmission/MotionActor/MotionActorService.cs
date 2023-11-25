using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MocapSignalTransmission.MotionDataSource;
using Debug = UnityEngine.Debug;

namespace MocapSignalTransmission.MotionActor
{
    public sealed class MotionActorService
    {
        private readonly MotionActorServiceContext _context;
        private readonly IMotionActorFactory _motionActorFactory;

        private int _humanoidMotionActorCount;

        public MotionActorService(MotionActorServiceContext context, IMotionActorFactory motionActorFactory)
        {
            _context = context;
            _motionActorFactory = motionActorFactory;
        }

        public void UpdateMotionActorPose(
            IReadOnlyList<IBodyTrackingDataSource> bodyTrackingDataSources,
            IReadOnlyList<IFingerTrackingDataSource> fingerTrackingDataSources)
        {
            // NOTE: Avoid boxing allocation that occur when using foreach.
            for (var dataSourceIndex = 0; dataSourceIndex < bodyTrackingDataSources.Count; dataSourceIndex++)
            {
                var bodyTrackingDataSource = bodyTrackingDataSources[dataSourceIndex];

                if (bodyTrackingDataSource != null &&
                    bodyTrackingDataSource.TryPeek(out var bodyTrackingFrame) &&
                    _context._bodyTrackingActorIds.ContainsKey(bodyTrackingDataSource.Id))
                {
                    var bodyTrackingActorIds = _context._bodyTrackingActorIds[bodyTrackingDataSource.Id];
                    for (var index = 0; index < bodyTrackingActorIds.Count; index++)
                    {
                        _context.BodyTrackingActors[bodyTrackingActorIds[index]]?.UpdatePose(bodyTrackingFrame);
                    }
                }
            }

            // NOTE: Avoid boxing allocation that occur when using foreach.
            for (var dataSourceIndex = 0; dataSourceIndex < fingerTrackingDataSources.Count; dataSourceIndex++)
            {
                var fingerTrackingDataSource = fingerTrackingDataSources[dataSourceIndex];

                if (fingerTrackingDataSource != null &&
                    fingerTrackingDataSource.TryPeek(out var fingerTrackingFrame) &&
                    _context._fingerTrackingActorIds.ContainsKey(fingerTrackingDataSource.Id))
                {
                    var fingerTrackingActorIds = _context._fingerTrackingActorIds[fingerTrackingDataSource.Id];
                    for (var index = 0; index < fingerTrackingActorIds.Count; index++)
                    {
                        _context.FingerTrackingActors[fingerTrackingActorIds[index]]?.UpdatePose(fingerTrackingFrame);
                    }
                }
            }

            foreach (var actor in _context.HumanoidMotionActors)
            {
                actor.UpdateHumanPose();
            }
        }

        public async Task<HumanoidMotionActor> AddHumanoidMotionActorAsync(string resourcePath)
        {
            var actorId = Interlocked.Increment(ref _humanoidMotionActorCount) - 1;

            var motionActor = await _motionActorFactory.CreateAsync(actorId, resourcePath);
            if (motionActor is HumanoidMotionActor humanoidMotionActor)
            {
                _context._bodyTrackingActors.Add(humanoidMotionActor.BodyTrackingActorBehaviour);
                _context._fingerTrackingActors.Add(humanoidMotionActor.FingerTrackingActorBehaviour);
                _context._humanoidMotionActors.Add(humanoidMotionActor);
                _context._motionActorAddedEventPublisher.Publish(humanoidMotionActor);
                return humanoidMotionActor;
            }
            else
            {
                Debug.LogError($"[{nameof(MotionActorService)}] The motion actor is not humanoid. Resource path: {resourcePath}");
                // TODO: Add comment
                _context._bodyTrackingActors.Add(null);
                _context._fingerTrackingActors.Add(null);
                _context._humanoidMotionActors.Add(null);
                return null;
            }
        }

        public bool TryAddDataSourceMapping(int actorId, int dataSourceId)
        {
            foreach (var value in _context._motionActorDataSourcePairs)
            {
                if (value.ActorId == actorId)
                {
                    Debug.Log($"[{nameof(MotionActorService)}] Actor[{actorId}] is already mapped to DataSource[{dataSourceId}].");
                    return false;
                }
            }

            // Body tracking
            if (_context._bodyTrackingActorIds.ContainsKey(dataSourceId))
            {
                if (!_context._bodyTrackingActorIds[dataSourceId].Contains(actorId))
                {
                    _context._bodyTrackingActorIds[dataSourceId].Add(actorId);
                    _context._bodyTrackingActorIds[dataSourceId].Sort();
                }
            }
            else
            {
                _context._bodyTrackingActorIds.Add(dataSourceId, new List<int> { actorId });
            }

            // Finger tracking
            if (_context._fingerTrackingActorIds.ContainsKey(dataSourceId))
            {
                if (!_context._fingerTrackingActorIds[dataSourceId].Contains(actorId))
                {
                    _context._fingerTrackingActorIds[dataSourceId].Add(actorId);
                    _context._fingerTrackingActorIds[dataSourceId].Sort();
                }
            }
            else
            {
                _context._fingerTrackingActorIds.Add(dataSourceId, new List<int> { actorId });
            }

            // Data source mapping
            _context._motionActorDataSourcePairs.Add((actorId, dataSourceId));
            _context._dataSourceMappingUpdatedEventPublisher.Publish(_context._motionActorDataSourcePairs);

            return true;
        }

        public void RemoveDataSourceMapping(int actorId, int dataSourceId)
        {
            _context._bodyTrackingActorIds[dataSourceId].Remove(actorId);
            _context._fingerTrackingActorIds[dataSourceId].Remove(actorId);
            _context._motionActorDataSourcePairs.Remove((actorId, dataSourceId));
            _context._dataSourceMappingUpdatedEventPublisher.Publish(_context._motionActorDataSourcePairs);
        }
    }
}
