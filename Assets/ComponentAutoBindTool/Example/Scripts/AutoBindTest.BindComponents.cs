using System;
using ComponentAutoBindTool.Scripts;
using UnityEngine;
using UnityEngine.UI;

//自动生成于：2024/10/29 22:53:51
	public partial class AutoBindTest
	{

		[Serializable]
		public class UIView
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

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool.Scripts.ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool.Scripts.ComponentAutoBindTool>();

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
