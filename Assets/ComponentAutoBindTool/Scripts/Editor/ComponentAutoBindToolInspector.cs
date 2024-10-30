using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using ComponentAutoBindTool.Scripts;
using ComponentAutoBindTool.Scripts.Editor;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using BindData = ComponentAutoBindTool.Scripts.ComponentAutoBindTool.BindData;

namespace AutoBindTool.Scripts.Editor
{
    [CustomEditor(typeof(ComponentAutoBindTool.Scripts.ComponentAutoBindTool))]
    public class ComponentAutoBindToolInspector : UnityEditor.Editor
    {
        private ComponentAutoBindTool.Scripts.ComponentAutoBindTool m_Target;

        private SerializedProperty m_BindDatas;
        private SerializedProperty m_BindComs;
        private List<BindData> m_TempList = new List<BindData>();
        private List<string> m_TempFiledNames = new List<string>();
        private List<string> m_TempComponentTypeNames = new List<string>();

        private string[] s_AssemblyNames = { "Assembly-CSharp" };
        private string[] m_HelperTypeNames;
        private string m_HelperTypeName;
        private int m_HelperTypeNameIndex;

        private AutoBindGlobalSetting m_Setting;
        private AutoBindKeyMapSetting m_KeyMapSetting;

        private SerializedProperty m_targetScript;
        private SerializedProperty m_Namespace;
        private SerializedProperty m_ClassName;
        private SerializedProperty m_CodePath;

        private void OnEnable()
        {
            m_Target = (ComponentAutoBindTool.Scripts.ComponentAutoBindTool)target;
            m_BindDatas = serializedObject.FindProperty("BindDatas");
            m_BindComs = serializedObject.FindProperty("m_BindComs");

            m_HelperTypeNames = GetTypeNames(typeof(IAutoBindRuleHelper), s_AssemblyNames);

            string[] paths = AssetDatabase.FindAssets($"t:{nameof(AutoBindGlobalSetting)}");
            if (paths.Length == 0)
            {
                Debug.LogError($"不存在 {nameof(AutoBindGlobalSetting)}");
                return;
            }

            if (paths.Length > 1)
            {
                Debug.LogError($"{nameof(AutoBindGlobalSetting)} 数量大于1");
                return;
            }

            string path = AssetDatabase.GUIDToAssetPath(paths[0]);
            m_Setting = AssetDatabase.LoadAssetAtPath<AutoBindGlobalSetting>(path);
            
            string[] keyMapPaths = AssetDatabase.FindAssets($"t:{nameof(AutoBindKeyMapSetting)}");
            if (keyMapPaths.Length == 0)
            {
                Debug.LogError($"不存在 {nameof(AutoBindKeyMapSetting)}");
                return;
            }

            if (keyMapPaths.Length > 1)
            {
                Debug.LogError($"{nameof(AutoBindKeyMapSetting)} 数量大于1");
                return;
            }

            string keyMapPath = AssetDatabase.GUIDToAssetPath(keyMapPaths[0]);
            m_KeyMapSetting = AssetDatabase.LoadAssetAtPath<AutoBindKeyMapSetting>(keyMapPath);

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

            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawTip();

            DrawTopButton();

            // DrawHelperSelect();

            DrawSetting();

            DrawKvData();

            serializedObject.ApplyModifiedProperties();
        }

        private bool isGroupEnabled;

        /// <summary>
        /// 绘制组件缩写前缀映射表
        /// </summary>
        private void DrawTip()
        {
            // 开始一个垂直的组
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginVertical();
            GUILayout.Box($"<color=red><===节点名称包含 {m_Setting.IgnoreStr}, 不会被自动绑定组件检测===></color>", new GUIStyle() { richText = true });
            EditorGUILayout.EndVertical();
            
            // "组件前缀提示" 可折叠组
            var isGroupEnabledStrTip = isGroupEnabled ? "打开" : "未打开";
            isGroupEnabled =
                EditorGUILayout.BeginToggleGroup($"<===组件缩写和组件名字映射表===>     状态:{isGroupEnabledStrTip}", isGroupEnabled);
            EditorGUILayout.Space();

            if (isGroupEnabled)
            {
                var groupA = new List<string>();
                var groupB = new List<string>();
                var addToGroupA = true;

                foreach (var item in m_KeyMapSetting.DefaultComponentKeyMap)
                {
                    string formattedItem =
                        $"{{ \"<color=white>{item.Key}</color>\", \"<color=red>{item.Value}</color>\" }}";
                    if (addToGroupA)
                    {
                        groupA.Add(formattedItem);
                    }
                    else
                    {
                        groupB.Add(formattedItem);
                    }

                    addToGroupA = !addToGroupA;
                }
                
                foreach (var item in m_KeyMapSetting.extraComponentKeyMap)
                {
                    string formattedItem =
                        $"{{ \"<color=white>{item.Key}</color>\", \"<color=red>{item.Value}</color>\" }}";
                    if (addToGroupA)
                    {
                        groupA.Add(formattedItem);
                    }
                    else
                    {
                        groupB.Add(formattedItem);
                    }

                    addToGroupA = !addToGroupA;
                }
                
                // foreach (var item in m_Target.RuleHelper.GetPrefixesDict())
                // {
                //     string formattedItem =
                //         $"{{ \"<color=white>{item.Key}</color>\", \"<color=red>{item.Value}</color>\" }}";
                //     if (addToGroupA)
                //     {
                //         groupA.Add(formattedItem);
                //     }
                //     else
                //     {
                //         groupB.Add(formattedItem);
                //     }
                //
                //     addToGroupA = !addToGroupA;
                // }
                
                EditorGUILayout.BeginVertical();
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical();
                GUILayout.Box(string.Join("\n", groupA.ToArray()), new GUIStyle() { richText = true });
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                GUILayout.Box(string.Join("\n", groupB.ToArray()), new GUIStyle() { richText = true });
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }

            // 结束 "组件前缀提示" 可折叠组
            EditorGUILayout.EndToggleGroup();

            // 增加垂直布局的高度
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制顶部按钮
        /// </summary>
        private void DrawTopButton()
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("排序"))
            {
                Sort();
            }

            if (GUILayout.Button("全部删除"))
            {
                RemoveAll();
            }

            if (GUILayout.Button("删除空引用"))
            {
                RemoveNull();
            }

            if (GUILayout.Button("自动绑定组件"))
            {
                AutoBindComponent();
            }

            if (GUILayout.Button("生成绑定代码"))
            {
                GenAutoBindCode();
            }

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 排序
        /// </summary>
        private void Sort()
        {
            m_TempList.Clear();
            foreach (BindData data in m_Target.BindDatas)
            {
                m_TempList.Add(new BindData(data.Name, data.BindCom));
            }

            m_TempList.Sort((x, y) => { return string.Compare(x.Name, y.Name, StringComparison.Ordinal); });

            m_BindDatas.ClearArray();
            foreach (BindData data in m_TempList)
            {
                AddBindData(data.Name, data.BindCom);
            }

            SyncBindComs();
        }

        /// <summary>
        /// 全部删除
        /// </summary>
        private void RemoveAll()
        {
            m_BindDatas.ClearArray();

            SyncBindComs();
        }

        /// <summary>
        /// 删除空引用
        /// </summary>
        private void RemoveNull()
        {
            for (int i = m_BindDatas.arraySize - 1; i >= 0; i--)
            {
                SerializedProperty element = m_BindDatas.GetArrayElementAtIndex(i).FindPropertyRelative("BindCom");
                if (element.objectReferenceValue == null)
                {
                    m_BindDatas.DeleteArrayElementAtIndex(i);
                }
            }

            SyncBindComs();
        }

        private void GetChildRoots(Transform transform, ref List<Transform> childRoots)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var curChildRoot = transform.GetChild(i);
                if (curChildRoot.GetComponent<ComponentAutoBindTool.Scripts.ComponentAutoBindTool>() 
                    || curChildRoot.name.Contains(m_Setting.IgnoreStr))//curChildRoot.name == "----NonRoot----" || ✦ SkipThisNode
                {
                    continue;
                }
                childRoots.Add(curChildRoot);
                GetChildRoots(curChildRoot, ref childRoots);
            }
        }

        /// <summary>
        /// 自动绑定组件
        /// </summary>
        private void AutoBindComponent()
        {
            m_BindDatas.ClearArray();

            // Transform[] childs = m_Target.gameObject.GetComponentsInChildren<Transform>(true);
            
            var childRoots = new List<Transform>();
            GetChildRoots(m_Target.transform, ref childRoots);
            Transform[] childs = childRoots.ToArray();
            
            foreach (Transform child in childs)
            {
                m_TempFiledNames.Clear();
                m_TempComponentTypeNames.Clear();

                if (IsValidBind(child, m_TempFiledNames, m_TempComponentTypeNames))//m_Target.RuleHelper.
                {
                    for (int i = 0; i < m_TempFiledNames.Count; i++)
                    {
                        Component com = child.GetComponent(m_TempComponentTypeNames[i]);
                        if (com == null)
                        {
                            Debug.LogError($"{child.name}上不存在{m_TempComponentTypeNames[i]}的组件");
                        }
                        else
                        {
                            AddBindData(m_TempFiledNames[i], child.GetComponent(m_TempComponentTypeNames[i]));
                        }
                    }
                }
            }

            SyncBindComs();
        }
        
        private bool IsValidBind(Transform target, List<string> filedNames, List<string> componentTypeNames)
        {
            string[] strArray = target.name.Split('_');

            if (strArray.Length == 1)
            {
                return false;
            }

            string filedName = strArray[strArray.Length - 1];

            for (int i = 0; i < strArray.Length - 1; i++)
            {
                string str = strArray[i];
                string comName;
                if (m_KeyMapSetting.DefaultComponentKeyMap.TryGetValue(str, out comName) ||
                    m_KeyMapSetting.extraComponentKeyMap.TryGetValue(str, out comName))
                {
                    filedNames.Add($"{str}_{filedName}");
                    componentTypeNames.Add(comName);
                }
                else
                {
                    Debug.LogError($"{target.name}的命名中{str}不存在对应的组件类型，绑定失败");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 绘制辅助器选择框
        /// </summary>
        private void DrawHelperSelect()
        {
            m_HelperTypeName = nameof(CustomAutoBindRuleHelper);//m_HelperTypeNames[0];

            if (m_Target.RuleHelper != null)
            {
                m_HelperTypeName = m_Target.RuleHelper.GetType().Name;

                for (int i = 0; i < m_HelperTypeNames.Length; i++)
                {
                    if (m_HelperTypeName == m_HelperTypeNames[i])
                    {
                        m_HelperTypeNameIndex = i;
                    }
                }
            }
            else
            {
                IAutoBindRuleHelper helper =
                    (IAutoBindRuleHelper)CreateHelperInstance(m_HelperTypeName, s_AssemblyNames);
                m_Target.RuleHelper = helper;
            }

            foreach (GameObject go in Selection.gameObjects)
            {
                ComponentAutoBindTool.Scripts.ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool.Scripts.ComponentAutoBindTool>();
                if (autoBindTool.RuleHelper == null)
                {
                    IAutoBindRuleHelper helper =
                        (IAutoBindRuleHelper)CreateHelperInstance(m_HelperTypeName, s_AssemblyNames);
                    autoBindTool.RuleHelper = helper;
                }
            }

            int selectedIndex = EditorGUILayout.Popup("AutoBindRuleHelper", m_HelperTypeNameIndex, m_HelperTypeNames);
            if (selectedIndex != m_HelperTypeNameIndex)
            {
                m_HelperTypeNameIndex = selectedIndex;
                m_HelperTypeName = m_HelperTypeNames[selectedIndex];
                IAutoBindRuleHelper helper =
                    (IAutoBindRuleHelper)CreateHelperInstance(m_HelperTypeName, s_AssemblyNames);
                m_Target.RuleHelper = helper;
            }
        }

        /// <summary>
        /// 绘制设置项
        /// </summary>
        private void DrawSetting()
        {
            EditorGUILayout.BeginHorizontal();
            if (m_targetScript != null)
            {
                EditorGUILayout.PropertyField(m_targetScript, true);

                var objectReferenceValue = m_targetScript.objectReferenceValue;
                if (objectReferenceValue != null)
                {
                    var scriptType = objectReferenceValue.GetType();
                    var namespaceName = scriptType.Namespace;
                    var className = scriptType.Name;
                    var scriptPath = AssetDatabase.FindAssets("t:MonoScript " + className) //objectReferenceValue.name
                        .Select(AssetDatabase.GUIDToAssetPath)
                        .FirstOrDefault();
                    var scriptFolder = Path.GetDirectoryName(scriptPath);

                    m_Namespace.stringValue = namespaceName;
                    m_ClassName.stringValue = className;
                    m_CodePath.stringValue = scriptFolder;

                    // Debug.Log("Namespace: " + namespaceName);
                    // Debug.Log("Class Name: " + className);
                    // Debug.Log("Script Path: " + scriptPath);

                    serializedObject.ApplyModifiedProperties();
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            m_Namespace.stringValue = EditorGUILayout.TextField(new GUIContent("命名空间："), m_Namespace.stringValue);
            // if (GUILayout.Button("默认设置"))
            // {
            //     m_Namespace.stringValue = m_Setting.Namespace;
            // }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            m_ClassName.stringValue = EditorGUILayout.TextField(new GUIContent("类名："), m_ClassName.stringValue);
            // if (GUILayout.Button("物体名"))
            // {
            //     m_ClassName.stringValue = m_Target.gameObject.name;
            // }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("代码保存路径：");
            EditorGUILayout.LabelField(m_CodePath.stringValue);
            EditorGUILayout.BeginHorizontal();
            // if (GUILayout.Button("选择路径"))
            // {
            //     string temp = m_CodePath.stringValue;
            //     m_CodePath.stringValue = EditorUtility.OpenFolderPanel("选择代码保存路径", Application.dataPath, "");
            //     if (string.IsNullOrEmpty(m_CodePath.stringValue))
            //     {
            //         m_CodePath.stringValue = temp;
            //     }
            // }
            //
            // if (GUILayout.Button("默认设置"))
            // {
            //     m_CodePath.stringValue = m_Setting.CodePath;
            // }

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制键值对数据
        /// </summary>
        private void DrawKvData()
        {
            //绘制key value数据

            int needDeleteIndex = -1;

            EditorGUILayout.BeginVertical();
            SerializedProperty property;

            for (int i = 0; i < m_BindDatas.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"[{i}]", GUILayout.Width(25));
                property = m_BindDatas.GetArrayElementAtIndex(i).FindPropertyRelative("Name");
                property.stringValue = EditorGUILayout.TextField(property.stringValue, GUILayout.Width(150));
                property = m_BindDatas.GetArrayElementAtIndex(i).FindPropertyRelative("BindCom");
                property.objectReferenceValue =
                    EditorGUILayout.ObjectField(property.objectReferenceValue, typeof(Component), true);

                if (GUILayout.Button("X"))
                {
                    //将元素下标添加进删除list
                    needDeleteIndex = i;
                }

                EditorGUILayout.EndHorizontal();
            }

            //删除data
            if (needDeleteIndex != -1)
            {
                m_BindDatas.DeleteArrayElementAtIndex(needDeleteIndex);
                SyncBindComs();
            }

            EditorGUILayout.EndVertical();
        }


        /// <summary>
        /// 添加绑定数据
        /// </summary>
        private void AddBindData(string name, Component bindCom)
        {
            int index = m_BindDatas.arraySize;
            m_BindDatas.InsertArrayElementAtIndex(index);
            SerializedProperty element = m_BindDatas.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("Name").stringValue = name;
            element.FindPropertyRelative("BindCom").objectReferenceValue = bindCom;
        }

        /// <summary>
        /// 同步绑定数据
        /// </summary>
        private void SyncBindComs()
        {
            m_BindComs.ClearArray();

            for (int i = 0; i < m_BindDatas.arraySize; i++)
            {
                SerializedProperty property = m_BindDatas.GetArrayElementAtIndex(i).FindPropertyRelative("BindCom");
                m_BindComs.InsertArrayElementAtIndex(i);
                m_BindComs.GetArrayElementAtIndex(i).objectReferenceValue = property.objectReferenceValue;
            }
        }

        /// <summary>
        /// 获取指定基类在指定程序集中的所有子类名称
        /// </summary>
        private string[] GetTypeNames(Type typeBase, string[] assemblyNames)
        {
            List<string> typeNames = new List<string>();
            foreach (string assemblyName in assemblyNames)
            {
                Assembly assembly = null;
                try
                {
                    assembly = Assembly.Load(assemblyName);
                }
                catch
                {
                    continue;
                }

                if (assembly == null)
                {
                    continue;
                }

                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    if (type.IsClass && !type.IsAbstract && typeBase.IsAssignableFrom(type))
                    {
                        typeNames.Add(type.FullName);
                    }
                }
            }

            typeNames.Sort();
            return typeNames.ToArray();
        }

        /// <summary>
        /// 创建辅助器实例
        /// </summary>
        private object CreateHelperInstance(string helperTypeName, string[] assemblyNames)
        {
            foreach (string assemblyName in assemblyNames)
            {
                Assembly assembly = Assembly.Load(assemblyName);

                object instance = assembly.CreateInstance(helperTypeName);
                if (instance != null)
                {
                    return instance;
                }
            }

            return null;
        }


        /// <summary>
        /// 生成自动绑定代码
        /// </summary>
        private void GenAutoBindCode()
        {
            GameObject go = m_Target.gameObject;

            string className = !string.IsNullOrEmpty(m_Target.ClassName) ? m_Target.ClassName : go.name;
            string codePath = !string.IsNullOrEmpty(m_Target.CodePath) ? m_Target.CodePath : m_Setting.CodePath;

            if (!Directory.Exists(codePath))
            {
                Debug.LogError($"{go.name}的代码保存路径{codePath}无效");
            }

            using (StreamWriter sw = new StreamWriter($"{codePath}/{className}.BindComponents.cs"))
            {
                // if (m_HelperTypeName == nameof(CustomAutoBindRuleHelper))
                    sw.WriteLine("using System;");
                sw.WriteLine($"using {m_Target.GetType().Namespace};");
                sw.WriteLine("using UnityEngine;");
                sw.WriteLine("using UnityEngine.UI;");
                //根据索引获取
                if (m_Target.BindDatas.Any(data => data.BindCom.GetType().Name == "TextMeshProUGUI"))
                    sw.WriteLine("using TMPro;");
                if (m_Target.BindDatas.Any(data => data.BindCom.GetType().Name == "LeanButton") || 
                    m_Target.BindDatas.Any(data => data.BindCom.GetType().Name == "LeanSwitch"))
                    sw.WriteLine("using Lean.Gui;");
                sw.WriteLine("");

                sw.WriteLine("//自动生成于：" + DateTime.Now);

                if (!string.IsNullOrEmpty(m_Target.Namespace))
                {
                    //命名空间
                    sw.WriteLine("namespace " + m_Target.Namespace);
                    sw.WriteLine("{");
                    sw.WriteLine("");
                }

                //类名
                sw.WriteLine($"\tpublic partial class {className}");
                sw.WriteLine("\t{");
                sw.WriteLine("");

                // switch (m_HelperTypeName)
                // {
                    // case nameof(DefaultAutoBindRuleHelper):
                    //     //组件字段
                    //     foreach (BindData data in m_Target.BindDatas)
                    //     {
                    //         data.Name = data.Name.Replace("_", "");
                    //         data.Name = Regex.Replace(data.Name, "^[A-Z]", m => m.Value.ToLower());
                    //         sw.WriteLine($"\t\tprivate {data.BindCom.GetType().Name} _{data.Name};");
                    //     }
                    //     sw.WriteLine("");
                    //
                    //     sw.WriteLine("\t\tprivate void GetBindComponents(GameObject go)");
                    //     sw.WriteLine("\t\t{");
                    //
                    //     //获取autoBindTool上的Component
                    //     sw.WriteLine($"\t\t\tComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();");
                    //     sw.WriteLine("");
                    //
                    //     //根据索引获取
                    //     for (int i = 0; i < m_Target.BindDatas.Count; i++)
                    //     {
                    //         BindData data = m_Target.BindDatas[i];
                    //         // 将每个单词的首字母小写
                    //         string filedName = $"_{data.Name}";
                    //         sw.WriteLine(
                    //             $"\t\t\t{filedName} = autoBindTool.GetBindComponent<{data.BindCom.GetType().Name}>({i});");
                    //     }
                    //
                    //     sw.WriteLine("\t\t}");
                    //
                    //     sw.WriteLine("\t}");
                    //
                    //     if (!string.IsNullOrEmpty(m_Target.Namespace))
                    //     {
                    //         sw.WriteLine("}");
                    //     }
                    //     break;
                    // case nameof(CustomAutoBindRuleHelper):
                        sw.WriteLine("\t\t[Serializable]");
                        sw.WriteLine("\t\tpublic class UIView");
                        sw.WriteLine("\t\t{");
                        //组件字段
                        foreach (BindData data in m_Target.BindDatas)
                        {
                            data.Name = data.Name.Replace("_", "");
                            data.Name = Regex.Replace(data.Name, "^[A-Z]", m => m.Value.ToLower());
                            sw.WriteLine($"\t\t\tpublic {data.BindCom.GetType().Name} {data.Name};");
                        }
                        sw.WriteLine("\t\t}");
                        sw.WriteLine("\t\t/// <summary>");
                        sw.WriteLine("\t\t/// ========== UI组件 ==========");
                        sw.WriteLine("\t\t/// </summary>");
                        sw.WriteLine("\t\tprivate UIView view;");
                        
                        sw.WriteLine("");

                        sw.WriteLine("\t\tprivate void GetBindComponents(GameObject go)");
                        sw.WriteLine("\t\t{");

                        //获取autoBindTool上的Component
                        sw.WriteLine(
                            $"\t\t\t{typeof(ComponentAutoBindTool.Scripts.ComponentAutoBindTool).FullName} autoBindTool = go.GetComponent<{typeof(ComponentAutoBindTool.Scripts.ComponentAutoBindTool).FullName}>();");
                        sw.WriteLine("");
                        
                        sw.WriteLine("\t\t\tview = new UIView");
                        sw.WriteLine("\t\t\t{");

                        //根据索引获取
                        for (int i = 0; i < m_Target.BindDatas.Count; i++)
                        {
                            BindData data = m_Target.BindDatas[i];
                            // 将每个单词的首字母小写
                            string filedName = $"{data.Name}";
                            sw.WriteLine(
                                $"\t\t\t\t{filedName} = autoBindTool.GetBindComponent<{data.BindCom.GetType().Name}>({i}),");
                        }
                        sw.WriteLine("\t\t\t};");

                        sw.WriteLine("\t\t}");

                        sw.WriteLine("\t}");

                        if (!string.IsNullOrEmpty(m_Target.Namespace))
                        {
                            sw.WriteLine("}");
                        }
                    //     break;
                    // default:
                    //     break;
                // }
            }

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("提示", "代码生成完毕", "OK");
        }
    }
}