using UnityEngine;
using VContainer.Unity;

namespace Third_Party.ComponentAutoBindTool.Example.Scripts
{
    public class AutoBindTestBase : LifetimeScope
    {
        protected virtual void GetBindComponents(GameObject go){ }
    }
}