using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace Third_Party.ComponentAutoBindTool.Scripts.ViewCore
{
    public enum ViewType
    {
        /// <summary>
        /// 菜单
        /// </summary>
        Menu,
        /// <summary>
        /// 弹窗
        /// </summary>
        Panel,
    }
    
    public enum DestroyType
    {
        /// <summary>
        /// 不销毁
        /// </summary>
        NonDestroy,
        /// <summary>
        /// 延迟销毁（带时间）
        /// </summary>
        DelayDestroy,
    }

    // public enum ViewEventTrigger
    // {
    //     
    // }
    
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
    [RequireComponent(typeof(Third_Party.ComponentAutoBindTool.Scripts.Core.ComponentAutoBindTool))]
    public class ViewLifetimeScope : LifetimeScope
    {
        [SerializeField] public ViewType viewType;
        [SerializeField] public DestroyType destroyType;

        /// <summary>
        /// 用于存储和传递给视图的参数集合。
        /// </summary>
        private ViewBundle ViewBundle { get; } = new();

        /// <summary>
        /// 界面名称
        /// </summary>
        public string ViewName { get; private set; }

        protected override void Configure(IContainerBuilder builder)
        {
            GetBindComponents(gameObject);
        }

        protected virtual void GetBindComponents(GameObject go) { }
        
        protected void RegisterCommon<T>(IContainerBuilder builder, IUiViewComponent iView)
        {
            CCDebug.GameLog($"{ViewName} 配置相关注入");
            builder.RegisterInstance(iView).AsSelf();
            builder.RegisterEntryPoint<T>().WithParameter("vb", ViewBundle);
            builder.RegisterEntryPointExceptionHandler(Debug.LogError);
        }
        
        public virtual void Init(params object[] args)
        {
            CCDebug.GameLog($"{ViewName} 设置数据");
            ViewBundle.SetViewBundle(args);
        }

        /// <summary>
        /// 设置画布层级
        /// </summary>
        /// <param name="order"></param>
        public void SetCanvasOrder(int order)
        {
            GetComponent<Canvas>().sortingOrder = order;
        }

        public void SetViewName(string viewName)
        {
            ViewName = viewName;
        }

        /// <summary>
        /// 展示界面
        /// </summary>
        public virtual void Show()
        {
        }

        /// <summary>
        /// 关闭界面
        /// </summary>
        public virtual void Hide()
        {
        }

        /// <summary>
        /// 暂停界面
        /// </summary>
        public virtual void Pause()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 恢复界面
        /// </summary>
        public virtual void Resume()
        {
            gameObject.SetActive(true);
        }
    }
}
