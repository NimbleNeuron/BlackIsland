using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	public abstract class SilviaHumanState : SkillScript
	{
		
		protected override void Start()
		{
			base.Start();
			if (SwitchSkillSet(SkillSlotIndex.Attack, SkillSlotSet.Attack_1))
			{
				SwitchSkillSet(SkillSlotIndex.Active1, SkillSlotSet.Active1_1);
				SwitchSkillSet(SkillSlotIndex.Active2, SkillSlotSet.Active2_1);
				SwitchSkillSet(SkillSlotIndex.Active3, SkillSlotSet.Active3_1);
				SwitchSkillSet(SkillSlotIndex.Active4, SkillSlotSet.Active4_1);
			}

			UnlockSkillSlot(SkillSlotSet.WeaponSkill);
			Caster.RemoveStateByGroup(
				GameDB.characterState.GetData(Singleton<SilviaSkillBikeData>.inst.BikeSpeedUpState[1]).group,
				Caster.ObjectId);
			Caster.RemoveStateByGroup(Singleton<SilviaSkillBikeData>.inst.BikeSpeedCalculateStateGroup,
				Caster.ObjectId);
			Caster.RemoveStateByGroup(
				GameDB.characterState.GetData(Singleton<SilviaSkillBikeData>.inst.BikeSpeedDownStateCode).group,
				Caster.ObjectId);
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForFrame();
		}
	}
}