# AutoBind UI for Unity + VContainer

> 通过命名约定扫描 UI 层级，自动生成强类型 `UIView` 绑定代码，并将视图对象接入 VContainer 生命周期的 Unity UI 基础设施。

## 项目定位

这是一个 **Unity 6 示例工程**，不是单纯的单文件脚本仓库。

它把三个问题放在同一个项目里解决：

1. **编辑器期自动收集 UI 组件引用**
2. **代码生成强类型视图绑定层**
3. **在 VContainer 生命周期中消费这些绑定结果**

如果把它当成一个新项目来理解，这个仓库的核心定位可以概括为：

- 一个面向 Unity UI 的 **AutoBind 工作流**
- 一个面向 VContainer 的 **View 基础设施样例**
- 一个可以继续抽离成独立包的 **原型仓库**

## 它解决什么问题

在传统 Unity UI 开发里，最常见的问题是：

- 脚本里要手写大量字段
- Prefab 或场景对象需要反复拖拽引用
- 重构层级或改名时容易漏改
- Presenter / ViewModel 想拿到 View 引用时，通常缺少清晰的中间层

这个项目的方案是：

- 用 **节点命名前缀** 代表要绑定的组件类型
- 用扫描器在编辑器期收集绑定信息
- 用代码生成器生成 `partial class + UIView`
- 在运行时通过 `ComponentAutoBindTool` 提供强类型取值
- 在 `ViewLifetimeScope` 里提前执行 `EnsureAutoBind`，让 `UIView` 可以直接注入到 Presenter

## 核心能力

### AutoBind 工具链

- 根据层级节点命名自动识别组件类型
- 支持单节点多组件绑定
- 支持默认映射 + 自定义映射
- 支持绑定数据可视化编辑与重扫
- 支持生成 `.BindComponents.cs` 强类型代码文件
- 支持生成前校验，减少错误代码产出

### 运行时绑定层

- 运行时缓存 `Component` 列表与字段名列表
- 支持 `字段名 -> 索引` 的查找回退机制
- 绑定失败时会输出明确错误信息
- 避免业务层直接依赖字符串查找组件

### VContainer 视图层示例

- `ViewLifetimeScope` 在 `Configure` 时自动触发 AutoBind
- `UIView` 可作为依赖直接注入到 Presenter
- `ViewManager` 提供简单的弹窗栈管理能力
- 支持 `Normal / Top / System / HUD` 视图层级概念

## 一分钟理解它的工作流

```text
命名后的 UI 层级
    -> AutoBindHierarchyScanner 扫描
    -> BindDatas（编辑器绑定数据）
    -> AutoBindSerializedBindingUtility 同步运行时缓存
    -> AutoBindCodeGenerator 生成 partial class
    -> EnsureAutoBind() 构造 UIView
    -> VContainer 将 UIView 注入 Presenter
```

## 快速开始

### 环境要求

当前仓库实际使用的环境：

- Unity `6000.0.68f1`
- `jp.hadashikick.vcontainer` `1.17.0`
- `com.unity.ugui` `2.0.0`
- `com.cysharp.unitask` `2.5.10`

### 打开项目后先看哪里

建议按这个顺序了解仓库：

1. 打开场景 `Assets/Third Party/ComponentAutoBindTool/Example/Scenes/Example.unity`
2. 查看示例脚本 `Assets/Third Party/ComponentAutoBindTool/Example/Scripts/AutoBindTest.cs`
3. 查看生成结果 `Assets/Third Party/ComponentAutoBindTool/Scripts/ViewBindComponents/AutoBindTest.BindComponents.cs`
4. 查看核心 Inspector 实现 `Assets/Third Party/ComponentAutoBindTool/Scripts/Core/Editor/ComponentAutoBindToolInspector.cs`

> 注意：当前仓库里的 `EditorBuildSettings.asset` 仍然指向旧路径 `Assets/ComponentAutoBindTool/Example/Scenes/Example.unity`。因此请手动打开示例场景，不要依赖 Build Settings。

### 体验一次完整生成流程

1. 在一个 UI 根节点上挂载你的业务脚本。
2. 在同一个对象上挂载 `ComponentAutoBindTool`。
3. 给子节点按约定命名，例如 `Btn_Start`、`Txt_Title`、`Img_Icon`。
4. 在 Inspector 中点击 `重扫校验`。
5. 确认校验通过后点击 `生成代码`。
6. 在生成的 `UIView` 中使用强类型字段。
7. 在 `ViewLifetimeScope` / Presenter 中消费这个 `UIView`。

## 命名规则

### 基本格式

```text
Prefix_FieldSuffix
PrefixA_PrefixB_FieldSuffix
```

规则说明：

- 最后一个 `_` 之后的部分是字段后缀
- 最后一个 `_` 之前的每一段都会被当成组件前缀
- 每个前缀都必须能在 `AutoBindKeyMapSetting` 中解析到组件类型
- 没有 `_` 的节点不会参与绑定

### 示例

| 节点名 | 解析结果 | 生成字段 |
| --- | --- | --- |
| `Btn_Start` | `Button` | `btnStart` |
| `Img_Icon` | `Image` | `imgIcon` |
| `Txt_Title` | `Text` | `txtTitle` |
| `Drop_Img_Quality` | `Dropdown` + `Image` | `dropQuality` + `imgQuality` |
| `TMTxt_Title` | `TMPro.TextMeshProUGUI` | `tMTxtTitle` |

### 字段名生成规则

字段名由 `AutoBindFieldNameUtility` 生成，逻辑很直接：

- 去掉所有 `_`
- 将结果的首字符转换为小写

因此：

- `Btn_Start` -> `btnStart`
- `Img_PlayerAvatar` -> `imgPlayerAvatar`
- `TMTxt_Title` -> `tMTxtTitle`

这也意味着带有连续大写缩写的前缀时，生成字段名不一定完全符合常见命名审美，但它和当前实现保持一致。

### 跳过规则

以下节点不会被当前根节点纳入扫描：

- 子节点上再次挂了 `ComponentAutoBindTool`
- 节点名包含 `NonRoot ==>`
- 节点名不包含 `_`

### 一个必须知道的命名陷阱

```text
Btn_Login_Close
```

这会被解析成：

- 前缀：`Btn`
- 前缀：`Login`
- 后缀：`Close`

如果 `Login` 不是一个合法组件前缀，扫描会直接报错。

也就是说：

- 这个工具 **不会** 把 `Login_Close` 自动理解成一个完整字段名
- 多单词字段后缀请写成 `LoginClose`

推荐写法：

```text
Btn_LoginClose
```

## 默认组件映射

项目内置了一组常用组件前缀，典型项如下：

| Prefix | Type |
| --- | --- |
| `Tf` | `Transform` |
| `Rtf` | `RectTransform` |
| `Btn` | `Button` |
| `Img` | `Image` |
| `Txt` | `Text` |
| `Inf` | `InputField` |
| `Sld` | `Slider` |
| `Tog` | `Toggle` |
| `SRect` | `ScrollRect` |
| `Drop` | `Dropdown` |
| `CGroup` | `CanvasGroup` |
| `TMTxt` | `TMPro.TextMeshProUGUI` |
| `TMInf` | `TMPro.TMP_InputField` |
| `TMDrop` | `TMPro.TMP_Dropdown` |

自定义映射保存在：

- `Assets/Third Party/ComponentAutoBindTool/AutoBindKeyMapSetting.asset`

当前仓库中还额外定义了一条项目侧映射：

```text
Nbi -> AAGame.Scripts.Views.Menu.Main.NavigationBarItem
```

## 典型开发流程

### 1. 创建或选择一个 View 根节点

如果你使用本仓库里的运行时方案，推荐让根节点脚本继承：

- `ViewLifetimeScope`

这样根节点会自动具备：

- `Canvas`
- `GraphicRaycaster`
- `ComponentAutoBindTool`

这三个约束是通过 `[RequireComponent]` 写死在 `ViewLifetimeScope` 上的。

### 2. 让目标脚本和 AutoBindTool 在同一个对象上

这是生成前校验的硬性要求。

如果不是同一个对象，校验会失败。

你也可以对任意 `MonoBehaviour` 使用右键菜单：

- `Add AutoBindComponentTool`

它会自动把 `ComponentAutoBindTool` 挂到同一个对象，并将当前脚本写入 `m_targetScript`。

### 3. 配置输出路径与命名空间

项目使用 `AutoBindGlobalSetting` 管理默认输出行为。

默认配置文件路径：

- `Assets/Third Party/ComponentAutoBindTool/AutoBindGlobalSetting.asset`

当前仓库默认值：

- 默认命名空间：空
- 默认输出目录：`Assets/Third Party/ComponentAutoBindTool/Scripts/ViewBindComponents`
- `跟随目标脚本的路径`：关闭

实际行为是：

- **关闭**：所有生成文件统一输出到全局目录
- **开启**：根据目标脚本所在目录同步每个对象自己的代码输出路径

### 4. 扫描、校验、生成

`ComponentAutoBindTool` 的 Inspector 是整个工作流入口，核心按钮只有三个：

- `重扫校验`
- `生成代码`
- `清空`

其中：

- `重扫校验` 会重新扫描层级、重建绑定数据，并弹出校验结果
- `生成代码` 会在生成前再次执行校验
- `清空` 只会清除当前对象的绑定列表

### 5. 在业务代码中使用生成结果

生成文件会为目标脚本补齐一个 `partial class`，并生成：

- `UIView` 嵌套类
- `private UIView view;`
- `EnsureAutoBind(GameObject go)` 方法

## 生成代码长什么样

当前示例生成出来的代码形态如下：

```csharp
public partial class AutoBindTest
{
    [Serializable]
    public class UIView : IUiViewComponent
    {
        public Image imgTest1;
        public Button btnTest2;
        public Text txtTest3;
        public Dropdown dropTest4;
        public Image imgTest4;
    }

    private UIView view;

    public override void EnsureAutoBind(GameObject go)
    {
        if (view != null)
        {
            return;
        }

        var autoBindTool = go.GetComponent<ComponentAutoBindTool>();
        view = new UIView
        {
            imgTest1 = autoBindTool.GetBindComponent<Image>(0, nameof(UIView.imgTest1)),
            btnTest2 = autoBindTool.GetBindComponent<Button>(1, nameof(UIView.btnTest2)),
            txtTest3 = autoBindTool.GetBindComponent<Text>(2, nameof(UIView.txtTest3)),
            dropTest4 = autoBindTool.GetBindComponent<Dropdown>(3, nameof(UIView.dropTest4)),
            imgTest4 = autoBindTool.GetBindComponent<Image>(4, nameof(UIView.imgTest4)),
        };
    }
}
```

这个设计有几个明显优点：

- 业务脚本本体保持干净
- 生成代码和手写代码分离
- `UIView` 天然适合作为 Presenter 的注入对象
- 运行时仍然能走索引访问，避免完全依赖字符串查找

## 运行时架构

### `ComponentAutoBindTool`

职责：

- 保存编辑器期的 `BindDatas`
- 保存运行时的 `m_BindComs`
- 保存运行时的 `m_BindFieldNames`
- 提供 `GetBindComponent<T>()` 访问接口

运行时查找策略：

1. 先尝试通过字段名匹配索引
2. 匹配不到时回退到传入索引
3. 类型不匹配或引用为空时输出明确错误日志

### `ViewLifetimeScope`

职责：

- 作为 VContainer 的 View 根节点基类
- 在 `Configure` 中调用 `EnsureAutoBind(gameObject)`
- 提供 `RegisterCommon<T>()` 简化 Presenter 注册
- 管理视图层级、初始化参数、显示与暂停生命周期

### `ViewManager`

这是仓库里附带的一个轻量级视图管理器，主要能力：

- `ShowView()`：打开界面
- `HideView()`：关闭栈顶界面
- `ShowViewAtHidePeekView()`：关闭当前栈顶并强行插入新界面
- `Tick()`：当栈为空时，从等待队列中补开界面

当前实现重点管理的是 `Normal` 层弹窗：

- `Normal` 层会入栈
- 新 `Normal` 打开时会暂停旧栈顶
- 关闭当前界面时会恢复上一层
- `Top` / `System` 目前只参与排序，不默认入栈

### `ViewBundle`

`ViewBundle` 是一个极轻量的参数容器：

- 用来在 `ViewManager` 打开界面时传入 `object[] args`
- 在 Presenter 或 View 内消费这些参数

## VContainer 集成方式

示例脚本：

- `Assets/Third Party/ComponentAutoBindTool/Example/Scripts/AutoBindTest.cs`
- `Assets/Third Party/ComponentAutoBindTool/Example/Scripts/AutoBindTestPresenter.cs`

示例流程是：

1. `AutoBindTest : ViewLifetimeScope`
2. `base.Configure(builder)` 内部先执行 `EnsureAutoBind`
3. `RegisterCommon<AutoBindTestPresenter>(builder, view)` 把生成的 `view` 注入容器
4. `AutoBindTestPresenter` 通过 `[Inject]` 拿到 `AutoBindTest.UIView`
5. Presenter 直接操作 `view.txtTest3` 等字段

这让项目里的依赖关系变成：

```text
Scene / Prefab
    -> ViewLifetimeScope
    -> AutoBind generated UIView
    -> Presenter
```

而不是让 Presenter 直接去 `GetComponent` 或自己维护大量序列化字段。

## 目录结构

```text
Assets/
  Third Party/
    ComponentAutoBindTool/
      AutoBindGlobalSetting.asset
      AutoBindKeyMapSetting.asset
      Example/
        Scenes/
          Example.unity
        Scripts/
          AutoBindTest.cs
          AutoBindTestPresenter.cs
      Scripts/
        Core/
          ComponentAutoBindTool.cs
          CustomContextMenu.cs
          Editor/
            ComponentAutoBindToolInspector.cs
            AutoBindHierarchyScanner.cs
            AutoBindCodeGenerator.cs
            AutoBindValidator.cs
        ViewCore/
          ViewLifetimeScope.cs
          ViewManager.cs
          ViewBundle.cs
        ViewBindComponents/
          AutoBindTest.BindComponents.cs
```

另外，仓库中还带有以下辅助依赖或工具：

- `generic-serializable-dictionary`：用于可编辑的自定义组件映射字典
- `Kogane.DebugLogger` / Console 工具：用于日志与编辑器体验
- `vFolders`：编辑器目录增强工具
- `NuGetForUnity`、`Handlebars.Net`：当前仓库中可见，但并不是 AutoBind 主链路的核心依赖

## 示例项目里最值得看的文件

如果你是第一次接手这个项目，建议优先看这几处：

- `Scripts/Core/ComponentAutoBindTool.cs`
  运行时绑定容器本体
- `Scripts/Core/Editor/ComponentAutoBindToolInspector.cs`
  整个编辑器工作流入口
- `Scripts/Core/Editor/AutoBindHierarchyScanner.cs`
  命名扫描规则的真实实现
- `Scripts/Core/Editor/AutoBindCodeGenerator.cs`
  生成文件的真实来源
- `Scripts/ViewCore/ViewLifetimeScope.cs`
  AutoBind 与 VContainer 衔接的关键点
- `Scripts/ViewCore/ViewManager.cs`
  弹窗栈和视图调度逻辑
- `Example/Scripts/AutoBindTest.cs`
  最小可用 View 示例
- `Scripts/ViewBindComponents/AutoBindTest.BindComponents.cs`
  生成结果样例

## 校验规则

代码生成前，工具会检查：

- 已设置 `m_targetScript`
- `m_targetScript` 与 `ComponentAutoBindTool` 在同一个 `GameObject`
- Inspector 上显示的类名与目标脚本真实类名一致
- Inspector 上显示的命名空间与目标脚本真实命名空间一致
- 输出路径存在于 `Assets` 目录之下
- 绑定名称能生成合法 C# 标识符
- 生成后的字段名没有重复
- 绑定组件不为空
- 绑定组件位于当前 AutoBind 根节点层级内

这套校验让它更像一个“有约束的工作流工具”，而不是单纯的字符串扫描器。

## 当前仓库状态与已知问题

以下内容不是 README 想象中的理想设计，而是当前代码库真实状态：

### 1. 这是 Unity 工程，不是独立包

当前仓库并没有整理成可直接通过 UPM 分发的包结构。

它更像：

- 一个工作中的原型仓库
- 一个示例工程
- 一个准备继续抽离的基础设施项目

### 2. 配置资源被设计成单例

`AutoBindEditorAssetLocator` 会按类型查找资源，并要求项目中：

- `AutoBindGlobalSetting` 只能有一份
- `AutoBindKeyMapSetting` 只能有一份

如果有多份资源，Inspector 会直接报错，而不是做多配置管理。

### 3. `Add Component To AutoBindKeyMapSetting` 向导存在硬编码路径问题

当前向导读取的是：

```text
Assets/ComponentAutoBindTool/AutoBindKeyMapSetting.asset
```

但当前仓库里的真实资源路径是：

```text
Assets/Third Party/ComponentAutoBindTool/AutoBindKeyMapSetting.asset
```

因此在当前仓库状态下，这个右键向导大概率无法正常工作。

### 4. Build Settings 里的示例场景路径已经过期

`ProjectSettings/EditorBuildSettings.asset` 当前仍指向旧路径：

```text
Assets/ComponentAutoBindTool/Example/Scenes/Example.unity
```

实际场景路径是：

```text
Assets/Third Party/ComponentAutoBindTool/Example/Scenes/Example.unity
```

### 5. `m_UseGlobalDefaultSavePath` 的命名和行为不一致

从代码行为看，这个布尔值为 `true` 时表示：

- **跟随目标脚本路径**

而不是“使用全局默认保存路径”。

Inspector 文案已经按“跟随目标脚本的路径”展示，但字段名本身仍然容易误导维护者。

### 6. 当前仓库没有针对 AutoBind 主流程的自动化测试

项目里包含 Unity 包自带的测试工程引用，但 `Assets/Third Party/ComponentAutoBindTool` 这部分没有独立的 Editor / PlayMode 自动化测试覆盖。

## 如果把它继续产品化，下一步应该做什么

如果你打算把这个仓库真的发展成一个独立可复用项目，优先级最高的工作建议是：

1. 修掉所有硬编码路径
2. 把 `ComponentAutoBindTool` 抽成 UPM 包结构
3. 为扫描、校验、代码生成补 Editor 测试
4. 为命名规则增加更明确的字段后缀语义支持
5. 把代码生成从字符串拼接升级为模板化方案
6. 给 `ViewManager` 补齐 Top / System 层的完整生命周期策略
7. 加上 prefab 工作流和批量生成能力

## 适合谁使用

这个项目适合：

- 想减少 Unity UI 手工拖拽引用的团队
- 已经在项目里使用 VContainer 的团队
- 希望建立 `View -> Presenter` 清晰边界的项目
- 想把 UI 绑定生成这件事纳入规范化工作流的团队

它不太适合：

- 想直接拿一个零配置、即插即用的成熟商业插件的人
- 只需要偶尔绑定几个对象，不需要生成层和约束工作流的人
- 不打算接受命名规范约束的人

## 致谢

- 思路来源参考了 ET 框架中的 `ReferenceCollector`
- 使用了 `generic-serializable-dictionary` 支持自定义映射的可编辑序列化

## License

见根目录 `LICENSE`。
