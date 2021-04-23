using Blis.Common;
using UnityEngine;
using UnityEngine.AI;

namespace Blis.Server
{
	
	[SkillScript(SkillId.SisselaActive3Grab)]
	public class SisselaActive3Grab : Grab
	{
		
		private readonly SkillScriptParameterCollection damageParam = SkillScriptParameterCollection.Create();

		
		
		protected override bool ifNotOnNavDoWrap => false;

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			LockSkillSlotWithPacket(SkillSlotSet.Active2_1 | SkillSlotSet.Active3_1, false);
			WorldSummonServant wilson = SisselaSkillScript.GetWilson((WorldPlayerCharacter) Caster.Character);
			wilson.SetCanControl(true);
			int skillLevel = Caster.GetSkillLevel(SkillSlotIndex.Active3);
			if (Caster.GetHostileType(Target.Character) == HostileType.Ally)
			{
				ShieldState shieldState =
					CreateState<ShieldState>(Target, Singleton<SisselaSkillData>.inst.A3ShieldState);
				shieldState.Init(Singleton<SisselaSkillData>.inst.A3ApShield,
					Singleton<SisselaSkillData>.inst.A3BaseShield[skillLevel]);
				AddState(Target, shieldState);
				ModifySkillCooldown(Caster, SkillSlotSet.Active3_1,
					Singleton<SisselaSkillData>.inst.A3ShieldCooldownReduce);
				PlaySkillAction(Target, SkillId.SisselaActive3Grab, 2, wilson.ObjectId);
			}
			else
			{
				damageParam.Clear();
				damageParam.Add(SkillScriptParameterType.Damage,
					Singleton<SisselaSkillData>.inst.A3BaseDamage[skillLevel]);
				damageParam.Add(SkillScriptParameterType.DamageApCoef, Singleton<SisselaSkillData>.inst.A3ApDamage);
				DamageTo(Target, DamageType.Skill, DamageSubType.Normal, 0, damageParam,
					Singleton<SisselaSkillData>.inst.A3EffectSoundCode);
				AddState(Target, Singleton<SisselaSkillData>.inst.A3StunState);
				PlaySkillAction(Target, SkillId.SisselaActive3Grab, 1, wilson.ObjectId);
			}

			NavMeshHit navMeshHit;
			Vector3 vector;
			if (Target != null && !NavMesh.SamplePosition(Target.Position, out navMeshHit, 0.2f, 2147483640) &&
			    MoveAgent.SampleWidePosition(Target.Position, 2147483640, out vector))
			{
				Vector3 position = Target.Position;
				float num = GameUtil.DistanceOnPlane(position, vector);
				if (position.y != vector.y)
				{
					Vector3 destination = position;
					destination.y = vector.y;
					Target.WarpTo(destination, false);
				}

				if (num > 0f)
				{
					Vector3 direction = GameUtil.DirectionOnPlane(position, vector);
					KnockbackState knockbackState = CreateState<KnockbackState>(Target, 2000010);
					knockbackState.Init(direction, num, num / Singleton<SisselaSkillData>.inst.A3KnockBackSpeed,
						EasingFunction.Ease.Linear, true);
					Target.AddState(knockbackState, Caster.ObjectId);
				}
			}
		}
	}
}