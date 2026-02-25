namespace Third_Party.ComponentAutoBindTool.Scripts.ViewCore
{
    public interface IViewManager
    {
        ViewLifetimeScope GetPeekView();
        ViewLifetimeScope ShowView(string viewName, bool immediately = true, params object[] args);
        ViewLifetimeScope ShowViewAtHidePeekView(string viewName, params object[] args);
        void HideView();
    }
}
