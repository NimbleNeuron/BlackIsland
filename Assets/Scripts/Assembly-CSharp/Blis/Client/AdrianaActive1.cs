using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.AdrianaActive1)]
	public class AdrianaActive1 : LocalSkillScript
	{
		private const string FX_BI_Adriana_Skill01_Ready_key = "FX_BI_Adriana_Skill01_Ready_key";


		private const string FX_BI_Adriana_Skill01_Fire = "FX_BI_Adriana_Skill01_Fire";


		private const string FX_BI_Adriana_Skill01_Fire_key = "FX_BI_Adriana_Skill01_Fire_key";


		private const string FX_BI_Adriana_Skill01_End = "FX_BI_Adriana_Skill01_End";


		private const string FX_BI_Adriana_Skill01_End_key = "FX_BI_Adriana_Skill01_End_key";


		private const string FX_BI_Adriana_Skill01_Point = "Adriana_WP_Fx";


		private const string Skill01_atk = "Skill01_atk";


		private const string FX_BI_Adriana_Skill01_Range = "FX_BI_Adriana_Skill01_Range";


		private const string FX_BI_Adriana_Skill01_Range_key = "FX_BI_Adriana_Skill01_Range_key";


		private const string adriana_Skill02_FireGround = "adriana_Skill02_FireGround";


		private const string Adriana_Skill01_Sfx = "Adriana_Skill01";


		private readonly int hMoveHash = Animator.StringToHash("hMove");


		private readonly int vMoveHash = Animator.StringToHash("vMove");


		private float hMoveOld;


		private Vector3 lastFramePosition;


		private Vector3 lastMoveDirection;


		private ParticleSystem ps;


		private Transform trsfFx;


		private float updateTime;


		private float vMoveOld;

		public override void Start()
		{
			SetAnimatorLayer(Self, "Skill01_atk", 1f);
			SetAnimation(Self, BooleanSkill01, true);
			PlayAnimation(Self, TriggerSkill01);
			StartCoroutine(UpdateAnimationParameter());
			PlaySoundChildManual(Self, "Adriana_Skill01", 15);
			StartCoroutine(CoroutineUtil.DelayedAction(0.13f, delegate
			{
				GameObject gameObject = PlayEffectChild(Self, "FX_BI_Adriana_Skill01_Fire", "Adriana_WP_Fx");
				trsfFx = gameObject != null ? gameObject.transform : null;
				PlayEffectChildManual(Self, "FX_BI_Adriana_Skill01_Range_key", "FX_BI_Adriana_Skill01_Range");
			}));
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			if (action == 1)
			{
				PlaySoundPoint(Self, "adriana_Skill02_FireGround", 15);
			}
		}


		public override void Finish(bool cancel)
		{
			SetAnimatorLayer(Self, "Skill01_atk", 0f);
			StopSoundChildManual(Self, "Adriana_Skill01");
			PlaySoundChildManual(Self, "Adriana_Skill01_End", 15);
			StopEffectChildManual(Self, "FX_BI_Adriana_Skill01_Range", true);
			StopEffectChildManual(Self, "FX_BI_Adriana_Skill01_Range_key", true);
			if (trsfFx != null)
			{
				ps = trsfFx.GetComponent<ParticleSystem>();
				ps.Stop();
				trsfFx.parent = null;
			}

			StopCoroutines();
			PlayEffectChildManual(Self, "FX_BI_Adriana_Skill01_End_key", "FX_BI_Adriana_Skill01_End", "Adriana_WP_Fx");
			SetAnimation(Self, BooleanSkill01, false);
		}


		private IEnumerator UpdateAnimationParameter()
		{
			updateTime = 0f;
			vMoveOld = 0f;
			hMoveOld = 0f;
			lastMoveDirection = Vector3.zero;
			lastFramePosition = Self.GetPosition();
			lastFramePosition.y = 0f;
			for (;;)
			{
				Vector3 position = Self.GetPosition();
				position.y = 0f;
				float animatorFloatParameter = GetAnimatorFloatParameter(Self, vMoveHash);
				float animatorFloatParameter2 = GetAnimatorFloatParameter(Self, hMoveHash);
				Vector3 normalized = (position - lastFramePosition).normalized;
				if (0.01f <= (normalized - lastMoveDirection).sqrMagnitude)
				{
					updateTime = 0f;
					vMoveOld = animatorFloatParameter;
					hMoveOld = animatorFloatParameter2;
					lastMoveDirection = normalized;
				}

				float b = Vector3.Dot(normalized, SelfForward);
				float b2 = Vector3.Dot(normalized, SelfRight);
				updateTime += Time.deltaTime * 5f;
				if (1f < updateTime)
				{
					updateTime = 1f;
				}

				SetAnimation(Self, vMoveHash, Mathf.Lerp(vMoveOld, b, updateTime));
				SetAnimation(Self, hMoveHash, Mathf.Lerp(hMoveOld, b2, updateTime));
				lastFramePosition = position;
				yield return null;
			}
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<AdrianaSkillActive1Data>.inst.DamageTerm.ToString();
				case 1:
					return Mathf.RoundToInt(Singleton<AdrianaSkillActive1Data>.inst.ApCoef[skillData.level] *
					                        SelfStat.AttackPower).ToString();
				case 2:
					return Singleton<AdrianaSkillActive1Data>.inst.Damage[skillData.level].ToString();
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			switch (index)
			{
				case 0:
					return "ToolTipType/TrueDamage";
				case 1:
					return "ToolTipType/SkillApCoef";
				case 2:
					return "ToolTipType/CoolTime";
				case 3:
					return "ToolTipType/Cost";
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<AdrianaSkillActive1Data>.inst.Damage[level].ToString();
				case 1:
					return Singleton<AdrianaSkillActive1Data>.inst.ApCoef[level].ToString();
				case 2:
					return skillData.cooldown.ToString();
				case 3:
					return skillData.cost.ToString();
				default:
					return "";
			}
		}
	}
}