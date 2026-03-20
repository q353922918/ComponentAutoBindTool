using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using BindData = Third_Party.ComponentAutoBindTool.Scripts.Core.ComponentAutoBindTool.BindData;

namespace Third_Party.ComponentAutoBindTool.Scripts.Core.Editor
{
    internal static class AutoBindValidator
    {
        public static AutoBindValidationResult Validate(ComponentAutoBindTool target, AutoBindGlobalSetting setting)
        {
            var result = new AutoBindValidationResult();
            if (target == null)
            {
                result.AddError("AutoBind 目标为空。");
                return result;
            }

            ValidateTargetScript(target, result);
            ValidateCodePath(target, setting, result);
            ValidateBindDatas(target, result);
            return result;
        }

        private static void ValidateTargetScript(ComponentAutoBindTool target, AutoBindValidationResult result)
        {
            MonoBehaviour targetScript = target.m_targetScript;
            if (targetScript == null)
            {
                result.AddError("未指定目标脚本（m_targetScript）。");
                return;
            }

            if (targetScript.gameObject != target.gameObject)
            {
                result.AddError("目标脚本必须挂在当前 AutoBind 根节点上。");
            }

            Type targetType = targetScript.GetType();
            string configuredClassName = target.ClassName ?? string.Empty;
            string configuredNamespace = target.Namespace ?? string.Empty;
            string actualNamespace = targetType.Namespace ?? string.Empty;

            if (!string.Equals(configuredClassName, targetType.Name, StringComparison.Ordinal))
            {
                result.AddError(
                    $"类名与目标脚本不一致：当前为 `{configuredClassName}`，目标脚本为 `{targetType.Name}`。");
            }

            if (!string.Equals(configuredNamespace, actualNamespace, StringComparison.Ordinal))
            {
                result.AddError(
                    $"命名空间与目标脚本不一致：当前为 `{configuredNamespace}`，目标脚本为 `{actualNamespace}`。");
            }
        }

        private static void ValidateCodePath(ComponentAutoBindTool target, AutoBindGlobalSetting setting,
            AutoBindValidationResult result)
        {
            string resolvedCodePath = AutoBindPathUtility.GetResolvedCodePath(setting, target);
            if (string.IsNullOrWhiteSpace(resolvedCodePath))
            {
                result.AddError("代码保存路径为空。");
                return;
            }

            if (!AutoBindPathUtility.IsPathUnderAssets(resolvedCodePath))
            {
                result.AddError($"代码保存路径不在 Assets 目录下：{resolvedCodePath}");
                return;
            }

            if (!Directory.Exists(resolvedCodePath))
            {
                result.AddWarning($"代码保存路径不存在，生成时会自动创建：{resolvedCodePath}");
            }
        }

        private static void ValidateBindDatas(ComponentAutoBindTool target, AutoBindValidationResult result)
        {
            List<BindData> bindDatas = target.BindDatas;
            if (bindDatas == null || bindDatas.Count == 0)
            {
                result.AddWarning("当前没有绑定数据，生成代码后 UIView 将为空。");
                return;
            }

            var generatedFieldNameToIndex = new Dictionary<string, int>(StringComparer.Ordinal);
            for (int i = 0; i < bindDatas.Count; i++)
            {
                BindData bindData = bindDatas[i];
                string bindName = bindData.Name;
                Component bindComponent = bindData.BindCom;

                if (string.IsNullOrWhiteSpace(bindName))
                {
                    result.AddError($"[{i}] 绑定名称为空。");
                }
                else
                {
                    string fieldName = AutoBindFieldNameUtility.BuildFieldName(bindName);
                    if (!AutoBindFieldNameUtility.IsValidIdentifier(fieldName))
                    {
                        result.AddError($"[{i}] 生成字段名非法：`{fieldName}`（源名称：`{bindName}`）。");
                    }
                    else if (generatedFieldNameToIndex.TryGetValue(fieldName, out int duplicateIndex))
                    {
                        result.AddError($"[{i}] 生成字段名重复：`{fieldName}`（与 [{duplicateIndex}] 冲突）。");
                    }
                    else
                    {
                        generatedFieldNameToIndex.Add(fieldName, i);
                    }
                }

                if (bindComponent == null)
                {
                    result.AddError($"[{i}] 组件引用为空。");
                    continue;
                }

                if (!IsComponentUnderRoot(target.transform, bindComponent))
                {
                    result.AddError($"[{i}] 组件 `{bindComponent.name}` 不在当前 AutoBind 根节点下。");
                }
            }
        }

        private static bool IsComponentUnderRoot(Transform root, Component component)
        {
            return component.transform == root || component.transform.IsChildOf(root);
        }
    }
}
