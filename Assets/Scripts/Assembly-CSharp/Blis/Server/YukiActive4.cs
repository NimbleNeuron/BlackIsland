using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.YukiActive4)]
	public class YukiActive4 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		private CollisionSector3D sector;

		
		protected override void Start()
		{
			base.Start();
			if (sector == null)
			{
				sector = new CollisionSector3D(Vector3.zero, SkillRange, SkillAngle, Vector3.zero);
			}
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Vector3 direction = GameUtil.Direction(Caster.Position, GetSkillPoint());
			LookAtDirection(Caster, direction);
			CasterLockRotation(true);
			Start();
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			StartConcentration();
			yield return WaitForSeconds(SkillConcentrationTime);
			FinishConcentration(false);
			if (SkillCastingTime2 > 0f)
			{
				yield return SecondCastingTime();
			}

			sector.UpdatePosition(Caster.Position);
			sector.UpdateNormalized(direction);
			sector.UpdateAngle(SkillAngle);
			sector.UpdateRadius(SkillRange);
			List<SkillAgent> enemyCharacters = GetEnemyCharacters(sector);
			if (enemyCharacters != null && enemyCharacters.Any<SkillAgent>())
			{
				parameterCollection.Clear();
				parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
					Singleton<YukiSkillActive4Data>.inst.SkillApCoef);
				parameterCollection.Add(SkillScriptParameterType.Damage,
					Singleton<YukiSkillActive4Data>.inst.SkillDamage[SkillLevel]);
				PlaySkillAction(Caster, 1);
				bool flag = Caster.Status.ExtraPoint > 0;
				int effectAndSoundCode =
					flag
						? Singleton<YukiSkillActive4Data>.inst.PassiveEffectAndSound
						: Singleton<YukiSkillActive4Data>.inst.EffectAndSound;
				DamageTo(enemyCharacters, DamageType.Skill, DamageSubType.Normal, 42, parameterCollection,
					effectAndSoundCode);
				if (flag)
				{
					Caster.ExtraPointModifyTo(Caster, -1);
					if (Caster.Status.ExtraPoint == 0)
					{
						PlaySkillAction(Caster, SkillId.YukiPassive, 3);
					}
				}

				foreach (SkillAgent target in enemyCharacters)
				{
					int stateCode = flag
						? Singleton<YukiSkillActive4Data>.inst.DebuffState_2[SkillLevel]
						: Singleton<YukiSkillActive4Data>.inst.DebuffState_1[SkillLevel];
					CharacterState characterState = CreateState(target, stateCode);
					characterState.AddExternalStat(StatType.MoveSpeedRatio,
						Singleton<YukiSkillActive4Data>.inst.DebuffMoveSpeedRatio, StatType.None, 0f);
					AddState(target, characterState);
				}

				yield return WaitForSeconds(Singleton<YukiSkillActive4Data>.inst.FinalMotionTime);
			}

			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}