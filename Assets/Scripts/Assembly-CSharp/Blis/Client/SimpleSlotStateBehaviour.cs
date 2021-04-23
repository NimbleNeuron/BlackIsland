using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class SimpleSlotStateBehaviour : SimpleSlotBehaviour
	{
		private readonly CharacterStateData stateData;


		public SimpleSlotStateBehaviour(SimpleSlot simpleSlot, CharacterStateData stateData) : base(simpleSlot)
		{
			this.stateData = stateData;
		}


		public override Sprite GetIcon()
		{
			return stateData.GroupData.GetSprite();
		}


		public override Sprite GetBackground()
		{
			return SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite(stateData == null
				? "Img_EmptySlot_Bg"
				: "Img_Skill_BasicSlot");
		}


		public override void ShowTooltip()
		{
			MonoBehaviourInstance<Tooltip>.inst.SetStateEffect(stateData, null, false);
			MonoBehaviourInstance<Tooltip>.inst.ShowFixed(simpleSlot.GetParentWindow(),
				Tooltip.TooltipPosition.TargetInfoPosition);
		}
	}
}