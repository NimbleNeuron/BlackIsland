using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.WolfHowling)]
	public class WolfHowling : SkillScript
	{
		
		private readonly CollisionCircle3D sector = new CollisionCircle3D(Vector3.zero, 0f);

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			CharacterStateData state = GameDB.characterState.GetData(Singleton<WolfSkillHowlingData>.inst.HowlingState);
			AddState(Caster, state.code);
			sector.UpdatePosition(Caster.Position);
			sector.UpdateRadius(info.SkillRange);
			List<SkillAgent> allies = GetAllies(sector);
			foreach (SkillAgent skillAgent in allies)
			{
				WorldMonster worldMonster = skillAgent.Character as WorldMonster;
				if (!(worldMonster == null) && !worldMonster.IsInCombat &&
				    worldMonster.MonsterData.monster == MonsterType.Wolf &&
				    !worldMonster.SkillAgent.AnyHaveStateByGroup(state.group))
				{
					AddState(worldMonster.SkillAgent, state.code);
					yield return WaitForSeconds(1f);
				}
			}

			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}