using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.HartActive2)]
	public class HartActive2 : HartSkillScript
	{
		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			AddState(Caster, Singleton<HartSkillActive2Data>.inst.BuffState[SkillLevel]);
			if (0 < SkillEvolutionLevel)
			{
				AddState(Caster, Singleton<HartSkillActive2Data>.inst.BuffState2);
			}

			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}