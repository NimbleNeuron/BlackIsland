using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.ChiaraActive2ShieldBreak)]
	public class LocalChiaraActive2ShieldBreak : LocalSkillScript
	{
		public override void Start()
		{
			SetAnimation(Self, BooleanSkill03, true);
			SetAnimation(Self, BooleanMotionWait, true);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return (GameDB.characterState.GetData(Singleton<ChiaraSkillData>.inst.A2ShieldState).duration / 2f)
						.ToString();
				case 1:
					return Singleton<ChiaraSkillData>.inst.A2BaseDamage[skillData.level].ToString();
				case 2:
					return ((int) (Singleton<ChiaraSkillData>.inst.A2ApDamage * SelfStat.AttackPower)).ToString();
				default:
					return "";
			}
		}
	}
}