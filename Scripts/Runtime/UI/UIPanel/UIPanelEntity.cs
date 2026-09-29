using UnityEngine;

namespace GameFramework.Runtime.UI
{
    public class UIPanelEntity : MonoBehaviour
    {
        private PanelLayerEntity _groupEntity;

        protected int Layer => _groupEntity.Layer;
        protected UIGroup Group => _groupEntity.Group;

        /// <summary>
        /// 界面首次打开加载完毕时调用
        /// </summary>
        protected internal virtual void OnCreate(object userData)
        {

        }

        /// <summary>
        /// 界面每次打开时调用
        /// </summary>
        protected internal virtual void OnOpen(object userData)
        {

        }

        /// <summary>
        /// 界面关闭时调用
        /// </summary>
        protected internal virtual void OnClose(object userData)
        {

        }

        /// <summary>
        /// 界面释放时调用
        /// </summary>
        protected internal virtual void OnRelease(object userData)
        {

        }
    }
}