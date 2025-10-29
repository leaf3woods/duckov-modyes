
using BepInEx.Logging;
using UnityEngine;

namespace Modding.Core
{
    public class ModLogger
    {
        public ManualLogSource? Logger { get; set; }
        public LoadingMode Mode { get; set; }
        public string SourceName { get; set; } = null!;

        public static ModLogger Initialize<T>(LoadingMode mode, string? sourceName = null)
        {
            var logger = new ModLogger
            {
                Mode = mode,
                SourceName = sourceName ?? typeof(T).Name,
            };
            if (mode == LoadingMode.BepInEx)
                logger.Logger = BepInEx.Logging.Logger.CreateLogSource(sourceName);
            return logger;
        }

        public void LogDebug(string msg)
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

        public void LogInformation(string msg)
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

        public void LogWarning(string msg)
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

        public void LogError(string msg)
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
