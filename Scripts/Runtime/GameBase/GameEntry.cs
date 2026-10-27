using System;
using Dories.Fsm.Runtime.Procedure;
using Dories.Logging;
using GameFramework.Runtime.GameBase.GameProfile;
using GameFramework.Runtime.UI;
using GameFramework.Runtime.Utils;
using UnityEngine;
#if DORIES_YOOASSET_SUPPORT
using Dories.YooassetSystem.Runtime.AssetLoader;
using Dories.YooAssetSystem.Runtime.Patch;
#endif
#if DORIES_OBJECTPOOL_SUPPORT
using Dories.ObjectPool.Runtime;
#endif
#if DORIES_AUDIO_SUPPORT
using Dories.Audio.Runtime;
#endif
#if DORIES_EVENT_SUPPORT
using Dories.Event.Runtime;
#endif

namespace GameFramework.Runtime.GameBase
{
    /// <summary>
    /// 游戏入口：跨场景常驻，静态暴露各系统，并按 Inspector 配置的流程启动游戏。
    /// 各系统挂在 GameEntry 的子节点上，Awake 时取到；流程在 Start 时注册并进入入口流程。
    /// </summary>
    public class GameEntry : MonoBehaviour
    {
        [SerializeField, Range(30, 120)] private float _targetFrameRate = 60f;
        [SerializeField, Range(0f, 10f)] private float _timeScale = 1f;

        [SerializeField] private string _gameProfileInfo;
        [SerializeField] private string _logSink;

        [SerializeField, Tooltip("要注册的流程类型（完整类型名）")] 
        private string[] _procedures = Array.Empty<string>();
        
        [SerializeField, Tooltip("启动时进入的流程")] 
        private string _entranceProcedure;

        public static IGameProfileInfo GameProfileInfo { get; private set; }
        public static ProcedureSystem Procedure { get; private set; }
        public static UISystem UI { get; private set; }
#if DORIES_YOOASSET_SUPPORT
        public static AssetLoaderEntity Asset { get; private set; }
        public static PatchEntity Patch { get; private set; }
#endif
#if DORIES_OBJECTPOOL_SUPPORT
        public static GameObjectPoolEntity ObjectPool { get; private set; }
#endif
#if DORIES_AUDIO_SUPPORT
        public static AudioEntity Audio { get; private set; }
#endif
#if DORIES_EVENT_SUPPORT
        public static EventEntity Event { get; private set; }
#endif

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            ApplyTimeSettings();

            // 留空则沿用 Log.Sink 当前值（默认 UnityLogSink，或已在代码里设置的实现）
            if (!string.IsNullOrEmpty(_logSink))
                Log.Sink = TypeUtil.CreateTypeInstance<ILogSink>(_logSink);

            GameProfileInfo = TypeUtil.CreateTypeInstance<IGameProfileInfo>(_gameProfileInfo);

            UI = GetComponentInChildren<UISystem>(true);
#if DORIES_YOOASSET_SUPPORT
            Asset = GetComponentInChildren<AssetLoaderEntity>(true);
            Patch = GetComponentInChildren<PatchEntity>(true);
#endif
#if DORIES_OBJECTPOOL_SUPPORT
            ObjectPool = GetComponentInChildren<GameObjectPoolEntity>(true);
#endif
#if DORIES_AUDIO_SUPPORT
            Audio = GetComponentInChildren<AudioEntity>(true);
#endif
#if DORIES_EVENT_SUPPORT
            Event = GetComponentInChildren<EventEntity>(true);
#endif

            Log.Info("GameEntry", $"GameProfileInfo: {GameProfileInfo.GameVersion}");
        }

        // 放在 Start：子节点各系统的 Awake 都已执行
        private void Start()
        {
            Procedure = new ProcedureSystem();
            Type entrance = null;
            foreach (var typeName in _procedures)
            {
                if (string.IsNullOrEmpty(typeName))
                    continue;

                var procedure = TypeUtil.CreateTypeInstance<ProcedureBase>(typeName);
                Procedure.RegisterProcedure(procedure);
                if (typeName == _entranceProcedure)
                    entrance = procedure.GetType();
            }

            if (entrance == null)
            {
                Log.Warn("GameEntry", "未配置入口流程或入口流程未勾选注册，不启动流程", this);
                return;
            }

            Procedure.StartProcedure(entrance);
        }

        private void OnDestroy()
        {
            Procedure?.Dispose();
            Procedure = null;
            UI = null;
#if DORIES_YOOASSET_SUPPORT
            Asset = null;
            Patch = null;
#endif
#if DORIES_OBJECTPOOL_SUPPORT
            ObjectPool = null;
#endif
#if DORIES_AUDIO_SUPPORT
            Audio = null;
#endif
#if DORIES_EVENT_SUPPORT
            Event = null;
#endif
        }

        // Play Mode 下在 Inspector 调整时即时生效
        private void OnValidate()
        {
            if (Application.isPlaying)
                ApplyTimeSettings();
        }

        private void ApplyTimeSettings()
        {
            Application.targetFrameRate = Mathf.RoundToInt(_targetFrameRate);
            Time.timeScale = _timeScale;
        }
    }
}
