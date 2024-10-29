using System.Collections.Generic;
using AutoBindTool.Scripts;
using UnityEngine;

namespace ComponentAutoBindTool.Scripts
{
    public class CustomAutoBindRuleHelper : IAutoBindRuleHelper
    {
        /// <summary>
        /// 命名前缀与类型的映射
        /// </summary>
        private readonly Dictionary<string, string> _mPrefixesDict = new Dictionary<string, string>()
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
            { "TMTxt", "TMPro.TextMeshProUGUI" },
            { "Inf", "InputField" },
            { "Sld", "Slider" },
            { "Mask", "Mask" },
            { "Mask2D", "RectMask2D" },
            { "Tog", "Toggle" },
            { "Sbr", "Scrollbar" },
            { "SRect", "ScrollRect" },
            { "Drop", "Dropdown" },
        
            { "LeanBtn", "LeanButton" },
            { "LeanSt", "LeanSwitch" },
        };

        public Dictionary<string, string> GetPrefixesDict()
        {
            return _mPrefixesDict;
        }
        public bool IsValidBind(Transform target, List<string> filedNames, List<string> componentTypeNames)
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
                if (_mPrefixesDict.TryGetValue(str, out comName))
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
    }
}
