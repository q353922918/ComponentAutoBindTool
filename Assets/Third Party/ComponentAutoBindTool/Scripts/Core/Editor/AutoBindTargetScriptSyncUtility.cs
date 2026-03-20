using UnityEditor;
using UnityEngine;

namespace Third_Party.ComponentAutoBindTool.Scripts.Core.Editor
{
    internal static class AutoBindTargetScriptSyncUtility
    {
        public static bool SyncFromTargetScript(SerializedProperty targetScriptProperty, SerializedProperty namespaceProperty,
            SerializedProperty classNameProperty, SerializedProperty codePathProperty, AutoBindGlobalSetting setting)
        {
            if (targetScriptProperty == null)
            {
                return false;
            }

            Object objectReferenceValue = targetScriptProperty.objectReferenceValue;
            if (objectReferenceValue == null)
            {
                return false;
            }

            MonoBehaviour targetScript = objectReferenceValue as MonoBehaviour;
            if (targetScript == null)
            {
                Debug.LogError("目标脚本不是有效的 MonoBehaviour。");
                return false;
            }

            System.Type scriptType = targetScript.GetType();
            MonoScript monoScript = MonoScript.FromMonoBehaviour(targetScript);
            string scriptPath = monoScript != null ? AssetDatabase.GetAssetPath(monoScript) : string.Empty;

            namespaceProperty.stringValue = scriptType.Namespace ?? string.Empty;
            classNameProperty.stringValue = scriptType.Name;

            string codePath = AutoBindPathUtility.GetInspectorCodePath(setting, scriptPath);
            if (string.IsNullOrEmpty(codePath))
            {
                Debug.LogError("无法解析 AutoBind 代码保存路径。");
                return false;
            }

            codePathProperty.stringValue = codePath;
            return true;
        }
    }
}
