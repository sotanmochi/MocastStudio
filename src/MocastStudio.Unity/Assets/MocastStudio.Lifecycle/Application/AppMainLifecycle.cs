using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace MocastStudio.Application.Lifecycle
{
    public sealed class AppMainLifecycle : IInitializable
    {
        readonly AppMain _appMain;

        public AppMainLifecycle(AppMain appMain)
        {
            _appMain = appMain;
        }

        void IInitializable.Initialize()
        {
            _appMain.InitializeAsync().Forget();
        }
    }
}
