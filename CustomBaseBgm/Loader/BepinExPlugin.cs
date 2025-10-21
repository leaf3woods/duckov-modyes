using BepInEx;
using HarmonyLib;

namespace CustomBaseBgm.Loader
{
    [BepInPlugin("yesmod.duckov.bepinex.custombgm", "Custom Base BGM", "1.0.0")]
    //[BepInDependency("com.bepinex.plugin.important")]
    public class BepinExPlugin : BaseUnityPlugin
    {
        private Harmony _harmony = null!;

        /// <summary>
        ///     对象创建时调用（在 Start 前）
        /// </summary>
        protected void Awake()
        {
            Util.LoadByBepinEx = true;
            Util.Logger =  BepInEx.Logging.Logger.CreateLogSource(nameof(CustomBaseBgm));
        }

        /// <summary>
        ///     脚本启用的第一帧调用
        /// </summary>
        protected void Start()
        {
        }

        /// <summary>
        ///     每帧调用
        /// </summary>
        protected void Update()
        {

        }

        /// <summary>
        ///     固定时间步（物理更新）
        /// </summary>
        protected void FixedUpdate()
        {

        }


        /// <summary>
        ///     所有 Update 之后调用
        /// </summary>
        protected void LateUpdate()
        {

        }

        /// <summary>
        ///     启用脚本时调用
        /// </summary>
        protected void OnEnable()
        {
            _harmony = new Harmony("yesmod.duckov.bepinex.custombgm");
            Util.LogInformation("harmony is created by bepinex");
            _harmony.PatchAll();
            Util.LogInformation("mod is patched by bepinex");
        }

        /// <summary>
        ///     禁用脚本时调用
        /// </summary>
        protected void OnDisable()
        {
            //_harmony.UnpatchAll();
        }

        /// <summary>
        ///     销毁对象时调用
        /// </summary>
        protected void OnDestroy()
        {
            //_harmony.UnpatchAll();
        }
    }
}
