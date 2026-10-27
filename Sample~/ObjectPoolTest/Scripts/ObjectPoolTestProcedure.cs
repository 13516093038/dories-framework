using Cysharp.Threading.Tasks;
using Dories.Fsm.Runtime;
using Dories.Fsm.Runtime.Procedure;
using GameFramework.Runtime.GameBase;

namespace GameFramework.Test.ObjectPoolTest.Scripts
{
    /// <summary>
    /// 对象池示例流程：补丁完成后进入，进入时预热子弹池。
    /// </summary>
    public class ObjectPoolTestProcedure : ProcedureBase
    {
        // 收集规则 AddressByFileName，地址即预制体文件名
        public const string BulletKey = "PoolBullet";

        protected override void OnEnter(FsmEntity<ProcedureSystem> fsm)
        {
            base.OnEnter(fsm);

            // 预热 20 个，keepAlive 后不参与空闲释放
            GameEntry.ObjectPool.PrewarmAsync(BulletKey, 20, keepAlive: true).Forget();
        }
    }
}
