using System;
using UnityEditor;
using UnityEngine;

namespace Third_Party.ComponentAutoBindTool.Scripts.Core.Editor
{
    [CustomEditor(typeof(Third_Party.ComponentAutoBindTool.Scripts.Core.ComponentAutoBindTool))]
    public class ComponentAutoBindToolInspector : UnityEditor.Editor
    {
        private Third_Party.ComponentAutoBindTool.Scripts.Core.ComponentAutoBindTool m_Target;

        private SerializedProperty m_BindDatas;
        private SerializedProperty m_BindComs;
        private SerializedProperty m_BindFieldNames;

        private AutoBindGlobalSetting m_Setting;
        private AutoBindKeyMapSetting m_KeyMapSetting;

        private SerializedProperty m_targetScript;
        private SerializedProperty m_Namespace;
        private SerializedProperty m_ClassName;
        private SerializedProperty m_CodePath;

        private void OnEnable()
        {
            m_Target = (Third_Party.ComponentAutoBindTool.Scripts.Core.ComponentAutoBindTool)target;
            m_BindDatas = serializedObject.FindProperty("BindDatas");
            m_BindComs = serializedObject.FindProperty("m_BindComs");
            m_BindFieldNames = serializedObject.FindProperty("m_BindFieldNames");

            m_Setting = AutoBindEditorAssetLocator.LoadSingletonAsset<AutoBindGlobalSetting>();
            if (m_Setting == null)
            {
                return;
            }

            m_KeyMapSetting = AutoBindEditorAssetLocator.LoadSingletonAsset<AutoBindKeyMapSetting>();
            if (m_KeyMapSetting == null)
            {
                return;
            }

            m_targetScript = serializedObject.FindProperty("m_targetScript");
            m_Namespace = serializedObject.FindProperty("m_Namespace");
            m_ClassName = serializedObject.FindProperty("m_ClassName");
            m_CodePath = serializedObject.FindProperty("m_CodePath");

            m_Namespace.stringValue = string.IsNullOrEmpty(m_Namespace.stringValue)
                ? m_Setting.Namespace
                : m_Namespace.stringValue;
            m_ClassName.stringValue = string.IsNullOrEmpty(m_ClassName.stringValue)
                ? m_Target.gameObject.name
                : m_ClassName.stringValue;
            m_CodePath.stringValue =
                string.IsNullOrEmpty(m_CodePath.stringValue) ? m_Setting.CodePath : m_CodePath.stringValue;

            if (AutoBindSerializedBindingUtility.NeedsRuntimeBindingSync(m_BindDatas, m_BindComs, m_BindFieldNames))
            {
                AutoBindSerializedBindingUtility.SyncRuntimeBindingCaches(m_BindDatas, m_BindComs, m_BindFieldNames);
            }

            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawHeaderSection();
            AutoBindInspectorDrawer.DrawBindComponentFoldout(m_BindDatas, ref isComponentKvDataEnabled, DrawKvData);
            AutoBindInspectorDrawer.DrawKeyMapTip(m_KeyMapSetting, ref isGroupEnabled);

            if (serializedObject.hasModifiedProperties)
            {
                AutoBindSerializedBindingUtility.SyncRuntimeBindingCaches(m_BindDatas, m_BindComs, m_BindFieldNames);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private bool isGroupEnabled;
        private bool isComponentKvDataEnabled;

        private void DrawHeaderSection()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            DrawActionSection();
            GUILayout.Space(2f);
            DrawSetting();
            EditorGUILayout.EndVertical();
        }

        private void DrawActionSection()
        {
            AutoBindInspectorDrawer.DrawActionSection(
                HandleAutoBindAndValidate,
                HandleGenerateCode,
                HandleRemoveAll);
        }

        /// <summary>
        /// 自动绑定组件
        /// </summary>
        private void AutoBindComponent()
        {
            AutoBindScanResult scanResult = AutoBindHierarchyScanner.CollectBindings(m_Target, m_KeyMapSetting);
            AutoBindSerializedBindingUtility.RebuildBindDatas(m_BindDatas, scanResult.BindDatas);
            ApplySerializedChanges(true);

            if (scanResult.HasErrors)
            {
                for (int i = 0; i < scanResult.Errors.Count; i++)
                {
                    Debug.LogError(scanResult.Errors[i], m_Target);
                }
            }
        }

        /// <summary>
        /// 绘制设置项
        /// </summary>
        private void DrawSetting()
        {
            bool targetScriptChanged = false;
            if (m_targetScript != null)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_targetScript, new GUIContent("脚本"));
                targetScriptChanged = EditorGUI.EndChangeCheck();
            }

            if (targetScriptChanged || m_targetScript.objectReferenceValue != null)
            {
                AutoBindTargetScriptSyncUtility.SyncFromTargetScript(m_targetScript, m_Namespace, m_ClassName, m_CodePath,
                    m_Setting);
                serializedObject.ApplyModifiedProperties();
            }

            AutoBindInspectorDrawer.DrawSettingsSection(
                m_Namespace.stringValue,
                m_ClassName.stringValue,
                m_CodePath.stringValue);
        }

        /// <summary>
        /// 绘制键值对数据
        /// </summary>
        private void DrawKvData()
        {
            int needDeleteIndex = AutoBindInspectorDrawer.DrawBindDataList(m_BindDatas);
            if (needDeleteIndex != -1)
            {
                m_BindDatas.DeleteArrayElementAtIndex(needDeleteIndex);
                AutoBindSerializedBindingUtility.SyncRuntimeBindingCaches(m_BindDatas, m_BindComs, m_BindFieldNames);
            }
        }

        private void ApplySerializedChanges(bool forceSyncRuntimeBindings = false)
        {
            if (forceSyncRuntimeBindings || serializedObject.hasModifiedProperties)
            {
                AutoBindSerializedBindingUtility.SyncRuntimeBindingCaches(m_BindDatas, m_BindComs, m_BindFieldNames);
            }

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }

        private void HandleRemoveAll()
        {
            ApplySerializedChanges();
            m_BindDatas.ClearArray();
            ApplySerializedChanges(true);
        }

        private void HandleAutoBindAndValidate()
        {
            ApplySerializedChanges();
            AutoBindComponent();
            ValidateBindingsAndNotify();
        }

        private void HandleGenerateCode()
        {
            ApplySerializedChanges();
            GenAutoBindCode();
        }

        private void ValidateBindingsAndNotify()
        {
            AutoBindValidationResult validationResult = AutoBindValidator.Validate(m_Target, m_Setting);
            string report = validationResult.BuildReport();
            string title = validationResult.IsValid ? "校验通过" : "校验失败";
            string summary =
                $"{title}。\n错误 {validationResult.ErrorCount} 项，警告 {validationResult.WarningCount} 项。\n\n{report}";

            if (validationResult.IsValid)
            {
                Debug.Log($"[{m_Target.name}] AutoBind 校验通过。\n{report}", m_Target);
            }
            else
            {
                Debug.LogError($"[{m_Target.name}] AutoBind 校验失败。\n{report}", m_Target);
            }

            EditorUtility.DisplayDialog(title, summary, "OK");
        }

        /// <summary>
        /// 生成自动绑定代码
        /// </summary>
        private void GenAutoBindCode()
        {
            GameObject go = m_Target.gameObject;
            AutoBindValidationResult validationResult = AutoBindValidator.Validate(m_Target, m_Setting);
            string report = validationResult.BuildReport();
            if (!validationResult.IsValid)
            {
                Debug.LogError($"[{go.name}] AutoBind 生成前校验失败。\n{report}", go);
                EditorUtility.DisplayDialog("生成失败",
                    $"生成前校验未通过。\n错误 {validationResult.ErrorCount} 项，警告 {validationResult.WarningCount} 项。\n\n{report}",
                    "OK");
                return;
            }

            if (validationResult.WarningCount > 0)
            {
                Debug.LogWarning($"[{go.name}] AutoBind 生成前存在警告。\n{report}", go);
            }

            AutoBindCodeGenerator.Generate(m_Target, m_Setting);
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("提示", "代码生成完毕", "OK");
        }
    }
}
