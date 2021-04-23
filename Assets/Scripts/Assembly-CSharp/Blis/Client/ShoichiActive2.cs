using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.ShoichiActive2)]
	public class ShoichiActive2 : LocalSkillScript
	{
		private const string Shoichi_Skill02 = "Shoichi_Skill02";


		private const string FX_BI_Shoichi_Skill02_Slash = "FX_BI_Shoichi_Skill02_Slash";


		private const string FX_BI_Shoichi_Skill02_Slash2 = "FX_BI_Shoichi_Skill02_Slash2";

		public override bool PickingOrderCompare(LocalObject prevTarget, LocalObject nextTarget)
		{
			ObjectOrder objectOrder = prevTarget.GetObjectOrder();
			ObjectOrder objectOrder2 = nextTarget.GetObjectOrder();
			if (objectOrder2 != ObjectOrder.SummonObjectMy_Camera)
			{
				return objectOrder > objectOrder2;
			}

			if (objectOrder.Equals(ObjectOrder.AlivePlayerEnemy) || objectOrder.Equals(ObjectOrder.AliveEnemy))
			{
				return objectOrder > objectOrder2;
			}

			LocalSummonBase localSummonBase;
			return (localSummonBase = nextTarget as LocalSummonBase) != null && localSummonBase.SummonData.code ==
				Singleton<ShoichiSkillPassiveData>.inst.PassiveSummonObjectId || objectOrder > objectOrder2;
		}


		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill02);
			PlaySoundPoint(Self, "Shoichi_Skill02");
			Vector3 direction = targetObject.GetPosition() - Self.GetPosition();
			float num = Vector3.Distance(targetObject.GetPosition(), Self.GetPosition());
			float num2 = 1f;
			if (targetObject.ObjectType != ObjectType.SummonCamera)
			{
				num2 = 0f;
			}

			float z = (num - num2) / SkillRange;
			Vector3 scale = new Vector3(1f, 1f, z);
			Vector3 offset = Vector3.zero;
			if (Self.GetPosition() != Self.GetPosition())
			{
				offset = Self.GetPosition() - Self.GetPosition();
			}

			GameObject Effect1 = Self.PlayLocalEffectPoint("FX_BI_Shoichi_Skill02_Slash", offset,
				GameUtil.LookRotation(direction.normalized));
			if (Effect1 != null)
			{
				Effect1.transform.localScale = scale;
				StartCoroutine(CoroutineUtil.DelayedAction(0.01f, delegate
				{
					if (Effect1 != null)
					{
						GameObject gameObject = Self.PlayLocalEffectPoint("FX_BI_Shoichi_Skill02_Slash2", offset,
							GameUtil.LookRotation(direction.normalized));
						if (gameObject != null)
						{
							gameObject.transform.position = Effect1.transform.position;
							gameObject.transform.localScale = scale;
						}
					}
				}));
			}
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			SetAnimation(Self, BooleanSkill02, true);
			RotationLocalObject(Self, SelfForward);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			if (index == 0)
			{
				return Singleton<ShoichiSkillActive2Data>.inst.DamageByLevel[skillData.level].ToString();
			}

			if (index != 1)
			{
				return "";
			}

			return ((int) (Singleton<ShoichiSkillActive2Data>.inst.SkillApCoef * SelfStat.AttackPower)).ToString();
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			if (index == 0)
			{
				return "ToolTipType/Damage";
			}

			if (index != 1)
			{
				return "";
			}

			return "ToolTipType/CoolTime";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				return Singleton<ShoichiSkillActive2Data>.inst.DamageByLevel[skillData.level].ToString();
			}

			if (index != 1)
			{
				return "";
			}

			return skillData.cooldown.ToString();
		}
	}
}