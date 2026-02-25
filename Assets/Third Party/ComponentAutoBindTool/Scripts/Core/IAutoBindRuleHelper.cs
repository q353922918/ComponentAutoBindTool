using System.Collections.Generic;
using UnityEngine;

namespace Third_Party.ComponentAutoBindTool.Scripts.Core
{
    /// <summary>
    /// 自动绑定规则辅助器接口
    /// </summary>
    public interface IAutoBindRuleHelper
    {
        /// <summary>
        /// 是否为有效绑定
        /// </summary>
        bool IsValidBind(Transform target,List<string> filedNames,List<string> componentTypeNames);

        Dictionary<string, string> GetPrefixesDict();
    }
}
