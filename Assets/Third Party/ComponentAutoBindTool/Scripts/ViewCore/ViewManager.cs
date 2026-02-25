using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Third_Party.ComponentAutoBindTool.Scripts.ViewCore
{
    public class OpenViewInfo
    {
        public string ViewName;
        public object[] Params;
    }
    
    public class ViewManager : IViewManager
    {
        protected readonly LifetimeScope ParentScope;
        protected readonly IViewPrefabHelper ViewPrefabHelper;
        protected readonly IViewRootHelper ViewRootHelper;
        
        /// <summary>
        /// 菜单界面画布的基础排序值
        /// </summary>
        protected readonly int MenuBaseOrder = 1000;
        /// <summary>
        /// 弹窗画布的基础排序值
        /// </summary>
        protected readonly int ViewBaseOrder = 10000;
        
        /// <summary>
        /// 当前存在的界面
        /// </summary>
        protected readonly Stack<ViewLifetimeScope> CurOpenViews = new();
        
        /// <summary>
        /// 将要打开的界面（当 CurOpenViews 的个数 == 0 的时候会自动补充）
        /// </summary>
        protected readonly Queue<OpenViewInfo> WaitingOpenSingleViewInfos = new();

        [Inject]
        public ViewManager(LifetimeScope parentScope, IViewPrefabHelper viewPrefabHelper, IViewRootHelper viewRootHelper)
        {
            ParentScope = parentScope;
            ViewPrefabHelper = viewPrefabHelper;
            ViewRootHelper = viewRootHelper;
        }

        /// <summary>
        /// 获取当前栈顶界面
        /// </summary>
        /// <returns></returns>
        public ViewLifetimeScope GetPeekView()
        {
            CurOpenViews.TryPeek(out var view);
            return view;
        }

        /// <summary>
        /// 展示界面
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="immediately">是否立即打开（立即打开则放入栈中，否则缓存数据等待栈中元素为0时再入栈）</param>
        /// <param name="args"></param>
        /// <returns></returns>
        public ViewLifetimeScope ShowView(string viewName, bool immediately = true, params object[] args)
        {
            if (immediately)
            {
                if (CurOpenViews.Count > 0)
                {
                    // 检查栈顶界面是否需要隐藏
                    var oldPeekView = CurOpenViews.Peek();
                    // 栈顶重复的界面检查
                    if (oldPeekView.ViewName == viewName)
                    {
                        return oldPeekView;
                    }
                    oldPeekView.Pause();
                }
                
                return SetSpawnView(viewName, args);
            }

            var openViewInfo = UnityEngine.Pool.GenericPool<OpenViewInfo>.Get();
            openViewInfo.ViewName = viewName;
            openViewInfo.Params = args;
            WaitingOpenSingleViewInfos.Enqueue(openViewInfo);
            
            return null;
        }

        /// <summary>
        /// 在关闭栈顶界面的同时，强行插入新界面（栈内界面展示延后）
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public ViewLifetimeScope ShowViewAtHidePeekView(string viewName, params object[] args)
        {
            if (CurOpenViews.Count > 0)
            {
                var oldPeekView = CurOpenViews.Pop();
                oldPeekView.Hide();   
            }
            
            return SetSpawnView(viewName, args);
        }

        private ViewLifetimeScope SetSpawnView(string viewName, params object[] args)
        {
            var view = SpawnView(viewName, args);
            view.SetViewName(viewName);
            CurOpenViews.Push(view);
            // 设置画布层级
            switch (view.viewType)
            {
                case ViewType.Menu:
                    view.SetCanvasOrder(MenuBaseOrder);
                    break;
                case ViewType.Panel:
                    view.SetCanvasOrder(ViewBaseOrder + CurOpenViews.Count);
                    break;
            }

            var newPeekView = CurOpenViews.Peek();
            newPeekView.Show();
            return view;
        }

        private ViewLifetimeScope SpawnView(string viewName, params object[] args)
        {
            ViewLifetimeScope view; 
            using (LifetimeScope.EnqueueParent(ParentScope))
            {
                var viewPrefab = ViewPrefabHelper.GetViewPrefab(viewName);
                view = Object.Instantiate(viewPrefab, ViewRootHelper.GetViewRoot()).GetComponent<ViewLifetimeScope>();
                view.Init(args);
            }
            return view;
        }

        /// <summary>
        /// 关闭栈顶界面
        /// </summary>
        public void HideView()
        {
            if (CurOpenViews.Count == 0)
            {
                Debug.LogError("No view to hide...");       
                return;
            }
            
            var oldPeekView = CurOpenViews.Pop();
            oldPeekView.Hide();

            if (CurOpenViews.Count > 0)
            {
                var newPeekView = CurOpenViews.Peek();
                newPeekView.Resume();   
            }
        }

        public void HideAllViews()
        {
            while (CurOpenViews.Count > 0)
            {
                HideView();
            }
        }

        public void Tick()
        {
            if (WaitingOpenSingleViewInfos.Count > 0 && CurOpenViews.Count == 0)
            {
                var openViewInfo = WaitingOpenSingleViewInfos.Dequeue();
                ShowView(openViewInfo.ViewName, true, openViewInfo.Params);
            }
        }
    }
}