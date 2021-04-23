using Blis.Common;
using Blis.Common.Utils;
using ParadoxNotion.Design;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("스킬 쿨타임을 체크한다. 사용 가능하면 true, 아니면 false")]
	public class AiCheckSkillCooldown : ConditionTaskBase
	{
		
		protected override bool OnCheckCommandFrame()
		{
			if (this.skillSlotIndex == SkillSlotIndex.WeaponSkill)
			{
				if (!base.agent.IsTypeOf<WorldBotPlayerCharacter>())
				{
					return false;
				}
				WorldBotPlayerCharacter worldBotPlayerCharacter = (WorldBotPlayerCharacter)base.agent;
				return base.agent.CharacterSkill.GetSkillLevel(worldBotPlayerCharacter.GetEquipWeaponMasteryType()) > 0 && base.agent.CharacterSkill.CheckCooldown(worldBotPlayerCharacter.GetEquipWeaponMasteryType(), MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime) && !base.agent.CharacterSkill.IsHoldCooldown(worldBotPlayerCharacter.GetEquipWeaponMasteryType());
			}
			else
			{
				if (base.agent.CharacterSkill.GetSkillLevel(this.skillSlotIndex) <= 0)
				{
					return false;
				}
				bool? flag = base.agent.CharacterSkill.CheckCooldown(this.skillSlotIndex, MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime);
				if (flag == null)
				{
					return false;
				}
				bool? flag2 = base.agent.CharacterSkill.IsHoldCooldown(this.skillSlotIndex);
				return flag2 != null && flag.Value && !flag2.Value;
			}
		}

		
		public SkillSlotIndex skillSlotIndex;
	}
}
