using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.LenoxActive4)]
	public class LenoxActive4 : SkillScript
	{
		
		private CollisionBox3D collision;

		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		protected override void Start()
		{
			base.Start();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			Caster.LockRotation(false);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			Vector3 vector = GameUtil.Direction(Caster.Position, info.cursorPosition);
			LookAtDirection(Caster, vector);
			Caster.LockRotation(true);
			ProjectileProperty projectileProperty = PopProjectileProperty(Caster,
				Singleton<LenoxSkillActive4Data>.inst.Active4SkillPointDisplayProjectileCode);
			projectileProperty.SetTargetDirection(vector);
			LaunchProjectile(projectileProperty, info.cursorPosition);
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			Caster.AttachSightPosition(info.cursorPosition,
				Singleton<LenoxSkillActive4Data>.inst.Active4AttackSightRange,
				Singleton<LenoxSkillActive4Data>.inst.Active4AttackSightDuration, false);
			int SkillLv = SkillLevel;
			Active4Attack(1, SkillLv, Singleton<LenoxSkillActive4Data>.inst.EffectAndSoundCodeFirst);
			if (SkillCastingTime2 > 0f)
			{
				yield return SecondCastingTime();
			}

			Active4Attack(2, SkillLv, Singleton<LenoxSkillActive4Data>.inst.EffectAndSoundCodeSecond);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		private void Active4Attack(int hitNum, int SkillLv, int effectSoundCode)
		{
			PlaySkillAction(Caster, hitNum, null, info.cursorPosition);
			Vector3 normalized =
				Singleton<LenoxSkillActive4Data>.inst.Active4ColisionBoxAngleY[hitNum] * Caster.Forward;
			if (collision == null)
			{
				collision = new CollisionBox3D(info.cursorPosition, SkillWidth, SkillInnerRange, normalized);
			}
			else
			{
				collision.UpdatePosition(info.cursorPosition);
				collision.UpdateNormalized(normalized);
			}

			List<SkillAgent> enemyCharacters = GetEnemyCharacters(collision);
			if (enemyCharacters.Count <= 0)
			{
				return;
			}

			parameterCollection.Clear();
			parameterCollection.Add(SkillScriptParameterType.Damage,
				Singleton<LenoxSkillActive4Data>.inst.DamageByLevel[SkillLv]);
			parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
				Singleton<LenoxSkillActive4Data>.inst.SkillApCoef);
			DamageTo(enemyCharacters, DamageType.Skill, DamageSubType.Normal, 0, parameterCollection, effectSoundCode);
			for (int i = 0; i < enemyCharacters.Count; i++)
			{
				if (enemyCharacters[i]
					.RemoveStateByGroup(Singleton<LenoxSkillActive4Data>.inst.Active4NormalDeBuffGroup,
						Caster.ObjectId))
				{
					AddState(enemyCharacters[i],
						Singleton<LenoxSkillActive4Data>.inst.Active4UpgradeDeBuffCode[SkillLv]);
				}
				else
				{
					AddState(enemyCharacters[i],
						Singleton<LenoxSkillActive4Data>.inst.Active4NormalDeBuffCode[SkillLv]);
				}
			}
		}
	}
}