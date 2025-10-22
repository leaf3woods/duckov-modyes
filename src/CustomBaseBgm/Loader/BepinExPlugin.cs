using BepInEx;
using HarmonyLib;

namespace CustomBaseBgm.Loader
{
    [BepInPlugin(Util.BepinExUuid, "Custom Base BGM", "1.0.0")]
    //[BepInDependency("com.bepinex.plugin.important")]
    public class BepinExPlugin : BaseUnityPlugin
    {
        private Harmony _harmony = null!;

        /// <summary>
        ///     对象创建时调用（在 Start 前）
        /// </summary>
        public void Awake()
        {
            Util.LoadByBepinEx = true;
            Util.Logger =  BepInEx.Logging.Logger.CreateLogSource(nameof(CustomBaseBgm));
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
            Util.LogInformation($"plugin enabled, patching harmony({Util.BepinExUuid})...");
            _harmony = new Harmony(Util.BepinExUuid);
            Util.LogInformation("harmony is created by bepinex");
            _harmony.PatchAll();
            Util.LogInformation("mod is patched by bepinex");
            SceneLoader.onBeforeSetSceneActive += BaseBgmPatch.StopRuntimeBgm;
        }

        /// <summary>
        ///     禁用脚本时调用
        /// </summary>
        public void OnDisable()
        {
            Util.LogInformation("plugin disabled, unpatch harmony...");
            _harmony.UnpatchAll(Util.BepinExUuid);
            SceneLoader.onBeforeSetSceneActive -= BaseBgmPatch.StopRuntimeBgm;
        }

        /// <summary>
        ///     销毁对象时调用
        /// </summary>
        public void OnDestroy()
        {
            //_harmony.UnpatchAll();
        }
    }
}
