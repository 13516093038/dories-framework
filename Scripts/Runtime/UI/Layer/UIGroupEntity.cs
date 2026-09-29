using UnityEngine;

namespace GameFramework.Runtime.UI
{
    public class UIGroupEntity : MonoBehaviour
    {
        internal UIGroup _group;

        internal int GetUILayer(int LayerPriority)
        {
            return ((int)_group + LayerPriority * 100);
        }
    }
}