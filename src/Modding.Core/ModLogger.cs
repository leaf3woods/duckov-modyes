
using BepInEx.Logging;
using UnityEngine;

namespace Modding.Core
{
    public class ModLogger
    {
        public static ManualLogSource? Logger { get; set; }
        public static LoadingMode Mode { get; set; }
        public static string SourceName { get; set; } = null!;

        public static void Initialize<T>(LoadingMode mode)
        {
            Mode = mode;
            SourceName = typeof(T).Name;
            if(mode == LoadingMode.BepInEx)
            Logger = BepInEx.Logging.Logger.CreateLogSource(SourceName);
        }

        public static void LogDebug(string msg)
        {
            switch (Mode)
            {
                case LoadingMode.BepInEx:
                    Logger!.LogDebug(msg);
                    break;
                case LoadingMode.None:
                    Debug.Log($"[Mod:{SourceName}] => " + msg);
                    break;
                default:
                    Debug.Log(msg);
                    break;
            }  
        }

        public static void LogInformation(string msg)
        {
            switch (Mode)
            {
                case LoadingMode.BepInEx:
                    Logger!.LogInfo(msg);
                    break;
                case LoadingMode.None:
                    Debug.Log($"[Mod:{SourceName}] => " + msg);
                    break;
                default:
                    Debug.Log(msg);
                    break;
            }
        }

        public static void LogWarning(string msg)
        {
            switch (Mode)
            {
                case LoadingMode.BepInEx:
                    Logger!.LogWarning(msg);
                    break;
                case LoadingMode.None:
                    Debug.LogWarning($"[Mod:{SourceName}] => " + msg);
                    break;
                default:
                    Debug.LogWarning(msg);
                    break;
            }
        }

        public static void LogError(string msg)
        {
            switch (Mode)
            {
                case LoadingMode.BepInEx:
                    Logger!.LogError(msg);
                    break;
                case LoadingMode.None:
                    Debug.LogError($"[Mod:{SourceName}] => " + msg);
                    break;
                default:
                    Debug.LogError(msg);
                    break;
            }
        }
    }

    public enum LoadingMode
    {
        None,
        BepInEx
    }
}
