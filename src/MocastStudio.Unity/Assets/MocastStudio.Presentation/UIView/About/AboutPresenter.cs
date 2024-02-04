using System;
using MessagePipe;
using R3;

namespace MocastStudio.Presentation.UIView.About
{
    public sealed class AboutPresenter : IDisposable
    {
        readonly UIViewContext _context;
        readonly AboutView _aboutView;
        readonly AcknowledgementsView _acknowledgementsView;
        readonly CompositeDisposable _compositeDisposable = new();

        public AboutPresenter(UIViewContext context, AboutView aboutView, AcknowledgementsView acknowledgementsView)
        {
            _context = context;
            _aboutView = aboutView;
            _acknowledgementsView = acknowledgementsView;
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
                    if (status.ViewType == UIViewType.About)
                    {
                        var visible = status.StatusType == UIViewStatusType.Visible;
                        _aboutView.gameObject.SetActive(visible);
                    }
                    else if (status.ViewType == UIViewType.Acknowledgements)
                    {
                        var visible = status.StatusType == UIViewStatusType.Visible;
                        _acknowledgementsView.gameObject.SetActive(visible);
                    }
                })
                .AddTo(_compositeDisposable);

            _aboutView.OnOpenAcknowledgements
                .Subscribe(_ =>
                {
                    _context.UpdateViewStatus(UIViewType.Acknowledgements, UIViewStatusType.Visible);
                })
                .AddTo(_compositeDisposable);

            _aboutView.OnClose
                .Subscribe(_ =>
                {
                    _context.UpdateViewStatus(UIViewType.About, UIViewStatusType.Invisible);
                })
                .AddTo(_compositeDisposable);
            
            _acknowledgementsView.OnClose
                .Subscribe(_ =>
                {
                    _context.UpdateViewStatus(UIViewType.Acknowledgements, UIViewStatusType.Invisible);
                })
                .AddTo(_compositeDisposable);
        }
    }
}