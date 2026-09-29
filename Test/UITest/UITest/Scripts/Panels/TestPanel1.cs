using GameFramework.Runtime.UI;
using UnityEngine;

namespace GameFramework.Test.UITest.Scripts.Panels
{
    public class TestPanel1 : UIPanelEntity
    {
        protected internal override void OnCreate(object userData)
        {
            Debug.Log("TestPanel1 OnCreate");
        }
    }
}