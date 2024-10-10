using System;
using AutoBindTool.Scripts;
using UnityEngine;
using UnityEngine.UI;

//自动生成于：2024/10/11 0:02:39
	public partial class AutoBindTest
	{

		[Serializable]
		public class UIView
		{
			public Button btnTest2;
			public Dropdown dropTest4;
			public Image imgTest1;
			public Image imgTest4;
			public Text txtTest3;
		}
		/// <summary>
		/// ========== UI组件 ==========
		/// </summary>
		private UIView view;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			view = new UIView
			{
				btnTest2 = autoBindTool.GetBindComponent<Button>(0),
				dropTest4 = autoBindTool.GetBindComponent<Dropdown>(1),
				imgTest1 = autoBindTool.GetBindComponent<Image>(2),
				imgTest4 = autoBindTool.GetBindComponent<Image>(3),
				txtTest3 = autoBindTool.GetBindComponent<Text>(4),
			};
		}
	}
