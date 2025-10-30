using HarmonyLib;
using Modding.Core;
using Modding.Core.PluginLoader;

namespace Modding.CustomBaseBgm
{
    public class ModBehaviour : ModBehaviourBase
    {
        protected override string PluginId => Util.BepinExUuid;
        protected override string PluginName => Util.PluginName;

        public override void InitializeLogger()
        {
            BaseBgmPatch.ModLogger = ModLogger.Initialize<BaseBgmPatch>(LoadingMode.None, Util.PluginName);
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
                ModLogger.LogInformation("mod is enabled by offical plugin");
                BaseBgmPatch.ToggleEvent();
                ModLogger.LogInformation("event handler enabled!");
            }
        }

        /// <summary>
        ///     禁用脚本时调用
        /// </summary>
        protected override void BeforeUnpatching()
        {
            BaseBgmPatch.ToggleEvent();
        }
    }
}
