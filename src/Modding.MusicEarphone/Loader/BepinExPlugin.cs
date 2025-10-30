using BepInEx;
using Modding.Core;
using Modding.Core.PluginLoader;

namespace Modding.MusicEarphone
{
    [BepInPlugin(Util.BepinExUuid, "Music Earphone", "1.0.0")]
    //[BepInDependency("com.bepinex.plugin.important")]
    public class BepinExPlugin : BepInExBase
    {
        protected override string PluginId => Util.BepinExUuid;
        protected override string PluginName => Util.PluginName;

        public override void InitializeLogger()
        {
            MusicEarphonePatch.ModLogger = ModLogger.Initialize<MusicEarphonePatch>(LoadingMode.BepInEx, Util.PluginName);
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
                ModLogger.LogInformation("mod is enabled by bepinex");
                MusicEarphonePatch.ToggleEvent();
                ModLogger.LogInformation("event handler enabled!");
            }
        }

        /// <summary>
        ///     禁用脚本时调用
        /// </summary>
        public override void OnDisable()
        {
            //MusicEarphonePatch.ToggleEvent();
            ModLogger.LogInformation("event handler disabled!");
        }
    }
}
