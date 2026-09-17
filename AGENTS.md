# 仓库指南

## 项目结构与模块组织

本仓库是基于 .NET 8 的 Windows Forms 桌面应用。`Program.cs` 负责配置日志并启动 `MainForm`。主要业务逻辑位于 `MainForm.cs`，界面布局和资源分别位于 `MainForm.Designer.cs` 与 `MainForm.resx`。`EditorItem.cs`、`VersionRecord.cs` 和 `PuppeteerSharpExtensions.cs` 提供数据模型及辅助功能。静态数据与脚本存放在 `editor.json`、`fetch.js` 和 `Resources/` 中；项目资源、许可证配置及启动设置位于 `Properties/`。`bin/`、`obj/` 是生成目录，不应提交。

## 构建、测试与开发命令

请在 Windows 环境下从仓库根目录执行：

- `dotnet restore UnityDownloader.sln`：还原 DevExpress、PuppeteerSharp 等 NuGet 依赖。
- `dotnet build UnityDownloader.sln -c Debug`：构建调试版本。
- `dotnet run --project UnityDownloader.csproj`：本地启动 WinForms 应用。
- `dotnet build UnityDownloader.sln -c Release`：提交前验证发布版本能否成功构建。

当前仓库没有自动化测试项目。后续添加测试后，使用 `dotnet test UnityDownloader.sln` 运行全部测试。

## 编码风格与命名约定

所有文本文件使用 `.editorconfig` 要求的 UTF-8 编码。C# 代码采用四空格缩进和文件范围命名空间；类型、方法及公开成员使用 `PascalCase`，局部变量和参数使用 `camelCase`，异步方法以 `Async` 结尾。应明确处理异步流程和可空值。不要手动修改 `*.Designer.cs` 或自动生成的资源代码，界面调整应优先通过 WinForms 设计器完成。修改 `fetch.js` 时，需确保与调用它的 PuppeteerSharp 流程兼容。

## 测试指南

涉及界面或下载流程的改动，应手动验证：应用启动、Unity 版本列表加载、下载进度、取消操作以及浏览器实例清理。新增测试项目建议命名为 `UnityDownloader.Tests`，测试文件采用 `<类名>Tests.cs`，测试方法应描述具体行为，例如 `LoadVersionsAsync_ReturnsParsedEditors`。

## 提交与拉取请求规范

近期提交信息通常使用简短、明确的动词短语，例如“升级依赖”或 `Improve error handling...`。每个提交应只包含一个清晰主题。拉取请求必须说明改动目的、验证步骤及关联问题；界面改动需附截图。依赖、资源或打包方式的变化应单独说明。禁止提交本机配置、构建产物、凭据或私有许可证材料。
