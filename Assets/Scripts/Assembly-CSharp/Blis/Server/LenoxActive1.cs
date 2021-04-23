using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.LenoxActive1)]
	public class LenoxActive1 : SkillScript
	{
		
		private readonly CollisionCircle3D collision = new CollisionCircle3D(Vector3.zero, 0f);

		
		private readonly List<SkillAgent> collisionEnemies = new List<SkillAgent>();

		
		private readonly List<SkillAgent> farEnemies = new List<SkillAgent>();

		
		private readonly SkillScriptParameterCollection
			farParameterCollection = SkillScriptParameterCollection.Create();

		
		private readonly List<SkillAgent> nearEnemies = new List<SkillAgent>();

		
		private readonly SkillScriptParameterCollection nearParameterCollection =
			SkillScriptParameterCollection.Create();

		
		protected override void Start()
		{
			base.Start();
			collisionEnemies.Clear();
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			bool IsAddBuff = false;
			float endTime = Singleton<LenoxSkillActive1Data>.inst.Active1DemageWaitTime +
			                MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
			while (endTime > MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime)
			{
				PlaySkillAction(Caster, 1);
				collision.UpdatePosition(Caster.Position);
				collision.UpdateRadius(SkillRange);
				List<SkillAgent> enemyCharacters = GetEnemyCharacters(collision);
				for (int i = 0; i < collisionEnemies.Count; i++)
				{
					enemyCharacters.Remove(collisionEnemies[i]);
				}

				nearEnemies.Clear();
				farEnemies.Clear();
				for (int j = 0; j < enemyCharacters.Count; j++)
				{
					if (GameUtil.DistanceOnPlane(enemyCharacters[j].Position, Caster.Position) < SkillInnerRange)
					{
						nearEnemies.Add(enemyCharacters[j]);
					}
					else
					{
						farEnemies.Add(enemyCharacters[j]);
					}
				}

				int skillLevel = SkillLevel;
				if (nearEnemies.Count > 0)
				{
					nearParameterCollection.Clear();
					nearParameterCollection.Add(SkillScriptParameterType.Damage,
						Singleton<LenoxSkillActive1Data>.inst.DamageByLevel[skillLevel]);
					nearParameterCollection.Add(SkillScriptParameterType.DamageApCoef,
						Singleton<LenoxSkillActive1Data>.inst.SkillApCoef);
					DamageTo(nearEnemies, DamageType.Skill, DamageSubType.Normal, 0, nearParameterCollection,
						Singleton<LenoxSkillActive1Data>.inst.EffectAndSoundCode);
					collisionEnemies.AddRange(nearEnemies);
				}

				if (farEnemies.Count > 0)
				{
					farParameterCollection.Clear();
					farParameterCollection.Add(SkillScriptParameterType.Damage,
						Singleton<LenoxSkillActive1Data>.inst.DamageByLevel[skillLevel]);
					farParameterCollection.Add(SkillScriptParameterType.DamageApCoef,
						Singleton<LenoxSkillActive1Data>.inst.SkillApCoef);
					farParameterCollection.Add(SkillScriptParameterType.DamageCasterMaxHpCoef,
						Singleton<LenoxSkillActive1Data>.inst.SmageMaxHPPerDamageByLevel[skillLevel]);
					DamageTo(farEnemies, DamageType.Skill, DamageSubType.Normal, 0, farParameterCollection,
						Singleton<LenoxSkillActive1Data>.inst.SmashEffectAndSoundCode);
					collisionEnemies.AddRange(farEnemies);
				}

				if (collisionEnemies.Count > 0 && !IsAddBuff)
				{
					IsAddBuff = true;
					AddState(Caster, Singleton<LenoxSkillActive1Data>.inst.Active1BuffCode, 1);
				}

				yield return WaitForFrames(2);
			}

			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}