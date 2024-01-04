using System;
using MessagePipe;
using MocastStudio.Presentation.UIView;
using UniRx;
using VContainer.Unity;

namespace MocastStudio.Presentation.CameraSystem
{
    public sealed class CameraSystemPresenter : IDisposable, IInitializable
    {
        readonly UIViewContext _context;
        readonly CameraSwitcher _cameraSwitcher;
        readonly CompositeDisposable _compositeDisposable = new();

        public CameraSystemPresenter(UIViewContext context, CameraSwitcher view)
        {
            _context = context;
            _cameraSwitcher = view;
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
        }
    }
}