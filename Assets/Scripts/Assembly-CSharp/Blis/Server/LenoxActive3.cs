using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.LenoxActive3)]
	public class LenoxActive3 : SkillScript
	{
		
		private const float partitionDuration = 0.051999997f;

		
		private const int partitionCount = 5;

		
		private const int halfCount = 2;

		
		private const bool isAddHalfRange = false;

		
		private readonly List<SkillAgent> AttactedEnemys = new List<SkillAgent>();

		
		private readonly CollisionBox3D collision = new CollisionBox3D(Vector3.zero, 0f, 0f, Vector3.zero);

		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		protected override void Start()
		{
			base.Start();
			AttactedEnemys.Clear();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			Caster.LockRotation(false);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			LookAtPosition(Caster, info.cursorPosition);
			Caster.LockRotation(true);
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			float num = SkillRange / 5f;
			Vector3 partitionSize = Caster.Forward * num;
			collision.UpdateWidth(SkillWidth);
			collision.UpdateNormalized(Caster.Forward);
			collision.UpdateDepth(num);
			int num2;
			for (int index = 0; index < 5; index = num2)
			{
				Vector3 a = Caster.Position - partitionSize * 2f;
				collision.UpdatePosition(a + partitionSize * index);
				int skillLevel = SkillLevel;
				parameterCollection.Clear();
				parameterCollection.Add(SkillScriptParameterType.Damage,
					Singleton<LenoxSkillActive3Data>.inst.DamageByLevel[skillLevel]);
				parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
					Singleton<LenoxSkillActive3Data>.inst.SkillApCoef);
				List<SkillAgent> enemyCharacters = GetEnemyCharacters(collision);
				for (int i = 0; i < enemyCharacters.Count; i++)
				{
					if (!AttactedEnemys.Contains(enemyCharacters[i]))
					{
						AttactedEnemys.Add(enemyCharacters[i]);
						DamageTo(enemyCharacters[i], DamageType.Skill, DamageSubType.Normal, 0, parameterCollection,
							Singleton<LenoxSkillActive3Data>.inst.EffectAndSoundCode);
						AddState(enemyCharacters[i], Singleton<LenoxSkillActive3Data>.inst.Active3KnockbackCode);
						AddState(enemyCharacters[i], Singleton<LenoxSkillActive3Data>.inst.Active3SlowCode[skillLevel]);
					}
				}

				yield return WaitForSeconds(0.051999997f);
				num2 = index + 1;
			}

			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}