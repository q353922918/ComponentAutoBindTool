using UnityEditor;
using UnityEngine;

namespace ComponentAutoBindTool.Scripts
{
    public class CustomContextMenu
    {
        [MenuItem("GameObject/Add Root Ignore", false, 0)]
        private static void AddSuffix()
        {
            var paths = AssetDatabase.FindAssets($"t:{nameof(AutoBindGlobalSetting)}");
            switch (paths.Length)
            {
                case 0:
                    Debug.LogError($"不存在 {nameof(AutoBindGlobalSetting)}");
                    return;
                case > 1:
                    Debug.LogError($"{nameof(AutoBindGlobalSetting)} 数量大于1");
                    return;
            }

            var path = AssetDatabase.GUIDToAssetPath(paths[0]);
            var setting = AssetDatabase.LoadAssetAtPath<AutoBindGlobalSetting>(path);
            
            // 获取选中的游戏对象
            GameObject selectedObject = Selection.activeGameObject;

            // 检查是否有选中的对象
            if (selectedObject != null)
            {
                // 在对象名称后添加后缀
                selectedObject.name += setting.IgnoreStr;
                // 记录更改
                EditorUtility.SetDirty(selectedObject);
            }
            else
            {
                Debug.LogWarning("没有选中的对象。");
            }
        }

        [MenuItem("GameObject/Add Root Ignore", true)]
        private static bool AddSuffixValidate()
        {
            // 确保只有在选中游戏对象时才启用该菜单项
            return Selection.activeGameObject != null;
        }
        
        
        
        
        
        
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