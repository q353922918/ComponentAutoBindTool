using System;
using UnityEngine;
using UnityEngine.UI;
using Third_Party.ComponentAutoBindTool.Scripts.ViewCore;

//自动生成于：2026/2/25 15:42:33
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

		protected override void GetBindComponents(GameObject go)
		{
			Third_Party.ComponentAutoBindTool.Scripts.Core.ComponentAutoBindTool autoBindTool = go.GetComponent<Third_Party.ComponentAutoBindTool.Scripts.Core.ComponentAutoBindTool>();

			view = new UIView
			{
				imgTest1 = autoBindTool.GetBindComponent<Image>(0),
				btnTest2 = autoBindTool.GetBindComponent<Button>(1),
				txtTest3 = autoBindTool.GetBindComponent<Text>(2),
				dropTest4 = autoBindTool.GetBindComponent<Dropdown>(3),
				imgTest4 = autoBindTool.GetBindComponent<Image>(4),
			};
		}
	}
}
