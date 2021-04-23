using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UILocalizeLabel : BaseUI
	{
		[SerializeField] private string lnKey = default;


		private Text text = default;


		protected override void OnEnable()
		{
			if (text != null)
			{
				text.text = Ln.Get(lnKey);
			}
		}

		protected override void OnAwakeUI()
		{
			text = GetComponent<Text>();
			base.OnAwakeUI();
		}
	}
}