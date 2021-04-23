using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.HartActive2)]
	public class HartActive2 : LocalSkillScript
	{
		private const string Hart_Skill02_Trail = "Hart_Skill02_Trail";


		private const string Fx_BI_Hart_Skill02_Trail = "Fx_BI_Hart_Skill02_Trail";


		private const string Fx_Guitar_Head = "Fx_Guitar_Head";


		private readonly Dictionary<int, string> Effect_Skill = new Dictionary<int, string>();


		private readonly Dictionary<int, string> SkillActive_SFX = new Dictionary<int, string>();

		public HartActive2()
		{
			Effect_Skill.Add(0, "Fx_BI_Hart_Skill02");
			Effect_Skill.Add(1, "Fx_BI_Hart_Skill02_Evolve");
			Effect_Skill.Add(2, "Fx_BI_Hart_Skill02_Evolve");
			SkillActive_SFX.Add(0, "hart_Skill02_Active");
			SkillActive_SFX.Add(1, "hart_Skill02_Evo_Active");
			SkillActive_SFX.Add(2, "hart_Skill02_Evo_Active");
		}


		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill02);
			SetAnimation(Self, BooleanSkill02, true);
			StartCoroutine(CoroutineUtil.DelayedAction(0.5f, delegate { SetAnimation(Self, BooleanSkill02, false); }));
			PlayEffectChildManual(Self, "Hart_Skill02_Trail", "Fx_BI_Hart_Skill02_Trail", "Fx_Guitar_Head");
			PlayEffectChild(Self, Effect_Skill[evolutionLevel]);
			PlaySoundPoint(Self, SkillActive_SFX[evolutionLevel]);
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "Hart_Skill02_Trail", true);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return GameDB.characterState
						.GetData(Singleton<HartSkillActive2Data>.inst.BuffState[skillData.level]).duration.ToString();
				case 1:
				{
					float statValue = GameDB.characterState
						.GetData(Singleton<HartSkillActive2Data>.inst.BuffState[skillData.level]).statValue1;
					return string.Format("{0}%", statValue);
				}
				case 2:
				{
					int num = evolutionLevel;
					if (num < 1)
					{
						num = 1;
					}

					float num2 = Mathf.Abs(GameDB.characterState
						.GetData(Singleton<HartSkillActive2Data>.inst.DebuffState[num]).statValue1);
					return string.Format("{0}%", num2);
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
					return "StatType/AttackPowerRatio";
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
				{
					float statValue = GameDB.characterState
						.GetData(Singleton<HartSkillActive2Data>.inst.BuffState[level]).statValue1;
					return string.Format("{0}%", statValue);
				}
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