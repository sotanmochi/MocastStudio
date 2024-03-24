using System;
using System.IO;
using MessagePipe;
using MocapSignalTransmission.BinaryDataProvider;
using MocapSignalTransmission.MotionActor;
using R3;

namespace MocastStudio.Presentation.UIView.MotionActor
{
    public sealed class MotionActorPresenter : IDisposable
    {
        readonly MotionActorService _motionActorService;
        readonly MotionActorServiceContext _motionActorServiceContext;
        readonly MotionActorListView _motionActorListView;
        readonly MotionActorLoaderView _motionActorLoaderView;
        readonly CompositeDisposable _compositeDisposable = new();

        public MotionActorPresenter(
            MotionActorService motionActorService,
            MotionActorServiceContext motionActorServiceContext,
            MotionActorListView motionActorListView,
            MotionActorLoaderView motionActorLoaderView)
        {
            _motionActorService = motionActorService;
            _motionActorServiceContext = motionActorServiceContext;
            _motionActorListView = motionActorListView;
            _motionActorLoaderView = motionActorLoaderView;
        }

        public void Dispose()
        {
            _compositeDisposable.Dispose();
        }

        public void Initialize()
        {
            UnityEngine.Debug.Log($"<color=lime>[{nameof(MotionActorPresenter)}] Initialize</color>");

            _motionActorLoaderView.OnLoadingRequested
                .Subscribe(parameters =>
                {
                    var directoryPath = Path.GetDirectoryName(parameters.ResourcePath);
                    var filename = Path.GetFileName(parameters.ResourcePath);
                    _motionActorService.AddHumanoidMotionActorAsync(new LocalFileLoadingRequest()
                    {
                        DirectoryPath = directoryPath,
                        Filename = filename,
                    });
                })
                .AddTo(_compositeDisposable);

            _motionActorListView.RootBoneOffsetStatus
                .Subscribe(status =>
                {
                    _motionActorService.EnableRootBoneOffset(status.ActorId, status.RootBoneOffsetEnabled);
                })
                .AddTo(_compositeDisposable);

            _motionActorServiceContext.OnMotionActorAdded
                .Subscribe(motionActor =>
                {
                    _motionActorListView.UpdateView(motionActor.ActorId, motionActor.Name);
                })
                .AddTo(_compositeDisposable);
        }
    }
}
