using HarmonyLib;
using UnityEngine;

namespace CustomBaseBgm.Loader
{
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        private Harmony _harmony = null!;

        /// <summary>
        ///     对象创建时调用（在 Start 前）
        /// </summary>
        protected void Awake()
        {
            Util.LoadByBepinEx = true;
        }

        /// <summary>
        ///     脚本启用的第一帧调用
        /// </summary>
        protected void Start()
        {
            _harmony.PatchAll();
            Debug.Log("mod is patched by modbehaviour");
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
            _harmony = new Harmony("yesmod.duckov.modbehaviour.custombgm");
            Debug.Log("harmony is created modbehaviour");
        }

        /// <summary>
        ///     禁用脚本时调用
        /// </summary>
        protected void OnDisable()
        {
            _harmony.UnpatchAll();
        }

        /// <summary>
        ///     销毁对象时调用
        /// </summary>
        protected void OnDestroy()
        {
            _harmony.UnpatchAll();
        }
    }
}
