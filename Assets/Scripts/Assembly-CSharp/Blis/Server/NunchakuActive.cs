using System.Collections;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.NunchakuActive)]
	public class NunchakuActive : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		private bool isShoot;

		
		protected override void Start()
		{
			base.Start();
			isShoot = false;
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Vector3 direction = GameUtil.Direction(Caster.Position, GetSkillPoint());
			LookAtDirection(Caster, direction);
			CasterLockRotation(true);
			Start();
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			StartConcentration();
			float concentrationTime = SkillConcentrationTime;
			float waitingTime = Singleton<NunchakuSkillActiveData>.inst.WaitingTime;
			while (MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - startTime <
				concentrationTime + waitingTime && !isShoot)
			{
				yield return WaitForFrame();
			}

			FinishConcentration(false);
			bool bonusEffect = Singleton<NunchakuSkillActiveData>.inst.BonusEffectChargingTime <=
			                   MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - startTime;
			float rating = (MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - startTime) /
			               concentrationTime;
			if (1f < rating)
			{
				rating = 1f;
			}

			PlaySkillAction(Caster, 1);
			yield return WaitForSeconds(Singleton<NunchakuSkillActiveData>.inst.SkillDamageDelay);
			float distance = Mathf.Lerp(Singleton<NunchakuSkillActiveData>.inst.MinSkillRange,
				Singleton<NunchakuSkillActiveData>.inst.MaxSkillRange, rating);
			int damage = (int) Mathf.Lerp(Singleton<NunchakuSkillActiveData>.inst.MinDamageByLevel[SkillLevel],
				Singleton<NunchakuSkillActiveData>.inst.MaxDamageByLevel[SkillLevel], rating);
			float apCoef = Mathf.Lerp(Singleton<NunchakuSkillActiveData>.inst.MinApCoef,
				Singleton<NunchakuSkillActiveData>.inst.MaxApCoef, rating);
			int dataCode = bonusEffect
				? Singleton<NunchakuSkillActiveData>.inst.ProjectileCode2
				: Singleton<NunchakuSkillActiveData>.inst.ProjectileCode;
			ProjectileProperty projectileProperty = PopProjectileProperty(Caster, dataCode);
			projectileProperty.SetTargetDirection(direction);
			projectileProperty.SetDistance(distance);
			projectileProperty.SetActionOnCollisionCharacter(
				delegate(SkillAgent targetAgent, AttackerInfo attackerInfo, Vector3 damagePoint,
					Vector3 damageDirection)
				{
					parameterCollection.Clear();
					parameterCollection.Add(SkillScriptParameterType.Damage, damage);
					parameterCollection.Add(SkillScriptParameterType.DamageApCoef, apCoef);
					DamageTo(targetAgent, DamageType.Skill, DamageSubType.Normal, 0, parameterCollection,
						Singleton<NunchakuSkillActiveData>.inst.EffectAndSound);
					if (bonusEffect)
					{
						AddState(targetAgent, 2000009, Singleton<NunchakuSkillActiveData>.inst.StunDuration);
					}
				});
			LaunchProjectile(projectileProperty);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		protected override void OnPlayAgain(Vector3 hitPoint)
		{
			base.OnPlayAgain(hitPoint);
			isShoot = true;
		}
	}
}