using Cysharp.Threading.Tasks;
using Duckov.UI.DialogueBubbles;
using HarmonyLib;
using Modding.Core;
using System.Collections.Generic;

namespace Modding.CustomBaseBgm.Patches
{
    /// <summary>
    ///     the patch for interactable
    /// </summary>
    [HarmonyPatch(typeof(InteractableBase))]
    public class InteractablePatch : IPatching
    {
        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        public static void InteractableBaseStartPostfix(InteractableBase __instance, ref List<InteractableBase> ___otherInterablesInGroup)
        {
            if (___otherInterablesInGroup.Count == 1 && ___otherInterablesInGroup[0] &&
                ___otherInterablesInGroup[0].name == "Last")
            {
                PluginCore.ModLogger.LogInformation("add player mode interact...");
                var original = ___otherInterablesInGroup[0];
                var selectMode = UnityEngine.Object.Instantiate(original, original.transform.parent);
                selectMode.name = "NextMode";
                selectMode.InteractName = $"切换播放模式";
                selectMode.enabled = true;
                selectMode.MarkerActive = true;
                selectMode.interactableGroup = ___otherInterablesInGroup[0].interactableGroup;
                selectMode.OnInteractStartEvent.RemoveAllListeners();
                selectMode.OnInteractFinishedEvent.RemoveAllListeners();
                //selectMode.OnInteractFinishedEvent.AddListener(HandleInteractFinished);
                ___otherInterablesInGroup.Add(selectMode);

                var togglePausle = UnityEngine.Object.Instantiate(original, original.transform.parent);
                togglePausle.name = "TogglePause";
                togglePausle.InteractName = $"暂停/播放";
                togglePausle.enabled = true;
                togglePausle.MarkerActive = true;
                togglePausle.interactableGroup = ___otherInterablesInGroup[0].interactableGroup;
                togglePausle.OnInteractStartEvent.RemoveAllListeners();
                togglePausle.OnInteractFinishedEvent.RemoveAllListeners();
                //selectMode.OnInteractFinishedEvent.AddListener(HandleInteractFinished);
                ___otherInterablesInGroup.Add(togglePausle);
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(InteractableBase.StartInteract))]
        public static bool StartInteractPrefix(InteractableBase __instance, CharacterMainControl _interactCharacter)
        {
            if (__instance.name == "NextMode")
            {
                // 阻止原逻辑
                PluginCore.ModLogger.LogInformation($"handle interact finfished, interact is: {__instance.name}, {__instance.InteractName}");
                PluginCore.NextMode();
                var msg = $"已切换至：[{PluginCore.MusicPlayer.LoopModePlainText}]!";
                DialogueBubblesManager.Show(msg, __instance.transform, 0.8f, false, false, 200f, 2f).Forget();
                return false; // false => 阻止原 StartInteract 执行
            }
            else if (__instance.name == "TogglePause")
            {
                // 阻止原逻辑
                PluginCore.ModLogger.LogInformation($"handle interact finfished, interact is: {__instance.name}, {__instance.InteractName}");
                PluginCore.MusicPlayer.TogglePause();
                var msg = $"已 [{(PluginCore.MusicPlayer.IsPasued ? "暂停" : "恢复")}]!";
                DialogueBubblesManager.Show(msg, __instance.transform, 0.8f, false, false, 200f, 2f).Forget();
                return false; // false => 阻止原 StartInteract 执行
            }
            return true;
        }
    }
}
