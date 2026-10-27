using GameFramework.Runtime.GameBase;
using UnityEngine;

namespace GameFramework.Test.EventTest.Scripts
{
    /// <summary>
    /// 发布方：只发消息，不知道谁在听。也负责创建/销毁订阅方，演示订阅随对象销毁自动结束。
    /// </summary>
    public class EventTestNode : MonoBehaviour
    {
        private GUIStyle _buttonStyle;
        private int _gold;
        [SerializeField] private GoldListener _listener;

        private void OnGUI()
        {
            if (GameEntry.Procedure?.CurrentProcedure is not EventTestProcedure) return;

            _buttonStyle ??= new GUIStyle(GUI.skin.button) { fontSize = 24 };

            if (GUI.Button(new Rect(10, 10, 200, 80), "+30 Gold", _buttonStyle))
            {
                _gold += 30;
                GameEntry.Event.Publish(new GoldChanged { Gold = _gold });
            }

            if (GUI.Button(new Rect(10, 100, 200, 80), "Hit", _buttonStyle))
            {
                GameEntry.Event.Publish(new PlayerHit { Damage = 1 });
            }

            if (GUI.Button(new Rect(10, 190, 200, 80), "Hit x10", _buttonStyle))
            {
                for (int i = 0; i < 10; i++)
                    GameEntry.Event.Publish(new PlayerHit { Damage = 1 });
            }

            if (GUI.Button(new Rect(10, 280, 200, 80), _listener == null ? "Spawn Listener" : "Destroy Listener", _buttonStyle))
            {
                if (_listener == null)
                    _listener = new GameObject("GoldListener").AddComponent<GoldListener>();
                else
                    Destroy(_listener.gameObject);
            }
        }
    }
}
