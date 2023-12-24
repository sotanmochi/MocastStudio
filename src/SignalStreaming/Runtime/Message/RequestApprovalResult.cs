using MessagePack;

namespace SignalStreaming
{
    [MessagePackObject]
    public struct RequestApprovalResult
    {
        [Key(0)]
        public readonly bool Approved;
        [Key(1)]
        public readonly string Message;

        public RequestApprovalResult(bool approved, string message)
        {
            Approved = approved;
            Message = message;
        }
    }
}
