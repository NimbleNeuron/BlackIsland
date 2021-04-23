using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.PistolActive)]
	public class PistolActive : SkillScript
	{
		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			AddState(Caster, Singleton<PistolSkillActiveData>.inst.BuffState[SkillLevel]);
			Caster.GunReload(false, Singleton<PistolSkillActiveData>.inst.GunReloadTime);
			SkillSlotSet skillSlotSetFlag = SkillSlotIndex.Active1.Index2SlotSets() |
			                                SkillSlotIndex.Active2.Index2SlotSets() |
			                                SkillSlotIndex.Active3.Index2SlotSets() |
			                                SkillSlotIndex.Active4.Index2SlotSets();
			ModifySkillCooldown(Caster, skillSlotSetFlag, Singleton<PistolSkillActiveData>.inst.SkillCooldownReduce);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}