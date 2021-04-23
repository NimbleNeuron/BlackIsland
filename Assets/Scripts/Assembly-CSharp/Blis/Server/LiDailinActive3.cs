using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.LiDailinActive3)]
	public class LiDailinActive3 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameters = SkillScriptParameterCollection.Create();

		
		private CollisionSector3D collision;

		
		private bool isReinforce;

		
		protected override void Start()
		{
			base.Start();
			isReinforce = Caster.Status.ExtraPoint >= Singleton<LiDailinSkillData>.inst.SkillReinforceExtraPoint;
			if (isReinforce)
			{
				Caster.ExtraPointModifyTo(Caster, Singleton<LiDailinSkillData>.inst.A3ReinforceConsumeExtraPoint);
			}

			if (collision == null)
			{
				collision = new CollisionSector3D(Vector3.zero, 0f, 0f, Vector3.zero);
			}
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			if (isReinforce)
			{
				AddState(Caster, Singleton<LiDailinSkillData>.inst.PassiveDoubleAttackStateCode);
			}
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			Vector3 direction = GameUtil.Direction(Caster.Position, GetSkillPoint());
			LookAtDirection(Caster, direction);
			CasterLockRotation(true);
			PlaySkillAction(Caster, isReinforce ? 2 : 1);
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			StartConcentration();
			float skillDuration = Singleton<LiDailinSkillData>.inst.A3Duration;
			int skillLv = SkillLevel;
			int baseDebuff = Singleton<LiDailinSkillData>.inst.A3BaseDebuff[skillLv];
			int reinforceDebuff = Singleton<LiDailinSkillData>.inst.A3ReinforceDebuff[skillLv];
			int effectCode = isReinforce
				? Singleton<LiDailinSkillData>.inst.A3EffectCodeReinforce
				: Singleton<LiDailinSkillData>.inst.A3EffectCodeBase;
			collision.UpdatePosition(Caster.Position);
			collision.UpdateRadius(Singleton<LiDailinSkillData>.inst.A3Radius);
			collision.UpdateAngle(Singleton<LiDailinSkillData>.inst.A3Angle);
			collision.UpdateNormalized(direction);
			HashSet<int> hitedObjectId = new HashSet<int>();
			while (skillDuration > 0f)
			{
				List<SkillAgent> enemyCharacters = GetEnemyCharacters(collision);
				if (enemyCharacters.Any<SkillAgent>())
				{
					casterInfo.SetAttackerStat(Caster.Owner, casterCachedStat);
				}

				foreach (SkillAgent skillAgent in enemyCharacters)
				{
					if (!hitedObjectId.Contains(skillAgent.ObjectId))
					{
						parameters.Clear();
						parameters.Add(SkillScriptParameterType.Damage,
							Singleton<LiDailinSkillData>.inst.A3BaseDamage[skillLv]);
						parameters.Add(SkillScriptParameterType.DamageApCoef,
							Singleton<LiDailinSkillData>.inst.A3ApDamage);
						DamageTo(skillAgent, casterInfo, DamageType.Skill, DamageSubType.Normal, 0, parameters,
							SkillSlotSet.Active3_1, skillAgent.Position,
							GameUtil.DirectionOnPlane(Caster.Position, skillAgent.Position), effectCode);
						AddState(skillAgent, baseDebuff);
						if (isReinforce)
						{
							AddState(skillAgent, reinforceDebuff);
						}

						hitedObjectId.Add(skillAgent.ObjectId);
					}
				}

				yield return WaitForFrame();
				skillDuration -= MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
			}

			FinishConcentration(false);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}