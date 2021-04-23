using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.YukiActive1)]
	public class YukiActive1 : SkillScript
	{
		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			AddState(Caster, Singleton<YukiSkillActive1Data>.inst.BuffState);
			Caster.CancelNormalAttack();
			Caster.ReadyNormalAttack();
			Caster.MountNormalAttack(Singleton<YukiSkillActive1Data>.inst.YukiActive1Attack[SkillLevel]);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}