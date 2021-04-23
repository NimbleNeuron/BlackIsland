using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.NadineActive3_2)]
	public class NadineActive3_2 : SkillScript
	{
		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			int summonCode = Singleton<NadineSkillActive3Data>.inst.SummonCode;
			Vector3 vector = Vector3.zero;
			WorldPlayerCharacter worldPlayerCharacter = Caster.Character as WorldPlayerCharacter;
			WorldSummonBase anchor = worldPlayerCharacter.GetOwnSummon(worldSummonBase =>
				!(worldSummonBase.Owner == null) && worldSummonBase.Owner.ObjectId == Caster.ObjectId &&
				worldSummonBase.SummonData.code == summonCode);
			if (anchor != null)
			{
				anchor.ResetDuration(5f);
				vector = anchor.GetPosition();
				LookAtPosition(Caster, GameUtil.DirectionOnPlane(Caster.Position, vector));
				Vector3 vector2;
				bool flag;
				float seconds;
				Caster.MoveToDestinationAtSpeed(vector, Singleton<NadineSkillActive3Data>.inst.DashSpeed,
					EasingFunction.Ease.Linear, true, out vector2, out flag, out seconds);
				yield return WaitForSeconds(seconds);
				WorldSummonBase worldSummonBase2 = anchor;
				if (worldSummonBase2 != null)
				{
					worldSummonBase2.Dead(DamageType.Normal);
				}
			}

			yield return WaitForSeconds(0.13f);
			Finish();
		}

		
		protected override void Finish(bool cancel = false)
		{
			PlaySkillAction(Caster, info.skillData.PassiveSkillId, 2);
			base.Finish(cancel);
			CharacterStateData data =
				GameDB.characterState.GetData(Singleton<NadineSkillActive3Data>.inst.BuffState1[SkillLevel]);
			if (Caster.IsHaveStateByGroup(data.group, Caster.ObjectId))
			{
				Caster.OverwriteState(data.code, Caster.ObjectId);
				return;
			}

			AddState(Caster, data.code);
		}
	}
}