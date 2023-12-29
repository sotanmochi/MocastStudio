using System;
using System.Collections.Generic;
using MessagePipe;

namespace MocapSignalTransmission.MotionActor
{
    public sealed class MotionActorServiceContext : IDisposable
    {
        internal readonly IDisposablePublisher<IMotionActor> _motionActorAddedEventPublisher;
        internal readonly IDisposablePublisher<IReadOnlyList<(int ActorId, int DataSourceId)>> _dataSourceMappingUpdatedEventPublisher;

        internal readonly List<HumanoidMotionActor> _humanoidMotionActors = new();
        internal readonly List<BodyTrackingActorBehaviour> _bodyTrackingActors = new();
        internal readonly List<FingerTrackingActorBehaviour> _fingerTrackingActors = new();

        internal readonly List<(int ActorId, int DataSourceId)> _motionActorDataSourcePairs = new();
        internal readonly Dictionary<int, List<int>> _bodyTrackingActorIds = new(); // Key: DataSourceId, Value: ActorIds
        internal readonly Dictionary<int, List<int>> _fingerTrackingActorIds = new(); // Key: DataSourceId, Value: ActorIds
        internal readonly Dictionary<int, List<int>> _humanPoseTrackingActorIds = new(); // Key: DataSourceId, Value: ActorIds

        public ISubscriber<IMotionActor> OnMotionActorAdded { get; }
        public ISubscriber<IReadOnlyList<(int ActorId, int DataSourceId)>> OnDataSourceMappingUpdated { get; }

        public IReadOnlyList<IMotionActor> MotionActors => _humanoidMotionActors;

        public IReadOnlyList<HumanoidMotionActor> HumanoidMotionActors => _humanoidMotionActors;
        public IReadOnlyList<BodyTrackingActorBehaviour> BodyTrackingActors => _bodyTrackingActors;
        public IReadOnlyList<FingerTrackingActorBehaviour> FingerTrackingActors => _fingerTrackingActors;

        public MotionActorServiceContext(EventFactory eventFactory)
        {
            (_motionActorAddedEventPublisher, OnMotionActorAdded) = eventFactory.CreateEvent<IMotionActor>();
            (_dataSourceMappingUpdatedEventPublisher, OnDataSourceMappingUpdated) = eventFactory.CreateEvent<IReadOnlyList<(int ActorId, int DataSourceId)>>();
        }

        public void Dispose()
        {
            _motionActorAddedEventPublisher.Dispose();
            _dataSourceMappingUpdatedEventPublisher.Dispose();

            _humanoidMotionActors.Clear();
            _bodyTrackingActors.Clear();
            _fingerTrackingActors.Clear();

            _motionActorDataSourcePairs.Clear();
            _bodyTrackingActorIds.Clear();
            _fingerTrackingActorIds.Clear();
            _humanPoseTrackingActorIds.Clear();
        }
    }
}
