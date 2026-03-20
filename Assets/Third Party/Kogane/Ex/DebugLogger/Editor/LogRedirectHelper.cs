#if UNITY_EDITOR
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;

namespace Third_Party.Kogane.Ex.DebugLogger.Editor
{
    [InitializeOnLoad]
    public static class LogRedirectHelper
    {
        private static bool _mHasForceMono;

        private static readonly string TargetIdentifier = "[CCDebug]";

        // 需要忽略的封装类名，跳转时会跳过这些类
        private static readonly string[] IgnoredFiles =
        {
            "Assets/Third Party/Kogane/Ex/DebugLogger/CCDebug.cs",
            "Assets/Third Party/Kogane/Kogane.DebugLogger-master/Runtime/DefaultDebugLogger.cs",
            "Assets/Third Party/Kogane/Kogane.DebugLogger-master/Runtime/IDebugLogger.cs",
            "Assets/Third Party/Kogane/Kogane.DebugLogger-master/Runtime/IDebugLoggerExt.cs",
            "Assets/Third Party/Kogane/Kogane.DebugLogger-master/Runtime/NullDebugLogger.cs",
            "Assets/Third Party/Kogane/Kogane.DebugLogger-master/Runtime/TaggedDebugLogger.cs",
        };

        private static readonly IgnoreFileSo IgnoreFileSo;

        static LogRedirectHelper()
        {
            IgnoreFileSo =
                AssetDatabase.LoadAssetAtPath<IgnoreFileSo>("Assets/Third Party/Kogane/Ex/IgnoreFileSo.asset");
            // Register callback on editor load
            EditorApplication.delayCall += () =>
            {
                // Just force the class to load (to trigger static constructor)
            };
        }

        [UnityEditor.Callbacks.OnOpenAsset(0)]
        private static bool OnOpenAsset(int instanceID, int line)
        {
            if (_mHasForceMono) return false;

            string stackTrace = GetStackTrace();
            if (string.IsNullOrEmpty(stackTrace) || !stackTrace.Contains(TargetIdentifier))
                return false;

            Match match = Regex.Match(stackTrace, @"\(at (.+?):(\d+)\)", RegexOptions.IgnoreCase);
            while (match.Success)
            {
                string path = match.Groups[1].Value;
                string lineStr = match.Groups[2].Value;

                if (!IsIgnoredFile(path))
                {
                    if (int.TryParse(lineStr, out int targetLine))
                    {
                        _mHasForceMono = true;
                        AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path), targetLine);
                        _mHasForceMono = false;
                        return true;
                    }
                }

                match = match.NextMatch();
            }

            return false;
        }

        /// <summary>
        /// 获取当前 Console 窗口的堆栈跟踪信息（StackTrace）
        /// </summary>
        /// <returns></returns>
        private static string GetStackTrace()
        {
            try
            {
                Type consoleWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow");
                FieldInfo fieldInfo =
                    consoleWindowType?.GetField("ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic);
                object consoleWindowInstance = fieldInfo?.GetValue(null);

                if (consoleWindowInstance != null && EditorWindow.focusedWindow == (EditorWindow)consoleWindowInstance)
                {
                    FieldInfo activeTextField = consoleWindowType.GetField("m_ActiveText",
                        BindingFlags.Instance | BindingFlags.NonPublic);
                    return activeTextField?.GetValue(consoleWindowInstance)?.ToString();
                }

                return null;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"Failed to get stack trace: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// 检查目标文件是否在忽略文件列表中
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static bool IsIgnoredFile(string filePath)
        {
            foreach (var ignored in IgnoreFileSo.FilePaths)
            {
                if (filePath.Contains(ignored))
                    return true;
            }

            return false;
        }
    }
}
#endif