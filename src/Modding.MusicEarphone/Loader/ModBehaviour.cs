using Modding.Core;
using Modding.Core.PluginLoader;
using Saves;

namespace Modding.MusicEarphone
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
            MusicEarphonePatch.LoadEarphoneMusics();
            SceneLoader.onStartedLoadingScene += MusicEarphonePatch.HandleSceneChanged;
            CharacterMainControl.OnMainCharacterSlotContentChangedEvent += MusicEarphonePatch.HandleSlotContentChanged;
            AIMainBrain.OnSoundSpawned += MusicEarphonePatch.HandleSoundSpawned;
            SavesSystem.OnCollectSaveData += MusicEarphonePatch.SaveIndex;
        }

        /// <summary>
        ///     禁用脚本时调用
        /// </summary>
        protected override void BeforeUnpatching()
        {
            SceneLoader.onStartedLoadingScene -= MusicEarphonePatch.HandleSceneChanged;
            CharacterMainControl.OnMainCharacterSlotContentChangedEvent -= MusicEarphonePatch.HandleSlotContentChanged;
            AIMainBrain.OnSoundSpawned -= MusicEarphonePatch.HandleSoundSpawned;
            SavesSystem.OnCollectSaveData -= MusicEarphonePatch.SaveIndex;
        }
    }
}
