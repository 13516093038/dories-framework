# Changelog

## [1.0.2] - 2026-10-27

### Changed

- 示例目录 `Sample` 改回 `Sample~`，与 package.json 的 samples 路径一致，未导入时不参与使用方工程的导入与编译
- 包内不再带 `AssemblyScanConfig` 与 `BundleCollectorSetting` 资产，由使用方工程自己持有；避免与工程自有的 YooAsset 收集器配置冲突（YooAsset 要求全工程唯一）

## [1.0.1] - 2026-10-14

### Fixed

- UniTask 与 YooAssetSystem 改为 Git 依赖，避免 Package Manager 按注册表版本号解析失败

## [1.0.0] - 2026-10-14

### Added

- 补齐 UPM 包描述、许可，以及 Runtime / Editor 程序集定义
