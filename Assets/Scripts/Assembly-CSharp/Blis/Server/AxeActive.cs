using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.AxeActive)]
	public class AxeActive : SkillScript
	{
		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			CharacterStateData data = GameDB.characterState.GetData(Singleton<AxeSkillActiveData>.inst.BuffState);
			Caster.ActivateStatCalculator(data.group, Caster.ObjectId, true);
			Caster.SetExternalNonCalculateStatValue(data.group, Caster.ObjectId,
				Singleton<AxeSkillActiveData>.inst.BuffIncreaseValue);
			if (Singleton<AxeSkillActiveData>.inst.Duration[SkillLevel] > 0f)
			{
				yield return WaitForSeconds(Singleton<AxeSkillActiveData>.inst.Duration[SkillLevel]);
			}

			Finish();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			CharacterStateData data = GameDB.characterState.GetData(Singleton<AxeSkillActiveData>.inst.BuffState);
			Caster.ActivateStatCalculator(data.group, Caster.ObjectId, false);
			Caster.SetExternalNonCalculateStatValue(data.group, Caster.ObjectId, 0f);
		}
	}
}