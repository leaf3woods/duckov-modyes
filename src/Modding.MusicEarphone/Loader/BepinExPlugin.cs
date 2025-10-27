using BepInEx;
using Modding.Core.PluginLoader;

namespace Modding.MusicEarphone.Loader
{
    [BepInPlugin(Util.BepinExUuid, "Music Earphone", "1.0.0")]
    //[BepInDependency("com.bepinex.plugin.important")]
    public class BepinExPlugin : BepInExBase
    {
        protected override string PluginId => Util.BepinExUuid;

        /// <summary>
        ///     启用脚本时调用
        /// </summary>
        public override void OnEnable()
        {
            //SceneLoader.onBeforeSetSceneActive += BaseBgmPatch.StopRuntimeBgm;
        }

        /// <summary>
        ///     禁用脚本时调用
        /// </summary>
        public override void OnDisable()
        {
            //SceneLoader.onBeforeSetSceneActive -= BaseBgmPatch.StopRuntimeBgm;
        }

        /// <summary>
        ///     销毁对象时调用
        /// </summary>
        public override void OnDestroy()
        {
            //_harmony.UnpatchAll();
        }
    }
}
