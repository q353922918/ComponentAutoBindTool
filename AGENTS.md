# Repository Guidelines

## Project Structure & Module Organization
`Assets/Third Party/ComponentAutoBindTool/` holds the AutoBind runtime, editor tooling, example scene/scripts, and generated bindings in `Scripts/ViewBindComponents/`. `Assets/AAGame/` is reserved for project-side scenes, UI, prefabs, art, audio, configs, and shaders. `Assets/AATest/` is the intended home for repository tests. Package dependencies live in `Packages/manifest.json`; Unity version and editor settings live in `ProjectSettings/`.

## Build, Test, and Development Commands
Open the project with Unity `6000.0.68f1` from Hub, or launch directly:

```sh
<UNITY> -projectPath "$(pwd)"
```

Run a batchmode compile/import smoke check:

```sh
<UNITY> -batchmode -projectPath "$(pwd)" -quit -logFile Logs/compile.log
```

Run edit mode tests when test assemblies are present:

```sh
<UNITY> -batchmode -projectPath "$(pwd)" -runTests -testPlatform EditMode -logFile Logs/editmode.log -quit
```

For manual validation, open `Assets/Third Party/ComponentAutoBindTool/Example/Scenes/Example.unity`, click `重扫校验`, then `生成代码`.

## Coding Style & Naming Conventions
Use 4 spaces in hand-written C# and keep braces on new lines, matching the existing runtime/editor scripts. Use PascalCase for public types and methods, prefix interfaces with `I`, and keep serialized/private fields in the `m_FieldName` style. Put editor-only code inside an `Editor/` folder. Do not hand-edit generated `*.BindComponents.cs` files; regenerate them from the inspector workflow.

AutoBind node names follow `Prefix_FieldSuffix`, for example `Btn_Start` or `TMTxt_Title`. Generated `UIView` members become lower camel case such as `btnStart`.

## Agent Collaboration Boundaries
Keep changes inside the task’s mounted scope and avoid touching unrelated directories, generated caches, or another agent’s working area. If the task is executed under `.ai/domains/<domain>/`, treat that mounted domain as the only writable area unless the task explicitly expands the scope. Cross-domain or cross-module edits are prohibited without ownership and approval.

## Testing Guidelines
`com.unity.test-framework` is installed, but the repository does not currently include committed test assemblies. Add new edit mode tests under `Assets/AATest/` or a package-local `Tests` folder with an `.asmdef`. At minimum, validate AutoBind or editor changes by rescanning, regenerating bindings, and confirming the project compiles cleanly in Unity.

## Commit & Pull Request Guidelines
Recent history mixes Conventional Commit prefixes such as `feat:` and `docs:` with brief Chinese summaries. Prefer short, imperative commit subjects; use `feat:`, `fix:`, or `docs:` when they fit. Pull requests should state the Unity version used, list touched scenes/assets, and mention generated files or `.asset` settings changed. Include screenshots or GIFs for inspector or UI changes and summarize manual verification steps.

Do not commit Unity caches or IDE output such as `Library/`, `Logs/`, `Temp/`, `obj/`, `.idea/`, `*.csproj`, or `*.sln`.
