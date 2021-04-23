using System;
using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.ShoichiPassive)]
	public class ShoichiPassive : ShoichiSkillScript
	{
		
		private readonly CollisionCircle3D checkCollision = new CollisionCircle3D(Vector3.zero,
			Singleton<ShoichiSkillPassiveData>.inst.PassiveDaggerSkillRange);

		
		private bool IsStackAddTiming;

		
		private readonly Dictionary<ObjectType, int> objectTypeSort = new Dictionary<ObjectType, int>
		{
			{
				ObjectType.PlayerCharacter,
				1
			},
			{
				ObjectType.BotPlayerCharacter,
				1
			},
			{
				ObjectType.Monster,
				2
			},
			{
				ObjectType.Dummy,
				2
			}
		};

		
		private readonly SkillScriptParameterCollection passiveDaggerparameterCollection =
			SkillScriptParameterCollection.Create();

		
		private readonly HashSet<WorldSummonBase> passiveDaggers = new HashSet<WorldSummonBase>();

		
		private readonly HashSet<PassiveDaggerTargetInfo> passiveDaggerTargetInfo =
			new HashSet<PassiveDaggerTargetInfo>();

		
		protected override void Start()
		{
			base.Start();
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnAfterSkillDamageProcess = (Action<WorldCharacter, DamageInfo>) Delegate.Combine(
				inst.OnAfterSkillDamageProcess, new Action<WorldCharacter, DamageInfo>(OnAfterSkillDamageProcess));
			BattleEventCollector inst2 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst2.OnBeforeSkillActiveEvent = (Action<WorldCharacter, SkillData, SkillSlotSet>) Delegate.Combine(
				inst2.OnBeforeSkillActiveEvent,
				new Action<WorldCharacter, SkillData, SkillSlotSet>(OnBeforeSkillActiveEvent));
			BattleEventCollector inst3 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst3.OnAfterInstalledTrapEvent = (Action<WorldSummonBase, WorldCharacter, SummonData>) Delegate.Combine(
				inst3.OnAfterInstalledTrapEvent,
				new Action<WorldSummonBase, WorldCharacter, SummonData>(OnAfterInstalledTrapEvent));
		}

		
		public void OnAfterInstalledTrapEvent(WorldSummonBase summon, WorldCharacter owner, SummonData summonData)
		{
			if (!Caster.IsAlive)
			{
				return;
			}

			if (Caster.ObjectId != owner.ObjectId)
			{
				return;
			}

			if (summonData.code != Singleton<ShoichiSkillPassiveData>.inst.PassiveSummonObjectId)
			{
				return;
			}

			if (passiveDaggers.Contains(summon))
			{
				return;
			}

			summon.SetCustomAction(CustomAction, CustomActionCondition);
			passiveDaggers.Add(summon);
		}

		
		private bool CustomActionCondition(WorldSummonBase summonBase)
		{
			return !Caster.IsDyingCondition && !(summonBase.Owner == null) &&
			       summonBase.Owner.ObjectId.Equals(Caster.ObjectId) &&
			       !Caster.IsPlayingScript(SkillId.ShoichiActive4) &&
			       GameUtil.DistanceOnPlane(Caster.Position, summonBase.GetPosition()) <=
			       Singleton<ShoichiSkillPassiveData>.inst.PassiveDaggerSkillActiveRange;
		}

		
		private SkillAgent GetTarget(Vector3 position)
		{
			checkCollision.UpdatePosition(position);
			checkCollision.UpdateRadius(Singleton<ShoichiSkillPassiveData>.inst.PassiveDaggerSkillRange);
			List<SkillAgent> enemyCharacters = GetEnemyCharacters(checkCollision);
			enemyCharacters.RemoveAll(agent => !objectTypeSort.ContainsKey(agent.ObjectType));
			if (enemyCharacters.Count <= 0)
			{
				return null;
			}

			enemyCharacters.Sort(delegate(SkillAgent A, SkillAgent B)
			{
				int num = objectTypeSort[A.ObjectType].CompareTo(objectTypeSort[B.ObjectType]);
				if (num == 0)
				{
					num = A.IsDyingCondition.CompareTo(B.IsDyingCondition);
				}

				if (num == 0)
				{
					float num2 = GameUtil.DistanceOnPlane(position, A.Position);
					float value = GameUtil.DistanceOnPlane(position, B.Position);
					num = num2.CompareTo(value);
				}

				return num;
			});
			return enemyCharacters[0];
		}

		
		private void CustomAction(WorldSummonBase worldSummonBase)
		{
			if (!passiveDaggers.Contains(worldSummonBase))
			{
				return;
			}

			passiveDaggers.Remove(worldSummonBase);
			Vector3 position = worldSummonBase.GetPosition();
			worldSummonBase.DestroySelf();
			Active2Cooldown();
			if (Caster.IsHaveStateByGroup(Singleton<OneHandSwordSkillActiveData>.inst.BuffGroup1, Caster.ObjectId))
			{
				Caster.RemoveStateByGroup(Singleton<OneHandSwordSkillActiveData>.inst.BuffGroup1, Caster.ObjectId);
			}

			SkillAgent target = GetTarget(position);
			if (target == null)
			{
				AddState(Caster, Singleton<ShoichiSkillPassiveData>.inst.PassiveDaggerRemoveStateCode);
				return;
			}

			float num = passiveDaggerTargetInfo.Count *
			            Singleton<ShoichiSkillPassiveData>.inst.PassiveDaggerLaunchDelay2;
			float damageCoef =
				(Singleton<ShoichiSkillPassiveData>.inst.PassiveDaggerMaxCount - passiveDaggerTargetInfo.Count) /
				Singleton<ShoichiSkillPassiveData>.inst.PassiveDaggerMaxCount;
			passiveDaggerTargetInfo.Add(new PassiveDaggerTargetInfo(target, position,
				Singleton<ShoichiSkillPassiveData>.inst.PassiveDaggerLaunchDelay + num, damageCoef));
			AddState(Caster, Singleton<ShoichiSkillPassiveData>.inst.PassiveDaggerStateCode);
		}

		
		private void Active2Cooldown()
		{
			float skillCooldown = Caster.GetSkillCooldown(SkillSlotSet.Active2_1);
			if (skillCooldown <= Singleton<ShoichiSkillPassiveData>.inst.Active2ChangeCoolTime)
			{
				ModifySkillCooldown(Caster, SkillSlotSet.Active2_1,
					Singleton<ShoichiSkillPassiveData>.inst.Active2ChangeCoolTime - skillCooldown);
				return;
			}

			ModifySkillCooldown(Caster, SkillSlotSet.Active2_1,
				-skillCooldown + Singleton<ShoichiSkillPassiveData>.inst.Active2ChangeCoolTime);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			ShoichiPassive shoichiPassive1 = this;
			shoichiPassive1.Start();
			while (shoichiPassive1.Caster.IsAlive)
			{
				foreach (PassiveDaggerTargetInfo daggerTargetInfo in shoichiPassive1
					.passiveDaggerTargetInfo)
				{
					ShoichiPassive shoichiPassive = shoichiPassive1;
					PassiveDaggerTargetInfo info = daggerTargetInfo;
					info.LaunchTime -= MonoBehaviourInstance<GameService>.inst.ServerFrameDeltaTime;
					if (info.LaunchTime <= 0.0)
					{
						ProjectileProperty projectile = shoichiPassive1.PopProjectileProperty(shoichiPassive1.Caster,
							Singleton<ShoichiSkillPassiveData>.inst.PassiveProjectileId);
						projectile.SetTargetObject(info.target.ObjectId);
						projectile.SetActionOnCollisionCharacter(
							(targetAgent, attackerInfo,
								damagePoint, damageDirection) =>
							{
								// co: not resolved completely. closure_0.passiveDaggerparameterCollection.Clear() ...
								shoichiPassive1.passiveDaggerparameterCollection.Clear();
								float num =
									Singleton<ShoichiSkillPassiveData>.inst.PassiveDaggerDamageByLevel[
										shoichiPassive1.SkillLevel] * info.DamageCoef;
								shoichiPassive1.passiveDaggerparameterCollection.Add(
									SkillScriptParameterType.DamageApCoef,
									Singleton<ShoichiSkillPassiveData>.inst.SkillApCoef * info.DamageCoef);
								shoichiPassive1.passiveDaggerparameterCollection.Add(SkillScriptParameterType.Damage,
									num);
								shoichiPassive1.DamageTo(targetAgent, attackerInfo,
									projectile.ProjectileData.damageType, projectile.ProjectileData.damageSubType, 0,
									shoichiPassive1.passiveDaggerparameterCollection,
									projectile.SkillUseInfo.skillSlotSet, damagePoint, damageDirection, 0);
								shoichiPassive1.AddPassiveSkill();
							});
						shoichiPassive1.LaunchProjectile(projectile);
					}
				}

				shoichiPassive1.passiveDaggerTargetInfo.RemoveWhere(
					info =>
						(double) info.LaunchTime <= 0.0 || info.target == null);
				shoichiPassive1.passiveDaggers.RemoveWhere(dagger => !dagger.IsAlive);
				yield return shoichiPassive1.WaitForFrames(1);
			}

			shoichiPassive1.Finish();

			// co: dotPeek
			// this.Start();
			// while (base.Caster.IsAlive)
			// {
			// 	using (HashSet<ShoichiPassive.PassiveDaggerTargetInfo>.Enumerator enumerator = this.passiveDaggerTargetInfo.GetEnumerator())
			// 	{
			// 		while (enumerator.MoveNext())
			// 		{
			// 			ShoichiPassive.<>c__DisplayClass13_0 CS$<>8__locals1 = new ShoichiPassive.<>c__DisplayClass13_0();
			// 			CS$<>8__locals1.<>4__this = this;
			// 			CS$<>8__locals1.info = enumerator.Current;
			// 			CS$<>8__locals1.info.LaunchTime -= MonoBehaviourInstance<GameService>.inst.ServerFrameDeltaTime;
			// 			if (CS$<>8__locals1.info.LaunchTime <= 0f)
			// 			{
			// 				ProjectileProperty projectile = base.PopProjectileProperty(base.Caster, Singleton<ShoichiSkillPassiveData>.inst.PassiveProjectileId);
			// 				projectile.SetTargetObject(CS$<>8__locals1.info.target.ObjectId);
			// 				projectile.SetActionOnCollisionCharacter(delegate(SkillAgent targetAgent, AttackerInfo attackerInfo, Vector3 damagePoint, Vector3 damageDirection)
			// 				{
			// 					CS$<>8__locals1.<>4__this.passiveDaggerparameterCollection.Clear();
			// 					float value = (float)Singleton<ShoichiSkillPassiveData>.inst.PassiveDaggerDamageByLevel[CS$<>8__locals1.<>4__this.SkillLevel] * CS$<>8__locals1.info.DamageCoef;
			// 					float value2 = Singleton<ShoichiSkillPassiveData>.inst.SkillApCoef * CS$<>8__locals1.info.DamageCoef;
			// 					CS$<>8__locals1.<>4__this.passiveDaggerparameterCollection.Add(SkillScriptParameterType.DamageApCoef, value2);
			// 					CS$<>8__locals1.<>4__this.passiveDaggerparameterCollection.Add(SkillScriptParameterType.Damage, value);
			// 					CS$<>8__locals1.<>4__this.DamageTo(targetAgent, attackerInfo, projectile.ProjectileData.damageType, projectile.ProjectileData.damageSubType, 0, CS$<>8__locals1.<>4__this.passiveDaggerparameterCollection, projectile.SkillUseInfo.skillSlotSet, damagePoint, damageDirection, 0, true, 0, 1f, true);
			// 					CS$<>8__locals1.<>4__this.AddPassiveSkill();
			// 				});
			// 				base.LaunchProjectile(projectile);
			// 			}
			// 		}
			// 	}
			// 	this.passiveDaggerTargetInfo.RemoveWhere((ShoichiPassive.PassiveDaggerTargetInfo info) => info.LaunchTime <= 0f || info.target == null);
			// 	this.passiveDaggers.RemoveWhere((WorldSummonBase dagger) => !dagger.IsAlive);
			// 	yield return base.WaitForFrames(1);
			// }
			// this.Finish(false);
			// yield break;
		}

		
		private void OnAfterSkillDamageProcess(WorldCharacter victim, DamageInfo damageInfo)
		{
			if (!Caster.IsAlive)
			{
				return;
			}

			if (!IsStackAddTiming)
			{
				return;
			}

			if (damageInfo == null || damageInfo.Attacker == null)
			{
				return;
			}

			if (Caster.ObjectId != damageInfo.Attacker.ObjectId)
			{
				return;
			}

			IsStackAddTiming = false;
			AddPassiveSkill();
		}

		
		private void AddPassiveSkill()
		{
			int skillLevel = SkillLevel;
			CharacterState characterState =
				Caster.FindStateByGroup(Singleton<ShoichiSkillPassiveData>.inst.BuffStateGroup, Caster.ObjectId);
			int num = characterState == null ? 0 : characterState.StackCount;
			if (num <= Singleton<ShoichiSkillPassiveData>.inst.BuffMaxStackCount)
			{
				if (num < Singleton<ShoichiSkillPassiveData>.inst.BuffMaxStackCount)
				{
					AddState(Caster, Singleton<ShoichiSkillPassiveData>.inst.BuffStateLevel[skillLevel], 1);
				}
				else
				{
					AddState(Caster, Singleton<ShoichiSkillPassiveData>.inst.BuffStateLevel[skillLevel], 0);
				}

				characterState = characterState == null
					? Caster.FindStateByGroup(Singleton<ShoichiSkillPassiveData>.inst.BuffStateGroup, Caster.ObjectId)
					: characterState;
				num = characterState == null ? 0 : characterState.StackCount;
				if (num >= Singleton<ShoichiSkillPassiveData>.inst.BuffMaxStackCount)
				{
					AddState(Caster, Singleton<ShoichiSkillPassiveData>.inst.BuffMaxStateLevel[skillLevel]);
				}
			}
		}

		
		private void OnBeforeSkillActiveEvent(WorldCharacter victim, SkillData data, SkillSlotSet skillSlotSet)
		{
			if (data == null)
			{
				return;
			}

			if (victim == null)
			{
				return;
			}

			if (!Caster.IsAlive)
			{
				return;
			}

			if (Caster.ObjectId != victim.ObjectId)
			{
				return;
			}

			if (data.SkillId == SkillId.ShoichiPassive || data.SkillId == SkillId.ShoichiNormalAttack)
			{
				return;
			}

			IsStackAddTiming = true;
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnAfterSkillDamageProcess = (Action<WorldCharacter, DamageInfo>) Delegate.Remove(
				inst.OnAfterSkillDamageProcess, new Action<WorldCharacter, DamageInfo>(OnAfterSkillDamageProcess));
			BattleEventCollector inst2 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst2.OnBeforeSkillActiveEvent = (Action<WorldCharacter, SkillData, SkillSlotSet>) Delegate.Remove(
				inst2.OnBeforeSkillActiveEvent,
				new Action<WorldCharacter, SkillData, SkillSlotSet>(OnBeforeSkillActiveEvent));
			BattleEventCollector inst3 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst3.OnAfterInstalledTrapEvent = (Action<WorldSummonBase, WorldCharacter, SummonData>) Delegate.Remove(
				inst3.OnAfterInstalledTrapEvent,
				new Action<WorldSummonBase, WorldCharacter, SummonData>(OnAfterInstalledTrapEvent));
			foreach (WorldSummonBase worldSummonBase in passiveDaggers)
			{
				worldSummonBase.DestroySelf();
			}

			passiveDaggers.Clear();
		}

		
		public class PassiveDaggerTargetInfo
		{
			
			public float DamageCoef;

			
			public float LaunchTime;

			
			public Vector3 startPosition;

			
			public SkillAgent target;

			
			public PassiveDaggerTargetInfo(SkillAgent target, Vector3 startPosition, float launchTime, float damageCoef)
			{
				this.target = target;
				this.startPosition = startPosition;
				LaunchTime = launchTime;
				DamageCoef = damageCoef;
			}
		}
	}
}