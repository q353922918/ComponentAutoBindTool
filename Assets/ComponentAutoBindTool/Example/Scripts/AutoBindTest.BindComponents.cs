using System;
using ComponentAutoBindTool.Scripts;
using UnityEngine;
using UnityEngine.UI;

//自动生成于：2024/10/30 21:29:56
namespace ComponentAutoBindTool.Example.Scripts
{

	public partial class AutoBindTest
	{

		[Serializable]
		public class UIView
		{
			public Image imgTest1;
			public Button btnTest2;
			public Dropdown dropTest4;
			public Image imgTest4;
		}
		/// <summary>
		/// ========== UI组件 ==========
		/// </summary>
		private UIView view;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool.Scripts.ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool.Scripts.ComponentAutoBindTool>();

			view = new UIView
			{
				imgTest1 = autoBindTool.GetBindComponent<Image>(0),
				btnTest2 = autoBindTool.GetBindComponent<Button>(1),
				dropTest4 = autoBindTool.GetBindComponent<Dropdown>(2),
				imgTest4 = autoBindTool.GetBindComponent<Image>(3),
			};
		}
	}
}
