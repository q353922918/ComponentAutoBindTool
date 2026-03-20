#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Third_Party.ComponentAutoBindTool.Scripts.Core
{
    public class AutoBindKeyMapSetting : ScriptableObject
    {
        /// <summary>
        /// 默认组件映射
        /// </summary>
        public readonly Dictionary<string, string> DefaultComponentKeyMap = new()
        {
            { "Tf", "Transform" },
            { "OAni", "Animation" },
            { "NAni", "Animator" },

            { "Rtf", "RectTransform" },
            { "Cav", "Canvas" },
            { "CGroup", "CanvasGroup" },
            { "VLGroup", "VerticalLayoutGroup" },
            { "HLGroup", "HorizontalLayoutGroup" },
            { "GLGroup", "GridLayoutGroup" },
            { "TGroup", "ToggleGroup" },

            { "Btn", "Button" },
            { "Img", "Image" },
            { "RImg", "RawImage" },
            { "Txt", "Text" },
            { "Inf", "InputField" },
            { "Sld", "Slider" },
            { "Mask", "Mask" },
            { "Mask2D", "RectMask2D" },
            { "Tog", "Toggle" },
            { "Sbr", "Scrollbar" },
            { "SRect", "ScrollRect" },
            { "Drop", "Dropdown" },
                
            { "TMTxt", "TMPro.TextMeshProUGUI" },
            { "TMDrop", "TMPro.TMP_Dropdown" },
            { "TMInf", "TMPro.TMP_InputField" },
        };
        
        public GenericDictionary<string, string> extraComponentKeyMap;

        public bool TryGetComponentTypeName(string key, out string componentTypeName)
        {
            if (DefaultComponentKeyMap.TryGetValue(key, out componentTypeName))
            {
                return true;
            }

            if (extraComponentKeyMap != null && extraComponentKeyMap.TryGetValue(key, out componentTypeName))
            {
                return true;
            }

            componentTypeName = null;
            return false;
        }

        public IEnumerable<KeyValuePair<string, string>> GetAllComponentKeyMaps()
        {
            foreach (KeyValuePair<string, string> item in DefaultComponentKeyMap.OrderBy(pair => pair.Key))
            {
                yield return item;
            }

            if (extraComponentKeyMap == null)
            {
                yield break;
            }

            foreach (KeyValuePair<string, string> item in extraComponentKeyMap.OrderBy(pair => pair.Key))
            {
                yield return item;
            }
        }

        [MenuItem("自动绑定UI组件/创建组件映射文件")]
        private static void CreateAutoBindKeyMapSetting()
        {
            string[] paths = AssetDatabase.FindAssets("t:AutoBindKeyMapSetting");
            if (paths.Length >= 1)
            {
                string path = AssetDatabase.GUIDToAssetPath(paths[0]);
                EditorUtility.DisplayDialog("警告", $"已存在AutoBindKeyMapSetting，路径:{path}", "确认");
                return;
            }

            AutoBindKeyMapSetting setting = CreateInstance<AutoBindKeyMapSetting>();
            AssetDatabase.CreateAsset(setting, "Assets/Third Party/ComponentAutoBindTool/AutoBindKeyMapSetting.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
#endif
