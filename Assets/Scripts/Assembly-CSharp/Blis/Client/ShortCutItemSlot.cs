using Blis.Common;
using UnityEngine.UI;

namespace Blis.Client
{
	public class ShortCutItemSlot : ItemDataSlot
	{
		private Image bg;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			bg = GameUtil.Bind<Image>(gameObject, "Bg");
		}


		public override void ResetSlot()
		{
			base.ResetSlot();
			if (bg != null)
			{
				bg.enabled = false;
			}
		}


		public override void SetItemData(ItemData itemData)
		{
			base.SetItemData(itemData);
			if (bg != null)
			{
				bg.enabled = true;
			}
		}
	}
}