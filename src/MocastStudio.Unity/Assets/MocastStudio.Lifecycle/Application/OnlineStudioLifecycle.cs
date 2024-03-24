using MocastStudio.Application.OnlineStudio;
using SignalStreaming;
using VContainer.Unity;

namespace MocastStudio.Lifecycle
{
    public sealed class OnlineStudioLifecycle : IInitializable, ITickable
    {
        readonly ConnectionService _connectionService;
        readonly ISignalTransport _signalStreamingTransport;

        public OnlineStudioLifecycle(
            ConnectionService connectionService,
            ISignalTransport signalStreamingTransport)
        {
            _connectionService = connectionService;
            _signalStreamingTransport = signalStreamingTransport;
        }

        void IInitializable.Initialize()
        {
            _connectionService.Initialize();
        }

        void ITickable.Tick()
        {
            _signalStreamingTransport.DequeueIncomingSignals();
        }
    }
}