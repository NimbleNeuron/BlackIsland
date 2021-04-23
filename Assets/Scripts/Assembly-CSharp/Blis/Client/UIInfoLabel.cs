using Blis.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UIInfoLabel : UITooltipComponent
	{
		private Text desc;


		private Text key;


		private Text title;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			title = GameUtil.Bind<Text>(gameObject, "TitleContainer/Title");
			key = GameUtil.Bind<Text>(gameObject, "TitleContainer/Key");
			desc = GameUtil.Bind<Text>(gameObject, "Desc");
		}


		public void SetTitle(string text)
		{
			title.text = text;
			transform.localScale = Vector3.one;
		}


		public void SetDesc(string text)
		{
			desc.text = text;
		}


		public void SetKey(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				key.text = "";
				return;
			}

			key.text = "[" + text + "]";
		}


		public override void Clear()
		{
			base.Clear();
			transform.localScale = Vector3.zero;
		}
	}
}