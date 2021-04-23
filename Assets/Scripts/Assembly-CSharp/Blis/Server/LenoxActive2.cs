using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.LenoxActive2)]
	public class LenoxActive2 : SkillScript
	{
		
		private readonly List<SkillAgent> attacktedTargets = new List<SkillAgent>();

		
		private readonly CollisionCircle3D firstCollision = new CollisionCircle3D(Vector3.zero, 0f);

		
		private readonly float minDuration = Singleton<LenoxSkillActive2Data>.inst.SecondAttackPartitionTime /
		                                     Singleton<LenoxSkillActive2Data>.inst.SecondAttackLengthRiseCount;

		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		private readonly CollisionBox3D secondCollision = new CollisionBox3D(Vector3.zero, 0f, 0f, Vector3.zero);

		
		protected override void Start()
		{
			base.Start();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			Vector3 vector = GameUtil.Direction(Caster.Position, info.cursorPosition);
			LookAtDirection(Caster, vector);
			PlaySkillAction(Caster, 3, null, vector);
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			int SkillLv = SkillLevel;
			firstCollision.UpdatePosition(Caster.Position);
			firstCollision.UpdateRadius(SkillRange);
			List<SkillAgent> enemyCharacters = GetEnemyCharacters(firstCollision);
			if (enemyCharacters.Count > 0)
			{
				parameterCollection.Clear();
				parameterCollection.Add(SkillScriptParameterType.Damage,
					Singleton<LenoxSkillActive2Data>.inst.FirstDamageByLevel[SkillLv]);
				parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
					Singleton<LenoxSkillActive2Data>.inst.FirstSkillApCoef);
				DamageTo(enemyCharacters, DamageType.Skill, DamageSubType.Normal, 0, parameterCollection,
					Singleton<LenoxSkillActive2Data>.inst.EffectAndSoundCodeFirst);
				AddState(enemyCharacters, Singleton<LenoxSkillActive2Data>.inst.Active2DeBuffCodeSecond);
			}

			PlaySkillAction(Caster, 1);
			if (SkillCastingTime2 > 0f)
			{
				yield return SecondCastingTime();
			}

			attacktedTargets.Clear();
			secondCollision.UpdateWidth(SkillWidth);
			float minLength = info.SkillLength / Singleton<LenoxSkillActive2Data>.inst.SecondAttackLengthRiseCount;
			int num2;
			for (int index = 1;
				index <= Singleton<LenoxSkillActive2Data>.inst.SecondAttackLengthRiseCount;
				index = num2)
			{
				float num = minLength * index;
				Vector3 position = Caster.Position + Caster.Forward * num * 0.5f;
				secondCollision.UpdatePosition(position);
				secondCollision.UpdateNormalized(Caster.Forward);
				secondCollision.UpdateDepth(num);
				enemyCharacters = GetEnemyCharacters(secondCollision);
				for (int i = 0; i < attacktedTargets.Count; i++)
				{
					enemyCharacters.Remove(attacktedTargets[i]);
				}

				if (enemyCharacters.Count > 0)
				{
					parameterCollection.Clear();
					parameterCollection.Add(SkillScriptParameterType.Damage,
						Singleton<LenoxSkillActive2Data>.inst.SecondDamageByLevel[SkillLv]);
					parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
						Singleton<LenoxSkillActive2Data>.inst.SecondSkillApCoef);
					DamageTo(enemyCharacters, DamageType.Skill, DamageSubType.Normal, 0, parameterCollection,
						Singleton<LenoxSkillActive2Data>.inst.EffectAndSoundCodeSecond);
					AddState(enemyCharacters, Singleton<LenoxSkillActive2Data>.inst.Active2DeBuffCodeFirst);
					AddState(Caster, Singleton<LenoxSkillActive2Data>.inst.Active2BuffCode);
					attacktedTargets.AddRange(enemyCharacters);
				}

				yield return WaitForSeconds(minDuration);
				num2 = index + 1;
			}

			PlaySkillAction(Caster, 2);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}