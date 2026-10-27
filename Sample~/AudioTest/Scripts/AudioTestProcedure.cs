using Cysharp.Threading.Tasks;
using Dories.Fsm.Runtime;
using Dories.Fsm.Runtime.Procedure;
using GameFramework.Runtime.GameBase;

namespace GameFramework.Test.AudioTest.Scripts
{
    /// <summary>
    /// 音效示例流程：补丁完成后进入，进入时预加载高频音效。
    /// </summary>
    public class AudioTestProcedure : ProcedureBase
    {
        public const string Click = "Click";

        protected override void OnEnter(FsmEntity<ProcedureSystem> fsm)
        {
            base.OnEnter(fsm);

            // 高频音效预加载并常驻，避免首次播放的加载延迟
            GameEntry.Audio.PreloadAsync(Click, keepAlive: true).Forget();
        }
    }
}
