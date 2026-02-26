using Third_Party.ComponentAutoBindTool.Scripts.ViewCore;
using VContainer;
using VContainer.Unity;

namespace Third_Party.ComponentAutoBindTool.Example.Scripts
{
    public class AutoBindTestPresenter : IStartable
    {
        private AutoBindTest.UIView _view;
        private readonly ViewBundle _originData;
        
        public AutoBindTestPresenter(ViewBundle vb)
        {
            _originData = vb;
            CCDebug.GameLog($"AutoBindTestPresenter 构造函数");
        }
        
        [Inject]
        private void Register(AutoBindTest.UIView view)
        {
            _view = view;
        }
        
        public void Start()
        {
            CCDebug.GameLog("TestPresenter.Start");
            if (_originData.DataBundle is { Length: > 0 })
            {
                var param1 = (string)_originData.DataBundle[0];
                _view.txtTest3.text = param1;
            }
            else
            {
                _view.txtTest3.text = "Hello World";
            }
        }
    }
}