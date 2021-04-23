using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.FioraActive2)]
	public class FioraActive2 : SkillScript
	{
		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			AddState(Caster, Singleton<FioraSkillActive2Data>.inst.BuffState[SkillLevel]);
			Caster.CancelNormalAttack();
			Caster.ReadyNormalAttack();
			Caster.MountNormalAttack(Singleton<FioraSkillActive2Data>.inst.FioraActive2Attack[SkillLevel]);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}