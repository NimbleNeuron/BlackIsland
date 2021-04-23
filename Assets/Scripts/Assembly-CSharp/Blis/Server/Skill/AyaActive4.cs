using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.AyaActive4)]
	public class AyaActive4 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		private float concentrationStartTime;

		
		private bool isDoDamageOnArea;

		
		private bool isSkip;

		
		private float rating = -1f;

		
		private CollisionCircle3D sector;

		
		protected override void Start()
		{
			base.Start();
			isDoDamageOnArea = false;
			rating = -1f;
			isSkip = false;
			concentrationStartTime = 0f;
			if (sector == null)
			{
				sector = new CollisionCircle3D(Caster.Position, SkillRange);
			}
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			StartConcentration();
			concentrationStartTime = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
			float concentrationTimeEndTime = SkillConcentrationTime + concentrationStartTime;
			while (MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime < concentrationTimeEndTime && !isSkip)
			{
				yield return WaitForFrame();
			}

			CalculateRate();
			DamageOnArea();
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			if (!isDoDamageOnArea)
			{
				DamageOnArea();
			}
		}

		
		protected override void OnPlayAgain(Vector3 hitPoint)
		{
			base.OnPlayAgain(hitPoint);
			isSkip = true;
		}

		
		private void CalculateRate()
		{
			rating = (MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - concentrationStartTime) /
			         SkillConcentrationTime;
			if (1f < rating)
			{
				rating = 1f;
			}
		}

		
		private void DamageOnArea()
		{
			if (isDoDamageOnArea)
			{
				return;
			}

			if (rating == -1f)
			{
				CalculateRate();
			}

			isDoDamageOnArea = true;
			PlaySkillAction(Caster, 1);
			sector.UpdatePosition(Caster.Position);
			sector.UpdateRadius(SkillRange);
			List<SkillAgent> enemyCharacters = GetEnemyCharacters(sector);
			if (enemyCharacters.Count > 0)
			{
				int skillLevel = SkillLevel;
				parameterCollection.Clear();
				float value = Mathf.Lerp(Singleton<AyaSkillActive4Data>.inst.SkillMinApCoef,
					Singleton<AyaSkillActive4Data>.inst.SkillMaxApCoef, rating);
				int num = (int) Mathf.Lerp(Singleton<AyaSkillActive4Data>.inst.DamageByMinLevel[skillLevel],
					Singleton<AyaSkillActive4Data>.inst.DamageByMaxLevel[skillLevel], rating);
				parameterCollection.Add(SkillScriptParameterType.DamageApCoef, value);
				parameterCollection.Add(SkillScriptParameterType.Damage, num);
				DamageTo(enemyCharacters, DamageType.Skill, DamageSubType.Normal, 0, parameterCollection, 0);
				float value2 = Mathf.Lerp(Singleton<AyaSkillActive4Data>.inst.DebuffStateMinDuration,
					Singleton<AyaSkillActive4Data>.inst.DebuffStateMaxDuration[skillLevel], rating);
				AddState(enemyCharacters, Singleton<AyaSkillActive4Data>.inst.DebuffState, value2);
			}

			MonoBehaviourInstance<GameService>.inst.Announce.MakeNoise(Caster.WorldObject, null, NoiseType.Gunshot);
		}
	}
}