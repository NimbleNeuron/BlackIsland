using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	public abstract class SisselaSkillScript : SkillScript
	{
		
		protected void SetWilsonState(bool isUnion, WorldSummonServant wilson)
		{
			if (IsWilsonUnion() == isUnion)
			{
				return;
			}

			if (isUnion)
			{
				bool isPastStateSeperate =
					Caster.RemoveStateByGroup(Singleton<SisselaSkillData>.inst.WilsonSeparateStateGroup,
						Caster.ObjectId);
				AddState(Caster, Singleton<SisselaSkillData>.inst.WilsonUnionState);
				StartWilsonUnion(isPastStateSeperate, wilson);
				return;
			}

			Caster.RemoveStateByGroup(Singleton<SisselaSkillData>.inst.WilsonUnionStateGroup, Caster.ObjectId);
			AddState(Caster, Singleton<SisselaSkillData>.inst.WilsonSeparateState);
			StartWilsonSeperate(wilson);
		}

		
		private void StartWilsonUnion(bool isPastStateSeperate, WorldSummonServant wilson)
		{
			Transform transform = wilson.transform;
			transform.SetParent(Caster.Character.transform);
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			if (isPastStateSeperate && IsReadySkill(Caster, SkillSlotSet.Passive_1))
			{
				AddState(Caster, Singleton<SisselaSkillData>.inst.PassiveNormalAttackMountState);
			}
		}

		
		private void StartWilsonSeperate(WorldSummonServant wilson)
		{
			wilson.transform.SetParent(((WilsonData) wilson.ServantData).OriginalWilsonParent);
		}

		
		protected bool IsWilsonUnion()
		{
			return Caster.AnyHaveStateByGroup(Singleton<SisselaSkillData>.inst.WilsonUnionStateGroup);
		}

		
		private static bool Condition(WorldSummonBase summon)
		{
			return summon.SummonData.code == Singleton<SisselaSkillData>.inst.WilsonSummonCode;
		}

		
		public static WorldSummonServant GetWilson(WorldPlayerCharacter owner)
		{
			return (WorldSummonServant) owner.GetOwnSummon(Condition);
		}
	}
}