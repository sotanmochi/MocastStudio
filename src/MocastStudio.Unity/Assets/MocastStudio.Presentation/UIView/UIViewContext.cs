using System;
using System.Collections.Generic;
using MessagePipe;

namespace MocastStudio.Universal.UIView
{
    public sealed class UIViewContext : IDisposable
    {
        readonly IDisposablePublisher<UIViewStatus> _statusUpdateEventPublisher;
        readonly Dictionary<UIViewType, UIViewStatus> _status = new();

        public ISubscriber<UIViewStatus> OnStatusUpdated { get; }

        public UIViewContext(EventFactory eventFactory)
        {
            (_statusUpdateEventPublisher, OnStatusUpdated) = eventFactory.CreateEvent<UIViewStatus>();
        }

        void IDisposable.Dispose()
        {
            _statusUpdateEventPublisher.Dispose();
            _status.Clear();
        }

        public void UpdateViewStatus(UIViewType viewType, UIViewStatusType statusType)
        {
            _status[viewType] = new UIViewStatus(viewType, statusType);
            _statusUpdateEventPublisher.Publish(new UIViewStatus(viewType, statusType));
        }
    }
}
