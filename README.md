# Dories GameFramework

游戏入口、UI 面板调度与平台安全区适配。

## 安装

在 Package Manager 中用 Git URL 添加：

```text
https://github.com/13516093038/dories-framework.git
```

会一并拉取：

- UniTask `2.5.11`：`https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask#2.5.11`
- YooAssetSystem：`https://github.com/13516093038/dories-yooassetsystem.git#split/yooassetsystem`
- `com.unity.ugui` 1.0.0

`Prefabs/GameEntry.prefab` 里的 `ObjectPool`、`Audio`、`Event` 节点分别需要 `com.dories.objectpool`、`com.dories.audio`、`com.dories.event`（后者还需 R3），不用可以删掉对应节点。

## 程序集

- `GameFramework.Runtime`：`Scripts/Runtime`
- `GameFramework.Editor`：`Scripts/Editor`，仅编辑器

`DORIES_UNITASK_SUPPORT` 与 `DORIES_YOOASSET_SUPPORT` 会在对应包存在时由程序集自动定义。抖音安全区代码包在 `UNITY_WEBGL && DOUYINMINIGAME` 内。

## 工程配置

包内不带任何配置资产，以下配置都放在使用方工程的 `Assets` 下：

- **AssemblyScanConfig**：GameEntry Inspector 从这些程序集里查找流程类型。首次需要时自动创建 `Assets/AssemblyScanConfig.asset`（默认扫描 `Assembly-CSharp` 和 `GameFramework.Runtime`），把业务程序集加进去，否则 Inspector 会把业务流程当残留项清掉。
- **YooAsset 收集器配置**（`BundleCollectorSetting`）：工程内只能有一份，由工程自己建。

## Sample

Package Manager 中导入 **UI Test**。示例在 `Sample~`，未导入时不参与工程编译。示例用到的收集器配置在 `BundleCollectorConfig.xml`，导入后在 YooAsset 收集器窗口里导入该 xml。

## 内容

- **GameEntry**：跨场景常驻（重复的会被销毁），应用帧率与时间缩放，创建 `IGameProfileInfo`，并静态暴露各系统：`GameEntry.UI / Asset / Patch / ObjectPool / Audio / Event / Procedure`（对应包存在时才有）。
- **启动流程**：GameEntry Inspector 里只勾选要注册的流程、选入口流程；流程之间的跳转由各流程用 `ChangeState<T>(fsm)` 自己决定。
- 补丁、登录等业务流程由项目自己写成 `ProcedureBase` 子类，框架不内置，可参考示例里的 `XxxPatchProcedure`：

```csharp
public class ProcedureLaunch : ProcedureBase
{
    protected override void OnEnter(FsmEntity<ProcedureSystem> fsm)
    {
        base.OnEnter(fsm);
        GameEntry.Patch
            .BuildNeedUpdateListener(downloader => downloader.StartDownload())
            .BuildPatchCompleteListener(() =>
            {
                GameEntry.Asset.SetDefaultPackage("DefaultPackage");
                ChangeState<ProcedureMenu>(fsm);
            })
            .StartPatch();
    }
}
```

```csharp
public class ProcedureMenu : ProcedureBase
{
    protected override void OnEnter(FsmEntity<ProcedureSystem> fsm)
    {
        base.OnEnter(fsm);
        GameEntry.Audio.PlayMusicAsync("MenuBgm").Forget();
        GameEntry.UI.OpenPanelAsync<MenuPanel>(UIGroup.Main);
    }
}
```
- **ObjectPool**：`GameEntry` 预制体下的 `GameObjectPoolEntity`，自动用同场景的 `AssetLoaderEntity` 加载预制体，Inspector 在 Play Mode 显示各池状态。
- **Audio**：`GameEntry` 预制体下的 `AudioEntity`，背景音乐淡入淡出、音效复用、分通道音量，AudioClip 经 `AssetLoaderEntity` 加载。
- **Event**：`GameEntry` 预制体下的 `EventEntity`，基于 R3 的类型化事件总线，`Publish` / `Receive<T>()`。
- **UISystem / PanelScheduler**：按层级打开、关闭、预加载和弹出面板。
- **SafeArea**：按平台读取安全区。通用实现走 `Screen.safeArea`，抖音小游戏走 `TT.GetSystemInfo()`。
