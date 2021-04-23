using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class SimpleSlotSkillBehaviour : SimpleSlotBehaviour
	{
		private readonly SkillData skillData;


		public SimpleSlotSkillBehaviour(SimpleSlot simpleSlot, SkillData skillData) : base(simpleSlot)
		{
			this.skillData = skillData;
		}


		public override Sprite GetIcon()
		{
			if (skillData != null)
			{
				return GameDB.skill.GetSkillIcon(skillData.Icon);
			}

			return null;
		}


		public override Sprite GetBackground()
		{
			return SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite(skillData == null
				? "Img_EmptySlot_Bg"
				: "Img_Skill_BasicSlot");
		}


		public override void ShowTooltip()
		{
			MonoBehaviourInstance<Tooltip>.inst.SetSkill(skillData, "", false);
			MonoBehaviourInstance<Tooltip>.inst.ShowFixed(simpleSlot.GetParentWindow(),
				Tooltip.TooltipPosition.TargetInfoPosition);
		}
	}
}