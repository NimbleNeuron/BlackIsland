using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Server
{
	
	[SkillScript(SkillId.EmmaActive1Summon)]
	public class EmmaActive1Summon : EmmaSkillScript
	{
		
		private List<WorldSummonServant> ownPigeonList = new List<WorldSummonServant>();

		
		protected override void Start()
		{
			base.Start();
			LookAtPosition(Caster, info.cursorPosition);
			if (ownPigeonList == null)
			{
				ownPigeonList = new List<WorldSummonServant>();
			}

			ownPigeonList.Clear();
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (0f < SkillCastingTime1)
			{
				yield return FirstCastingTime();
			}

			WorldPlayerCharacter worldPlayerCharacter = Caster.Character as WorldPlayerCharacter;
			List<WorldSummonBase> ownSummons = worldPlayerCharacter.GetOwnSummons(IsPigeon);
			if (ownSummons != null)
			{
				foreach (WorldSummonBase worldSummonBase in ownSummons)
				{
					ownPigeonList.Add(worldSummonBase as WorldSummonServant);
				}
			}

			foreach (WorldSummonServant worldSummonServant in ownPigeonList)
			{
				AddState(worldSummonServant, worldSummonServant.SkillAgent,
					Singleton<EmmaSkillActive1Data>.inst.PigeonDealerAttackStateCode);
			}

			AddState(Caster, Singleton<EmmaSkillActive1Data>.inst.PigeonDealerAttackStateCode);
			WorldSummonBase worldSummonBase2 = MonoBehaviourInstance<GameService>.inst.Spawn.SpawnSummon(
				worldPlayerCharacter, Singleton<EmmaSkillActive1Data>.inst.PigeonSummonCode, Caster.Position);
			LookAtDirection(worldSummonBase2.SkillAgent, Caster.Forward);
			if (0f < SkillFinishDelayTime)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish();
		}
	}
}