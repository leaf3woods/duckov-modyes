using Cysharp.Threading.Tasks;
using Duckov;
using Duckov.UI.DialogueBubbles;
using HarmonyLib;
using Modding.Core;
using Modding.Core.MusicPlayer.Base;
using Modding.Core.MusicPlayer.FMod;
using Modding.Core.PluginLoader;
using Saves;
using SodaCraft.StringUtilities;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Modding.CustomBaseBgm
{
    /// <summary>
    ///     the patch for custom base BGM
    /// </summary>
    [HarmonyPatch(typeof(BaseBGMSelector))]
    public class BaseBgmPatch
    {
        public static ModLogger ModLogger = (BepInExBase.ModLogger ?? ModBehaviourBase.ModLogger)!;

        public static float BgmVolume => BgmMasterVolume * BgmMusicVolume;
        public static string BaseSceneName = "Base";
        /// <summary>
        ///     自定义BGM音量绑定到哪个通道
        /// </summary>
        public const string MasterBus = "Master";
        public static float BgmMasterVolume = 1f;
        public const string MusicBus = "Master/Music";
        public static float BgmMusicVolume = 0.5f;

        public static FModMusicPlayer<BaseBGMSelector.Entry> MusicPlayer = new FModMusicPlayer<BaseBGMSelector.Entry>();

        [HarmonyPrefix]
        [HarmonyPatch("Load")]
        public static bool LoadPrefixPatch(BaseBGMSelector __instance, bool play)
        {
            ModLogger.LogInformation($"bgm load patched!");
            var bgmDir = Path.Combine(Environment.CurrentDirectory, "MyBGM");
            if(!Directory.Exists(bgmDir)) Directory.CreateDirectory(bgmDir);
            var files = FModMusicPlayer<BaseBGMSelector.Entry>.SupportedMusicExtensions
                .SelectMany(pa => Directory.GetFiles(bgmDir, pa))
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
                MusicPlayer.Start(LoopMode.Random, BgmVolume, ShuffleMode.FisherYates, false);
                MusicPlayer.Load(files);
                ModLogger.LogInformation($"custom bgm loaded! total {files.Count()} musics.");
                return false; // continue original method
            }
            else
            {
                ModLogger.LogWarning("no custom bgm found! use default.");
                return true; // continue original method
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch("Set", new Type[] { typeof(int), typeof(bool), typeof(bool) })]
        public static bool SetPrefixPatch(BaseBGMSelector __instance, ref DialogueBubbleProxy ___proxy, bool showInfo, bool play)
        {
            if(MusicPlayer.Count == 0) return true; // continue original method
            AudioManager.StopBGM();
            
            if (play)
            {
                var index = SavesSystem.Load<int>(Util.PluginName);
                index = index > MusicPlayer.Count ? -1 : index;
                MusicPlayer.Play(index);
            }
            var prop = AccessTools.Property(typeof(BaseBGMSelector), "BGMInfoFormat");
            var bgmInfoFormat = (string)prop?.GetValue(__instance)!;
            var msg = bgmInfoFormat.Format(new
            {
                name = MusicPlayer.Current.music.Info.musicName,
                author = MusicPlayer.Current.music.Info.author,
                index = MusicPlayer.Current.index
            });
            if (showInfo)
            {
                //___proxy.Pop(msg, 200f);
                DialogueBubblesManager.Show(msg, ___proxy.transform, ___proxy.yOffset, false, false, 200f, 2f).Forget();
            }
            else
            {
                //___proxy.Pop(msg, 200f);
                DialogueBubblesManager.Show(msg, ___proxy.transform, ___proxy.yOffset, false, false, 200f, 4f).Forget();
            }
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("SetNext")]
        public static bool SetNextPatch(BaseBGMSelector __instance, ref DialogueBubbleProxy ___proxy)
        {
            if(MusicPlayer.Count == 0) return true; // continue original method
            AudioManager.StopBGM();
            
            MusicPlayer.Next();
            var prop = AccessTools.Property(typeof(BaseBGMSelector), "BGMInfoFormat");
            var bgmInfoFormat = (string)prop?.GetValue(__instance)!;
            var msg = bgmInfoFormat.Format(new
            {
                name = MusicPlayer.Current.music.Info.musicName,
                author = MusicPlayer.Current.music.Info.author,
                index = MusicPlayer.Current.index
            });
            DialogueBubblesManager.Show(msg, ___proxy.transform, ___proxy.yOffset, false, false, 200f, 2f).Forget();
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("SetPrevious")]
        public static bool SetPreviousPatch(BaseBGMSelector __instance, ref DialogueBubbleProxy ___proxy)
        {
            if (MusicPlayer.Count == 0) return true; // continue original method
            AudioManager.StopBGM();

            MusicPlayer.Previous();
            var prop = AccessTools.Property(typeof(BaseBGMSelector), "BGMInfoFormat");
            var bgmInfoFormat = (string)prop?.GetValue(__instance)!;
            var msg = bgmInfoFormat.Format(new
            {
                name = MusicPlayer.Current.music.Info.musicName,
                MusicPlayer.Current.music.Info.author,
                index = MusicPlayer.Current.index
            });
            DialogueBubblesManager.Show(msg, ___proxy.transform, ___proxy.yOffset, false, false, 200f, 2f).Forget();
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UI_Bus_Slider), "OnValueChanged")]
        public static void OnValueChangedPostfixPatch(AudioManager.Bus ___busRef)
        {
            if(___busRef.Name == MasterBus)
            {
                BgmMasterVolume = ___busRef.Volume;
            }
            else if(___busRef.Name == MusicBus)
            {
                BgmMusicVolume = ___busRef.Volume;
            }
            //  当用户调整此总线上的音量时，设置运行时音乐
            MusicPlayer?.ApplyVolume(BgmVolume);
        }

        public static void HandleSceneChanged(SceneLoadingContext context)
        {
            // 切换场景时获取绑定通道的音量
            BgmMasterVolume = AudioManager.GetBus(MusicBus).Volume;
            BgmMusicVolume = AudioManager.GetBus(MasterBus).Volume;
            ModLogger.LogInformation($"current sceneName is {context.sceneName}, current volume is {BgmVolume * 100f:0}");
            //不在地堡时停止实时播放
            if (!context.sceneName.Contains(BaseSceneName))
            {
                Task.Run(() => MusicPlayer.FadeToAsync(3f));
                ModLogger.LogInformation("runtime music stoped!");
                SavesSystem.Save<int>(Util.PluginName, MusicPlayer.Current.index);
            }
        }
    }
}
