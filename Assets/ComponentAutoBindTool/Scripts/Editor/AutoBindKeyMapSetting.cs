using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ComponentAutoBindTool.Scripts.Editor
{
    public class AutoBindKeyMapSetting : ScriptableObject
    {
        /// <summary>
        /// 默认组件映射
        /// </summary>
        // [DisplayOnly] 
        public Dictionary<string, string> defaultComponentKeyMap = new()
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
            AssetDatabase.CreateAsset(setting, "Assets/ComponentAutoBindTool/AutoBindKeyMapSetting.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}