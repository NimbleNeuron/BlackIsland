using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.NadineActive4)]
	public class NadineActive4 : SkillScript
	{
		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			AddState(Caster, Singleton<NadineSkillActive4Data>.inst.BuffState1[SkillLevel]);
			AddState(Caster, Singleton<NadineSkillActive4Data>.inst.BuffState2[SkillLevel]);
			CharacterStateData data =
				GameDB.characterState.GetData(Singleton<NadineSkillActive4Data>.inst.BuffState2[SkillLevel]);
			if (1 < data.maxStack)
			{
				Caster.ModifyStateValue(data.group, Caster.ObjectId, 0f, data.maxStack - 1, true);
			}

			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}