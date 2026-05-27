# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## 项目概述

JsonViewer 是一个 .NET Framework WinForms 桌面应用，用于可视化查看 JSON 数据。有两种使用模式：
- **独立应用** (`JsonView`) — 打开/粘贴 JSON 文件进行浏览
- **VS 调试器可视化工具** (`JsonVisualizerVSIX`) — 在 Visual Studio 调试时直接可视化查看集合和字符串对象

## 构建方式

使用 Visual Studio 2017+ 打开 `JsonViewer.sln` 构建，或通过 MSBuild 命令行：

```
msbuild JsonViewer.sln /p:Configuration=Release
```

NuGet 包管理使用 `packages.config`（非 PackageReference），构建前需先还原：
```
nuget restore JsonViewer.sln
```

无自动化测试。`TestConsoleApp` 仅为手动测试用的控制台程序。

## 架构

### 项目依赖关系

```
JsonView (.NET 4.0, WinExe)       ──→ JsonViewer (核心库)
JsonVisualizerVSIX (.NET 4.6)     ──→ JsonViewer (核心库)
TestConsoleApp (.NET 4.6.1)        ──  独立，无项目引用
```

所有 UI 和解析逻辑都在 `JsonViewer/` 核心库中。`.csproj` 为旧式格式（非 SDK 风格），新增 `.cs` 文件需手动在 `.csproj` 中添加 `<Compile Include="..."/>`。

### 核心库 (`JsonViewer/`) 关键组件

- **`JsonObject`** — JSON 节点模型，包含 Id、Value、JsonType（Object/Array/Value）、Parent、Fields
- **`JsonFields`** — 双索引集合：`List<JsonObject>` 有序访问 + `Dictionary<string, JsonObject>` 按 id 查找
- **`JsonObjectTree`** — 使用 Newtonsoft.Json 将 JSON 字符串解析为 `JsonObject` 树结构，处理 `/Date(...)/` 格式
- **`JsonViewer` UserControl** — 核心 UI 控件，包含 TreeView 和 TextEditor 两个 Tab 页
- **`XTreeView`** — 自定义 owner-drawn TreeView，支持节点折叠和图标
- **`JsonFolding`** — 文本编辑器的代码折叠策略
- **`TextEditorManager`** — 管理 ICSharpCode.TextEditorEx 的配置

### 插件系统

通过 `IJsonViewerPlugin` 接口体系扩展可视化能力：
- `ICustomTextProvider` — 为节点提供自定义文本标注（如日期格式化）
- `IJsonVisualizer` — 提供自定义 WinForms 控件来可视化 `JsonObject`

插件通过 `App.config` 的 `<jsonViewer><plugins>` 配置节注册，`PluginsManager` 使用反射加载。配置失败时回退到 `InternalPlugins` 中的内置插件。

### VS 调试器可视化工具 (`JsonVisualizerVSIX/`)

`JsonVisualizer` 继承 `DialogDebuggerVisualizer`，通过 assembly-level 属性注册为多种 .NET 集合类型的可视化器（ArrayList、Dictionary<,>、List<> 等）。内部 `ObjectSource` 在调试端使用 `JsonConvert.SerializeObject()` 序列化对象，宿主端打开嵌入 `JsonViewer` 控件的对话框。

## 关键依赖

- `Newtonsoft.Json` 9.0.1 — JSON 解析/序列化
- `ICSharpCode.TextEditorEx` 1.0.0.6 — 语法高亮文本编辑器控件
- `Microsoft.VisualStudio.Shell.15.0` — VSIX 扩展所需的 VS SDK
