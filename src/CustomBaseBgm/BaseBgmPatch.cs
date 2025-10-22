using Cysharp.Threading.Tasks;
using Duckov;
using Duckov.UI.DialogueBubbles;
using FMOD;
using HarmonyLib;
using SodaCraft.StringUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CustomBaseBgm
{
    /// <summary>
    ///     the patch for custom base BGM
    /// </summary>
    [HarmonyPatch(typeof(BaseBGMSelector))]
    public class BaseBgmPatch
    {
        public static Dictionary<BaseBGMSelector.Entry, Sound> CustomBgmSounds = new Dictionary<BaseBGMSelector.Entry, Sound>();
        public static ChannelGroup BgmGroup;
        public static FMOD.Channel? BgmChannel;
        public static float BgmVolume = 0.5f;
        /// <summary>
        ///     自定义BGM音量绑定到哪个通道
        /// </summary>
        public const string BindBus = "Master/Music";

        [HarmonyPrefix]
        [HarmonyPatch("Load")]
        public static bool LoadPrefixPatch(BaseBGMSelector __instance, bool play)
        {
            Util.LogInformation($"load method patched! origin entries[{__instance.entries.Length}]:\r\n" +
                string.Join(',', __instance.entries.Select(e => $"<{e.switchName}>{e.musicName}:{e.author}")));

            //var bgmDir = Path.Combine(Util.LoadByBepinEx ?
            //    Path.GetDirectoryName(typeof(BaseBgmPatch).Assembly.Location) :
            //    Environment.CurrentDirectory + "/MyBGM");
            var bgmDir = Path.Combine(Path.GetDirectoryName(typeof(BaseBgmPatch).Assembly.Location) + "/MyBGM");
            if(!Directory.Exists(bgmDir)) Directory.CreateDirectory(bgmDir);
            var files = Util.SupportedMusicExtensions.SelectMany(pa => Directory.GetFiles(bgmDir, pa)).ToList();
            files.ForEach(f =>
            {
                var truename = Path.GetFileNameWithoutExtension(f);
                var spits = truename.Split('-', StringSplitOptions.RemoveEmptyEntries);
                var entry = new BaseBGMSelector.Entry
                {
                    switchName = Path.GetFileNameWithoutExtension(f),
                    musicName = spits[0],
                    author = spits.LastOrDefault() ?? "Unknown",
                };
                var result = FMODUnity.RuntimeManager.CoreSystem.createSound(f,
                    MODE.LOOP_NORMAL | MODE.CREATESTREAM,
                    out var sound);
                if (result == RESULT.OK && CustomBgmSounds.TryAdd(entry, sound))
                {
                    Util.LogInformation($"music: {entry.musicName} load succeed!");
                } 
                else Util.LogWarning($"music: {entry.musicName} load failed!");                   
            });
            if(CustomBgmSounds.Count != 0)
            {
                __instance.entries = CustomBgmSounds.Keys.ToArray();
                Util.LogInformation($"custom bgm loaded! total {CustomBgmSounds.Count} musics.");
            }
            else
            {
                Util.LogWarning("no custom bgm found! use default.");
            }

            Util.LogInformation($"change to custom entries[{__instance.entries.Length}]:\r\n" +
                string.Join(',', __instance.entries.Select(e => $"<{e.switchName}>{e.musicName}:{e.author}")));

            //var prop = AccessTools.Property(typeof(BaseBGMSelector), "BGMInfoFormat");
            //var bgmInfoFormat = (string)prop?.GetValue(__instance)!;
            //Util.LogWarning($"current bgm is {bgmInfoFormat}");

            return true; // continue original method
        }

        [HarmonyPrefix]
        [HarmonyPatch("Set", new Type[] { typeof(int), typeof(bool), typeof(bool) })]
        public static bool SetPrefixPatch(BaseBGMSelector __instance, ref DialogueBubbleProxy ___proxy, ref int index, bool showInfo, bool play)
        {
            Util.LogInformation($"set method patched! play index is {index}");
            index = index > CustomBgmSounds.Count ? 0 : index;
            AudioManager.StopBGM();

            BgmChannel?.stop();
            var pair = CustomBgmSounds.ElementAtOrDefault(index);
            if (play)
            {
                FMODUnity.RuntimeManager.CoreSystem.playSound(pair.Value, BgmGroup, false, out var channel);
                BgmChannel = channel;
            }
            var prop = AccessTools.Property(typeof(BaseBGMSelector), "BGMInfoFormat");
            var bgmInfoFormat = (string)prop?.GetValue(__instance)!;
            var msg = bgmInfoFormat.Format(new
            {
                name = pair.Key.musicName,
                author = pair.Key.author,
                index = index + 1
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

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UI_Bus_Slider), "OnValueChanged")]
        public static void OnValueChangedPostfixPatch(AudioManager.Bus ___busRef)
        {

            Util.LogInformation($"ui bus slider changed, target bus is {___busRef.Name}!");
            //  当用户调整此总线上的音量时，设置运行时音乐
            if(___busRef.Name.Contains(BindBus) && BgmChannel != null)
            {
                BgmChannel.Value.setVolume(___busRef.Volume);
                Util.LogInformation($"target bus: {___busRef.Name} volume changed to {(___busRef.Volume * 100f):0} succeed!");
            }
        }
    }
}
