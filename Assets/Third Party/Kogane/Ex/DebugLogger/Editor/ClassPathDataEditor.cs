#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Third_Party.Kogane.Ex.DebugLogger.Editor
{
    [CustomEditor(typeof(IgnoreFileSo))]
    public class ClassPathDataEditor : UnityEditor.Editor
    {
        private Vector2 _scrollPos;
        private string _rightClickedPath; // 记录当前右键点击的路径

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            IgnoreFileSo data = (IgnoreFileSo)target;

            // 按钮逻辑
            if (GUILayout.Button("更新路径列表"))
            {
                List<string> paths = ScriptPathHelper.GetScriptPaths(data.Files);
                Debug.Log("脚本路径列表:\n" + string.Join("\n", paths));
                data.RefreshFilePaths(paths);
            }

            // 绘制路径列表
            if (data.FilePaths != null && data.FilePaths.Count > 0)
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("文件路径列表:", EditorStyles.boldLabel);
                
                _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.MaxHeight(200));
                foreach (string path in data.FilePaths)
                {
                    // 使用 SelectableLabel 支持文本选择和右键菜单
                    Rect rect = EditorGUILayout.GetControlRect();
                    EditorGUI.SelectableLabel(rect, path, EditorStyles.wordWrappedLabel);

                    // 检测右键点击事件
                    if (Event.current.type == EventType.MouseDown && 
                        Event.current.button == 1 && 
                        rect.Contains(Event.current.mousePosition))
                    {
                        _rightClickedPath = path;
                        ShowContextMenu();
                        Event.current.Use(); // 阻止事件冒泡
                    }
                }
                EditorGUILayout.EndScrollView();
            }
        }

        private void ShowContextMenu()
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("复制路径"), false, () => 
            {
                EditorGUIUtility.systemCopyBuffer = _rightClickedPath;
                Debug.Log($"已复制: {_rightClickedPath}");
            });
            menu.ShowAsContext();
        }
    }
}
#endif