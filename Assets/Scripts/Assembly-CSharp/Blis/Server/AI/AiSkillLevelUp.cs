using System.Linq;
using Blis.Common;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("스킬 레벨y을 올린다. Use Common Skill Build를 체크하면 Bot Skill Build 데이터의 CharacterCode가 0번인 데이터를 사용한다.체크하지 않으면 현재 캐릭터의 CharacterCode로 사용")]
	public class AiSkillLevelUp : ActionTaskBase
	{
		
		protected override void OnExecuteCommandFrame()
		{
			if (!base.agent.IsTypeOf<WorldPlayerCharacter>())
			{
				base.EndAction(false);
				return;
			}
			WorldPlayerCharacter worldPlayerCharacter = (WorldPlayerCharacter)base.agent;
			BotSkillBuild botSkillBuild = GameDB.bot.GetBotSkillSequence(this.useCommonSkillBuild ? 0 : base.agent.CharacterCode).ElementAtOrDefault(this.skillBuildIndex.value);
			if (botSkillBuild == null)
			{
				base.EndAction(false);
				return;
			}
			SkillSlotIndex skillSlotIndex = botSkillBuild.GetSkillSlotIndex(this.difficulty.value);
			if (!worldPlayerCharacter.IsHaveSkillUpgradePoint(skillSlotIndex))
			{
				base.EndAction(false);
				return;
			}
			SkillSlotIndex skillSlotIndex2 = skillSlotIndex;
			worldPlayerCharacter.UpgradeSkill(skillSlotIndex2);
			BBParameter<int> bbparameter = this.skillBuildIndex;
			int value = bbparameter.value;
			bbparameter.value = value + 1;
			base.EndAction(true);
		}

		
		public bool useCommonSkillBuild;

		
		public BBParameter<int> skillBuildIndex;

		
		public BBParameter<BotDifficulty> difficulty;
	}
}
