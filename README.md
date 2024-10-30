# ComponentAutoBindTool
0202年了作为底层拼图仔的你还在Unity里手写组件绑定代码？快来试试这款船新的组件自动绑定工具吧，只需三分钟，你就会爱上它。

实现了editor时期使用字符串，runtime时期使用索引获取组件，以消除不必要的内存分配。

支持自定义自动绑定规则，提供了默认的自动绑定规则，根据物体命名前缀进行组件识别，支持单个物体的多组件识别绑定。

基本用法：为要进行自动绑定物体挂载ComponentAutoBindTool脚本，根据选择的绑定规则要求修改物体相关信息，点击自动绑定组件，然后设置自动生成的绑定代码的命名空间，类名与保存路径，最后点击生成绑定代码即可。

AutoBindGlobalSetting.asset为默认设置文件，可放置于Asset目录下任意位置，若不慎丢失可通过点击菜单栏的CatWorkflow/CreateAutoBindGlobalSetting进行创建。

本项目基于 https://github.com/egametang/ET 中的 ReferenceCollector 开发。

--------------------------------------粗糙的增加一些编辑的操作--------------------------------------

删除了RuleHelper的选择，创建了一个组件缩写和组件的映射SO资源文件，方便修改
![e97e80d0454c98682f119bf85c2d279](https://github.com/user-attachments/assets/ee23484c-d4c4-4170-8511-d662c9157b2e)
![ca0dbebfcc2de136329dbd829df3608](https://github.com/user-attachments/assets/4ce2715c-9712-475e-a703-9297389b7241)
![1730296840305](https://github.com/user-attachments/assets/6fb29c94-6d5e-47dc-837e-f37cc5cc9dfd)


增加了组件缩写和组件的映射显示，不记得了方便查看
![gouxuan](https://github.com/user-attachments/assets/0a769082-ece3-4d39-a49e-69740f02ae15)

增加对脚本右键添加自动绑定脚本
![addtool](https://github.com/user-attachments/assets/3690637d-b421-4019-9200-5fe5d95bfd5b)

增加右键菜单对界面名字添加忽略绑定字符
![addignorestr](https://github.com/user-attachments/assets/c2634e3e-9515-4577-b42f-79dc7c6fdf4b)
