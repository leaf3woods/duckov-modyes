using BepInEx.Logging;
using UnityEngine;

namespace CustomBaseBgm
{
    public class Util
    {
        public static bool LoadByBepinEx { get; set; } = false;

        /// <summary>
        ///     支持的音乐格式
        /// </summary>

        public static string[] SupportedMusicExtensions = { "*.mp3", "*.flac", "*.aac", "*.m4a" };
        public static ManualLogSource? Logger { get; set; }

        public static void LogDebug(string msg)
        {
            if (LoadByBepinEx) Logger!.LogDebug(msg);
            else Debug.Log(msg);
        }
        public static void LogInformation(string msg)
        {
            if (LoadByBepinEx) Logger!.LogInfo(msg);
            else Debug.Log(msg);
        }

        public static void LogWarning(string msg)
        {
            if (LoadByBepinEx) Logger!.LogWarning(msg);
            else Debug.LogWarning(msg);
        }

        public static void LogError(string msg)
        {
            if (LoadByBepinEx) Logger!.LogError(msg);
            else Debug.LogError(msg);
        }
    }
}
