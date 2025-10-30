using Cysharp.Threading.Tasks;
using Duckov;
using Duckov.UI.DialogueBubbles;
using HarmonyLib;
using ItemStatsSystem;
using ItemStatsSystem.Items;
using Modding.Core;
using Modding.Core.MusicPlayer.Base;
using Modding.Core.MusicPlayer.FMod;
using Saves;
using SodaCraft.Localizations;
using SodaCraft.StringUtilities;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace Modding.MusicEarphone
{
    /// <summary>
    ///     the patch for music earphone
    /// </summary>
    [HarmonyPatch]
    public class MusicEarphonePatch : IPatching
    {
        public static ModLogger ModLogger { get; set; } = null!;

        private const float _factor = 0.3f;
        private const int interval = 5;

        /// <summary>
        ///     自定义BGM音量绑定到哪个通道
        /// </summary>
        public static float MasterVolume = 1f;
        public static float MusicVolume = 0.5f;
        public static float EarphoneVolume => MasterVolume * MusicVolume * _factor;

        private static bool _initialized = false;
        private static string _lastSceneName = Shared.BaseSceneName;
        private static Timer _timer = new Timer();
        private static bool _isEventRegisterd = false;
        private static int _savedIndex = -1;
        private static bool isPatched = false;


        public static FModMusicPlayer<BaseBGMSelector.Entry> MusicPlayer = new FModMusicPlayer<BaseBGMSelector.Entry>();

        public static bool InitPatchDependency()
        {
            if (isPatched) return false;
            try
            {
                ModLogger.LogInformation($"loading earphone musics...");
                var musicDir = Path.Combine(Environment.CurrentDirectory, "MyBGM");
                if (!Directory.Exists(musicDir)) Directory.CreateDirectory(musicDir);
                var files = FModMusicPlayer<BaseBGMSelector.Entry>.SupportedMusicExtensions
                    .SelectMany(pa => Directory.GetFiles(musicDir, pa))
                    .Select(f =>
                    {
                        var truename = Path.GetFileNameWithoutExtension(f);
                        var spits = truename.Split('-', StringSplitOptions.RemoveEmptyEntries);
                        var entry = new BaseBGMSelector.Entry
                        {
                            switchName = Path.GetFileNameWithoutExtension(f),
                            musicName = spits[0],
                            author = spits.LastOrDefault() ?? "Unknown",
                        };
                        return new FModMusic<BaseBGMSelector.Entry>()
                        {
                            Info = entry,
                            Sound = f,
                        };
                    });
                if (files.Any())
                {
                    MusicPlayer.Start(LoopMode.Random, EarphoneVolume, ShuffleMode.FisherYates, false);
                    MusicPlayer.Load(files);
                    _savedIndex = SavesSystem.Load<int>(Util.PluginName);
                    ModLogger.LogInformation($"earphone musics loaded! total {files.Count()} musics, index is {_savedIndex}.");
                    isPatched = true;
                    return true;
                }
                else
                {
                    ModLogger.LogWarning($"no earphone musics found in {musicDir}!");
                    return false;
                }
            }
            catch
            {
                ModLogger.LogError($"init patch failure!");
                return false;
            }
        }

        public async static UniTaskVoid InitializeEarphoneItemAsync()
        {
            var exist = PlayerStorage.IncomingItemBuffer.FirstOrDefault(item => item.RootTypeID == Shared.HeadsetLV2TypeId);
            if(exist is null)
            {
                var earphone = await ItemAssetsCollection.InstantiateAsync(Shared.HeadsetLV2TypeId);
                earphone.name = "music earphone";
                earphone.DisplayNameRaw = "音乐耳机";
                earphone.Quality = 2333;
                earphone.DisplayQuality = DisplayQuality.Red;
                earphone.Value = 2333;
                PlayerStorage.Push(earphone, true);
                ModLogger.LogInformation($"give music earphone!");
            }
        }
        public static void ToggleEvent()
        {
            if (!_isEventRegisterd)
            {
                SceneLoader.onAfterSceneInitialize += HandleSceneChanged;
                CharacterMainControl.OnMainCharacterSlotContentChangedEvent += HandleSlotContentChanged;
                AIMainBrain.OnSoundSpawned += HandleSoundSpawned;
                SavesSystem.OnCollectSaveData += SaveIndex;
                LevelManager.OnEvacuated += HandleEvacuated;
                Health.OnDead += HandleOnDead;
                //Health.OnHurt += HandleOnHurt;
            }
            else
            {
                SceneLoader.onAfterSceneInitialize -= HandleSceneChanged;
                CharacterMainControl.OnMainCharacterSlotContentChangedEvent -= HandleSlotContentChanged;
                AIMainBrain.OnSoundSpawned -= HandleSoundSpawned;
                SavesSystem.OnCollectSaveData -= SaveIndex;
                LevelManager.OnEvacuated -= HandleEvacuated;
                Health.OnDead -= HandleOnDead;
                //Health.OnHurt -= HandleOnHurt;
            }
            _isEventRegisterd = !_isEventRegisterd;
        }

        /// <summary>
        /// 敌人消失
        /// </summary>
        public static void NoSoundTimerHandler(object sender, ElapsedEventArgs e)
        {
            if (MusicPlayer.IsPlaying && LevelManager.Instance.name.Contains(Shared.LevelSceneName))
            //音乐恢复
            MusicPlayer.TogglePause(false);
            ModLogger.LogInformation($"enemy disapear, music continue...");
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UI_Bus_Slider), "OnValueChanged")]
        public static void OnValueChangedPostfixPatch(AudioManager.Bus ___busRef)
        {
            if(___busRef.Name == Shared.MasterBus)
            {
                MasterVolume = ___busRef.Volume;
            }
            else if(___busRef.Name == Shared.MusicBus)
            {
                MusicVolume = ___busRef.Volume;
            }
            //  当用户调整此总线上的音量时，设置运行时音乐
            MusicPlayer?.ApplyVolume(EarphoneVolume);
        }
        public static void HandleSoundSpawned(AISound sound)
        {
            //var isVisiable = ((bool)sound.fromCharacter && (bool)sound.fromCharacter.characterModel &&
            //    !sound.fromCharacter.characterModel.Hidden && !GameCamera.Instance.IsOffScreen(sound.pos));
            //周围有鸭子进入战斗状态暂停
            if (MusicPlayer.IsPlaying &&
                Team.IsEnemy(Teams.player, sound.fromTeam) &&
                sound.fromCharacter && sound.fromCharacter.characterModel &&
                !GameCamera.Instance.IsOffScreen(sound.pos))
            {
                MusicPlayer.TogglePause(true);
                _timer.Stop();   // 先停止
                _timer.Start();  // 重新计时
            }
        }

        public static void HandleSlotContentChanged(CharacterMainControl main, Slot slot)
        {
            if (slot is null || slot.Key != Shared.HeadsetSlotKey || main is null ||
                LevelManager.Instance is null || LevelManager.Instance.IsBaseLevel || !LevelManager.Instance.isActiveAndEnabled) return;
            var msg = string.Empty;
            if (slot.Content is null)
            {
                ModLogger.LogInformation($"set item method patched, stoping music!");  
                msg = "耳机已经移除, 音乐停止!";
                Task.Run(async() =>
                {
                    await MusicPlayer.FadeOutAsync(Shared.FadeOutDuration);
                    MusicPlayer.Stop();
                });
                _timer.Elapsed -= NoSoundTimerHandler;
            }
            else
            {
                MusicPlayer.Play(_savedIndex);

                var bgmInfoFormat = Shared.BGMMsgFormat.ToPlainText();
                msg = bgmInfoFormat.Format(new
                {
                    name = MusicPlayer.Current.music.Info.musicName,
                    author = MusicPlayer.Current.music.Info.author,
                    index = MusicPlayer.Current.index
                });
                _timer.Elapsed += NoSoundTimerHandler;
                _timer.Interval = interval * 1000;
                _timer.AutoReset = false; //true-一直循环 ，false-循环一次 
                _timer.Enabled = true;
            }
            if (main.modelRoot)
            {
                DialogueBubblesManager.Show(msg, main.modelRoot, 0.8f, false, false, 200f, 2f).Forget();
            }
        }

        public static void HandleOnDead(Health health, DamageInfo info)
        {
            if (health.IsMainCharacterHealth)
            {
                Task.Run(async () =>
                {
                    await MusicPlayer.FadeOutAsync(Shared.FadeOutDuration);
                    MusicPlayer.Stop();
                });
                _timer.Stop();
            }
        }

        public static void HandleOnHurt(Health health, DamageInfo info)
        {
            if (health.IsMainCharacterHealth)
            {
                MusicPlayer.TogglePause(true);
                ModLogger.LogInformation("enemy in combat, music paused!");
                _timer.Stop();   // 先停止
                _timer.Start();  // 重新计时
            }
        }

        public static void HandleEvacuated(EvacuationInfo evacuationInfo)
        {
            Task.Run(async () =>
            {
                await MusicPlayer.FadeOutAsync(Shared.FadeOutDuration);
                MusicPlayer.Stop();
            });
            _timer.Stop();
        }

        public static void SaveIndex() => SavesSystem.Save<int>(Util.PluginName, MusicPlayer.Current.index);
        public static void HandleSceneChanged(SceneLoadingContext context)
        {
            // 切换场景时获取绑定通道的音量
            MasterVolume = AudioManager.GetBus(Shared.MusicBus).Volume;
            MusicVolume = AudioManager.GetBus(Shared.MasterBus).Volume;
            MusicPlayer.ApplyVolume(EarphoneVolume);
            ModLogger.LogInformation($"current sceneName is {context.sceneName}, current volume is {EarphoneVolume * 100f:0}");
            //进入地图后开始加载
            if (context.sceneName.Contains(Shared.LevelSceneName))
            {
                var slot = CharacterMainControl.Main.GetSlot(Shared.HeadsetSlotKey.GetHashCode());
                if(slot is null) ModLogger.LogInformation($"找不到");
                if (_lastSceneName == Shared.BaseSceneName && slot != null &&
                    slot.Content != null && slot.Content.TypeID == Shared.HeadsetLV2TypeId)
                {
                    MusicPlayer.Play(_savedIndex);
                }
                _lastSceneName = context.sceneName;
            }
            else
            {
                Task.Run(async () =>
                {
                    await MusicPlayer.FadeOutAsync(Shared.FadeOutDuration);
                    MusicPlayer.Stop();
                });
                _timer.Stop();
                if (!_initialized && context.sceneName.Contains(Shared.BaseSceneName))
                {
                    InitializeEarphoneItemAsync().Forget();
                    _initialized = true;
                }
            }
        }
    }
}
