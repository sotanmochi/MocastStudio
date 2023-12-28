using MocastStudio.Universal.UIView;
using MocastStudio.Universal.UIView.About;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MocastStudio.Universal.Lifecycle
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
            builder.Register<UIViewContext>(Lifetime.Singleton);

            builder.RegisterComponent(_systemMenuView);
            builder.RegisterComponent(_aboutView);
            builder.RegisterComponent(_acknowledgementsView);

            builder.RegisterEntryPoint<SystemMenuPresenter>();
            builder.RegisterEntryPoint<AboutPresenter>();
        }
    }
}
