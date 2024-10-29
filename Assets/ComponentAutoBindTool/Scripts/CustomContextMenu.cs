using UnityEditor;
using UnityEngine;

namespace ComponentAutoBindTool.Scripts
{
    public class CustomContextMenu
    {
        [MenuItem("CONTEXT/MonoBehaviour/Add AutoBindComponentTool", false, 0)]
        private static void AddAutoBindComponentTool(MenuCommand command)
        {
            // 获取右键点击的对象
            var mono = command.context as MonoBehaviour;
            if (mono != null)
            {
                var selectedObject = mono.gameObject;

                // 检查是否已经挂载了 MyScript
                if (selectedObject != null)
                    if (selectedObject.GetComponent<ComponentAutoBindTool>() == null)
                    {
                        // 挂载脚本
                        var com = selectedObject.AddComponent<ComponentAutoBindTool>();
                        com.m_targetScript = mono;
                        Debug.Log("MyScript has been added to: " + selectedObject.name);
                    }
                    else
                    {
                        Debug.LogWarning("MyScript is already attached to: " + selectedObject.name);
                    }
            }
        }
        
        
        // 添加一个方法来控制菜单项的可用性
        [MenuItem("CONTEXT/MonoBehaviour/Add AutoBindComponentTool", true)]
        private static bool ValidateAddAutoBindComponentTool(MenuCommand command)
        {
            var mono = command.context as MonoBehaviour;
            // 如果当前选中的 MonoBehaviour 是 ComponentAutoBindTool，则返回 false
            return mono != null && !(mono is ComponentAutoBindTool);
        }
    }
}