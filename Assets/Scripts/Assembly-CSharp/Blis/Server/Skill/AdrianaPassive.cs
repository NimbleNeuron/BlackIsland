using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.AdrianaPassive)]
	public class AdrianaPassive : AdrianaSkillScript
	{
		
		private readonly Dictionary<int, float> cachedEnemies = new Dictionary<int, float>();

		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		private CollisionCircle3D collision;

		
		protected override void Start()
		{
			base.Start();
			cachedEnemies.Clear();
			if (collision == null)
			{
				collision = new CollisionCircle3D(Vector3.zero, 0f);
			}
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			AdrianaPassive adrianaPassive = this;
			adrianaPassive.Start();
			WorldMovableCharacter cachedCasterCharacter = adrianaPassive.Caster.Character as WorldMovableCharacter;
			float pyromaniacHealTimeStack = 0.0f;
			bool existMyFireFlame = false;
			while (true)
			{
				int countFireFlameProjectile = 0;
				cachedCasterCharacter.DoOwnProjectileAction(
					IsAdrianaFireFlameProjectile,
					worldOwnProjectile =>
					{
						PyromaniacDamage(worldOwnProjectile);
						PyromaniacHeal(ref pyromaniacHealTimeStack);
						++countFireFlameProjectile;
					});
				if (!existMyFireFlame && 0 < countFireFlameProjectile)
				{
					existMyFireFlame = true;
					adrianaPassive.PlaySkillAction(adrianaPassive.Caster, adrianaPassive.info.skillData.PassiveSkillId,
						1);
					adrianaPassive.AddState(adrianaPassive.Caster,
						Singleton<AdrianaSkillPassiveData>.inst.PyromaniacStateCode);
				}
				else if (existMyFireFlame && countFireFlameProjectile == 0)
				{
					existMyFireFlame = false;
					adrianaPassive.PlaySkillAction(adrianaPassive.Caster, adrianaPassive.info.skillData.PassiveSkillId,
						2);
					if (adrianaPassive.Caster.AnyHaveStateByGroup(Singleton<AdrianaSkillPassiveData>.inst
						.PyromaniacStateGroupCode))
					{
						adrianaPassive.Caster.RemoveStateByGroup(
							Singleton<AdrianaSkillPassiveData>.inst.PyromaniacStateGroupCode,
							adrianaPassive.Caster.ObjectId);
					}
				}

				yield return adrianaPassive.WaitForSeconds(Singleton<AdrianaSkillPassiveData>.inst
					.PassiveUpdateTime);
			}

			// co: dotPeek
			// 	this.Start();
			// 	WorldMovableCharacter cachedCasterCharacter = base.Caster.Character as WorldMovableCharacter;
			// 	float pyromaniacHealTimeStack = 0f;
			// 	bool existMyFireFlame = false;
			// 	for (;;)
			// 	{
			// 		int countFireFlameProjectile = 0;
			// 		cachedCasterCharacter.DoOwnProjectileAction(new Func<WorldProjectile, bool>(base.IsAdrianaFireFlameProjectile), delegate(WorldProjectile worldOwnProjectile)
			// 		{
			// 			this.PyromaniacDamage(worldOwnProjectile);
			// 			this.PyromaniacHeal(ref pyromaniacHealTimeStack);
			// 			int countFireFlameProjectile = countFireFlameProjectile;
			// 			countFireFlameProjectile++;
			// 		});
			// 		if (!existMyFireFlame && 0 < countFireFlameProjectile)
			// 		{
			// 			existMyFireFlame = true;
			// 			base.PlaySkillAction(base.Caster, this.info.skillData.PassiveSkillId, 1, 0, null);
			// 			base.AddState(base.Caster, Singleton<AdrianaSkillPassiveData>.inst.PyromaniacStateCode, null);
			// 		}
			// 		else if (existMyFireFlame && countFireFlameProjectile == 0)
			// 		{
			// 			existMyFireFlame = false;
			// 			base.PlaySkillAction(base.Caster, this.info.skillData.PassiveSkillId, 2, 0, null);
			// 			if (base.Caster.AnyHaveStateByGroup(Singleton<AdrianaSkillPassiveData>.inst.PyromaniacStateGroupCode))
			// 			{
			// 				base.Caster.RemoveStateByGroup(Singleton<AdrianaSkillPassiveData>.inst.PyromaniacStateGroupCode, base.Caster.ObjectId);
			// 			}
			// 		}
			// 		yield return base.WaitForSeconds(Singleton<AdrianaSkillPassiveData>.inst.PassiveUpdateTime);
			// 	}
			// 	yield break;
		}

		
		private void PyromaniacDamage(WorldProjectile worldOwnProjectile)
		{
			collision.UpdatePosition(worldOwnProjectile.GetPosition());
			collision.UpdateRadius(worldOwnProjectile.Property.ProjectileData.explosionRadius);
			foreach (SkillAgent skillAgent in GetEnemyCharacters(collision))
			{
				if (!cachedEnemies.ContainsKey(skillAgent.ObjectId) ||
				    MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime >=
				    cachedEnemies[skillAgent.ObjectId])
				{
					cachedEnemies[skillAgent.ObjectId] =
						MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime +
						Singleton<AdrianaSkillPassiveData>.inst.FireFlameProjectileRefreshPeriod;
					int stackByGroup =
						skillAgent.Character.StateEffector.GetStackByGroup(
							Singleton<AdrianaSkillPassiveData>.inst.BurnsStateGroupCode, Caster.ObjectId);
					parameterCollection.Clear();
					parameterCollection.Add(SkillScriptParameterType.Damage,
						Singleton<AdrianaSkillPassiveData>.inst.BurnsDamageByLevel[SkillLevel]);
					parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
						Singleton<AdrianaSkillPassiveData>.inst.BurnsDamageApCoef);
					parameterCollection.Add(SkillScriptParameterType.FinalMoreDamage,
						stackByGroup * Singleton<AdrianaSkillPassiveData>.inst.BurnsDamageStackCoef);
					DamageTo(skillAgent, worldOwnProjectile.Property.ProjectileData.damageType,
						worldOwnProjectile.Property.ProjectileData.damageSubType, 0, parameterCollection,
						Singleton<AdrianaSkillPassiveData>.inst.BurnsDamageEffectAndSoundCode);
					AddState(skillAgent, Singleton<AdrianaSkillPassiveData>.inst.BurnsStateCode);
					if (skillAgent.AnyHaveStateByGroup(Singleton<AdrianaSkillPassiveData>.inst
						.BurnsMoveSpeedDownStateGroupCode))
					{
						skillAgent.RemoveStateByGroup(
							Singleton<AdrianaSkillPassiveData>.inst.BurnsMoveSpeedDownStateGroupCode, Caster.ObjectId);
					}

					AddState(skillAgent, Singleton<AdrianaSkillPassiveData>.inst.BurnsMoveSpeedDownStateCode);
				}
			}
		}

		
		private void PyromaniacHeal(ref float pyromaniacHealTimeStack)
		{
			if (Singleton<AdrianaSkillPassiveData>.inst.PyromaniacHealSPTerm <= pyromaniacHealTimeStack)
			{
				SpHealTo(Caster, 0, 0f, GetPyromaniacHealAmount(), false, 0);
				pyromaniacHealTimeStack -= Singleton<AdrianaSkillPassiveData>.inst.PyromaniacHealSPTerm;
				return;
			}

			pyromaniacHealTimeStack += MonoBehaviourInstance<GameService>.inst.ServerFrameDeltaTime;
		}

		
		private int GetPyromaniacHealAmount()
		{
			return (int) (Caster.Stat.MaxSp *
			              (Singleton<AdrianaSkillPassiveData>.inst.RecoverySpRatio[SkillLevel] * 0.01f));
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
		}
	}
}