using System;
using UniRx;
using VContainer.Unity;

namespace MocastStudio.Presentation.UIView
{
    public sealed class SystemMenuPresenter : IDisposable, IInitializable
    {
        readonly UIViewContext _context;
        readonly SystemMenuView _menuView;
        readonly CompositeDisposable _compositeDisposable = new();

        public SystemMenuPresenter(UIViewContext context, SystemMenuView menuView)
        {
            _context = context;
            _menuView = menuView;
        }

        void IInitializable.Initialize()
        {
            _menuView.OnItemSelected
                .Subscribe(UpdateViewStatus)
                .AddTo(_compositeDisposable);
        }

        void IDisposable.Dispose()
        {
            _compositeDisposable.Dispose();
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
