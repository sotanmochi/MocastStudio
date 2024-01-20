using System;

namespace SignalStreaming
{
    public interface IGroup
    {
        Ulid Id { get; }
        string Name { get; }
        bool IsActive { get; }
    }
}
