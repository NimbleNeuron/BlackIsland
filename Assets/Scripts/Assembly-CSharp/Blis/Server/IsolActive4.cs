using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.IsolActive4)]
	public class IsolActive4 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		protected override void Start()
		{
			base.Start();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			Vector3 skillPoint = GetSkillPoint();
			Vector3 installPositionOnCaster = skillPoint - Caster.Position;
			LookAtPosition(Caster, skillPoint);
			CasterLockRotation(true);
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			WorldPlayerCharacter worldPlayerCharacter = Caster.Character as WorldPlayerCharacter;
			if (worldPlayerCharacter == null)
			{
				Finish();
				yield break;
			}

			Vector3 vector = installPositionOnCaster + Caster.Position;
			Vector3 vector2;
			if (MoveAgent.SamplePosition(vector, Caster.WalkableNavMask, out vector2))
			{
				vector = vector2;
				if (GameUtil.DistanceOnPlane(vector, Caster.Character.GetPosition()) > SkillRange)
				{
					MoveAgent.CanStraightMoveToDestination(Caster.Position, vector, Caster.WalkableNavMask,
						out vector2);
					vector = vector2;
				}
			}
			else
			{
				MoveAgent.CanStraightMoveToDestination(Caster.Position, vector, Caster.WalkableNavMask, out vector2);
				vector = vector2;
			}

			(MonoBehaviourInstance<GameService>.inst.Spawn.SpawnSummon(worldPlayerCharacter,
				Singleton<IsolSkillActive4Data>.inst.SummonObjectCode, vector) as WorldSummonTrap).SetActionOnTrapBurst(
				delegate(List<WorldCharacter> pTargets, WorldSummonBase pWorldSummon)
				{
					WorldPlayerCharacter owner = pWorldSummon.Owner;
					int skillLevel = SkillLevel;
					float num = 1f + owner.Stat.TrapDamageRatio;
					int num2 = Mathf.RoundToInt(Singleton<IsolSkillActive4Data>.inst.Damage[skillLevel] * num);
					float value = Singleton<IsolSkillActive4Data>.inst.SkillApCoef[skillLevel] * num;
					int stateCode = Singleton<IsolSkillActive4Data>.inst.DebuffState[skillLevel];
					foreach (WorldCharacter worldCharacter in pTargets)
					{
						parameterCollection.Clear();
						parameterCollection.Add(SkillScriptParameterType.Damage, num2);
						parameterCollection.Add(SkillScriptParameterType.DamageApCoef, value);
						DirectDamageTo(worldCharacter.SkillAgent, DamageType.Skill, DamageSubType.Trap, 0,
							parameterCollection, 0);
						AddState(worldCharacter.SkillAgent, stateCode);
					}
				});
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}