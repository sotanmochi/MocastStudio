using System;
using MessagePipe;
using UniRx;

namespace MocastStudio.Presentation.UIView.MotionCapture
{
    public sealed class MotionCaptureSystemPresenter : IDisposable
    {
        readonly UIViewContext _context;
        readonly MotionCaptureSystemView _motionCaptureSystemView;
        readonly CompositeDisposable _compositeDisposable = new();

        public MotionCaptureSystemPresenter(UIViewContext context, MotionCaptureSystemView view)
        {
            _context = context;
            _motionCaptureSystemView = view;
        }

        public void Dispose()
        {
            _compositeDisposable.Dispose();
        }

        public void Initialize()
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