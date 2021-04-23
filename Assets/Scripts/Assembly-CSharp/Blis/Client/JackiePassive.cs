using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.JackiePassive)]
	public class JackiePassive : LocalSkillScript
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
						.GetData(Singleton<JackieSkillPassiveData>.inst.BuffState_2[skillData.level]).duration
						.ToString();
				case 1:
				{
					CharacterStateData data =
						GameDB.characterState.GetData(
							Singleton<JackieSkillPassiveData>.inst.BuffState[skillData.level]);
					CharacterStateData data2 =
						GameDB.characterState.GetData(
							Singleton<JackieSkillPassiveData>.inst.BuffState_2[skillData.level]);
					int num = (int) (data.statValue1 + data2.statValue1);
					return string.Format("{0}%", num);
				}
				case 2:
				{
					CharacterStateData data3 =
						GameDB.characterState.GetData(
							Singleton<JackieSkillPassiveData>.inst.BuffState_2[skillData.level]);
					return string.Format("{0}%", data3.statValue1);
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
					return "ToolTipType/Time";
				case 1:
					return "ToolTipType/JackieGrade1";
				case 2:
					return "ToolTipType/JackieGrade2";
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			switch (index)
			{
				case 0:
					return GameDB.characterState
						.GetData(Singleton<JackieSkillPassiveData>.inst.BuffState_2[skillData.level]).duration
						.ToString();
				case 1:
				{
					CharacterStateData data =
						GameDB.characterState.GetData(
							Singleton<JackieSkillPassiveData>.inst.BuffState[skillData.level]);
					CharacterStateData data2 =
						GameDB.characterState.GetData(
							Singleton<JackieSkillPassiveData>.inst.BuffState_2[skillData.level]);
					int num = (int) (data.statValue1 + data2.statValue1);
					return string.Format("{0}%", num);
				}
				case 2:
				{
					CharacterStateData data3 =
						GameDB.characterState.GetData(
							Singleton<JackieSkillPassiveData>.inst.BuffState_2[skillData.level]);
					return string.Format("{0}%", data3.statValue1);
				}
				default:
					return "";
			}
		}
	}
}