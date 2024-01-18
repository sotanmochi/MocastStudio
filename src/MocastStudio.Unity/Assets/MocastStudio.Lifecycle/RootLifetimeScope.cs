using MessagePipe;
using VContainer;
using VContainer.Unity;
using UIViewContext = MocastStudio.Presentation.UIView.UIViewContext;

namespace MocastStudio.Lifecycle
{
    public class RootLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            UnityEngine.Debug.Log($"[{nameof(RootLifetimeScope)}] Configure");
            builder.RegisterMessagePipe();
            builder.Register<UIViewContext>(Lifetime.Singleton);
        }
    }
}
