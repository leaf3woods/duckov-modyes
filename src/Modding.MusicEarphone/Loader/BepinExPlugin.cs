using BepInEx;
using Modding.Core;
using Modding.Core.PluginLoader;
using Saves;

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
            MusicEarphonePatch.ModLogger = ModLogger.Initialize<MusicEarphonePatch>(LoadingMode.BepInEx, Util.PluginName);
            Harmony.PatchAll();
            ModLogger!.LogInformation("mod is enabled by bepinex");
            MusicEarphonePatch.LoadEarphoneMusics();
            ModLogger!.LogInformation("scene loader handler enabled!");
            SceneLoader.onStartedLoadingScene += MusicEarphonePatch.HandleSceneChanged;
            CharacterMainControl.OnMainCharacterSlotContentChangedEvent += MusicEarphonePatch.HandleSlotContentChanged;
            AIMainBrain.OnSoundSpawned += MusicEarphonePatch.HandleSoundSpawned;
            SavesSystem.OnCollectSaveData += MusicEarphonePatch.SaveIndex;
        }

        /// <summary>
        ///     禁用脚本时调用
        /// </summary>
        public override void OnDisable()
        {
            //base.OnDisable();
            //SceneLoader.onStartedLoadingScene -= MusicEarphonePatch.HandleSceneChanged;
            ModLogger!.LogInformation("plugin disabled!");
        }

    }
}
