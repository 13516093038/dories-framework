using GameFramework.Runtime.UI;
using UnityEngine;

namespace GameFramework.Test.UITest.Scripts.Panels
{
    public class TestPanel2 : UIPanelEntity
    {
        protected internal override void OnCreate(object userData)
        {
            base.OnCreate(userData);
            Debug.Log("TestPanel2 OnCreate");
        }
    }
}