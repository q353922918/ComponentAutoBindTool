using System.Collections.Generic;
using UnityEngine;

namespace Third_Party.ComponentAutoBindTool.Scripts.Core.Editor
{
    internal static class AutoBindHierarchyScanner
    {
        public static AutoBindScanResult CollectBindings(ComponentAutoBindTool target, AutoBindKeyMapSetting keyMapSetting)
        {
            var result = new AutoBindScanResult();
            if (target == null)
            {
                result.AddError("AutoBind 根节点为空。");
                return result;
            }

            if (keyMapSetting == null)
            {
                result.AddError("AutoBindKeyMapSetting 为空。");
                return result;
            }

            var childRoots = new List<Transform>();
            CollectChildRoots(target.transform, childRoots);

            for (int i = 0; i < childRoots.Count; i++)
            {
                CollectTransformBindings(childRoots[i], keyMapSetting, result);
            }

            return result;
        }

        private static void CollectChildRoots(Transform transform, List<Transform> childRoots)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform currentChild = transform.GetChild(i);
                if (ShouldSkipTransform(currentChild))
                {
                    continue;
                }

                childRoots.Add(currentChild);
                CollectChildRoots(currentChild, childRoots);
            }
        }

        private static void CollectTransformBindings(Transform target, AutoBindKeyMapSetting keyMapSetting,
            AutoBindScanResult result)
        {
            string[] nameParts = target.name.Split('_');
            if (nameParts.Length <= 1)
            {
                return;
            }

            string fieldSuffix = nameParts[nameParts.Length - 1];
            for (int i = 0; i < nameParts.Length - 1; i++)
            {
                string prefix = nameParts[i];
                if (!keyMapSetting.TryGetComponentTypeName(prefix, out string componentTypeName))
                {
                    result.AddError($"{target.name}的命名中{prefix}不存在对应的组件类型，绑定失败");
                    return;
                }

                Component component = target.GetComponent(componentTypeName);
                if (component == null)
                {
                    result.AddError($"{target.name}上不存在{componentTypeName}的组件");
                    continue;
                }

                result.AddBindData($"{prefix}_{fieldSuffix}", component);
            }
        }

        private static bool ShouldSkipTransform(Transform target)
        {
            return target.GetComponent<ComponentAutoBindTool>() != null
                   || target.name.Contains("NonRoot ==>");
        }
    }
}
