using System;
using UnityEngine;
using UnityEngine.UI;
using Third_Party.ComponentAutoBindTool.Scripts.ViewCore;

//自动生成于：2026/3/20 18:19:35
namespace Third_Party.ComponentAutoBindTool.Example.Scripts
{
	public partial class AutoBindTest
	{
		[Serializable]
		public class UIView : IUiViewComponent
		{
			public Image imgTest1;
			public Button btnTest2;
			public Text txtTest3;
			public Dropdown dropTest4;
			public Image imgTest4;
		}

		/// <summary>
		/// ========== UI组件 ==========
		/// </summary>
		private UIView view;

		public override void EnsureAutoBind(GameObject go)
		{
			if (view != null)
			{
				return;
			}

			if (go == null)
			{
				Debug.LogError("[AutoBindTest] AutoBind 失败：目标 GameObject 为空。");
				return;
			}

			Third_Party.ComponentAutoBindTool.Scripts.Core.ComponentAutoBindTool autoBindTool = go.GetComponent<Third_Party.ComponentAutoBindTool.Scripts.Core.ComponentAutoBindTool>();

			if (autoBindTool == null)
			{
				Debug.LogError("[AutoBindTest] AutoBind 失败：未找到 ComponentAutoBindTool。", go);
				return;
			}

			view = new UIView
			{
				imgTest1 = autoBindTool.GetBindComponent<Image>(0, nameof(UIView.imgTest1)),
				btnTest2 = autoBindTool.GetBindComponent<Button>(1, nameof(UIView.btnTest2)),
				txtTest3 = autoBindTool.GetBindComponent<Text>(2, nameof(UIView.txtTest3)),
				dropTest4 = autoBindTool.GetBindComponent<Dropdown>(3, nameof(UIView.dropTest4)),
				imgTest4 = autoBindTool.GetBindComponent<Image>(4, nameof(UIView.imgTest4)),
			};
		}
	}
}
