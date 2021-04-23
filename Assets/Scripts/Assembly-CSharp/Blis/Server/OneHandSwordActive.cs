using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.OneHandSwordActive)]
	public class OneHandSwordActive : SkillScript
	{
		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			AddState(Caster, Singleton<OneHandSwordSkillActiveData>.inst.BuffState_2[SkillLevel]);
			AddState(Caster, Singleton<OneHandSwordSkillActiveData>.inst.BuffState[SkillLevel]);
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