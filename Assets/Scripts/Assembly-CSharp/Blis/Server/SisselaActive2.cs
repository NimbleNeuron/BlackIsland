using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.SisselaActive2)]
	public class SisselaActive2 : SisselaSkillScript
	{
		
		private readonly SkillScriptParameterCollection damage = SkillScriptParameterCollection.Create();

		
		private CollisionCircle3D collision;

		
		private bool isOnPlayAgain;

		
		private WorldSummonServant wilson;

		
		protected override void Start()
		{
			base.Start();
			isOnPlayAgain = false;
			LockSkillSlot(SkillSlotSet.Active1_1);
			LockSkillSlot(SkillSlotSet.Active3_1);
			LockSkillSlot(SkillSlotSet.Active4_1);
			if (wilson == null)
			{
				WorldPlayerCharacter owner = (WorldPlayerCharacter) Caster.Character;
				wilson = GetWilson(owner);
			}

			if (collision == null)
			{
				collision = new CollisionCircle3D(Vector3.zero, SkillRange);
			}
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			UnlockSkillSlot(SkillSlotSet.Active1_1);
			UnlockSkillSlot(SkillSlotSet.Active3_1);
			UnlockSkillSlot(SkillSlotSet.Active4_1);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (!IsWilsonUnion())
			{
				SetWilsonState(true, wilson);
			}

			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			float whileDuration =
				GameDB.characterState.GetData(Singleton<SisselaSkillData>.inst.A2UntargetableStateCode).duration -
				Singleton<SisselaSkillData>.inst.A2DamageDelay;
			AddState(Caster, Singleton<SisselaSkillData>.inst.A2UntargetableStateCode);
			PlaySkillAction(Caster, 1);
			while (!isOnPlayAgain && whileDuration > 0f)
			{
				yield return WaitForFrame();
				whileDuration -= MonoBehaviourInstance<GameService>.inst.ServerFrameDeltaTime;
			}

			PlaySkillAction(Caster, 2);
			if (Singleton<SisselaSkillData>.inst.A2DamageDelay > 0f)
			{
				yield return WaitForSeconds(Singleton<SisselaSkillData>.inst.A2DamageDelay);
			}

			Caster.RemoveStateByGroup(
				GameDB.characterState.GetData(Singleton<SisselaSkillData>.inst.A2UntargetableStateCode).group,
				Caster.ObjectId);
			damage.Clear();
			damage.Add(SkillScriptParameterType.Damage, Singleton<SisselaSkillData>.inst.A2BaseDamage[SkillLevel]);
			damage.Add(SkillScriptParameterType.DamageApCoef, Singleton<SisselaSkillData>.inst.A2ApDamage);
			collision.UpdatePosition(wilson.GetPosition());
			List<SkillAgent> enemyCharacters = GetEnemyCharacters(collision);
			foreach (SkillAgent skillAgent in enemyCharacters)
			{
				Vector3 vector = GameUtil.DirectionOnPlane(Caster.Position, skillAgent.Position);
				if (vector == Vector3.zero)
				{
					vector = Vector3.forward;
				}

				KnockbackState knockbackState = CreateState<KnockbackState>(skillAgent, 2000010);
				knockbackState.Init(vector, Singleton<SisselaSkillData>.inst.A2KnockBackDistance,
					Singleton<SisselaSkillData>.inst.A2KnockBackMoveDuration, EasingFunction.Ease.EaseOutQuad, false);
				skillAgent.AddState(knockbackState, Caster.ObjectId);
			}

			DamageTo(enemyCharacters, DamageType.Skill, DamageSubType.Normal, 0, damage,
				Singleton<SisselaSkillData>.inst.A2EffectCode);
			PlaySkillAction(Caster, 3);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		protected override void OnPlayAgain(Vector3 hitPoint)
		{
			base.OnPlayAgain(hitPoint);
			isOnPlayAgain = true;
		}
	}
}