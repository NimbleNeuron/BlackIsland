using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Server
{
	
	[SkillScript(SkillId.EmmaActive1Attack)]
	public class EmmaActive1Attack : EmmaSkillScript
	{
		
		private readonly SkillScriptParameterCollection damageParameter = SkillScriptParameterCollection.Create();

		
		protected override void Start()
		{
			base.Start();
			LookAtPosition(Caster, info.cursorPosition);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			EmmaActive1Attack emmaActive1Attack = this;
			emmaActive1Attack.Start();
			if (emmaActive1Attack.Caster.ObjectType == ObjectType.SummonServant)
			{
				(emmaActive1Attack.Caster.Character as WorldSummonServant).SetIsInvisible(true);
			}

			emmaActive1Attack.damageParameter.Clear();
			ProjectileProperty pigeonDealerProjectile =
				emmaActive1Attack.PopProjectileProperty(emmaActive1Attack.Caster,
					Singleton<EmmaSkillActive1Data>.inst.PigeonDealerProjectileCode);
			emmaActive1Attack.damageParameter.Add(SkillScriptParameterType.Damage,
				Singleton<EmmaSkillActive1Data>.inst.DamageBySkillLevel[emmaActive1Attack.SkillLevel]);
			emmaActive1Attack.damageParameter.Add(SkillScriptParameterType.DamageApCoef,
				Singleton<EmmaSkillActive1Data>.inst.DamageApCoef);
			pigeonDealerProjectile.SetTargetDirection(GameUtil.Direction(emmaActive1Attack.Caster.Position,
				emmaActive1Attack.info.cursorPosition));
			ObjectType casterType = emmaActive1Attack.Caster.ObjectType;
			int ownerObjectId = emmaActive1Attack.Caster.Owner.ObjectId;
			pigeonDealerProjectile.SetActionOnCollisionCharacter(
				(targetAgent, attackerInfo, damagePoint,
					damageDirection) =>
				{
					DamageTo(targetAgent, attackerInfo, pigeonDealerProjectile.ProjectileData.damageType,
						pigeonDealerProjectile.ProjectileData.damageSubType, 0, damageParameter, SkillSlotSet,
						damagePoint, damageDirection,
						Singleton<EmmaSkillActive1Data>.inst.PigeonDealerDamageEffectAndSoundCode);
					switch (casterType)
					{
						case ObjectType.PlayerCharacter:
							List<WorldSummonBase> ownSummons =
								(Caster.Character as WorldPlayerCharacter).GetOwnSummons(
									IsPigeon);
							if (ownSummons != null)
							{
								foreach (WorldSummonBase worldSummonBase in ownSummons)
								{
									SetPigeonDealerState(casterType, targetAgent, Caster.ObjectId);
								}
							}

							ModifySkillCooldown(Caster, SkillSlotSet.Active1_1,
								Singleton<EmmaSkillActive1Data>.inst.CooldownReduce);
							break;
						case ObjectType.SummonServant:
							SetPigeonDealerState(casterType, targetAgent, ownerObjectId);
							break;
					}
				});
			emmaActive1Attack.LaunchProjectile(pigeonDealerProjectile, emmaActive1Attack.Caster.Position);
			if (emmaActive1Attack.Caster.ObjectType == ObjectType.SummonServant)
			{
				MonoBehaviourInstance<GameService>.inst.Spawn.DestroyWorldObject(emmaActive1Attack.Caster.WorldObject);
			}

			yield return emmaActive1Attack.WaitForFrame();
			emmaActive1Attack.Finish();

			// co: dotPeek
			// this.Start();
			// if (base.Caster.ObjectType == ObjectType.SummonServant)
			// {
			// 	(base.Caster.Character as WorldSummonServant).SetIsInvisible(true);
			// }
			// this.damageParameter.Clear();
			// ProjectileProperty pigeonDealerProjectile = base.PopProjectileProperty(base.Caster, Singleton<EmmaSkillActive1Data>.inst.PigeonDealerProjectileCode);
			// this.damageParameter.Add(SkillScriptParameterType.Damage, (float)Singleton<EmmaSkillActive1Data>.inst.DamageBySkillLevel[base.SkillLevel]);
			// this.damageParameter.Add(SkillScriptParameterType.DamageApCoef, Singleton<EmmaSkillActive1Data>.inst.DamageApCoef);
			// pigeonDealerProjectile.SetTargetDirection(GameUtil.Direction(base.Caster.Position, this.info.cursorPosition));
			// ObjectType casterType = base.Caster.ObjectType;
			// int ownerObjectId = base.Caster.Owner.ObjectId;
			// pigeonDealerProjectile.SetActionOnCollisionCharacter(delegate(SkillAgent targetAgent, AttackerInfo attackerInfo, Vector3 damagePoint, Vector3 damageDirection)
			// {
			// 	this.DamageTo(targetAgent, attackerInfo, pigeonDealerProjectile.ProjectileData.damageType, pigeonDealerProjectile.ProjectileData.damageSubType, 0, this.damageParameter, this.SkillSlotSet, damagePoint, damageDirection, Singleton<EmmaSkillActive1Data>.inst.PigeonDealerDamageEffectAndSoundCode, true, 0, 1f, true);
			// 	ObjectType casterType = casterType;
			// 	if (casterType == ObjectType.PlayerCharacter)
			// 	{
			// 		List<WorldSummonBase> ownSummons = (this.Caster.Character as WorldPlayerCharacter).GetOwnSummons(new Func<WorldSummonBase, bool>(EmmaSkillScript.IsPigeon));
			// 		if (ownSummons != null)
			// 		{
			// 			foreach (WorldSummonBase worldSummonBase in ownSummons)
			// 			{
			// 				this.SetPigeonDealerState(casterType, targetAgent, this.Caster.ObjectId);
			// 			}
			// 		}
			// 		this.ModifySkillCooldown(this.Caster, SkillSlotSet.Active1_1, Singleton<EmmaSkillActive1Data>.inst.CooldownReduce);
			// 		return;
			// 	}
			// 	if (casterType != ObjectType.SummonServant)
			// 	{
			// 		return;
			// 	}
			// 	this.SetPigeonDealerState(casterType, targetAgent, ownerObjectId);
			// });
			// base.LaunchProjectile(pigeonDealerProjectile, base.Caster.Position);
			// if (base.Caster.ObjectType == ObjectType.SummonServant)
			// {
			// 	MonoBehaviourInstance<GameService>.inst.Spawn.DestroyWorldObject(base.Caster.WorldObject);
			// }
			// yield return base.WaitForFrame();
			// this.Finish(false);
			// yield break;
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish();
		}

		
		private void SetPigeonDealerState(ObjectType casterType, SkillAgent targetAgent, int casterObjectId)
		{
			if (casterType != ObjectType.PlayerCharacter)
			{
				if (casterType != ObjectType.SummonServant)
				{
					return;
				}

				if (targetAgent.FindStateByGroup(
					    Singleton<EmmaSkillActive1Data>.inst.PigeonDealerDamagedByPlayerStateGroupCode,
					    casterObjectId) !=
				    null)
				{
					targetAgent.RemoveStateByGroup(
						Singleton<EmmaSkillActive1Data>.inst.PigeonDealerDamagedByPlayerStateGroupCode, casterObjectId);
					AddState(targetAgent, Singleton<EmmaSkillActive1Data>.inst.PigeonDealerMoveSpeedDownStateCode);
					return;
				}

				AddState(targetAgent, Singleton<EmmaSkillActive1Data>.inst.PigeonDealerDamagedByPigeonStateCode);
			}
			else
			{
				if (targetAgent.FindStateByGroup(
					    Singleton<EmmaSkillActive1Data>.inst.PigeonDealerDamagedByPigeonStateGroupCode,
					    casterObjectId) !=
				    null)
				{
					targetAgent.RemoveStateByGroup(
						Singleton<EmmaSkillActive1Data>.inst.PigeonDealerDamagedByPigeonStateGroupCode, casterObjectId);
					AddState(targetAgent, Singleton<EmmaSkillActive1Data>.inst.PigeonDealerMoveSpeedDownStateCode);
					return;
				}

				AddState(targetAgent, Singleton<EmmaSkillActive1Data>.inst.PigeonDealerDamagedByPlayerStateCode);
			}
		}
	}
}