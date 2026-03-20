using Third_Party.ComponentAutoBindTool.Scripts.ViewCore;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Third_Party.ComponentAutoBindTool.Example.Scripts
{
    public class AutoBindTestPresenter : IStartable
    {
        private AutoBindTest.UIView view;
        private readonly ViewBundle originData;
        
        public AutoBindTestPresenter(ViewBundle vb)
        {
            originData = vb;
            Debug.Log($"AutoBindTestPresenter 构造函数");
        }
        
        [Inject]
        private void Register(AutoBindTest.UIView pView)
        {
            this.view = pView;
        }
        
        public void Start()
        {
            Debug.Log("TestPresenter.Start");
            if (originData.DataBundle is { Length: > 0 })
            {
                var param1 = (string)originData.DataBundle[0];
                view.txtTest3.text = param1;
            }
            else
            {
                view.txtTest3.text = "Hello World";
            }
        }
    }
}