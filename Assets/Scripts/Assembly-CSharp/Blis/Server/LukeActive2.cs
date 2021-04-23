using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.LukeActive2)]
	public class LukeActive2 : SkillScript
	{
		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (0f < SkillCastingTime1)
			{
				yield return FirstCastingTime();
			}

			AddState(Caster, Singleton<LukeSkillActive2Data>.inst.AddAttackBuffStateCodeByLevel[SkillLevel]);
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