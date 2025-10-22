using BepInEx.Logging;
using UnityEngine;

namespace CustomBaseBgm
{
    public class Util
    {

        public const string BepinExUuid = "yesmod.duckov.bepinex.custombgm";
        public const string OfficalPluginUuid = "yesmod.duckov.offical+.custombgm";

        public static bool LoadByBepinEx { get; set; } = false;

        /// <summary>
        ///     支持的音乐格式
        /// </summary>

        public static string[] SupportedMusicExtensions = { "*.mp3", "*.flac", "*.aac", "*.m4a" };
        public static ManualLogSource? Logger { get; set; }

        public static void LogDebug(string msg)
        {
            if (LoadByBepinEx) Logger!.LogDebug(msg);
            else Debug.Log($"Mod:[{nameof(CustomBaseBgm)}] => " + msg);
        }
        public static void LogInformation(string msg)
        {
            if (LoadByBepinEx) Logger!.LogInfo(msg);
            else Debug.Log($"Mod:[{nameof(CustomBaseBgm)}] => " + msg);
        }

        public static void LogWarning(string msg)
        {
            if (LoadByBepinEx) Logger!.LogWarning(msg);
            else Debug.LogWarning($"Mod:[{nameof(CustomBaseBgm)}] => " + msg);
        }

        public static void LogError(string msg)
        {
            if (LoadByBepinEx) Logger!.LogError(msg);
            else Debug.LogError($"Mod:[{nameof(CustomBaseBgm)}] => " + msg);
        }
    }
}
