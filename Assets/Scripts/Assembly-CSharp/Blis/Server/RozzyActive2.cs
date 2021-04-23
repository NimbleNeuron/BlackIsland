using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.RozziActive2)]
	public class RozzyActive2 : SkillScript
	{
		
		private readonly CollisionCircle3D collisionHit = new CollisionCircle3D(Vector3.zero, 0f);

		
		private readonly HashSet<int> damagedIds = new HashSet<int>();

		
		private readonly SkillScriptParameterCollection damageParam = SkillScriptParameterCollection.Create();

		
		protected override void Start()
		{
			base.Start();
			Caster.RemoveStateByGroup(
				GameDB.characterState.GetData(Singleton<RozziSkillActive1Data>.inst.Active1MoveStateCode).group,
				Caster.ObjectId);
			LockSkillSlot(SkillSlotSet.Attack_1);
			LockSkillSlot(SkillSlotSet.Active1_1);
			LockSkillSlot(SkillSlotSet.Active3_1);
			LockSkillSlot(SkillSlotSet.Active4_1);
			LockSkillSlot(SkillSlotSet.WeaponSkill);
			if (!Caster.IsFullBullet())
			{
				Caster.Owner.CancelGunReload();
				Caster.GunReload(false, 0f);
			}

			AddState(Caster, Singleton<RozziSkillActive2Data>.inst.Active2SpeedUpStateCode);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			collisionHit.UpdatePosition(Caster.Position);
			collisionHit.UpdateRadius(SkillWidth);
			float attackStartTime = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime +
			                        Singleton<RozziSkillActive2Data>.inst.A2AttackStartTime;
			float attackEndTime = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime +
			                      Singleton<RozziSkillActive2Data>.inst.A2AttackEndTime;
			damagedIds.Clear();
			while (MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime <= attackEndTime)
			{
				yield return WaitForFrame();
				if (MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime >= attackStartTime)
				{
					collisionHit.UpdatePosition(Caster.Position);
					foreach (SkillAgent skillAgent in GetEnemyCharacters(collisionHit))
					{
						if (!damagedIds.Contains(skillAgent.ObjectId))
						{
							damageParam.Clear();
							damageParam.Add(SkillScriptParameterType.Damage,
								Singleton<RozziSkillActive2Data>.inst.DamageByLevel[SkillLevel]);
							damageParam.Add(SkillScriptParameterType.DamageApCoef,
								Singleton<RozziSkillActive2Data>.inst.DamageApCoef);
							DamageTo(skillAgent, DamageType.Skill, DamageSubType.Normal, 0, damageParam,
								Singleton<RozziSkillActive2Data>.inst.Active2HitEffectSound);
							AddState(skillAgent,
								Singleton<RozziSkillActive2Data>.inst.DebuffStateCodeByLevel[SkillLevel]);
							damagedIds.Add(skillAgent.ObjectId);
						}
					}
				}
			}

			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			UnlockSkillSlot(SkillSlotSet.Attack_1);
			UnlockSkillSlot(SkillSlotSet.Active1_1);
			UnlockSkillSlot(SkillSlotSet.Active3_1);
			UnlockSkillSlot(SkillSlotSet.Active4_1);
			UnlockSkillSlot(SkillSlotSet.WeaponSkill);
		}
	}
}