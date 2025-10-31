﻿using Cysharp.Threading.Tasks;
using Duckov;
using Duckov.UI.DialogueBubbles;
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
    public class PluginCore
    {
        public const string PluginName = "MusicEarphone";

        public const string BepinExUuid = "yesmod.duckov.bepinex.musicearphone";
        public const string OfficalPluginUuid = "yesmod.duckov.offical+.musicearphone";

        private const int interval = 6;

        private static bool _initialized = false;
        private static string _lastSceneName = Shared.BaseSceneName;
        private static Timer _timer = new Timer();


        public static bool IsPatched = false;
        private static bool _isEventRegisterd = false;
        public static ModLogger ModLogger { get; set; } = null!;

        /// <summary>
        ///     自定义BGM音量绑定到哪个通道
        /// </summary>
        public static float MasterVolume = 1f;
        public static float MusicVolume = 0.5f;
        public const float Factor = 0.3f;
        public static float Volume => MasterVolume * MusicVolume * Factor;


        public static FModMusicPlayer<BaseBGMSelector.Entry> MusicPlayer = new FModMusicPlayer<BaseBGMSelector.Entry>();
        public static int SavedIndex = -1;

        public static bool InitDependency()
        {
            try
            {
                ModLogger.LogInformation($"loading earphone musics...");
                var musicDir = Path.Combine(Environment.CurrentDirectory, "MyBGM");
                if (!Directory.Exists(musicDir)) Directory.CreateDirectory(musicDir);
                var files = FModMusicPlayer<BaseBGMSelector.Entry>.SupportedTypes
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
                    MusicPlayer.Start(LoopMode.Random, Volume, ShuffleMode.FisherYates, false);
                    MusicPlayer.Load(files);
                    SavedIndex = SavesSystem.Load<int>(PluginName);
                    ModLogger.LogInformation($"earphone musics loaded! total {files.Count()} musics, index is {SavedIndex}.");
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

        public static void ToggleEvent(bool? enable = true)
        {
            if ((enable is null && !_isEventRegisterd) || (enable != null && enable.Value))
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

        public async static UniTaskVoid InitializeEarphoneItemAsync()
        {
            var exist = PlayerStorage.IncomingItemBuffer.FirstOrDefault(item => item.RootTypeID == Shared.HeadsetLV2TypeId);
            if (exist is null)
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

        /// <summary>
        /// 敌人消失
        /// </summary>
        public static void NoSoundTimerHandler(object sender, ElapsedEventArgs e)
        {
            //音乐恢复
            MusicPlayer.TogglePause(false);
            ModLogger.LogInformation($"enemy disapear, music continue...");
        }

        public static void HandleSoundSpawned(AISound sound)
        {
            //var isVisiable = ((bool)sound.fromCharacter && (bool)sound.fromCharacter.characterModel &&
            //    !sound.fromCharacter.characterModel.Hidden && !GameCamera.Instance.IsOffScreen(sound.pos));
            //周围有鸭子进入战斗状态暂停
            if (MusicPlayer.IsPlaying && !MusicPlayer.IsPasued &&
                Team.IsEnemy(Teams.player, sound.fromTeam) &&
                sound.fromCharacter && sound.fromCharacter.characterModel &&
                !GameCamera.Instance.IsOffScreen(sound.pos))
            {
                MusicPlayer.TogglePause(true);
                _timer.Stop();   // 先停止
                _timer.Start();  // 重新计时
            }
        }

        private static void InitializeTimer()
        {
            _timer.Elapsed += NoSoundTimerHandler;
            _timer.Interval = interval * 1000;
            _timer.AutoReset = false; //true-一直循环 ，false-循环一次 
            _timer.Enabled = false;
        }

        public static void HandleSlotContentChanged(CharacterMainControl main, Slot slot)
        {
            if (slot is null || slot.Key != Shared.HeadsetSlotKey || main is null ||
                LevelManager.Instance is null || LevelManager.Instance.IsBaseLevel || !LevelManager.Instance.isActiveAndEnabled) return;
            var msg = string.Empty;
            if (slot.Content is null)
            {
                ModLogger.LogInformation($"set item method patched, stoping music!");
                Task.Run(() => MusicPlayer.FadeOutAsync(Shared.FadeOutDuration, StopMode.Stop));
                _timer.Elapsed -= NoSoundTimerHandler;
                _timer.Stop();
                msg = "耳机已经移除, 音乐停止!";
            }
            else
            {
                Task.Run(() => MusicPlayer.FadeInAsync(Shared.FadeOutDuration));
                MusicPlayer.Play();
                InitializeTimer();
                var bgmInfoFormat = Shared.BGMMsgFormat.ToPlainText();
                msg = bgmInfoFormat.Format(new
                {
                    name = MusicPlayer.Current.music.Info.musicName,
                    MusicPlayer.Current.music.Info.author,
                    MusicPlayer.Current.index
                });
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
                Task.Run(() => MusicPlayer.FadeOutAsync(Shared.FadeOutDuration, StopMode.Stop));
                _timer.Elapsed -= NoSoundTimerHandler;
                _timer.Stop();
            }
        }

        public static void HandleOnHurt(Health health, DamageInfo info)
        {
            if (health.IsMainCharacterHealth)
            {
                ModLogger.LogInformation("enemy in combat, music paused!");
                Task.Run(() => MusicPlayer.FadeOutAsync(Shared.FadeOutDuration, StopMode.Pause));
                _timer.Stop();
            }
        }

        public static void HandleEvacuated(EvacuationInfo evacuationInfo)
        {
            Task.Run(() => MusicPlayer.FadeOutAsync(Shared.FadeOutDuration, StopMode.Stop));
            _timer.Elapsed -= NoSoundTimerHandler;
            _timer.Stop();
        }

        public static void SaveIndex() => SavesSystem.Save(PluginName, MusicPlayer.Current.index);
        public static void HandleSceneChanged(SceneLoadingContext context)
        {
            // 切换场景时获取绑定通道的音量
            MasterVolume = AudioManager.GetBus(Shared.MusicBus).Volume;
            MusicVolume = AudioManager.GetBus(Shared.MasterBus).Volume;
            MusicPlayer.ApplyVolume(Volume);
            ModLogger.LogInformation($"current sceneName is {context.sceneName}, current volume is {Volume * 100f:0}");
            //进入地图后开始加载
            if (context.sceneName.Contains(Shared.LevelSceneName))
            {
                var slot = CharacterMainControl.Main.GetSlot(Shared.HeadsetSlotKey.GetHashCode());
                if (_lastSceneName == Shared.BaseSceneName && slot != null &&
                    slot.Content != null && slot.Content.TypeID == Shared.HeadsetLV2TypeId)
                {
                    MusicPlayer.Play(SavedIndex);
                    InitializeTimer();
                }
                _lastSceneName = context.sceneName;
            }
            else
            {
                Task.Run(() => MusicPlayer.FadeOutAsync(Shared.FadeOutDuration, StopMode.Stop));
                _timer.Elapsed -= NoSoundTimerHandler;
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
