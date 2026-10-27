using Dories.ObjectPool.Runtime;
using UnityEngine;

namespace GameFramework.Test.ObjectPoolTest.Scripts
{
    /// <summary>
    /// 池化子弹：OnEnable 即借出时重置状态，寿命到了自己回收。
    /// </summary>
    public class PoolBullet : MonoBehaviour
    {
        [SerializeField] private float _speed = 5f;
        [SerializeField] private float _lifeTime = 2f;

        private float _age;

        public GameObjectPoolEntity Pool { get; set; }

        private void OnEnable()
        {
            _age = 0f;
        }

        private void Update()
        {
            transform.Translate(Vector3.forward * (_speed * Time.deltaTime));

            _age += Time.deltaTime;
            if (_age >= _lifeTime && Pool != null)
                Pool.Despawn(gameObject);
        }
    }
}
