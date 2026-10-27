using System;
using GameFramework.Runtime.GameBase;
using R3;
using UnityEngine;

namespace GameFramework.Test.EventTest.Scripts
{
    /// <summary>
    /// 订阅方：只认识消息类型，不引用发布方。AddTo(this) 让订阅随本对象销毁自动结束。
    /// </summary>
    public class GoldListener : MonoBehaviour
    {
        private int _gold;
        private int _hitCount;
        private int _throttledHitCount;
        private string _milestone = "-";
        private GUIStyle _labelStyle;

        private void Start()
        {
            var events = GameEntry.Event;

            events.Receive<GoldChanged>()
                .Subscribe(e => _gold = e.Gold)
                .AddTo(this);

            // 操作符：只关心第一次金币过百
            events.Receive<GoldChanged>()
                .Where(e => e.Gold >= 100)
                .Take(1)
                .Subscribe(e => _milestone = $"首次过百: {e.Gold}")
                .AddTo(this);

            events.Receive<PlayerHit>()
                .Subscribe(_ => _hitCount++)
                .AddTo(this);

            // 操作符：受击反馈 1 秒内只响应一次，连点也不会刷屏
            events.Receive<PlayerHit>()
                .ThrottleFirst(TimeSpan.FromSeconds(1))
                .Subscribe(_ => _throttledHitCount++)
                .AddTo(this);
        }

        private void OnGUI()
        {
            _labelStyle ??= new GUIStyle(GUI.skin.label) { fontSize = 24 };

            GUI.Label(new Rect(440, 10, 500, 40), $"[Listener] Gold: {_gold}", _labelStyle);
            GUI.Label(new Rect(440, 50, 500, 40), $"[Listener] {_milestone}", _labelStyle);
            GUI.Label(new Rect(440, 90, 500, 40), $"[Listener] Hit: {_hitCount}  Throttled: {_throttledHitCount}", _labelStyle);
        }
    }
}
