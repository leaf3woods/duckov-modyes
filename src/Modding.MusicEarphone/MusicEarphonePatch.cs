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
using System.Timers;

namespace Modding.MusicEarphone
{
    /// <summary>
    ///     the patch for music earphone
    /// </summary>
    [HarmonyPatch]
    public class MusicEarphonePatch : PatchBase
    {
        private static ModLogger ModLogger { get; set; } = null!;

        public const string LevelSceneName = "Level";
        public const string BaseSceneName = "Base";
        public const string MasterBus = "Master";
        public const string MusicBus = "Master/Music";
        private const float _factor = 0.3f;

        private const int interval = 5;
        private const int _earphoneTypeid = 1252;

        /// <summary>
        ///     自定义BGM音量绑定到哪个通道
        /// </summary>
        public static float MasterVolume = 1f;
        public static float MusicVolume = 0.5f;
        public static float EarphoneVolume => MasterVolume * MusicVolume * _factor;

        private static bool _initialized = false;
        private static string _lastSceneName = BaseSceneName;
        private static Timer _timer = new Timer();

        public static FModMusicPlayer<BaseBGMSelector.Entry> MusicPlayer = new FModMusicPlayer<BaseBGMSelector.Entry>();

        protected override void InitializeLogger()
        {
            ModLogger = ModLogger.Initialize<MusicEarphonePatch>(Util.LoadingMode, Util.PluginName);
        }

        public async static UniTaskVoid InitializeEarphoneItemAsync()
        {
            //var ears = ItemAssetsCollection.Instance.entries
            //    .Where(e =>
            //        e.prefab.DisplayNameRaw.Contains("Headset") ||
            //        e.prefab.DisplayName.Contains("Headset") ||
            //        e.prefab.name.Contains("Headset"))
            //    .Select(e => $"[type: {e.typeID}, name: {e.prefab.name}]");
            //;
            //ModLogger.LogInformation($"earphone names are: {string.Join(',', ears)}");
            var exist = PlayerStorage.IncomingItemBuffer.FirstOrDefault(item => item.RootTypeID == _earphoneTypeid);
            if(exist is null)
            {
                var earphone = await ItemAssetsCollection.InstantiateAsync(_earphoneTypeid);
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
        /// 周围出现敌人
        /// </summary>
        public static void NoSoundTimerHandler(object sender, ElapsedEventArgs e)
        {
            if(MusicPlayer.IsPlaying)
            //音乐恢复
            MusicPlayer.TogglePause(false);
            ModLogger.LogInformation($"enemy disapear, music continue...");
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UI_Bus_Slider), "OnValueChanged")]
        public static void OnValueChangedPostfixPatch(AudioManager.Bus ___busRef)
        {
            if(___busRef.Name == MasterBus)
            {
                MasterVolume = ___busRef.Volume;
            }
            else if(___busRef.Name == MusicBus)
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
            if (MusicPlayer.IsPlaying && !MusicPlayer.IsPasued &&
                Team.IsEnemy(Teams.player, sound.fromTeam) &&
                sound.fromCharacter && sound.fromCharacter.characterModel && !GameCamera.Instance.IsOffScreen(sound.pos))
            {
                MusicPlayer.TogglePause(true);
                ModLogger.LogInformation("enemy in combat, music paused!");
                _timer.Stop();   // 先停止
                _timer.Start();  // 重新计时
            }
        }

        public static void HandleSlotContentChanged(CharacterMainControl main, Slot slot)
        {
            if (slot is null || slot.Key != "Headset" || main is null ||
                LevelManager.Instance is null || LevelManager.Instance.IsBaseLevel) return;
            if (MusicPlayer.Count == 0)
            {
                ModLogger.LogInformation($"musics not exist, skipped playing!");
            }
            var msg = string.Empty;
            if (slot.Content is null)
            {
                ModLogger.LogInformation($"set item method patched, stoping music!");
                MusicPlayer.Stop();
                msg = "耳机已经移除, 音乐停止!";
                _timer.Elapsed -= NoSoundTimerHandler;
            }
            else
            {
                ModLogger.LogInformation($"headset slot is added content: {slot.Content.DisplayNameRaw}, now playing music!");
                MusicPlayer.Play(-1);
                var bgmInfoFormat = "BGMInfoFormat".ToPlainText();
                msg = bgmInfoFormat.Format(new
                {
                    name = MusicPlayer.Current.music.Info.musicName,
                    author = MusicPlayer.Current.music.Info.author,
                    index = MusicPlayer.Current.index
                });
                _timer.Elapsed += NoSoundTimerHandler;
                _timer.Interval = interval * 1000;
                _timer.AutoReset = false; //true-一直循环 ，false-循环一次   
                _timer.Enabled = false;
            }
            var root = main.modelRoot;
            DialogueBubblesManager.Show(msg, root, 0.8f, false, false, 200f, 2f).Forget();
        }

        public static void LoadEarphoneMusics()
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
                ModLogger.LogInformation($"earphone musics loaded! total {files.Count()} musics.");
            }
        }
        public static void SaveIndex() => SavesSystem.Save<int>(Util.PluginName, MusicPlayer.Current.index);
        public static void HandleSceneChanged(SceneLoadingContext context)
        {
            // 切换场景时获取绑定通道的音量
            MasterVolume = AudioManager.GetBus(MusicBus).Volume;
            MusicVolume = AudioManager.GetBus(MasterBus).Volume;
            ModLogger.LogInformation($"current sceneName is {context.sceneName}, current volume is {EarphoneVolume * 100f:0}");
            //进入地图后开始加载
            if (context.sceneName.Contains(LevelSceneName))
            {
                if(_lastSceneName == BaseSceneName && false)
                {
                    var index = SavesSystem.Load<int>(Util.PluginName);
                    MusicPlayer.Play(index);
                }
                _lastSceneName = context.sceneName;
            }
            else
            {
                MusicPlayer.Stop();
                _timer.Stop();
                if (!_initialized)
                {
                    InitializeEarphoneItemAsync().Forget();
                    _initialized = true;
                }
            }
        }
    }
}
