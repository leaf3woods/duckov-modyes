using Modding.Core;
using Modding.Core.PluginLoader;

namespace Modding.MusicEarphone
{
    public class ModBehaviour : ModBehaviourBase
    {
        protected override string PluginId => Util.BepinExUuid;
        protected override string PluginName => Util.PluginName;

        public override void InitializeLogger()
        {
            MusicEarphonePatch.ModLogger = ModLogger.Initialize<MusicEarphonePatch>(LoadingMode.None, Util.PluginName);
            ModLogger = MusicEarphonePatch.ModLogger;
        }

        /// <summary>
        ///     启用脚本时调用
        /// </summary>
        public override void OnEnable()
        {
            if (MusicEarphonePatch.InitPatchDependency())
            {
                Harmony.PatchAll();
                ModLogger.LogInformation("mod is enabled by offical plugin");
                MusicEarphonePatch.ToggleEvent();
                ModLogger.LogInformation("event handler enabled!");
            }
        }

        /// <summary>
        ///     禁用脚本时调用
        /// </summary>
        protected override void BeforeUnpatching()
        {
            MusicEarphonePatch.ToggleEvent();
            ModLogger.LogInformation("event handler disabled!");
        }
    }
}
