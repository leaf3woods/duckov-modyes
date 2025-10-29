using BepInEx;
using Modding.Core.PluginLoader;

namespace Modding.MusicEarphone
{
    [BepInPlugin(Util.BepinExUuid, "Music Earphone", "1.0.0")]
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
            Harmony.PatchAll();
            ModLogger!.LogInformation("mod is enabled by bepinex");
            ModLogger!.LogInformation("scene loader handler enabled!");
            MusicEarphonePatch.LoadEarphoneMusics();
            LevelManager.OnLevelBeginInitializing += MusicEarphonePatch.HandleSceneChanged;
            CharacterMainControl.OnMainCharacterSlotContentChangedEvent += MusicEarphonePatch.HandleSlotContentChanged;
            AIMainBrain.OnSoundSpawned += MusicEarphonePatch.HandleSoundSpawned;
        }

        /// <summary>
        ///     禁用脚本时调用
        /// </summary>
        public override void OnDisable()
        {
            //base.OnDisable();
            //LevelManager.OnLevelInitialized -= MusicEarphonePatch.HandleSceneChanged;
            ModLogger!.LogInformation("plugin disabled!");
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
