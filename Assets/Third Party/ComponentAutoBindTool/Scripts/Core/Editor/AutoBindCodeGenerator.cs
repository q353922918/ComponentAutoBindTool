using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Third_Party.ComponentAutoBindTool.Scripts.ViewCore;
using UnityEngine;
using BindData = Third_Party.ComponentAutoBindTool.Scripts.Core.ComponentAutoBindTool.BindData;

namespace Third_Party.ComponentAutoBindTool.Scripts.Core.Editor
{
    internal static class AutoBindCodeGenerator
    {
        public static string Generate(ComponentAutoBindTool target, AutoBindGlobalSetting setting)
        {
            string codePath = AutoBindPathUtility.GetResolvedCodePath(setting, target);
            if (!Directory.Exists(codePath))
            {
                Directory.CreateDirectory(codePath);
            }

            string className = !string.IsNullOrEmpty(target.ClassName) ? target.ClassName : target.gameObject.name;
            string filePath = Path.Combine(codePath, $"{className}.BindComponents.cs");
            string generatedCode = BuildCode(target, className);

            File.WriteAllText(filePath, generatedCode);
            return filePath;
        }

        private static string BuildCode(ComponentAutoBindTool target, string className)
        {
            var namespaces = new SortedSet<string>(StringComparer.Ordinal);
            var bindComponents = new List<BindComponentInfo>();
            Type targetScriptType = target.m_targetScript != null ? target.m_targetScript.GetType() : null;
            bool overridesEnsureAutoBind = HasOverridableEnsureAutoBindBase(targetScriptType);

            for (int i = 0; i < target.BindDatas.Count; i++)
            {
                BindData data = target.BindDatas[i];
                if (data.BindCom == null)
                {
                    continue;
                }

                Type componentType = data.BindCom.GetType();
                string componentNamespace = componentType.Namespace;
                if (!string.IsNullOrWhiteSpace(componentNamespace)
                    && componentNamespace != "System"
                    && componentNamespace != typeof(GameObject).Namespace
                    && componentNamespace != typeof(IUiViewComponent).Namespace)
                {
                    namespaces.Add(componentNamespace);
                }

                bindComponents.Add(new BindComponentInfo(
                    componentType.Name,
                    AutoBindFieldNameUtility.BuildFieldName(data.Name),
                    i));
            }

            var builder = new StringBuilder(1024);
            builder.AppendLine("using System;");
            builder.AppendLine("using UnityEngine;");

            foreach (string componentNamespace in namespaces)
            {
                builder.Append("using ").Append(componentNamespace).AppendLine(";");
            }

            builder.Append("using ").Append(typeof(IUiViewComponent).Namespace).AppendLine(";");
            builder.AppendLine();
            builder.Append("//自动生成于：").AppendLine(DateTime.Now.ToString());

            bool hasNamespace = !string.IsNullOrEmpty(target.Namespace);
            if (hasNamespace)
            {
                builder.Append("namespace ").Append(target.Namespace).AppendLine();
                builder.AppendLine("{");
            }

            string classIndent = hasNamespace ? "\t" : string.Empty;
            string memberIndent = classIndent + "\t";
            string innerIndent = memberIndent + "\t";
            string methodInnerIndent = innerIndent + "\t";

            builder.Append(classIndent).Append("public partial class ").Append(className);
            if (!overridesEnsureAutoBind)
            {
                builder.Append(" : ").Append(typeof(IAutoBindHost).Name);
            }

            builder.AppendLine();
            builder.Append(classIndent).AppendLine("{");
            builder.Append(memberIndent).AppendLine("[Serializable]");
            builder.Append(memberIndent).AppendLine("public class UIView : IUiViewComponent");
            builder.Append(memberIndent).AppendLine("{");

            foreach (BindComponentInfo bindComponent in bindComponents)
            {
                builder.Append(innerIndent)
                    .Append("public ")
                    .Append(bindComponent.TypeName)
                    .Append(' ')
                    .Append(bindComponent.FieldName)
                    .AppendLine(";");
            }

            builder.Append(memberIndent).AppendLine("}");
            builder.AppendLine();
            builder.Append(memberIndent).AppendLine("/// <summary>");
            builder.Append(memberIndent).AppendLine("/// ========== UI组件 ==========");
            builder.Append(memberIndent).AppendLine("/// </summary>");
            builder.Append(memberIndent).AppendLine("private UIView view;");
            builder.AppendLine();
            builder.Append(memberIndent)
                .Append(overridesEnsureAutoBind ? "public override void EnsureAutoBind(GameObject go)" : "public void EnsureAutoBind(GameObject go)")
                .AppendLine();
            builder.Append(memberIndent).AppendLine("{");
            builder.Append(innerIndent).AppendLine("if (view != null)");
            builder.Append(innerIndent).AppendLine("{");
            builder.Append(methodInnerIndent).AppendLine("return;");
            builder.Append(innerIndent).AppendLine("}");
            builder.AppendLine();
            builder.Append(innerIndent).AppendLine("if (go == null)");
            builder.Append(innerIndent).AppendLine("{");
            builder.Append(methodInnerIndent)
                .Append("Debug.LogError(\"[")
                .Append(className)
                .AppendLine("] AutoBind 失败：目标 GameObject 为空。\");");
            builder.Append(methodInnerIndent).AppendLine("return;");
            builder.Append(innerIndent).AppendLine("}");
            builder.AppendLine();
            builder.Append(innerIndent)
                .Append(typeof(ComponentAutoBindTool).FullName)
                .Append(" autoBindTool = go.GetComponent<")
                .Append(typeof(ComponentAutoBindTool).FullName)
                .AppendLine(">();");
            builder.AppendLine();
            builder.Append(innerIndent).AppendLine("if (autoBindTool == null)");
            builder.Append(innerIndent).AppendLine("{");
            builder.Append(methodInnerIndent)
                .Append("Debug.LogError(\"[")
                .Append(className)
                .AppendLine("] AutoBind 失败：未找到 ComponentAutoBindTool。\", go);");
            builder.Append(methodInnerIndent).AppendLine("return;");
            builder.Append(innerIndent).AppendLine("}");
            builder.AppendLine();
            builder.Append(innerIndent).AppendLine("view = new UIView");
            builder.Append(innerIndent).AppendLine("{");

            foreach (BindComponentInfo bindComponent in bindComponents)
            {
                builder.Append(innerIndent)
                    .Append('\t')
                    .Append(bindComponent.FieldName)
                    .Append(" = autoBindTool.GetBindComponent<")
                    .Append(bindComponent.TypeName)
                    .Append(">(")
                    .Append(bindComponent.Index)
                    .Append(", nameof(UIView.")
                    .Append(bindComponent.FieldName)
                    .AppendLine(")),");
            }

            builder.Append(innerIndent).AppendLine("};");
            builder.Append(memberIndent).AppendLine("}");

            builder.Append(classIndent).AppendLine("}");

            if (hasNamespace)
            {
                builder.AppendLine("}");
            }

            return builder.ToString();
        }

        private static bool HasOverridableEnsureAutoBindBase(Type targetScriptType)
        {
            for (Type currentType = targetScriptType?.BaseType; currentType != null && currentType != typeof(object); currentType = currentType.BaseType)
            {
                MethodInfo ensureMethod = currentType.GetMethod(
                    nameof(IAutoBindHost.EnsureAutoBind),
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly,
                    null,
                    new[] { typeof(GameObject) },
                    null);

                if (ensureMethod != null)
                {
                    return ensureMethod.IsVirtual && !ensureMethod.IsFinal;
                }
            }

            return false;
        }

        private readonly struct BindComponentInfo
        {
            public BindComponentInfo(string typeName, string fieldName, int index)
            {
                TypeName = typeName;
                FieldName = fieldName;
                Index = index;
            }

            public string TypeName { get; }
            public string FieldName { get; }
            public int Index { get; }
        }
    }
}
