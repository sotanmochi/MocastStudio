using MocastStudio.Presentation.UIView;
using MocastStudio.Presentation.UIView.About;
using VContainer;
using VContainer.Unity;

namespace MocastStudio.Lifecycle.Presentation
{
    public sealed class UIViewLifecycle : IInitializable, IStartable
    {
        readonly UIViewContext _context;
        readonly SystemMenuPresenter _systemMenuPresenter;
        readonly AboutPresenter _aboutPresenter;

        public UIViewLifecycle(
            UIViewContext contxt,
            SystemMenuPresenter systemMenuPresenter,
            AboutPresenter aboutPresenter)
        {
            _context = contxt;
            _systemMenuPresenter = systemMenuPresenter;
            _aboutPresenter = aboutPresenter;
        }

        void IInitializable.Initialize()
        {
            _systemMenuPresenter.Initialize();
            _aboutPresenter.Initialize();
        }
 
        void IStartable.Start()
        {
            _context.UpdateViewStatus(UIViewType.MotionCaptureSystem, UIViewStatusType.Visible);

            _context.UpdateViewStatus(UIViewType.MainCamera, UIViewStatusType.Invisible);
            _context.UpdateViewStatus(UIViewType.About, UIViewStatusType.Invisible);
            _context.UpdateViewStatus(UIViewType.Acknowledgements, UIViewStatusType.Invisible);
        }
    }
}
