using System;
using System.IO;
using UnityEngine;

namespace Third_Party.ComponentAutoBindTool.Scripts.Core.Editor
{
    internal static class AutoBindPathUtility
    {
        public static string GetResolvedCodePath(AutoBindGlobalSetting setting, ComponentAutoBindTool target)
        {
            if (setting == null || target == null)
            {
                return string.Empty;
            }

            return setting.UseGlobalDefaultSavePath
                ? NormalizeCodePath(target.CodePath)
                : NormalizeCodePath(setting.CodePath);
        }

        public static string GetInspectorCodePath(AutoBindGlobalSetting setting, string scriptAssetPath)
        {
            if (setting == null)
            {
                return string.Empty;
            }

            if (setting.UseGlobalDefaultSavePath)
            {
                if (string.IsNullOrWhiteSpace(scriptAssetPath))
                {
                    return string.Empty;
                }

                return ConvertAssetPathToStoredCodePath(Path.GetDirectoryName(scriptAssetPath));
            }

            return ConvertAbsolutePathToStoredCodePath(setting.CodePath);
        }

        public static bool IsPathUnderAssets(string fullPath)
        {
            if (string.IsNullOrWhiteSpace(fullPath))
            {
                return false;
            }

            string normalizedAssetsPath = NormalizeDirectoryPath(Application.dataPath);
            string normalizedTargetPath = NormalizeDirectoryPath(fullPath);
            return normalizedTargetPath.StartsWith(normalizedAssetsPath, StringComparison.OrdinalIgnoreCase);
        }

        private static string ConvertAbsolutePathToStoredCodePath(string codePath)
        {
            if (string.IsNullOrWhiteSpace(codePath))
            {
                return string.Empty;
            }

            string normalizedCodePath = NormalizeCodePath(codePath);
            string normalizedAssetsPath = NormalizeDirectoryPath(Application.dataPath);
            if (normalizedCodePath.StartsWith(normalizedAssetsPath, StringComparison.OrdinalIgnoreCase))
            {
                return normalizedCodePath.Substring(normalizedAssetsPath.Length).Replace('\\', '/');
            }

            return codePath.Replace('\\', '/');
        }

        private static string ConvertAssetPathToStoredCodePath(string assetPath)
        {
            if (string.IsNullOrWhiteSpace(assetPath))
            {
                return string.Empty;
            }

            string normalizedAssetPath = assetPath.Replace('\\', '/');
            if (string.Equals(normalizedAssetPath, "Assets", StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            if (normalizedAssetPath.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase))
            {
                return normalizedAssetPath.Substring("Assets".Length);
            }

            return normalizedAssetPath;
        }

        private static string NormalizeCodePath(string codePath)
        {
            if (string.IsNullOrWhiteSpace(codePath))
            {
                return string.Empty;
            }

            if (Path.IsPathRooted(codePath))
            {
                return Path.GetFullPath(codePath);
            }

            string relativePath = codePath.TrimStart('/', '\\');
            return Path.GetFullPath(Path.Combine(Application.dataPath, relativePath));
        }

        private static string NormalizeDirectoryPath(string path)
        {
            return Path.GetFullPath(path)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }
    }
}
