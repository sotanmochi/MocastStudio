using System;
using Animator = UnityEngine.Animator;
using Transform = UnityEngine.Transform;

namespace MocapSignalTransmission.Infrastructure.MotionActor
{
    public interface ICharacterAvatarResource : IDisposable
    {
        string Name { get; }
        Transform RootTransform { get; }
        Animator Animator { get; }
    }
}
