using System;
using System.Collections.Generic;
using MocapSignalTransmission.Transmitter;
using MocapSignalTransmission.Infrastructure.Constants;
using MocapSignalTransmission.Infrastructure.Transmitter.Serialization;
using MocapSignalTransmission.Infrastructure.Transmitter.Transport;
#if SIGNAL_STREAMING
using SignalStreaming;
#endif

namespace MocapSignalTransmission.Infrastructure.Transmitter
{
    public sealed class TransmitterFactory : ITransmitterFactory, IDisposable
    {
        private readonly Dictionary<int, ISerializer> _serializers = new Dictionary<int, ISerializer>();
        private readonly Dictionary<int, ITransport> _transports = new Dictionary<int, ITransport>();

#if SIGNAL_STREAMING
        private readonly ISignalStreamingClientFactory _signalStreamingClientFactory;

        public TransmitterFactory(ISignalStreamingClientFactory signalStreamingClientFactory)
        {
            _signalStreamingClientFactory = signalStreamingClientFactory;
        }
#endif

        public void Dispose()
        {
            foreach (var transport in _transports.Values)
            {
                transport.Dispose();
            }
            _transports.Clear();
            _serializers.Clear();
        }

        public ITransmitter Create(int transmitterId, TransmitterSettings settings)
        {
            if (TryGetTransport(settings, out var transport))
            {
                _transports[transmitterId] = transport;
            }
            else
            {
                if (settings.TransportType == (int)TransportType.Udp)
                {
                    _transports[transmitterId] = new UdpTransport(settings.ServerAddress, settings.Port);
                }
#if SIGNAL_STREAMING
                else if (settings.TransportType == (int)TransportType.SignalStreaming_ENet)
                {
                    var signalStreamingClient = _signalStreamingClientFactory.Create(settings.TransportType);
                    var connectParameters = _signalStreamingClientFactory.CreateConnectParameters(settings.TransportType, settings.ServerAddress, (ushort)settings.Port);
                    _transports[transmitterId] = new SignalStreamingTransportAdapter(signalStreamingClient, connectParameters, (int)SignalType.MotionCaptureData);
                }
#endif
            }

            if (TryGetSerializer(settings, out var serializer))
            {
                _serializers[transmitterId] = serializer;
            }
            else
            {
                if (settings.SerializerType == (int)SerializerType.HumanPose_OSC)
                {
                    _serializers[transmitterId] = new HumanPoseOscSerializer()
                    {
                        MessageAddress = OscMessageAddress.HumanPose
                    };
                }
                else if (settings.SerializerType == (int)SerializerType.HumanPose_MessagePack)
                {
                    _serializers[transmitterId] = new HumanPoseMessagePackSerializer();
                }
            }

            return new UnityHumanPoseTransmitter(transmitterId, _serializers[transmitterId], _transports[transmitterId]);
        }

        public bool TryGetTransport(TransmitterSettings settings, out ITransport output)
        {
            var serverAddress = settings.ServerAddress;
            var port = settings.Port;

            if (settings.TransportType == (int)TransportType.Udp)
            {
                foreach (var transport in _transports.Values)
                {
                    if (transport is UdpTransport udpTransport)
                    {
                        if (udpTransport.ServerHost == serverAddress &&
                            udpTransport.ServerPort == port)
                        {
                            output = udpTransport;
                            return true;
                        }
                    }
                }
            }

            output = null;
            return false;
        }

        public bool TryGetSerializer(TransmitterSettings settings, out ISerializer output)
        {
            if (settings.SerializerType == (int)SerializerType.HumanPose_OSC)
            {
                foreach (var serializer in _serializers.Values)
                {
                    if (serializer is HumanPoseOscSerializer oscSerializer)
                    {
                        if (oscSerializer.MessageAddress == OscMessageAddress.HumanPose)
                        {
                            output = oscSerializer;
                            return true;
                        }
                    }
                }
            }

            output = null;
            return false;
        }
    }
}
