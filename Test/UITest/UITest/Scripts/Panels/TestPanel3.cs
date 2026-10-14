using GameFramework.Runtime.UI;
using UnityEngine;

namespace GameFramework.Test.UITest.Scripts.Panels
{
    public class TestPanel3 : UIPanelEntity
    {
        protected override void OnCreate(object userData)
        {
            base.OnCreate(userData);
            Debug.Log("TestPanel3 OnCreate");
        }
    }
}