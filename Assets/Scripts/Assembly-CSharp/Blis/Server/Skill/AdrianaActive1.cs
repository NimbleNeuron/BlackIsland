using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.AdrianaActive1)]
	public class AdrianaActive1 : AdrianaSkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		private CollisionBox3D collisionSector;

		
		private bool isOnPlayAgain;

		
		protected override void Start()
		{
			base.Start();
			isOnPlayAgain = false;
			if (collisionSector == null)
			{
				collisionSector = new CollisionBox3D(Vector3.zero, 0f, 0f, Vector3.zero);
			}
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			Vector3 direction = GameUtil.Direction(Caster.Position, GetSkillPoint());
			LookAtDirection(Caster, direction);
			CasterLockRotation(true);
			if (0f < SkillCastingTime1)
			{
				yield return FirstCastingTime();
			}

			StartConcentration();
			float fireFlameTimeStack = Singleton<AdrianaSkillActive1Data>.inst.DamageTerm;
			collisionSector.UpdateWidth(SkillWidth);
			collisionSector.UpdateDepth(SkillRange);
			collisionSector.UpdateNormalized(direction);
			int attackCount = 0;
			while (attackCount < Singleton<AdrianaSkillActive1Data>.inst.MaxAttackCount && !isOnPlayAgain)
			{
				if (Singleton<AdrianaSkillActive1Data>.inst.DamageTerm <= fireFlameTimeStack)
				{
					collisionSector.UpdatePosition(Caster.Position + SkillRange * 0.5f * direction);
					List<SkillAgent> enemyCharacters = GetEnemyCharacters(collisionSector);
					if (enemyCharacters != null && 0 < enemyCharacters.Count)
					{
						int skillLevel = SkillLevel;
						parameterCollection.Clear();
						parameterCollection.Add(SkillScriptParameterType.Damage,
							Singleton<AdrianaSkillActive1Data>.inst.Damage[skillLevel]);
						parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
							Singleton<AdrianaSkillActive1Data>.inst.ApCoef[skillLevel]);
						DirectDamageTo(enemyCharacters, DamageType.Skill, DamageSubType.Normal, 0, parameterCollection,
							Singleton<AdrianaSkillActive1Data>.inst.FireFlameDamageEffectAndSoundCode);
					}

					int num = attackCount;
					attackCount = num + 1;
					if (ProcessChangeProjectileFromOilAreaToFireFlame1(Caster.Character as WorldMovableCharacter,
						collisionSector))
					{
						PlaySkillAction(Caster, 1);
					}

					fireFlameTimeStack -= Singleton<AdrianaSkillActive1Data>.inst.DamageTerm;
				}
				else
				{
					fireFlameTimeStack += MonoBehaviourInstance<GameService>.inst.ServerFrameDeltaTime;
				}

				yield return WaitForFrame();
			}

			FinishConcentration(false);
			if (0f < SkillFinishDelayTime)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
		}

		
		protected override void OnPlayAgain(Vector3 hitPoint)
		{
			base.OnPlayAgain(hitPoint);
			isOnPlayAgain = true;
		}
	}
}