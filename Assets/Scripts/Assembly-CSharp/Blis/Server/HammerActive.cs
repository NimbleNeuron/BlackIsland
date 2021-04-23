using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.HammerActive)]
	public class HammerActive : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		private CollisionBox3D sector;

		
		protected override void Start()
		{
			base.Start();
			if (sector == null)
			{
				sector = new CollisionBox3D(Vector3.zero, SkillWidth, SkillRange, Vector3.zero);
			}
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			Vector3 dirction = GameUtil.Direction(Caster.Position, GetSkillPoint());
			LookAtDirection(Caster, dirction);
			CasterLockRotation(true);
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			sector.UpdatePosition(Caster.Position + SkillRange * 0.5f * dirction);
			sector.UpdateNormalized(dirction);
			sector.UpdateWidth(SkillWidth);
			sector.UpdateDepth(SkillRange);
			List<SkillAgent> enemyCharacters = GetEnemyCharacters(sector);
			parameterCollection.Clear();
			int skillLevel = SkillLevel;
			parameterCollection.Add(SkillScriptParameterType.DamageDefCoef,
				Singleton<HammerSkillActiveData>.inst.DefCoefficient[skillLevel]);
			parameterCollection.Add(SkillScriptParameterType.Damage,
				Singleton<HammerSkillActiveData>.inst.DamageByLevel[skillLevel]);
			DamageTo(enemyCharacters, DamageType.Skill, DamageSubType.Normal, 0, parameterCollection,
				Singleton<HammerSkillActiveData>.inst.EffectAndSoundeCode);
			AddState(enemyCharacters, Singleton<HammerSkillActiveData>.inst.DebuffState[skillLevel]);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}