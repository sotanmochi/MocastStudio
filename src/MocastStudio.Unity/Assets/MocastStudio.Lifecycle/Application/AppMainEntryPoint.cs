using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace MocastStudio.Application.Lifecycle
{
    public sealed class AppMainEntryPoint : IInitializable
    {
        readonly AppMain _appMain;

        public AppMainEntryPoint(AppMain appMain)
        {
            _appMain = appMain;
        }

        void IInitializable.Initialize()
        {
            _appMain.InitializeAsync().Forget();
        }
    }
}
