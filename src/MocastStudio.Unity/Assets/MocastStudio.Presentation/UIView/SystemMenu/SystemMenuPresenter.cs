using System;
using UniRx;

namespace MocastStudio.Presentation.UIView
{
    public sealed class SystemMenuPresenter : IDisposable
    {
        readonly UIViewContext _context;
        readonly SystemMenuView _menuView;
        readonly CompositeDisposable _compositeDisposable = new();

        public SystemMenuPresenter(UIViewContext context, SystemMenuView menuView)
        {
            _context = context;
            _menuView = menuView;
        }

        public void Dispose()
        {
            _compositeDisposable.Dispose();
        }

        public void Initialize()
        {
            _menuView.OnItemSelected
                .Subscribe(UpdateViewStatus)
                .AddTo(_compositeDisposable);
        }

        void UpdateViewStatus(UIViewType uiViewType)
        {
            if (uiViewType == UIViewType.MainCamera)
            {
                _context.UpdateViewStatus(UIViewType.MainCamera, UIViewStatusType.Visible);

                _context.UpdateViewStatus(UIViewType.About, UIViewStatusType.Invisible);
                _context.UpdateViewStatus(UIViewType.Acknowledgements, UIViewStatusType.Invisible);
                _context.UpdateViewStatus(UIViewType.MotionCaptureSystem, UIViewStatusType.Invisible);
            }
            else
            {
                _context.UpdateViewStatus(uiViewType, UIViewStatusType.Visible);
            }
        }
    }
}
