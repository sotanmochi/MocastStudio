using System;
using MessagePipe;
using MocastStudio.Samples.Receiver.Domain.MotionActor;
using MocastStudio.Samples.Receiver.Infrastructure.StreamingReceiver;
using UnityEngine;
using VContainer.Unity;

namespace MocastStudio.Samples.Receiver.Application
{
    /// <summary>
    /// Entry point
    /// </summary>
    public sealed class AppMain : IInitializable, IDisposable
    {
        private readonly MotionActorService _motionActorService;
        private readonly HumanPoseStreamingReceiver _streamingReceiver;
        private readonly ISubscriber<HumanPose> _humanPoseSubscriber;

        private int _actorId;
        private IDisposable _disposable;

        public AppMain(
            MotionActorService motionActorService,
            HumanPoseStreamingReceiver streamingReceiver,
            ISubscriber<HumanPose> humanPoseSubscriber)
        {
            _motionActorService = motionActorService;
            _streamingReceiver = streamingReceiver;
            _humanPoseSubscriber = humanPoseSubscriber;
        }

        public void Dispose()
        {
            _disposable?.Dispose();
            _streamingReceiver.Stop();
            Debug.Log($"[{nameof(AppMain)}] Disposed");
        }

        void IInitializable.Initialize()
        {
            _streamingReceiver.Start();

            _disposable = _humanPoseSubscriber.Subscribe(humanPose =>
                _motionActorService.UpdateMotionActorPose(_actorId, ref humanPose));

            Debug.Log($"[{nameof(AppMain)}] Initialized");
        }
    }
}
