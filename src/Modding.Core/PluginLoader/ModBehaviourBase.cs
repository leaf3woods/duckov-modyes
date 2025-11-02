using HarmonyLib;

namespace Modding.Core.PluginLoader
{
    public abstract class ModBehaviourBase : Duckov.Modding.ModBehaviour, IPlugin
    {
        protected Harmony Harmony { get; set; } = null!;

        /// <summary>
        /// 子类重写此属性定义自己的唯一Id
        /// </summary>
        protected abstract string PluginId { get; }

        /// <summary>
        /// 子类重写此属性定义自己的唯一名称
        /// </summary>
        protected abstract string PluginName { get; }

        protected ModLogger ModLogger { get; set; } = null!;

        /// <summary>
        ///     脚本启用的第一帧调用
        /// </summary>
        public virtual void Start()
        {
        }

        /// <summary>
        ///     销毁对象时调用
        /// </summary>
        public virtual void OnDestroy()
        {
            ModLogger.LogInformation("plugin object destroied");
        }

        public virtual void OnDisable()
        {
            BeforeUnpatching();
            Harmony.UnpatchAll(PluginId);
            ModLogger.LogInformation("plugin object disabled");
        }

        protected abstract void BeforeUnpatching();

        public abstract void OnEnable();

        /// <summary>
        ///     对象创建时调用（在 Start 前）
        /// </summary>
        public void Awake()
        {
            InitializeLogger();
            ModLogger = ModLogger.Initialize<ModBehaviourBase>(LoadingMode.None, PluginName);
            ModLogger.LogInformation($"plugin enabled, patching harmony({PluginId})...");
            Harmony = new Harmony(PluginId);
            ModLogger.LogInformation("harmony is created by bepinex");
        }

        public abstract void InitializeLogger();

        /// <summary>
        ///     每帧调用
        /// </summary>
        public virtual void Update()
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
