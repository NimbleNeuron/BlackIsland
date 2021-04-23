using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.HyunwooActive4)]
	public class HyunwooActive4 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		private CollisionBox3D collision;

		
		private float concentrationStartTime;

		
		private Vector3 direction = Vector3.zero;

		
		private bool isDoDamageOnArea;

		
		private bool isSkip;

		
		private float rating = -1f;

		
		protected override void Start()
		{
			base.Start();
			isSkip = false;
			rating = -1f;
			isDoDamageOnArea = false;
			concentrationStartTime = 0f;
			if (collision == null)
			{
				collision = new CollisionBox3D(Vector3.zero, 0f, 0f, Vector3.zero);
			}
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			LookAtPosition(Caster, info.cursorPosition);
			Start();
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
				LookAtPosition(Caster, info.cursorPosition);
			}

			direction = GameUtil.Direction(Caster.Position, GetSkillPoint());
			StartConcentration();
			concentrationStartTime = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
			while (MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - startTime <
				SkillConcentrationTime + 2f && !isSkip)
			{
				yield return WaitForFrame();
			}

			FinishConcentration(false);
			PlaySkillAction(Caster, 1);
			CalculateRate();
			if (SkillCastingTime2 > 0f)
			{
				yield return SecondCastingTime();
			}

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
			float num = info.skillData.UseWeaponRange ? Caster.Stat.AttackRange : 0f;
			float a = Singleton<HyunwooSkillActive4Data>.inst.MinSkillRange + num;
			float b = SkillRange + num;
			float num2 = Mathf.Lerp(a, b, rating);
			collision.UpdatePosition(Caster.Position + num2 * 0.5f * direction);
			collision.UpdateNormalized(direction);
			collision.UpdateDepth(num2);
			collision.UpdateWidth(SkillWidth);
			List<SkillAgent> enemyCharacters = GetEnemyCharacters(collision);
			float value = Mathf.Lerp(Singleton<HyunwooSkillActive4Data>.inst.MinSkillApCoef,
				Singleton<HyunwooSkillActive4Data>.inst.MaxSkillApCoef, rating);
			int num3 = (int) Mathf.Lerp(Singleton<HyunwooSkillActive4Data>.inst.MinDamageByLevel[SkillLevel],
				Singleton<HyunwooSkillActive4Data>.inst.MaxDamageByLevel[SkillLevel], rating);
			parameterCollection.Clear();
			parameterCollection.Add(SkillScriptParameterType.DamageApCoef, value);
			parameterCollection.Add(SkillScriptParameterType.Damage, num3);
			DamageTo(enemyCharacters, DamageType.Skill, DamageSubType.Normal, 0, parameterCollection, 0);
		}

		
		protected override void OnPlayAgain(Vector3 hitPoint)
		{
			base.OnPlayAgain(hitPoint);
			isSkip = true;
		}

		
		protected override void GetSkillRange(ref float minRange, ref float maxRange)
		{
			minRange = Singleton<HyunwooSkillActive4Data>.inst.MinSkillRange;
			maxRange = SkillRange;
		}
	}
}