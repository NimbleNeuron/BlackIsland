using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.ChiaraPassive)]
	public class LocalChiaraPassive : LocalSkillScript
	{
		public override void Start() { }


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return GameDB.characterState
						.GetData(Singleton<ChiaraSkillData>.inst.PassiveDebuffStateCode[skillData.level]).maxStack
						.ToString();
				case 1:
					return GameDB.characterState
						.GetData(Singleton<ChiaraSkillData>.inst.PassiveDebuffStateCode[skillData.level]).duration
						.ToString();
				case 2:
				{
					CharacterStateData data =
						GameDB.characterState.GetData(
							Singleton<ChiaraSkillData>.inst.PassiveDebuffStateCode[skillData.level]);
					return string.Format("{0}%", Mathf.Abs(data.statValue1));
				}
				case 3:
				{
					CharacterStateData data2 =
						GameDB.characterState.GetData(
							Singleton<ChiaraSkillData>.inst.PassiveBuffStateCode[skillData.level]);
					return string.Format("{0}%", data2.statValue2);
				}
				case 4:
					return GameDB.characterState
						.GetData(Singleton<ChiaraSkillData>.inst.PassiveBuffStateCode[skillData.level]).duration
						.ToString();
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			if (index == 0)
			{
				return "ToolTipType/DecreaseDefenseRatio";
			}

			if (index != 1)
			{
				return "";
			}

			return "ToolTipType/MoveSpeedUpRatio";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				CharacterStateData data =
					GameDB.characterState.GetData(
						Singleton<ChiaraSkillData>.inst.PassiveDebuffStateCode[skillData.level]);
				return string.Format("{0}", Mathf.Abs(data.statValue1));
			}

			if (index != 1)
			{
				return "";
			}

			CharacterStateData data2 =
				GameDB.characterState.GetData(Singleton<ChiaraSkillData>.inst.PassiveBuffStateCode[skillData.level]);
			return string.Format("{0}", data2.statValue2);
		}
	}
}