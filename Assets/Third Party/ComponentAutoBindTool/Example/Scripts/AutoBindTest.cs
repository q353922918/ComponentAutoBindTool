using Third_Party.ComponentAutoBindTool.Scripts.ViewCore;
using VContainer;

namespace Third_Party.ComponentAutoBindTool.Example.Scripts
{
    public partial class AutoBindTest : ViewLifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            
            RegisterCommon<AutoBindTestPresenter>(builder, view);
        }
    }
}
