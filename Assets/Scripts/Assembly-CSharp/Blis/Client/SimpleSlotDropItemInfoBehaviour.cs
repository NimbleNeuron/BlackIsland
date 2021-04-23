using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class SimpleSlotDropItemInfoBehaviour : SimpleSlotBehaviour
	{
		private readonly string tooltip;

		public SimpleSlotDropItemInfoBehaviour(SimpleSlot simpleSlot, string tooltip) : base(simpleSlot)
		{
			this.tooltip = tooltip;
		}


		public override Sprite GetIcon()
		{
			return SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Ico_Query");
		}


		public override Sprite GetBackground()
		{
			return SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Ico_ItemGradebg_Empty");
		}


		public override void ShowTooltip()
		{
			MonoBehaviourInstance<Tooltip>.inst.SetLabel(Ln.Get(tooltip), 296f);
			MonoBehaviourInstance<Tooltip>.inst.ShowFixed(simpleSlot.GetParentWindow(),
				Tooltip.TooltipPosition.TargetInfoPosition);
		}
	}
}