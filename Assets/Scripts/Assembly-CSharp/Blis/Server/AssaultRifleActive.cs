using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.AssaultRifleActive)]
	public class AssaultRifleActive : SkillScript
	{
		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			AddState(Caster, Singleton<AssaultRifleSkillActiveData>.inst.BuffState[SkillLevel]);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}