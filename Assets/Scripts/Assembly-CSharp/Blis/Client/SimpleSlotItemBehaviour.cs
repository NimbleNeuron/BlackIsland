using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class SimpleSlotItemBehaviour : SimpleSlotBehaviour
	{
		private readonly ItemData itemData;

		public SimpleSlotItemBehaviour(SimpleSlot simpleSlot, ItemData itemData) : base(simpleSlot)
		{
			this.itemData = itemData;
		}


		public override Sprite GetIcon()
		{
			return itemData.GetSprite();
		}


		public override Sprite GetBackground()
		{
			if (itemData != null)
			{
				return itemData.GetGradeSprite();
			}

			return SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Img_EmptySlot_Bg");
		}


		public override void ShowTooltip()
		{
			MonoBehaviourInstance<Tooltip>.inst.SetItem(itemData, 0, false);
			MonoBehaviourInstance<Tooltip>.inst.ShowFixed(simpleSlot.GetParentWindow(),
				Tooltip.TooltipPosition.TargetInfoPosition);
		}
	}
}