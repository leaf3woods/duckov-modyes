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

        /// <summary>
        ///     启用脚本时调用
        /// </summary>
        public override void OnEnable()
        {
            Util.LoadingMode = LoadingMode.BepInEx;
            Harmony.PatchAll();
            ModLogger!.LogInformation("mod is enabled by bepinex");
            SceneLoader.onStartedLoadingScene += BaseBgmPatch.HandleSceneChanged;
            ModLogger.LogInformation("scene loader handler enabled!");
        }

        /// <summary>
        ///     禁用脚本时调用
        /// </summary>
        public override void OnDisable()
        {
            //base.OnDisable();
            //SceneLoader.onStartedLoadingScene -= BaseBgmPatch.HandleSceneChanged;
            ModLogger!.LogInformation("scene loader handler disabled!");
        }
    }
}
