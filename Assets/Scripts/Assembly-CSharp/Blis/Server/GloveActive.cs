using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.GloveActive)]
	public class GloveActive : SkillScript
	{
		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			int skillLevel = SkillLevel;
			CharacterState characterState =
				CreateState(Caster, Singleton<GloveSkillActiveData>.inst.BuffState[skillLevel], 0, 0f);
			characterState.AddExternalStat(StatType.AttackRange,
				Singleton<GloveSkillActiveData>.inst.UppercutIncreaseAttackRange[skillLevel], StatType.None, 0f);
			AddState(Caster, characterState);
			Caster.CancelNormalAttack();
			Caster.ReadyNormalAttack();
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}