using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.XiukaiActive2)]
	public class XiukaiActive2 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection
			parameterCollectionAdd = SkillScriptParameterCollection.Create();

		
		private readonly SkillScriptParameterCollection parameterCollectionNormal =
			SkillScriptParameterCollection.Create();

		
		private CollisionCircle3D sector;

		
		protected override void Start()
		{
			base.Start();
			if (sector == null)
			{
				sector = new CollisionCircle3D(Vector3.zero, SkillRange);
			}
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			sector.UpdatePosition(Caster.Position);
			sector.UpdateRadius(SkillRange);
			List<SkillAgent> enemyCharacters = GetEnemyCharacters(sector);
			int skillLevel = SkillLevel;
			for (int i = 0; i < enemyCharacters.Count; i++)
			{
				int stateCode;
				if (enemyCharacters[i].AnyHaveNegativelyAffectsMovementState())
				{
					float num = Singleton<XiukaiSkillActive2Data>.inst.SkillAddDamageMaxHpRatio[skillLevel] * 0.01f *
					            Caster.Stat.MaxHp;
					parameterCollectionAdd.Clear();
					parameterCollectionAdd.Add(SkillScriptParameterType.Damage,
						Singleton<XiukaiSkillActive2Data>.inst.BaseDamage[skillLevel] + num);
					parameterCollectionAdd.Add(SkillScriptParameterType.DamageApCoef,
						Singleton<XiukaiSkillActive2Data>.inst.SkillApCoef);
					DamageTo(enemyCharacters[i], DamageType.Skill, DamageSubType.Normal, 0, parameterCollectionAdd,
						1013302);
					stateCode = Singleton<XiukaiSkillActive2Data>.inst.DebuffState2[skillLevel];
				}
				else
				{
					parameterCollectionNormal.Clear();
					parameterCollectionNormal.Add(SkillScriptParameterType.Damage,
						Singleton<XiukaiSkillActive2Data>.inst.BaseDamage[skillLevel]);
					parameterCollectionNormal.Add(SkillScriptParameterType.DamageApCoef,
						Singleton<XiukaiSkillActive2Data>.inst.SkillApCoef);
					DamageTo(enemyCharacters[i], DamageType.Skill, DamageSubType.Normal, 0, parameterCollectionNormal,
						1013301);
					stateCode = Singleton<XiukaiSkillActive2Data>.inst.DebuffState[skillLevel];
				}

				AddState(enemyCharacters[i], stateCode);
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
		}
	}
}