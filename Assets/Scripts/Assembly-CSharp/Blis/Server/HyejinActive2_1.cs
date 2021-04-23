using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.HyejinActive2_1)]
	public class HyejinActive2_1 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection damageParam = SkillScriptParameterCollection.Create();

		
		private float chargeDuration;

		
		private float chargeStartTime;

		
		private CollisionBox3D collision;

		
		protected override void Start()
		{
			base.Start();
			if (collision == null)
			{
				collision = new CollisionBox3D(Vector3.zero, SkillWidth, SkillInnerRange, Vector3.zero);
				chargeDuration =
					GameDB.character.GetSummonData(Singleton<HyejinSkillData>.inst.A2SummonCodeBow).duration -
					Singleton<HyejinSkillData>.inst.A2MinActiveTime;
			}
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			LookAtPosition(Caster, info.cursorPosition);
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			WorldPlayerCharacter worldPlayerCharacter = Caster.Character as WorldPlayerCharacter;
			if (worldPlayerCharacter == null)
			{
				Log.E("hyejin W seq 0 error wpc null");
				Finish();
				yield break;
			}

			int summonCode = GetEquipWeaponMasteryType(Caster) == MasteryType.Bow
				? Singleton<HyejinSkillData>.inst.A2SummonCodeBow
				: Singleton<HyejinSkillData>.inst.A2SummonCodeShuriken;
			WorldSummonBase worldSummonBase =
				MonoBehaviourInstance<GameService>.inst.Spawn.SpawnSummon(worldPlayerCharacter, summonCode,
					GetSkillPoint());
			worldSummonBase.LookAt(Caster.Forward);
			chargeStartTime = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime +
			                  Singleton<HyejinSkillData>.inst.A2MinActiveTime;
			worldSummonBase.DeadAction(SummonDeadAction);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		private void SummonDeadAction(WorldSummonBase summonBase)
		{
			Vector3 forward = summonBase.transform.forward;
			Vector3 position = summonBase.GetPosition();
			collision.UpdatePosition(position);
			collision.UpdateWidth(SkillWidth);
			collision.UpdateDepth(SkillInnerRange);
			collision.UpdateNormalized(forward);
			List<SkillAgent> enemyCharacters = GetEnemyCharacters(collision);
			if (enemyCharacters.Any<SkillAgent>())
			{
				int skillLevel = SkillLevel;
				damageParam.Clear();
				float num = (MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - chargeStartTime) /
				            chargeDuration;
				if (num > 1f)
				{
					num = 1f;
				}

				int num2 = Mathf.RoundToInt(Singleton<HyejinSkillData>.inst.A2BaseMinDamage[skillLevel] * (1f - num) +
				                            Singleton<HyejinSkillData>.inst.A2BaseMaxDamage[skillLevel] * num);
				damageParam.Add(SkillScriptParameterType.Damage, num2);
				damageParam.Add(SkillScriptParameterType.DamageApCoef, Singleton<HyejinSkillData>.inst.A2ApDamage);
				casterInfo.SetAttackerStat(Caster.Owner, casterCachedStat);
				DamageTo(enemyCharacters, casterInfo, DamageType.Skill, DamageSubType.Normal, 1, damageParam,
					SkillSlotSet.Active2_1, position, (position - Caster.Position).normalized,
					Singleton<HyejinSkillData>.inst.A2DamageEffectSound);
				AddState(enemyCharacters, Singleton<HyejinSkillData>.inst.A2DebuffCode);
			}
		}
	}
}