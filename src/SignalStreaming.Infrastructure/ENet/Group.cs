using System;
using ENet;

namespace SignalStreaming.Infrastructure.ENet
{
    public class Group : IGroup
    {
        public Ulid Id { get; internal set; }
        public string Name { get; internal set; }
        public bool IsActive { get; internal set; }
        public Peer[] Clients { get; internal set; }
    }
}
