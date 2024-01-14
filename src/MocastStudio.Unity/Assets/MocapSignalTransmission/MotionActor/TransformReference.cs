using UnityEngine;

namespace MocapSignalTransmission.MotionActor
{
    public sealed class TransformReference
    {
        public string Name { get; }
        public Transform Transform { get; }

        public TransformReference(string name, Transform transform)
        {
            Name = name;
            Transform = transform;
        }
    }
}
