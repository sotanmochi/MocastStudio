using System;
using MessagePipe;
using MocapSignalTransmission.MotionActor;
using MocastStudio.Presentation.CameraSystem;
using TransformControl;
using VContainer;
using VContainer.Unity;
using Camera = UnityEngine.Camera;

namespace MocastStudio.Presentation.RuntimeGizmo
{
    public sealed class MotionActorReferencePointControl : IInitializable, IDisposable
    {
        readonly MotionActorServiceContext _motionActorServiceContext;
        readonly CameraSystemContext _cameraSystemContext;

        IDisposable _disposable;

        public MotionActorReferencePointControl(
            MotionActorServiceContext motionActorServiceContext,
            CameraSystemContext cameraSystemContext)
        {
            _motionActorServiceContext = motionActorServiceContext;
            _cameraSystemContext = cameraSystemContext;
        }

        void IDisposable.Dispose() => _disposable.Dispose();

        void IInitializable.Initialize()
        {
            var disposableBag = DisposableBag.CreateBuilder();

            _motionActorServiceContext.OnMotionActorAdded
                .Subscribe(motionActor =>
                {
                    if (motionActor is HumanoidMotionActor humanoidMotionActor)
                    {
                        var control = humanoidMotionActor.BodyTrackingActorBehaviour.gameObject.AddComponent<TransformController>();
                        control.renderTargetCamera = _cameraSystemContext.SceneViewCamera;
                        control.global = true;
                        control.useDistance = true;
                    }
                })
                .AddTo(disposableBag);

            _disposable = disposableBag.Build();

            UnityEngine.Debug.Log($"<color=lime>[{nameof(MotionActorReferencePointControl)}] Initialized</color>");
        }
    }
}
