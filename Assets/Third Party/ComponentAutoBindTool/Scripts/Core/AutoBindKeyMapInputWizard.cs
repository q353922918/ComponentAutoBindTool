#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Third_Party.ComponentAutoBindTool.Scripts.Core
{
    public class AutoBindKeyMapInputWizard : ScriptableWizard
    {
        public string key;
        public string typeName;
        
        private string warningMessage;
        
        protected void OnGUI()
        {
            // 绘制 lowerStr 字段（可编辑）
            key = EditorGUILayout.TextField("缩写前缀:", key);
            
            // 绘制 typeName 字段（只读）
            EditorGUILayout.LabelField("类型名称:", typeName);
            
            // 如果有警告信息，显示帮助框
            if (!string.IsNullOrEmpty(warningMessage))
            {
                EditorGUILayout.HelpBox(warningMessage, MessageType.Warning);
            }

            // 如果需要，添加其他 GUI 控件，例如按钮
            if (GUILayout.Button("确定"))
            {
                OnWizardCreate();
            }
        }

        private void OnWizardCreate()
        {
            // 加载 so 文件
            string assetPath = "Assets/ComponentAutoBindTool/AutoBindKeyMapSetting.asset";
            AutoBindKeyMapSetting obj = AssetDatabase.LoadAssetAtPath<AutoBindKeyMapSetting>(assetPath);
            if (obj != null)
            {
                if (obj.extraComponentKeyMap.ContainsKey(key) || obj.DefaultComponentKeyMap.ContainsKey(key))
                {
                    // 前缀一样，但是类型不一样
                    warningMessage = $"警告：键 '{key}' 的类型已存在，当前类型为 '{obj.extraComponentKeyMap[key]}', 新类型为 '{typeName}'。"; // 清除警告信息
                    return;
                }

                var targetKey = "";
                foreach (var (strKey, value) in obj.DefaultComponentKeyMap)
                {
                    if (value == typeName)
                    {
                        targetKey = strKey;
                        break;
                    }
                }
                if (!string.IsNullOrEmpty(targetKey))
                {
                    warningMessage = $"警告：'{typeName}' 的类型已存在，当前类型的 键 为 '{targetKey}'。"; // 显示警告信息
                    return;
                }
                foreach (var (strKey, value) in obj.extraComponentKeyMap)
                {
                    if (value == typeName)
                    {
                        targetKey = strKey;
                        break;
                    }
                }
                if (!string.IsNullOrEmpty(targetKey))
                {
                    warningMessage = $"警告：'{typeName}' 的类型已存在，当前类型的 键 为 '{targetKey}'。"; // 显示警告信息
                    return;
                }
                
                warningMessage = null;
                
                obj.extraComponentKeyMap.Add(key, typeName);
                // 关闭窗口
                Close();
            }
            else
            {
                Debug.LogError("未能加载指定的 ScriptableObject。");
            }
        }
    }
}

#endif