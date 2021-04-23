using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.ChiaraActive4judgment)]
	public class ChiaraActive4judgment : SkillScript
	{
		
		private readonly SkillScriptParameterCollection damageParam = SkillScriptParameterCollection.Create();

		
		private bool killSomeone;

		
		private int passiveMaxStack;

		
		private int passiveStateGroup;

		
		private void Init()
		{
			if (passiveStateGroup == 0)
			{
				CharacterStateData data =
					GameDB.characterState.GetData(Singleton<ChiaraSkillData>.inst.PassiveDebuffStateCode[1]);
				passiveStateGroup = data.group;
				passiveMaxStack = data.maxStack;
			}
		}

		
		protected override void Start()
		{
			base.Start();
			killSomeone = false;
			Init();
			LockSkillSlot(SkillSlotSet.Active1_1);
			LockSkillSlot(SkillSlotSet.Active2_1);
			LockSkillSlot(SkillSlotSet.Active3_1);
			LockSkillSlot(SkillSlotSet.WeaponSkill);
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			float num = killSomeone
				? Singleton<ChiaraSkillData>.inst.A4KillCooldownModify
				: Singleton<ChiaraSkillData>.inst.A4SkillActiveCooldownModify;
			if (num != 0f)
			{
				ModifySkillCooldown(Caster, SkillSlotSet.Active4_1,
					Caster.GetSkillCooldown(SkillSlotSet.Active4_1) * num);
			}

			Caster.RemoveStateByGroup(10008, Caster.ObjectId);
			UnlockSkillSlot(SkillSlotSet.Active1_1);
			UnlockSkillSlot(SkillSlotSet.Active2_1);
			UnlockSkillSlot(SkillSlotSet.Active3_1);
			UnlockSkillSlot(SkillSlotSet.WeaponSkill);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			AddState(Caster, 10008, 0f);
			if (Singleton<ChiaraSkillData>.inst.A4JudgmentMoveDelayTime > 0f)
			{
				yield return WaitForSeconds(Singleton<ChiaraSkillData>.inst.A4JudgmentMoveDelayTime);
			}

			float d = Caster.Stat.Radius + Target.Stat.Radius;
			Vector3 position = Target.Position;
			Vector3 vector = Caster.Position - Target.Position;
			Vector3 destination = position + vector.normalized * d;
			bool flag;
			float num;
			Caster.MoveToDestinationForTime(destination, 0f, EasingFunction.Ease.Linear, true, out vector, out flag,
				out num);
			if (Caster.IsMoving())
			{
				Caster.StopMove();
			}

			Caster.RemoveStateByGroup(
				GameDB.characterState.GetData(Singleton<ChiaraSkillData>.inst.A4TransformStateCode).group,
				Caster.ObjectId);
			PlaySkillAction(Caster, 1);
			if (Target.IsAlive)
			{
				damageParam.Clear();
				damageParam.Add(SkillScriptParameterType.Damage,
					Singleton<ChiaraSkillData>.inst.A4JudgmentBaseDamage[SkillLevel]);
				damageParam.Add(SkillScriptParameterType.DamageApCoef,
					Singleton<ChiaraSkillData>.inst.A4JudgmentApDamage);
				bool isDyingCondition = Target.IsDyingCondition;
				DirectDamageTo(Target, DamageType.Skill, DamageSubType.Normal, 0, damageParam,
					Singleton<ChiaraSkillData>.inst.A4JudgmentDamageEffectAndSound);
				if (!isDyingCondition)
				{
					killSomeone = !Target.IsAlive || Target.IsDyingCondition;
				}

				Target.RemoveStateByGroup(passiveStateGroup, Caster.ObjectId);
			}

			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		public override UseSkillErrorCode IsCanUseSkill(WorldCharacter hitTarget, Vector3? cursorPosition,
			WorldMovableCharacter caster)
		{
			Init();
			if (hitTarget == null || !hitTarget.IsAlive)
			{
				return UseSkillErrorCode.NotInvalidTarget;
			}

			CharacterState characterState =
				hitTarget.StateEffector.FindStateByGroup(passiveStateGroup, caster.ObjectId);
			if (characterState == null)
			{
				return UseSkillErrorCode.NotInvalidTarget;
			}

			if (characterState.StackCount < passiveMaxStack)
			{
				return UseSkillErrorCode.NotInvalidTarget;
			}

			return UseSkillErrorCode.None;
		}
	}
}