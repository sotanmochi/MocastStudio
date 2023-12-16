using System;
using UniRx;
using VContainer.Unity;

namespace MocastStudio.Universal.UIView
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
                .Subscribe(uiViewType =>
                {
                    _context.UpdateViewStatus(uiViewType, UIViewStatusType.Visible);
                })
                .AddTo(_compositeDisposable);
        }

        void IDisposable.Dispose()
        {
            _compositeDisposable.Dispose();
        }
    }
}
