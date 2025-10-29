using HarmonyLib;
using Modding.Core;
using Modding.Core.PluginLoader;
using Saves;

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
            Util.LoadingMode = LoadingMode.None;
            Harmony.PatchAll();
            ModLogger!.LogInformation("mod is patched by offical plugin");
            SceneLoader.onStartedLoadingScene += BaseBgmPatch.HandleSceneChanged;
            SavesSystem.OnCollectSaveData += BaseBgmPatch.SaveIndex;
        }

        /// <summary>
        ///     禁用脚本时调用
        /// </summary>
        protected override void BeforeUnpatching()
        {
            SceneLoader.onStartedLoadingScene -= BaseBgmPatch.HandleSceneChanged;
            SavesSystem.OnCollectSaveData -= BaseBgmPatch.SaveIndex;
        }
    }
}
