using MocastStudio.Presentation.UIView;
using MocastStudio.Presentation.UIView.About;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MocastStudio.Presentation.Lifecycle
{
    public sealed class UIViewLifecycle : LifetimeScope
    {
        [Header("UIView Components")]
        [SerializeField] SystemMenuView _systemMenuView;
        [SerializeField] AboutView _aboutView;
        [SerializeField] AcknowledgementsView _acknowledgementsView;

        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log($"[{nameof(UIViewLifecycle)}] Configure");

            builder.RegisterEntryPoint<UIViewEntryPoint>();

            builder.RegisterComponent(_systemMenuView);
            builder.RegisterComponent(_aboutView);
            builder.RegisterComponent(_acknowledgementsView);

            builder.Register<SystemMenuPresenter>(Lifetime.Singleton);
            builder.Register<AboutPresenter>(Lifetime.Singleton);
        }
    }
}
