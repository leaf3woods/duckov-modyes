using Modding.Core.PluginLoader;

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
            MusicEarphonePatch.LoadEarphoneMusics();
            LevelManager.OnLevelBeginInitializing += MusicEarphonePatch.HandleSceneChanged;
            CharacterMainControl.OnMainCharacterSlotContentChangedEvent += MusicEarphonePatch.HandleSlotContentChanged;
            AIMainBrain.OnSoundSpawned += MusicEarphonePatch.HandleSoundSpawned;
        }

        /// <summary>
        ///     禁用脚本时调用
        /// </summary>
        protected override void BeforeUnpatching()
        {
            LevelManager.OnLevelBeginInitializing -= MusicEarphonePatch.HandleSceneChanged;
            CharacterMainControl.OnMainCharacterSlotContentChangedEvent -= MusicEarphonePatch.HandleSlotContentChanged;
            AIMainBrain.OnSoundSpawned -= MusicEarphonePatch.HandleSoundSpawned;
        }
    }
}
