using System;
using MessagePipe;
using MocastStudio.Application.VideoFrameStreaming;
using MocastStudio.Presentation.UIView;
using R3;
using VContainer.Unity;

namespace MocastStudio.Presentation.CameraSystem
{
    public sealed class CameraSystemPresenter : IDisposable, IInitializable
    {
        readonly UIViewContext _context;
        readonly CameraSwitcher _cameraSwitcher;
        readonly IVideoFrameStreamer _videoFrameStreamer;
        readonly CompositeDisposable _compositeDisposable = new();

        public CameraSystemPresenter(UIViewContext context, CameraSwitcher view, IVideoFrameStreamer videoFrameStreamer)
        {
            _context = context;
            _cameraSwitcher = view;
            _videoFrameStreamer = videoFrameStreamer;
        }

        void IDisposable.Dispose()
        {
            _compositeDisposable.Dispose();
        }

        void IInitializable.Initialize()
        {
            _context.OnStatusUpdated
                .Subscribe(status =>
                {
                    if (status.ViewType == UIViewType.MainCamera
                    && status.StatusType == UIViewStatusType.Visible)
                    {
                        _cameraSwitcher.SwitchToMainCamera();
                    }
                    else if (status.ViewType == UIViewType.MotionCaptureSystem
                    && status.StatusType == UIViewStatusType.Visible)
                    {
                        var visible = status.StatusType == UIViewStatusType.Visible;
                        _cameraSwitcher.SwitchToSceneCamera();
                    }
                })
                .AddTo(_compositeDisposable);

            _cameraSwitcher.Initialize();
            _videoFrameStreamer.SourceTexture = _cameraSwitcher.MainRenderTexture;
            _videoFrameStreamer.SetEnable(true);
        }
    }
}