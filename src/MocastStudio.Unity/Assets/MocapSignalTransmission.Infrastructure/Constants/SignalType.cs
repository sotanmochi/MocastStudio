namespace MocapSignalTransmission.Infrastructure.Constants
{
    /// <summary>
    /// Message Ids for signal streaming.
    /// Some message IDs are reserved by the core module of SignalStreaming (ID: 250 ~ 255).
    /// </summary>
    public enum SignalType
    {
        MotionCaptureData = 249
    }
}
