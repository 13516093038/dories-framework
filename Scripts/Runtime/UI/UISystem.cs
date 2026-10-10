using System;
using System.Runtime.CompilerServices;
using UnityEngine;
#if TASK_BASED_OPEN_PANEL
using System.Threading.Tasks;
using GameFramework.Runtime.Utils;
#endif
#if DORIES_UNITASK_SUPPORT
using Cysharp.Threading.Tasks;
#endif

namespace GameFramework.Runtime.UI
{
    //UIRoot Canvas CanvasScaler
    //--Mian Group 0-100
    //---Panel Canvas 
    //--Window 101 -200
    //--PopUp 201 -300
    //--Top 301 -400

    [RequireComponent(typeof(Canvas))]
    public class UISystem : MonoBehaviour
    {
        [SerializeField] private bool _autoInit = true;
        [SerializeField] private bool _isAutoRelease = true;
        [SerializeField] private int _autoReleaseTime = 60;
        [SerializeField] private string _resourceProviderName;
        [SerializeField] private string _panelAddressResolverName;
        [SerializeField] private string _panelAddressName;

        private IResourceProvider _resourceProvider;
        private IPanelAddressResolver _panelAddressResolver;
        private IPanelAddress _panelAddress; 

        private Canvas _cachedCanvas;

        private bool _isInit;
        private PanelScheduler _panelScheduler;

        public bool IsInit => _isInit;
        public bool IsPopuping => _panelScheduler.IsPopuping;

        /// <summary>
        /// UI摄像机
        /// </summary>
        public Camera UICamera
        {
            get
            {
                ThrowIfNotInit();
                if (_cachedCanvas.renderMode is RenderMode.ScreenSpaceOverlay)
                {
                    return null;
                }

                return _cachedCanvas.worldCamera;
            }
            set
            {
                ThrowIfNotInit();
                if (_cachedCanvas.renderMode is RenderMode.ScreenSpaceOverlay)
                {
                    return;
                }

                _cachedCanvas.worldCamera = value;
            }
        }

        private void Awake()
        {
            if (_resourceProviderName != null)
            {
                _resourceProvider = TypeUtil.CreateTypeInstance<IResourceProvider>(_resourceProviderName);
            }

            if (_panelAddressResolverName != null)
            {
                _panelAddressResolver = TypeUtil.CreateTypeInstance<IPanelAddressResolver>(_panelAddressResolverName);
            }

            if (_panelAddressName != null)
            {
                _panelAddress = TypeUtil.CreateTypeInstance<IPanelAddress>(_panelAddressName);
            }

            if (_autoInit)
            {
                Init();
            }
        }

        private void Update()
        {
            if(!_isInit) return;

            if(_isAutoRelease)
            {
                _panelScheduler.AutoReleaseUpdate(_autoReleaseTime);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ThrowIfNotInit()
        {
            if(!_isInit)
            {
                throw new InvalidOperationException("UISystem not initialized");
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void Init(IResourceProvider resourceProvider = null, IPanelAddressResolver panelAddressResolver = null,
            IPanelAddress panelAddress = null)
        {
            if (_isInit)
            {
                Debug.LogWarning("UISystem already initialized");
                return;
            }

            _cachedCanvas = GetComponent<Canvas>();

            if (_cachedCanvas == null)
            {
                throw new InvalidOperationException("Canvas component is required");
            }

            if (resourceProvider != null)
            {
                _resourceProvider = resourceProvider;
            }

            if (panelAddressResolver != null)
            {
                _panelAddressResolver = panelAddressResolver;
            }

            if (panelAddress != null)
            {
                _panelAddress = panelAddress;
            }
            _panelScheduler = new PanelScheduler(_panelAddressResolver, _resourceProvider, this.transform);
            _isInit = true;
        }

        #region Open Panel

#if TASK_BASED_OPEN_PANEL
        public async Task<UIPanelEntity> OpenPanelAsync(Type panelType, UIGroup group, object userData = null, int layerPriority = 0)
        {
            ThrowIfNotInit();
            return await _panelScheduler.OpenPanelAsync(panelType, group, userData, layerPriority);
        }
#endif

#if DORIES_UNITASK_SUPPORT
        public async UniTask<UIPanelEntity> OpenPanelAsyncUniTask(Type panelType, UIGroup group, object userData = null, int layerPriority = 0)
        {
            ThrowIfNotInit();
            return await _panelScheduler.OpenPanelAsyncUniTask(panelType, group, userData, layerPriority);
        }
#endif

        public void OpenPanelAsync(Type panelType, UIGroup group, object userData = null, Action<UIPanelEntity> onComplete = null, int layerPriority = 0)
        {
            ThrowIfNotInit();
            _panelScheduler.OpenPanelAsync(panelType, group, userData, onComplete, layerPriority);
        }

        public void OpenPanelAsync<T>(UIGroup group, object userData = null, Action<UIPanelEntity> onComplete = null, int layerPriority = 0) where T : UIPanelEntity
        {
            ThrowIfNotInit();
            _panelScheduler.OpenPanelAsync<T>(group, userData, onComplete, layerPriority);
        }

        #endregion

        #region Close Panel

        public void ClosePanel(Type panelType, object userData = null)
        {
            ThrowIfNotInit();
            _panelScheduler.ClosePanel(panelType, userData);
        }

        public void ClosePanel<T>(object userData = null) where T : UIPanelEntity
        {
            ThrowIfNotInit();
            _panelScheduler.ClosePanel<T>(userData);
        }

        #endregion

        #region Release Panel

        public void ReleasePanel(Type panelType, object userData = null)
        {
            ThrowIfNotInit();
            _panelScheduler.ReleasePanel(panelType, userData);
        }

        public void ReleasePanel<T>(object userData = null) where T : UIPanelEntity
        {
            ThrowIfNotInit();
            _panelScheduler.ReleasePanel<T>(userData);
        }

        #endregion

        #region Preload Panel

        public void PreloadPanel(Type panelType, Action onComplete = null)
        {
            ThrowIfNotInit();
            _panelScheduler.PreloadPanel(panelType, onComplete);
        }

        public void PreloadPanel<T>(Action onComplete = null) where T : UIPanelEntity
        {
            ThrowIfNotInit();
            _panelScheduler.PreloadPanel<T>(onComplete);
        }

#if TASK_BASED_OPEN_PANEL
        public async Task PreloadPanelAsync(Type panelType)
        {
            ThrowIfNotInit();
            var task = new TaskCompletionSource<bool>();
            _panelScheduler.PreloadPanel(panelType, () =>
            {
                task.SetResult(true);
            });
            await task.Task;
        }

        public async Task PreloadPanelAsync<T>() where T : UIPanelEntity
        {
            ThrowIfNotInit();
            var task = new TaskCompletionSource<bool>();
            _panelScheduler.PreloadPanel<T>(() =>
            {
                task.SetResult(true);
            });
            await task.Task;
        }
#endif

#if DORIES_UNITASK_SUPPORT
        public async UniTask PreloadPanelAsyncUniTask(Type panelType)
        {
            ThrowIfNotInit();
            var task = new UniTaskCompletionSource();
            _panelScheduler.PreloadPanel(panelType, () =>
            {
                task.TrySetResult();
            });
            await task.Task;
        }

        public async UniTask PreloadPanelAsyncUniTask<T>() where T : UIPanelEntity
        {
            ThrowIfNotInit();
            var task = new UniTaskCompletionSource();
            _panelScheduler.PreloadPanel<T>(() =>
            {
                task.TrySetResult();
            });
            await task.Task;
        }
#endif

        #endregion

        #region Popup Panel

        /// <summary>
        /// 开始弹窗
        /// </summary>
        public void StartPopupPanel()
        {
            ThrowIfNotInit();
            _panelScheduler.StartPopupPanel();
        }

        public void AddPopUpPanel(Type panelType, Func<uint> getPopUpPriority, UIGroup group, bool isPreload = true, object userData = null, int layerPriority = 0)
        {
            ThrowIfNotInit();
            _panelScheduler.AddPopUpPanel(panelType, getPopUpPriority, group, isPreload, userData, layerPriority);
        }

        public void AddPopUpPanel(Type panelType, uint popUpPriority, UIGroup group, bool isPreload = true, object userData = null, int layerPriority = 0)
        {
            ThrowIfNotInit();
            _panelScheduler.AddPopUpPanel(panelType, popUpPriority, group, isPreload, userData, layerPriority);
        }

        public void AddPopUpPanel<T>(Func<uint> getPopUpPriority, UIGroup group, bool isPreload = true, object userData = null, int layerPriority = 0) where T : UIPanelEntity
        {
            ThrowIfNotInit();
            _panelScheduler.AddPopUpPanel<T>(getPopUpPriority, group, isPreload, userData, layerPriority);
        }

        public void AddPopUpPanel<T>(uint popUpPriority, UIGroup group, bool isPreload = true, object userData = null, int layerPriority = 0) where T : UIPanelEntity
        {
            ThrowIfNotInit();
            _panelScheduler.AddPopUpPanel<T>(popUpPriority, group, isPreload, userData, layerPriority);
        }

        public void RemovePopUpPanel(Type panelType)
        {
            ThrowIfNotInit();
            _panelScheduler.RemovePopUpPanel(panelType);
        }

        public void RemovePopUpPanel<T>() where T : UIPanelEntity
        {
            ThrowIfNotInit();
            _panelScheduler.RemovePopUpPanel<T>();
        }

        #endregion


    }
}