using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Third_Party.Kogane.Ex.DebugLogger
{
    [CreateAssetMenu( fileName = "IgnoreFileSo", menuName = "Kogane/IgnoreFileSo" )]
    public class IgnoreFileSo : ScriptableObject
    {
        [SerializeField]
        private List<Object> files;
        public List<Object> Files => files;
        
        [SerializeField, HideInInspector]
        private List<string> filePaths;
        /// <summary>
        /// 供外部使用的忽略文件路径
        /// </summary>
        public List<string> FilePaths => filePaths;

        public void RefreshFilePaths(List<string> paths)
        {
            filePaths = paths;
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }
    }
}
