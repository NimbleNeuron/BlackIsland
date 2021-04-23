using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.ChiaraActive1)]
	public class LocalChiaraActive1 : LocalSkillScript
	{
		private const string Chiara_Skill01_Fire = "FX_BI_Chiara_Skill01_01";


		private const string Chiara_Skill01_Fire_Sfx = "Chiara_Skill01_Attack";


		private const string Chiara_Skill01_wait = "FX_BI_Chiara_Skill01_Ready3";


		private const string Chiara_Skill01_wait_Sfx = "Chiara_Skill01_Ing";


		private const string Chiara_Skill01_wait_key = "FX_BI_Chiara_Skill01_Ready_key";


		private const string Chiara_Skill01_wait02 = "FX_BI_Chiara_Skill01_Ready4";


		private const string Chiara_Skill01_wait02_key = "FX_BI_Chiara_Skill01_Ready02_key";


		private const string Chiara_Skill01_Start_Sfx = "Chiara_Skill01_Start";


		private const string Effect_point = "Fx_Hand_R";


		private Coroutine timeCheckSkillFormChange;


		public override void Start()
		{
			SetAnimation(Self, BooleanSkill03, false);
			PlayAnimation(Self, TriggerSkill01);
			SetAnimation(Self, BooleanSkill01, true);
			PlaySoundPoint(Self, "Chiara_Skill01_Start", 15);
			SetAnimation(Self, BooleanMotionWait, true);
			PlayEffectChildManual(Self, "FX_BI_Chiara_Skill01_Ready_key", "FX_BI_Chiara_Skill01_Ready3", "Fx_Hand_R");
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			if (action == 11)
			{
				PlayAnimation(Self, TriggerSkill01_2);
				StartCoroutine(CoroutineUtil.DelayedAction(0.19f, delegate
				{
					PlaySoundPoint(Self, "Chiara_Skill01_Attack", 15);
					PlayEffectPoint(Self, "FX_BI_Chiara_Skill01_01");
				}));
				SetAnimation(Self, BooleanMotionWait, true);
				if (SingletonMonoBehaviour<PlayerController>.inst.IsMe(Self.ObjectId))
				{
					StopCoroutine(timeCheckSkillFormChange);
					SkillSlotSet? currentSkillSlotSet =
						SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.currentSkillSlotSet;
					SkillSlotSet skillSlotSet = SkillSlotSet.Active1_1;
					if ((currentSkillSlotSet.GetValueOrDefault() == skillSlotSet) & (currentSkillSlotSet != null))
					{
						SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.ReleaseFixedIndicator(true);
					}
				}
			}
			else if (action == 12)
			{
				PlayAnimation(Self, TriggerSkill01_4);
				StopEffectChildManual(Self, "FX_BI_Chiara_Skill01_Ready_key", true);
				StopEffectChildManual(Self, "FX_BI_Chiara_Skill01_Ready02_key", true);
				SetAnimation(Self, BooleanMotionWait, true);
				StartCoroutine(CoroutineUtil.DelayedAction(0.19f,
					delegate { StopSoundChildManual(Self, "Chiara_Skill01_Ing"); }));
				if (SingletonMonoBehaviour<PlayerController>.inst.IsMe(Self.ObjectId))
				{
					StopCoroutine(timeCheckSkillFormChange);
					SkillSlotSet? currentSkillSlotSet =
						SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.currentSkillSlotSet;
					SkillSlotSet skillSlotSet = SkillSlotSet.Active1_1;
					if ((currentSkillSlotSet.GetValueOrDefault() == skillSlotSet) & (currentSkillSlotSet != null))
					{
						SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.ReleaseFixedIndicator(true);
					}
				}
			}
			else if (action == 13)
			{
				StopEffectChildManual(Self, "FX_BI_Chiara_Skill01_Ready_key", true);
				StopEffectChildManual(Self, "FX_BI_Chiara_Skill01_Ready02_key", true);
				SetAnimation(Self, BooleanSkill01, false);
				StopSoundChildManual(Self, "Chiara_Skill01_Ing");
				SetAnimation(Self, BooleanMotionWait, false);
				if (SingletonMonoBehaviour<PlayerController>.inst.IsMe(Self.ObjectId))
				{
					StopCoroutine(timeCheckSkillFormChange);
					SkillSlotSet? currentSkillSlotSet =
						SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.currentSkillSlotSet;
					SkillSlotSet skillSlotSet = SkillSlotSet.Active1_1;
					if ((currentSkillSlotSet.GetValueOrDefault() == skillSlotSet) & (currentSkillSlotSet != null))
					{
						SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.ReleaseFixedIndicator(true);
					}
				}
			}
		}


		public override void Finish(bool cancel)
		{
			SetAnimation(Self, BooleanSkill01, false);
			StopEffectChildManual(Self, "FX_BI_Chiara_Skill01_Ready_key", true);
			if (SingletonMonoBehaviour<PlayerController>.inst.IsMe(Self.ObjectId))
			{
				StopCoroutine(timeCheckSkillFormChange);
				SkillSlotSet? currentSkillSlotSet =
					SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.currentSkillSlotSet;
				SkillSlotSet skillSlotSet = SkillSlotSet.Active1_1;
				if ((currentSkillSlotSet.GetValueOrDefault() == skillSlotSet) & (currentSkillSlotSet != null))
				{
					SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.ReleaseFixedIndicator(true);
				}
			}
		}


		public override void StartConcentration()
		{
			base.StartConcentration();
			if (SingletonMonoBehaviour<PlayerController>.inst.IsMe(Self.ObjectId))
			{
				if (SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.SetSkillIndicator(SkillSlotSet.Active1_1,
					data, true, SkillRange, SkillLength, null, SkillAngle))
				{
					PlayerSkill.ChangeCursorStatus onChangeCursorStatus =
						SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.OnChangeCursorStatus;
					if (onChangeCursorStatus != null)
					{
						onChangeCursorStatus(CursorStatus.SkillCanMove);
					}
				}

				timeCheckSkillFormChange = StartCoroutine(TimeCheckSkillFormChange());
			}
		}


		private IEnumerator TimeCheckSkillFormChange()
		{
			yield return new WaitForSeconds(Singleton<ChiaraSkillData>.inst.A1SkillChangeTimeClient -
			                                Singleton<GameTime>.inst.Rtt);
			PlayAnimation(Self, TriggerSkill01_3);
			PlayEffectChildManual(Self, "FX_BI_Chiara_Skill01_Ready02_key", "FX_BI_Chiara_Skill01_Ready4", "Fx_Hand_R");
			PlaySoundChildManual(Self, "Chiara_Skill01_Ing", 15);
			if (SingletonMonoBehaviour<PlayerController>.inst.IsMe(Self.ObjectId))
			{
				SkillSlotSet? currentSkillSlotSet =
					SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.currentSkillSlotSet;
				SkillSlotSet skillSlotSet = SkillSlotSet.Active1_1;
				if ((currentSkillSlotSet.GetValueOrDefault() == skillSlotSet) & (currentSkillSlotSet != null) &&
				    SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.SetSkillIndicator(SkillSlotSet.Active1_1,
					    data, false, Singleton<ChiaraSkillData>.inst.A1Range_2,
					    Singleton<ChiaraSkillData>.inst.A1Length_2, Singleton<ChiaraSkillData>.inst.A1Width_1))
				{
					PlayerSkill.ChangeCursorStatus onChangeCursorStatus =
						SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.OnChangeCursorStatus;
					if (onChangeCursorStatus != null)
					{
						onChangeCursorStatus(CursorStatus.SkillCanMove);
					}
				}
			}
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<ChiaraSkillData>.inst.A1SkillChangeTime.ToString();
				case 1:
					return Singleton<ChiaraSkillData>.inst.A1BaseDamage_1[skillData.level].ToString();
				case 2:
					return ((int) (Singleton<ChiaraSkillData>.inst.A1ApDamage_1 * SelfStat.AttackPower)).ToString();
				case 3:
					return Singleton<ChiaraSkillData>.inst.A1GroundProjectileDuration_2.ToString();
				case 4:
				{
					CharacterStateData data =
						GameDB.characterState.GetData(Singleton<ChiaraSkillData>.inst.A1GroundDebuffStateCode);
					return string.Format("{0}%", Mathf.Abs(data.statValue1));
				}
				case 5:
				{
					int num = (int) Mathf.Abs(Singleton<ChiaraSkillData>.inst.A1SkillActiveCooldownModify_1 * 100f);
					return string.Format("{0}%", num);
				}
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			switch (index)
			{
				case 0:
					return "ToolTipType/Damage";
				case 1:
					return "ToolTipType/CoolTime";
				case 2:
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
					return Singleton<ChiaraSkillData>.inst.A1BaseDamage_1[level].ToString();
				case 1:
					return skillData.cooldown.ToString();
				case 2:
					return skillData.cost.ToString();
				default:
					return "";
			}
		}
	}
}