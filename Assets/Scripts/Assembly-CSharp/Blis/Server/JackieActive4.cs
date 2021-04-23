using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.JackieActive4)]
	public class JackieActive4 : SkillScript
	{
		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			if (CanActionDuringSkillPlaying())
			{
				PlaySkillAction(Caster, 1);
			}

			int skillLevel = SkillLevel;
			AddState(Caster, Singleton<JackieSkillActive4Data>.inst.BuffState[skillLevel]);
			CharacterStateData data =
				GameDB.characterState.GetData(Singleton<JackieSkillActive4Data>.inst.BuffState[skillLevel]);
			Caster.SetInCombatTime(data.duration);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}