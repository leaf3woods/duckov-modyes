using Duckov;
using HarmonyLib;
using Modding.Core;

namespace Modding.CustomBaseBgm.Patches
{
    /// <summary>
    ///     the patch for option
    /// </summary>
    [HarmonyPatch]
    public class OptionPatch : IPatching
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UI_Bus_Slider), "OnValueChanged")]
        public static void OnValueChangedPostfixPatch(AudioManager.Bus ___busRef)
        {
            if (___busRef.Name == Shared.MasterBus)
            {
                PluginCore.MasterVolume = ___busRef.Volume;
            }
            else if (___busRef.Name == Shared.MusicBus)
            {
                PluginCore.MusicVolume = ___busRef.Volume;
            }
            //  当用户调整此总线上的音量时，设置运行时音乐
            PluginCore.MusicPlayer?.ApplyVolume(PluginCore.Volume);
        }

    }
}
