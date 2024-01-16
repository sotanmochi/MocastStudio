using System;
using MessagePipe;
using UniRx;
using VContainer.Unity;

namespace MocastStudio.Presentation.UIView.MotionCapture
{
    public sealed class MotionCaptureSystemPresenter : IDisposable, IInitializable
    {
        readonly UIViewContext _context;
        readonly MotionCaptureSystemView _motionCaptureSystemView;
        readonly CompositeDisposable _compositeDisposable = new();

        public MotionCaptureSystemPresenter(UIViewContext context, MotionCaptureSystemView view)
        {
            _context = context;
            _motionCaptureSystemView = view;
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
                    if (status.ViewType == UIViewType.MotionCaptureSystem)
                    {
                        var visible = status.StatusType == UIViewStatusType.Visible;
                        _motionCaptureSystemView.gameObject.SetActive(visible);
                    }
                })
                .AddTo(_compositeDisposable);
        }
    }
}