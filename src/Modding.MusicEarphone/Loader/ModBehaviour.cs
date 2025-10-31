using Modding.Core;
using Modding.Core.PluginLoader;
using Modding.MusicEarphone.Patches;

namespace Modding.MusicEarphone
{
    public class ModBehaviour : ModBehaviourBase
    {
        protected override string PluginId => PluginCore.BepinExUuid;
        protected override string PluginName => PluginCore.PluginName;

        public override void InitializeLogger()
        {
            PluginCore.ModLogger = ModLogger.Initialize<PluginCore>(LoadingMode.None, PluginCore.PluginName);
            ModLogger = PluginCore.ModLogger;
        }

        /// <summary>
        ///     启用脚本时调用
        /// </summary>
        public override void OnEnable()
        {
            if (!PluginCore.IsPatched && PluginCore.InitDependency())
            {
                Harmony.PatchAll();
                ModLogger.LogInformation("mod is enabled by offical plugin");
                PluginCore.ToggleEvent(true);
                ModLogger.LogInformation("event handler enabled!");
                PluginCore.IsPatched = true;
            }
        }

        /// <summary>
        ///     禁用脚本时调用
        /// </summary>
        protected override void BeforeUnpatching()
        {
            PluginCore.ToggleEvent(false);
            PluginCore.IsPatched = false;
            ModLogger.LogInformation("event handler disabled!");
        }
    }
}
