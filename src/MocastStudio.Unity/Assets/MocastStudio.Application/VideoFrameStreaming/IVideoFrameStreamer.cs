using UnityEngine;

namespace MocastStudio.Application.VideoFrameStreaming
{
    public interface IVideoFrameStreamer
    {
        string Name { get; set; }
        bool AlphaSupport { get; set; }
        Texture SourceTexture { get; set; }
        void SetEnable(bool enable);
    }
}
