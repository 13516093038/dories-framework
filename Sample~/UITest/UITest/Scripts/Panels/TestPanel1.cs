using GameFramework.Runtime.UI;
using UnityEngine;

namespace GameFramework.Test.UITest.Scripts.Panels
{
    public class TestPanel1 : UIPanelEntity
    {
        private void Awake()
        {
            Debug.Log("TestPanel1 Awake");
        }

        private void OnEnable()
        {
            Debug.Log("TestPanel1 OnEnable");
        }

        private void Start()
        {
            Debug.Log("TestPanel1 Start");
        }

        protected override void OnCreate(object userData)
        {
            Debug.Log("TestPanel1 OnCreate");
        }

        protected override void OnOpen(object userData)
        {
            Debug.Log("TestPanel1 OnOpen");
        }

        protected override void OnClose(object userData)
        {
            Debug.Log("TestPanel1 OnClose");
        }

        protected override void OnRelease(object userData)
        {
            Debug.Log("TestPanel1 OnRelease");
        }
    }
}