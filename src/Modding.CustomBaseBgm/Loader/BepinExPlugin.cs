using BepInEx;
using Modding.Core;
using Modding.Core.PluginLoader;

namespace Modding.CustomBaseBgm
{
    [BepInPlugin(Util.BepinExUuid, "Custom Base BGM", "1.0.0")]
    //[BepInDependency("com.bepinex.plugin.important")]
    public class BepinExPlugin : BepInExBase
    {
        protected override string PluginId => Util.BepinExUuid;
        protected override string PluginName => Util.PluginName;

        public override void InitializeLogger()
        {
            BaseBgmPatch.ModLogger = ModLogger.Initialize<BaseBgmPatch>(LoadingMode.BepInEx, Util.PluginName);
            ModLogger = BaseBgmPatch.ModLogger;
        }

        /// <summary>
        ///     启用脚本时调用
        /// </summary>
        public override void OnEnable()
        {
            if (BaseBgmPatch.InitPatchDependency())
            {
                Harmony.PatchAll();
                ModLogger.LogInformation("mod is enabled by bepinex");
                BaseBgmPatch.ToggleEvent();
                ModLogger.LogInformation("event handler enabled!");
            }
        }

        /// <summary>
        ///     禁用脚本时调用
        /// </summary>
        public override void OnDisable()
        {
            //BaseBgmPatch.ToggleEvent();
            ModLogger.LogInformation("event handler disabled!");
        }
    }
}
