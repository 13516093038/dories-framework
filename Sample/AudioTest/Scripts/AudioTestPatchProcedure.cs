using Dories.Fsm.Runtime;
using Dories.Fsm.Runtime.Procedure;
using GameFramework.Runtime.GameBase;
using UnityEngine;

namespace GameFramework.Test.AudioTest.Scripts
{
    /// <summary>
    /// 示例入口流程：跑补丁，完成后设默认包，进入 AudioTestProcedure。
    /// 补丁流程属于项目业务（下载确认、失败重试等按需处理），框架不内置。
    /// </summary>
    public class AudioTestPatchProcedure : ProcedureBase
    {
        protected override void OnEnter(FsmEntity<ProcedureSystem> fsm)
        {
            base.OnEnter(fsm);

            GameEntry.Patch
                .BuildNeedUpdateListener(downloader => downloader.StartDownload())
                .BuildPatchCompleteListener(() =>
                {
                    GameEntry.Asset.SetDefaultPackage("DefaultPackage");
                    ChangeState<AudioTestProcedure>(fsm);
                })
                .BuildPatchFailedListener(error => Debug.LogError($"Patch failed: {error}"))
                .BuildPatchErrorListener(error => Debug.LogError($"Patch error: {error}"))
                .StartPatch();
        }
    }
}
