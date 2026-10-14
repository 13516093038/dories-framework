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

## 程序集

- `GameFramework.Runtime`：`Scripts/Runtime`
- `GameFramework.Editor`：`Scripts/Editor`，仅编辑器

`DORIES_UNITASK_SUPPORT` 与 `DORIES_YOOASSET_SUPPORT` 会在对应包存在时由程序集自动定义。抖音安全区代码包在 `UNITY_WEBGL && DOUYINMINIGAME` 内。

## 内容

- **GameEntry**：启动时创建 `IGameProfileInfo`。
- **UISystem / PanelScheduler**：按层级打开、关闭、预加载和弹出面板。
- **SafeArea**：按平台读取安全区。通用实现走 `Screen.safeArea`，抖音小游戏走 `TT.GetSystemInfo()`。
