using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.AdrianaActive2)]
	public class AdrianaActive2 : LocalSkillScript
	{
		private const string adriana_Skill02_sfx = "adriana_Skill02";


		private const string adriana_Skill02_FireGround = "adriana_Skill02_FireGround";


		private const string adriana_Skill02_Ground_sfx = " Adriana_Skill02_Ground";

		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill02);
			PlaySoundPoint(Self, "adriana_Skill02", 15);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			if (action == 1)
			{
				PlaySoundPoint(Self, "adriana_Skill02_FireGround", 15);
			}
		}


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return GameDB.projectile.GetData(Singleton<AdrianaSkillActive2Data>.inst.OilAreaProjectileCode)
						.lifeTimeAfterExplosion.ToString();
				case 1:
					return GameDB.projectile.GetData(Singleton<AdrianaSkillActive2Data>.inst.FireFlame1ProjectileCode)
						.lifeTimeAfterExplosion.ToString();
				case 2:
				{
					int num = Mathf.Abs((int) GameDB.characterState
						.GetData(Singleton<AdrianaSkillActive2Data>.inst.OilAreaInfluenceStateCode).statValue1);
					return string.Format("{0}%", num);
				}
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			if (index == 0)
			{
				return "ToolTipType/CoolTime";
			}

			return "";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				return skillData.cooldown.ToString();
			}

			return "";
		}
	}
}