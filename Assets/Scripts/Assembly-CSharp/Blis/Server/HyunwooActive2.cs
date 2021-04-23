using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.HyunwooActive2)]
	public class HyunwooActive2 : SkillScript
	{
		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			AddState(Caster, Singleton<HyunwooSkillActive2Data>.inst.BuffState);
			AddState(Caster, Singleton<HyunwooSkillActive2Data>.inst.BuffState_2[SkillLevel]);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}