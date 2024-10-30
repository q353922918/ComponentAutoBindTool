using UnityEditor;
using UnityEngine;

namespace ComponentAutoBindTool.Scripts.Editor
{
    [CustomEditor(typeof(AutoBindGlobalSetting))]
    public class AutoBindGlobalSettingInspector : UnityEditor.Editor
    {
        private SerializedProperty m_IgnoreStr;
        private SerializedProperty m_Namespace;
        private SerializedProperty m_CodePath;

        private void OnEnable()
        {
            m_IgnoreStr = serializedObject.FindProperty("m_IgnoreStr");
            m_Namespace = serializedObject.FindProperty("m_Namespace");
            m_CodePath = serializedObject.FindProperty("m_CodePath");
        }

        public override void OnInspectorGUI()
        {
            m_IgnoreStr.stringValue = EditorGUILayout.TextField(new GUIContent("忽略节点的标识符"), m_IgnoreStr.stringValue);
            m_Namespace.stringValue = EditorGUILayout.TextField(new GUIContent("默认命名空间"), m_Namespace.stringValue);

            EditorGUILayout.LabelField("默认代码保存路径：");
            EditorGUILayout.LabelField(m_CodePath.stringValue);
            if (GUILayout.Button("选择路径", GUILayout.Width(140f)))
            {
                m_CodePath.stringValue = EditorUtility.OpenFolderPanel("选择代码保存路径", Application.dataPath, "");
            }

            serializedObject.ApplyModifiedProperties();
       
        }
    }
}
