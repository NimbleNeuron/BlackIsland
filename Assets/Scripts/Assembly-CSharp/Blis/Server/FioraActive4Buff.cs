using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.FioraActive4Buff)]
	public class FioraActive4Buff : SkillScript
	{
		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			int skillLv = Caster.GetSkillLevel(SkillSlotIndex.Active4);
			int cost = Singleton<FioraSkillActive4Data>.inst.ConsumeSp[skillLv];
			while (Caster.Status.Sp >= cost)
			{
				int skillLevel = Caster.GetSkillLevel(SkillSlotIndex.Active4);
				if (skillLv != skillLevel)
				{
					skillLv = skillLevel;
					cost = Singleton<FioraSkillActive4Data>.inst.ConsumeSp[skillLv];
				}

				yield return WaitForFrames(2);
			}

			Caster.Owner.EndSequenceSkill(SkillSlotSet.Active4_1, Caster.GetEquipWeaponMasteryType());
			Caster.RemoveStateByGroup(Singleton<FioraSkillActive4Data>.inst.BuffStateGroup, Caster.ObjectId);
			Finish();
		}
	}
}