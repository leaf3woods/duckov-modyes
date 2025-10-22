using HarmonyLib;

namespace CustomBaseBgm
{
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        private Harmony _harmony = null!;

        /// <summary>
        ///     对象创建时调用（在 Start 前）
        /// </summary>
        public void Awake()
        {
            Util.LoadByBepinEx = false;
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
            Util.LogInformation($"plugin enabled, patching harmony({Util.OfficalPluginUuid})...");
            _harmony = new Harmony(Util.OfficalPluginUuid);
            Util.LogInformation("harmony is created by offical plugin");
            _harmony.PatchAll();
            Util.LogInformation("mod is patched by offical plugin");
            SceneLoader.onBeforeSetSceneActive += BaseBgmPatch.StopRuntimeBgm;
        }

        /// <summary>
        ///     禁用脚本时调用
        /// </summary>
        public void OnDisable()
        {
            Util.LogInformation("plugin disabled, unpatch harmony...");
            _harmony.UnpatchAll(Util.OfficalPluginUuid);
            SceneLoader.onBeforeSetSceneActive -= BaseBgmPatch.StopRuntimeBgm;
        }

        /// <summary>
        ///     销毁对象时调用
        /// </summary>
        public void OnDestroy()
        {
            Util.LogInformation("plugin destroied");
            //_harmony.UnpatchAll();
        }
    }
}
