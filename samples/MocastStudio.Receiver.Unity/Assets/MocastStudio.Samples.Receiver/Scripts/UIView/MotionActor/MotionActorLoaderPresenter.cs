using System;
using MocastStudio.Samples.Receiver.Domain.MotionActor;
using UniRx;
using VContainer.Unity;

namespace MocastStudio.Samples.Receiver.UIView
{
    public sealed class MotionActorLoaderPresenter : IInitializable, IDisposable
    {
        private readonly MotionActorService _motionActorService;
        private readonly MotionActorLoaderUIView _motionActorLoaderUIView;
        private readonly CompositeDisposable _compositeDisposable = new();

        public MotionActorLoaderPresenter(
            MotionActorService motionActorService,
            MotionActorLoaderUIView motionActorLoaderUIView)
        {
            _motionActorService = motionActorService;
            _motionActorLoaderUIView = motionActorLoaderUIView;
        }

        void IDisposable.Dispose() => _compositeDisposable.Dispose();

        void IInitializable.Initialize()
        {
            _motionActorLoaderUIView.OnLoadingRequested
                .Subscribe(async resourcePath =>
                {
                    await _motionActorService.AddHumanoidMotionActorAsync(resourcePath);
                })
                .AddTo(_compositeDisposable);

            UnityEngine.Debug.Log($"<color=lime>[{nameof(MotionActorLoaderPresenter)}] Initialize</color>");
        }
    }
}
