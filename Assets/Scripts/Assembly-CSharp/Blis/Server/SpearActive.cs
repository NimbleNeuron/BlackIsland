using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.SpearActive)]
	public class SpearActive : SkillScript
	{
		
		private readonly List<SkillAgent> hits = new List<SkillAgent>();

		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		private readonly CollisionBox3D sector1 = new CollisionBox3D(Vector3.zero, 0f, 0f, Vector3.zero);

		
		private readonly CollisionSector3D sector2 = new CollisionSector3D(Vector3.zero, 0f, 0f, Vector3.zero);

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			Vector3 direction = GameUtil.Direction(Caster.Position, info.cursorPosition);
			LookAtDirection(Caster, direction);
			CasterLockRotation(true);
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			Vector3 a = Caster.Position + Caster.Forward * Caster.Stat.AttackRange;
			sector1.UpdatePosition(Caster.Position + SkillRange * 0.5f * direction);
			sector1.UpdateWidth(SkillWidth);
			sector1.UpdateDepth(SkillRange);
			sector1.UpdateNormalized(direction);
			List<SkillAgent> enemyCharacters = GetEnemyCharacters(sector1);
			int skillLv = SkillLevel;
			foreach (SkillAgent skillAgent in enemyCharacters)
			{
				parameterCollection.Clear();
				parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
					Singleton<SpearSkillActiveData>.inst.SkillApCoef[skillLv]);
				DamageTo(skillAgent, DamageType.Skill, DamageSubType.Normal, 0, parameterCollection,
					Singleton<SpearSkillActiveData>.inst.EffectAndSound);
				float num = Vector3.Distance(a, skillAgent.Position);
				if (num > Singleton<SpearSkillActiveData>.inst.knockBackThreshHold)
				{
					KnockbackState knockbackState = CreateState<KnockbackState>(skillAgent, 2000010);
					knockbackState.Init(direction, num, 0.2f, EasingFunction.Ease.EaseOutQuad, false);
					skillAgent.AddState(knockbackState, Caster.ObjectId);
				}

				hits.Add(skillAgent);
			}

			yield return WaitForSeconds(0.06f);
			sector2.UpdatePosition(Caster.Position);
			sector2.UpdateRadius(Singleton<SpearSkillActiveData>.inst.SecondRange);
			sector2.UpdateAngle(SkillAngle);
			sector2.UpdateNormalized(direction);
			foreach (SkillAgent skillAgent2 in GetEnemyCharacters(sector2))
			{
				parameterCollection.Clear();
				parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
					Singleton<SpearSkillActiveData>.inst.SkillApCoef[skillLv]);
				DamageTo(skillAgent2, DamageType.Skill, DamageSubType.Normal, 0, parameterCollection,
					Singleton<SpearSkillActiveData>.inst.EffectAndSound);
				hits.Add(skillAgent2);
			}

			foreach (SkillAgent target in hits)
			{
				AddState(target, Singleton<SpearSkillActiveData>.inst.DebuffState[skillLv]);
			}

			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		protected override void Start()
		{
			base.Start();
			hits.Clear();
		}
	}
}