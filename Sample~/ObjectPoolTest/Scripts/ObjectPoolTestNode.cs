using Cysharp.Threading.Tasks;
using Dories.ObjectPool.Runtime;
using GameFramework.Runtime.GameBase;
using UnityEngine;

namespace GameFramework.Test.ObjectPoolTest.Scripts
{
    public class ObjectPoolTestNode : MonoBehaviour
    {
        private GUIStyle _buttonStyle;
        private GUIStyle _labelStyle;

        private const string BulletKey = ObjectPoolTestProcedure.BulletKey;

        private async UniTaskVoid Fire(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var go = await GameEntry.ObjectPool.SpawnAsync(BulletKey, Vector3.zero, Random.rotation);
                if (go != null)
                    go.GetComponent<PoolBullet>().Pool = GameEntry.ObjectPool;
            }
        }

        private void OnGUI()
        {
            if (GameEntry.Procedure?.CurrentProcedure is not ObjectPoolTestProcedure) return;

            if (_buttonStyle == null)
            {
                _buttonStyle = new GUIStyle(GUI.skin.button) { fontSize = 24 };
                _labelStyle = new GUIStyle(GUI.skin.label) { fontSize = 24 };
            }

            if (GUI.Button(new Rect(10, 10, 200, 80), "Fire x10", _buttonStyle))
            {
                Fire(10).Forget();
            }

            if (GUI.Button(new Rect(10, 100, 200, 80), "Fire x100", _buttonStyle))
            {
                Fire(100).Forget();
            }

            if (GUI.Button(new Rect(10, 190, 200, 80), "Release Unused", _buttonStyle))
            {
                GameEntry.ObjectPool.ReleaseUnused();
            }

            if (GUI.Button(new Rect(10, 280, 200, 80), "Release Pool", _buttonStyle))
            {
                GameEntry.ObjectPool.ReleasePool(BulletKey);
            }

            GUI.Label(new Rect(230, 10, 400, 80),
                $"Active: {GameEntry.ObjectPool.CountActive(BulletKey)}  Inactive: {GameEntry.ObjectPool.CountInactive(BulletKey)}",
                _labelStyle);
        }
    }
}
