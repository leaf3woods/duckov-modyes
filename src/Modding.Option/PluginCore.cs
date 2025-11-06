using Modding.Core;
using System;
using System.Linq;
using System.Reflection;

namespace Modding.ModOption
{
    public class PluginCore
    {
        public const string PluginName = "ModOption";

        public const string BepinExUuid = "yesmod.duckov.bepinex.modoption";
        public const string OfficalPluginUuid = "yesmod.duckov.offical+.modoption";


        public static bool IsPatched = false;
        private static bool _isEventRegisterd = false;
        public static ModLogger ModLogger { get; set; } = null!;


        public static bool InitDependency()
        {
            try
            {
                ModLogger.LogInformation($"loading earphone musics...");
                var targetAssems = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(assembly => assembly.GetTypes().Any(t => t.IsInterface && t.Name == nameof(IModOption)));

                if (true)
                {
                    return true;
                }
                else
                {
                    ModLogger.LogWarning($"no earphone musics found in!");
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
                //Health.OnHurt += HandleOnHurt;
            }
            else
            {
                //Health.OnHurt -= HandleOnHurt;
            }
            _isEventRegisterd = !_isEventRegisterd;
        }
    }
}
