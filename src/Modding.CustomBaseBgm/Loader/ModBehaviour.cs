using HarmonyLib;
using Modding.Core.PluginLoader;

namespace Modding.CustomBaseBgm
{
    public class ModBehaviour : ModBehaviourBase
    {
        protected override string PluginId => Util.BepinExUuid;
        protected override string PluginName => Util.PluginName;

        /// <summary>
        ///     启用脚本时调用
        /// </summary>
        public override void OnEnable()
        {
            Harmony.PatchAll();
            ModLogger!.LogInformation("mod is patched by offical plugin");
            SceneLoader.onBeforeSetSceneActive += BaseBgmPatch.HandleSceneChanged;
        }

        /// <summary>
        ///     禁用脚本时调用
        /// </summary>
        protected override void BeforeUnpatching()
        {
            SceneLoader.onBeforeSetSceneActive -= BaseBgmPatch.HandleSceneChanged;
        }
    }
}
