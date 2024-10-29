using UnityEditor;
using UnityEngine;

namespace ComponentAutoBindTool.Scripts.Editor
{
    /// <summary>
    /// 自动绑定全局设置
    /// </summary>
    public class AutoBindGlobalSetting : ScriptableObject
    {
        [SerializeField]
        private string m_CodePath;

        [SerializeField]
        private string m_Namespace;

        public string CodePath
        {
            get
            {
                return m_CodePath;
            }

        }

        public string Namespace
        {
            get
            {
                return m_Namespace;
            }
      
        }

        [MenuItem("自动绑定UI组件/创建配置文件")]
        private static void CreateAutoBindGlobalSetting()
        {
            string[] paths = AssetDatabase.FindAssets("t:AutoBindGlobalSetting");
            if (paths.Length >= 1)
            {
                string path = AssetDatabase.GUIDToAssetPath(paths[0]);
                EditorUtility.DisplayDialog("警告", $"已存在AutoBindGlobalSetting，路径:{path}", "确认");
                return;
            }

            AutoBindGlobalSetting setting = CreateInstance<AutoBindGlobalSetting>();
            AssetDatabase.CreateAsset(setting, "Assets/ComponentAutoBindTool/AutoBindGlobalSetting");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
