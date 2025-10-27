using BepInEx;
using HarmonyLib;

namespace Modding.Core.PluginLoader
{
    public abstract class BepInExBase : BaseUnityPlugin, IPlugin
    {
        protected string Id { get; private set; } = null!;
        protected Harmony Harmony { get; set; } = null!;

        /// <summary>
        /// 子类重写此属性定义自己的唯一ID
        /// </summary>
        protected abstract string PluginId { get; }

        /// <summary>
        ///     脚本启用的第一帧调用
        /// </summary>
        public virtual void Start()
        {
        }

        public virtual void OnDestroy()
        {
            //Harmony.UnpatchAll(Id);
            ModLogger.LogInformation("plugin object destroied");
        }

        public virtual void OnDisable()
        {
            //Harmony.UnpatchAll(Id);
            ModLogger.LogInformation("plugin object disabled");
        }

        public abstract void OnEnable();

        /// <summary>
        ///     对象创建时调用（在 Start 前）
        /// </summary>
        public virtual void Awake()
        {
            Id = PluginId;
            ModLogger.Initialize<BepInExBase>(LoadingMode.BepInEx);
            ModLogger.LogInformation($"plugin enabled, patching harmony({Id})...");
            Harmony = new Harmony(Id);
            ModLogger.LogInformation("harmony is created by bepinex");        
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
    }
}
