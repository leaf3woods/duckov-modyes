
namespace Modding.Core.PluginLoader
{
    public interface IPlugin
    {
        void Start();

        void OnEnable();

        void OnDisable();

        void OnDestroy();
    }
}
