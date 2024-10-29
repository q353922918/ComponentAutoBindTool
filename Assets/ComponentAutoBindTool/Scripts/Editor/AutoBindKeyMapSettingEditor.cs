using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace ComponentAutoBindTool.Scripts.Editor
{
    [CustomEditor(typeof(AutoBindKeyMapSetting))]
    public class AutoBindKeyMapSettingEditor : UnityEditor.Editor
    {
        private SerializedProperty m_ExtraComponentKeyMapProperty;
        private bool m_ShowDefaultKeyMap = true; // 控制折叠状态

        private void OnEnable()
        {
            m_ExtraComponentKeyMapProperty = serializedObject.FindProperty("extraComponentKeyMap");
        }

        public override void OnInspectorGUI()
        {
            var autoBindKeyMapSetting = (AutoBindKeyMapSetting)target;
            
            serializedObject.Update();

            // 显示可折叠的字典
            m_ShowDefaultKeyMap = EditorGUILayout.Foldout(m_ShowDefaultKeyMap, 
                "Default Component Key Map", true);
            if (m_ShowDefaultKeyMap)
            {
                EditorGUI.indentLevel++; // 增加缩进
                foreach (var keyValuePair in autoBindKeyMapSetting.defaultComponentKeyMap)
                {                    
                    EditorGUILayout.LabelField($"组件缩写: {keyValuePair.Key}", 
                        $"组件名称: {keyValuePair.Value}");
                }
                EditorGUI.indentLevel--; // 恢复缩进
            }

            // 显示可编辑字典
            EditorGUILayout.PropertyField(m_ExtraComponentKeyMapProperty, true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif