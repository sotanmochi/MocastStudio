using SignalStreaming;
using VContainer.Unity;

namespace MocastStudio.Lifecycle
{
    public sealed class OnlineStudioLifecycle : ITickable
    {
        readonly ISignalTransport _signalStreamingTransport;

        public OnlineStudioLifecycle(ISignalTransport signalStreamingTransport)
        {
            _signalStreamingTransport = signalStreamingTransport;
        }

        void ITickable.Tick()
        {
            _signalStreamingTransport.DequeueIncomingSignals();
        }
    }
}