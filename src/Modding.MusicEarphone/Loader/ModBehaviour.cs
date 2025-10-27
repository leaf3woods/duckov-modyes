using HarmonyLib;
using Modding.Core;

namespace Modding.MusicEarphone.Loader
{
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        private Harmony _harmony = null!;

        /// <summary>
        ///     对象创建时调用（在 Start 前）
        /// </summary>
        public void Awake()
        {
            ModLogger.Initialize<ModBehaviour>(LoadingMode.None);
        }

        /// <summary>
        ///     脚本启用的第一帧调用
        /// </summary>
        public void Start()
        {
        }

        /// <summary>
        ///     每帧调用
        /// </summary>
        public void Update()
        {

        }

        /// <summary>
        ///     固定时间步（物理更新）
        /// </summary>
        public void FixedUpdate()
        {

        }


        /// <summary>
        ///     所有 Update 之后调用
        /// </summary>
        public void LateUpdate()
        {

        }

        /// <summary>
        ///     启用脚本时调用
        /// </summary>
        public void OnEnable()
        {
            ModLogger.LogInformation($"plugin enabled, patching harmony({Util.OfficalPluginUuid})...");
            _harmony = new Harmony(Util.OfficalPluginUuid);
            ModLogger.LogInformation("harmony is created by offical plugin");
            _harmony.PatchAll();
            ModLogger.LogInformation("mod is patched by offical plugin");
            //SceneLoader.onBeforeSetSceneActive += BaseBgmPatch.StopRuntimeBgm;
        }

        /// <summary>
        ///     禁用脚本时调用
        /// </summary>
        public void OnDisable()
        {
            ModLogger.LogInformation("plugin disabled, unpatch harmony...");
            _harmony.UnpatchAll(Util.OfficalPluginUuid);
            //SceneLoader.onBeforeSetSceneActive -= BaseBgmPatch.StopRuntimeBgm;
        }

        /// <summary>
        ///     销毁对象时调用
        /// </summary>
        public void OnDestroy()
        {
            ModLogger.LogInformation("plugin destroied");
            //_harmony.UnpatchAll();
        }
    }
}
