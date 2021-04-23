using System;
using System.Collections;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.SniperRifleActive)]
	public class SniperRifleActive : SkillScript
	{
		
		private int bullet;

		
		private readonly SkillScriptParameterCollection damage = SkillScriptParameterCollection.Create();

		
		private bool isCancel;

		
		private bool isDamaged;

		
		private bool isReady;

		
		private float lastShotTime;

		
		private SightRangeLink sightRangeLink;

		
		protected override void Start()
		{
			base.Start();
			isReady = false;
			isCancel = false;
			isDamaged = false;
			LookAtPosition(Caster, info.cursorPosition);
			LockSkillSlots(SkillSlotIndex.Active1);
			LockSkillSlots(SkillSlotIndex.Active2);
			LockSkillSlots(SkillSlotIndex.Active3);
			LockSkillSlots(SkillSlotIndex.Active4);
			bullet = Singleton<SniperRifleSkillActiveData>.inst.maxBullet;
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnBeforeDamageProcess = (Action<WorldCharacter, DamageInfo>) Delegate.Combine(
				inst.OnBeforeDamageProcess, new Action<WorldCharacter, DamageInfo>(OnBeforeDamageProcess));
			BattleEventCollector inst2 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst2.OnBeforeSkillActiveEvent = (Action<WorldCharacter, SkillData, SkillSlotSet>) Delegate.Combine(
				inst2.OnBeforeSkillActiveEvent,
				new Action<WorldCharacter, SkillData, SkillSlotSet>(OnBeforeSkillActiveEvent));
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			if (cancel)
			{
				Caster.BlockAllySight(BlockedSightType.SniperRifleActive, false);
				Caster.RemoveStateByGroup(3011000, Caster.ObjectId);
			}

			if (sightRangeLink != null)
			{
				sightRangeLink.RemoveSightRangeLink();
				sightRangeLink = null;
			}

			UnlockSkillSlots(SkillSlotIndex.Active1);
			UnlockSkillSlots(SkillSlotIndex.Active2);
			UnlockSkillSlots(SkillSlotIndex.Active3);
			UnlockSkillSlots(SkillSlotIndex.Active4);
			UnlockSkillSlot(SkillSlotSet.WeaponSkill);
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnBeforeDamageProcess = (Action<WorldCharacter, DamageInfo>) Delegate.Remove(
				inst.OnBeforeDamageProcess, new Action<WorldCharacter, DamageInfo>(OnBeforeDamageProcess));
			BattleEventCollector inst2 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst2.OnBeforeSkillActiveEvent = (Action<WorldCharacter, SkillData, SkillSlotSet>) Delegate.Remove(
				inst2.OnBeforeSkillActiveEvent,
				new Action<WorldCharacter, SkillData, SkillSlotSet>(OnBeforeSkillActiveEvent));
		}

		
		private void OnBeforeDamageProcess(WorldCharacter victim, DamageInfo damageInfo)
		{
			if (!Caster.IsAlive)
			{
				return;
			}

			if (Caster.ObjectId != victim.ObjectId)
			{
				return;
			}

			if (damageInfo == null)
			{
				return;
			}

			if (damageInfo.Damage > 0)
			{
				isDamaged = true;
			}
		}

		
		private void OnBeforeSkillActiveEvent(WorldCharacter skillCaster, SkillData skillData,
			SkillSlotSet skillSlotSet)
		{
			if (skillCaster == null)
			{
				return;
			}

			if (skillCaster.ObjectId != Caster.ObjectId)
			{
				return;
			}

			isCancel = true;
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			StartCoroutine(CoroutineUtil.DelayedAction(1, delegate { PlaySkillAction(Caster, 1); }));
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			StartConcentration();
			float concentrationStartTime = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
			while (!IsCancel())
			{
				if (!isReady && SkillConcentrationTime <=
					MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - concentrationStartTime)
				{
					isReady = true;
					Caster.BlockAllySight(BlockedSightType.SniperRifleActive, true);
					AddState(Caster, Singleton<SniperRifleSkillActiveData>.inst.BuffState);
				}

				yield return WaitForFrame();
			}

			FinishConcentration(false);
			PlaySkillAction(Caster, 4);
			Caster.BlockAllySight(BlockedSightType.SniperRifleActive, false);
			Caster.RemoveStateByGroup(3011000, Caster.ObjectId);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		protected override void OnPlayAgain(Vector3 hitPoint)
		{
			base.OnPlayAgain(hitPoint);
			isCancel = true;
		}

		
		private bool IsCancel()
		{
			return isDamaged || isCancel || bullet <= 0;
		}

		
		public override void OnSelect(WorldCharacter hitTarget, Vector3 hitPoint, Vector3 releasePoint)
		{
			base.OnSelect(hitTarget, hitPoint, releasePoint);
			if (hitTarget == null)
			{
				return;
			}

			if (hitTarget.SkillAgent == null)
			{
				return;
			}

			if (!hitTarget.IsAlive)
			{
				return;
			}

			if (IsCancel())
			{
				return;
			}

			if (!isReady)
			{
				return;
			}

			if (Caster.GetHostileType(hitTarget) == HostileType.Ally)
			{
				return;
			}

			if (!Caster.IsInSight(hitTarget.SightAgent, hitTarget.GetPosition(), hitTarget.Stat.Radius,
				hitTarget.IsInvisible))
			{
				return;
			}

			if (lastShotTime + Singleton<SniperRifleSkillActiveData>.inst.shotDelay +
			    Singleton<SniperRifleSkillActiveData>.inst.aimingDelay <
			    MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime)
			{
				lastShotTime = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
				StartCoroutine(Aiming(hitTarget.SkillAgent));
			}
		}

		
		private IEnumerator Aiming(SkillAgent target)
		{
			sightRangeLink = new SightRangeLink(target.Character, Caster.Character);
			PlaySkillAction(Caster, 2, target);
			yield return WaitForSeconds(Singleton<SniperRifleSkillActiveData>.inst.aimingDelay);
			if (target == null || !target.IsAlive)
			{
				yield break;
			}

			if (IsCancel())
			{
				yield break;
			}

			bullet--;
			PlaySkillAction(Caster, 3);
			ProjectileProperty projectile =
				PopProjectileProperty(Caster, Singleton<SniperRifleSkillActiveData>.inst.ProjectileCode);
			projectile.SetTargetObject(target.ObjectId);
			projectile.SetActionOnCollisionCharacter(
				delegate(SkillAgent targetAgent, AttackerInfo attackerInfo, Vector3 damagePoint,
					Vector3 damageDirection)
				{
					damage.Clear();
					damage.Add(SkillScriptParameterType.DamageApCoef,
						Singleton<SniperRifleSkillActiveData>.inst.ApCoefficient[SkillLevel]);
					DamageTo(targetAgent, attackerInfo, projectile.ProjectileData.damageType,
						projectile.ProjectileData.damageSubType, 0, damage, projectile.SkillUseInfo.skillSlotSet,
						damagePoint, damageDirection, 0);
				});
			LaunchProjectile(projectile);
			if (sightRangeLink != null)
			{
				sightRangeLink.RemoveSightRangeLink();
				sightRangeLink = null;
			}

			MonoBehaviourInstance<GameService>.inst.Announce.MakeNoise(Caster.WorldObject, null, NoiseType.Gunshot);
		}
	}
}