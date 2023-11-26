using UniGLTF;
using UnityEngine;

namespace MocastStudio.Samples.Receiver.Infrastructure.MotionActor
{
    public sealed class VrmAvatar : ICharacterAvatarResource
    {
        private readonly RuntimeGltfInstance _instance;

        public string Name { get; }
        public Transform RootTransform { get; }
        public Animator Animator { get; }

        public VrmAvatar(RuntimeGltfInstance instance)
        {
            _instance = instance;
            Name = "VRM Avatar";
            RootTransform = instance.Root.transform;
            Animator = instance.GetComponent<Animator>();
        }

        public VrmAvatar(RuntimeGltfInstance instance, string name)
        {
            _instance = instance;
            Name = name;
            RootTransform = instance.Root.transform;
            Animator = instance.GetComponent<Animator>();
        }

        public void Dispose()
        {
            _instance.Dispose();
        }

        public void ShowMeshes()
        {
            _instance.ShowMeshes();
        }

        public void EnableUpdateWhenOffscreen()
        {
            _instance.EnableUpdateWhenOffscreen();
        }
    }
}
