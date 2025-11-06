using Cysharp.Threading.Tasks;
using Duckov;
using Modding.Core;
using Modding.Core.MusicPlayer.Base;
using Modding.Core.MusicPlayer.FMod;
using Saves;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Modding.CustomBaseBgm
{
    public class PluginCore
    {
        public const string PluginName = "CustomBaseBgm";

        public const string BepinExUuid = "yesmod.duckov.bepinex.custombgm";
        public const string OfficalPluginUuid = "yesmod.duckov.offical+.custombgm";

        public static LoadingMode LoadingMode = LoadingMode.None;

        public static bool IsPatched = false;

        public static ModLogger ModLogger { get; set; } = null!;

        public static float Volume => MasterVolume * MusicVolume;
        /// <summary>
        ///     自定义BGM音量绑定到哪个通道
        /// </summary>
        public static float MasterVolume = 1f;
        public static float MusicVolume = 0.5f;

        public static FModMusicPlayer<BaseBGMSelector.Entry> MusicPlayer = new FModMusicPlayer<BaseBGMSelector.Entry>();
        private static bool _isEventRegisterd = false;

        public static bool InitDependency()
        {
            try
            {
                ModLogger.LogInformation($"bgm load patched!");
                var bgmDir = Path.Combine(Environment.CurrentDirectory, "MyBGM");
                if (!Directory.Exists(bgmDir)) Directory.CreateDirectory(bgmDir);
                var files = FModMusicPlayer<BaseBGMSelector.Entry>.SupportedTypes
                    .SelectMany(pa => Directory.EnumerateFiles(bgmDir, "*", SearchOption.AllDirectories)
                        .Where(f => f.EndsWith(pa, StringComparison.OrdinalIgnoreCase)))
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
                    ModLogger.LogInformation($"custom bgm loaded! total {files.Count()} musics.");
                    return true;
                }
                else
                {
                    ModLogger.LogWarning("no custom bgm found! use default.");
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
                SceneLoader.onStartedLoadingScene += HandleSceneChanged;
                SceneLoader.onAfterSceneInitialize += HandleSceneChanged;
                SavesSystem.OnCollectSaveData += SaveIndex;
            }
            else
            {
                SceneLoader.onStartedLoadingScene -= HandleSceneChanged;
                SceneLoader.onAfterSceneInitialize -= HandleSceneChanged;
                SavesSystem.OnCollectSaveData -= SaveIndex;
            }
            _isEventRegisterd = !_isEventRegisterd;
        }

        public static void SaveIndex() => SavesSystem.Save(PluginName, MusicPlayer.Current.index);

        public static void NextMode()
        {
            var currentMode = (int)MusicPlayer.LoopMode;
            MusicPlayer.LoopMode = (LoopMode)(++currentMode % 4);
        }

        public static void HandleSceneChanged(SceneLoadingContext context)
        {
            // 切换场景时获取绑定通道的音量
            MasterVolume = AudioManager.GetBus(Shared.MusicBus).Volume;
            MusicVolume = AudioManager.GetBus(Shared.MasterBus).Volume;
            MusicPlayer.ApplyVolume(Volume);
            ModLogger.LogInformation($"current sceneName is {context.sceneName}, current volume is {Volume * 100f:0}");
            //不在地堡时停止实时播放
            if (!context.sceneName.Contains(Shared.BaseSceneName))
            {
                Task.Run(() => MusicPlayer.FadeOutAsync(Shared.FadeOutDuration, StopMode.Stop));
                ModLogger.LogInformation("runtime music stoped!");
            }
        }
    }
}
