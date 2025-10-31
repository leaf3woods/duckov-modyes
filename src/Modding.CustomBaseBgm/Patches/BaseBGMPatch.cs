using Cysharp.Threading.Tasks;
using Duckov;
using Duckov.UI.DialogueBubbles;
using HarmonyLib;
using Modding.Core;
using Saves;
using SodaCraft.StringUtilities;
using System;

namespace Modding.CustomBaseBgm.Patches
{
    /// <summary>
    ///     the patch for custom base BGM
    /// </summary>
    [HarmonyPatch(typeof(BaseBGMSelector))]
    public class BaseBGMPatch : IPatching
    {

        [HarmonyPrefix]
        [HarmonyPatch("Load")]
        public static bool LoadPrefixPatch(BaseBGMSelector __instance, bool play)
        {
            //音乐加载成功, 拦截原先的音乐
            if (PluginCore.MusicPlayer.Count != 0) return false;
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("Set", new Type[] { typeof(int), typeof(bool), typeof(bool) })]
        public static bool SetPrefixPatch(BaseBGMSelector __instance, ref DialogueBubbleProxy ___proxy, bool showInfo, bool play)
        {
            AudioManager.StopBGM();

            if (play && LevelManager.Instance && LevelManager.Instance.isActiveAndEnabled &&
                LevelManager.Instance.IsBaseLevel)
            {
                var index = SavesSystem.Load<int>(PluginCore.PluginName);
                index = index > PluginCore.MusicPlayer.Count ? -1 : index;
                PluginCore.MusicPlayer.Play(index);
            }
            var prop = AccessTools.Property(typeof(BaseBGMSelector), "BGMInfoFormat");
            var bgmInfoFormat = (string)prop?.GetValue(__instance)!;
            var msg = bgmInfoFormat.Format(new
            {
                name = PluginCore.MusicPlayer.Current.music.Info.musicName,
                author = PluginCore.MusicPlayer.Current.music.Info.author,
                index = PluginCore.MusicPlayer.Current.index
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
            AudioManager.StopBGM();

            PluginCore.MusicPlayer.Next();
            var prop = AccessTools.Property(typeof(BaseBGMSelector), "BGMInfoFormat");
            var bgmInfoFormat = (string)prop?.GetValue(__instance)!;
            var msg = bgmInfoFormat.Format(new
            {
                name = PluginCore.MusicPlayer.Current.music.Info.musicName,
                author = PluginCore.MusicPlayer.Current.music.Info.author,
                index = PluginCore.MusicPlayer.Current.index
            });
            DialogueBubblesManager.Show(msg, ___proxy.transform, ___proxy.yOffset, false, false, 200f, 2f).Forget();
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("SetPrevious")]
        public static bool SetPreviousPatch(BaseBGMSelector __instance, ref DialogueBubbleProxy ___proxy)
        {
            AudioManager.StopBGM();

            PluginCore.MusicPlayer.Previous();
            var prop = AccessTools.Property(typeof(BaseBGMSelector), "BGMInfoFormat");
            var bgmInfoFormat = (string)prop?.GetValue(__instance)!;
            var msg = bgmInfoFormat.Format(new
            {
                name = PluginCore.MusicPlayer.Current.music.Info.musicName,
                PluginCore.MusicPlayer.Current.music.Info.author,
                index = PluginCore.MusicPlayer.Current.index
            });
            DialogueBubblesManager.Show(msg, ___proxy.transform, ___proxy.yOffset, false, false, 200f, 2f).Forget();
            return false;
        }
    }
}
