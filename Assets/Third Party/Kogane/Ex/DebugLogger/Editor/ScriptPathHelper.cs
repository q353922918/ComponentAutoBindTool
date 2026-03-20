#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Third_Party.Kogane.Ex.DebugLogger.Editor
{
    public static class ScriptPathHelper
    {
        public static List<string> GetScriptPaths(List<Object> scriptAssets)
        {
            return scriptAssets
                .Where(asset => asset is MonoScript) // 过滤非脚本文件
                .Select(AssetDatabase.GetAssetPath)
                .ToList();
        }
    }
}
#endif