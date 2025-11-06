using BepInEx;
using Modding.Core;
using Modding.Core.PluginLoader;

namespace Modding.ModOption
{
    [BepInPlugin(PluginCore.BepinExUuid, "Music Earphone", "1.0.0")]
    //[BepInDependency("com.bepinex.plugin.important")]
    public class BepinExPlugin : BepInExBase
    {
        protected override string PluginId => PluginCore.BepinExUuid;
        protected override string PluginName => PluginCore.PluginName;

        public override void InitializeLogger()
        {
            PluginCore.ModLogger = ModLogger.Initialize<PluginCore>(LoadingMode.BepInEx, PluginCore.PluginName);
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
                ModLogger.LogInformation("mod is enabled by bepinex");
                PluginCore.ToggleEvent(true);
                ModLogger.LogInformation("event handler enabled!");
                PluginCore.IsPatched = true;
            }
        }

        /// <summary>
        ///     禁用脚本时调用
        /// </summary>
        public override void OnDisable()
        {
            //MusicEarphonePatch.ToggleEvent(false);
            //MusicEarphonePatch.IsPatched = false;
            ModLogger.LogInformation("event handler disabled!");
        }
    }
}
