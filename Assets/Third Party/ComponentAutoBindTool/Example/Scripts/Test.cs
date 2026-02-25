#if UNITY_EDITOR
using UnityEngine;

namespace DefaultNamespace
{
    public class Test : MonoBehaviour
    {
        [DisplayOnly]
        public int myInt;
    }
}
#endif