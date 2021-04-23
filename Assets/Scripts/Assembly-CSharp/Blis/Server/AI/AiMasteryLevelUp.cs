using Blis.Common;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("캐릭터의 마스터리 레벨을 지정한 값을 사용하여 설정한다")]
	public class AiMasteryLevelUp : ActionTaskBase
	{
		
		protected override void OnExecuteCommandFrame()
		{
			if (!base.agent.IsTypeOf<WorldBotPlayerCharacter>())
			{
				base.EndAction(false);
				return;
			}
			if (this.difficulty.value == BotDifficulty.TUTORIAL1 || this.difficulty.value == BotDifficulty.TUTORIAL2)
			{
				base.EndAction(true);
				return;
			}
			WorldBotPlayerCharacter worldBotPlayerCharacter = (WorldBotPlayerCharacter)base.agent;
			foreach (BotMastery botMastery in GameDB.bot.GetBotMasteryBySetPoint(this.masterySetPoint.value))
			{
				MasteryType masteryType = botMastery.type;
				if (masteryType.IsWeaponMastery())
				{
					Item weapon = worldBotPlayerCharacter.GetWeapon();
					if (weapon == null)
					{
						continue;
					}
					masteryType = weapon.ItemData.GetMasteryType();
				}
				worldBotPlayerCharacter.MasteryLevelUpToTarget(masteryType, botMastery.GetMasteryLevel(this.difficulty.value));
			}
			if (worldBotPlayerCharacter.CanSkillUpgrade(SkillSlotIndex.WeaponSkill))
			{
				worldBotPlayerCharacter.CharacterSkill.UpgradeSkill(worldBotPlayerCharacter.GetEquipWeaponMasteryType());
			}
			base.EndAction(true);
		}

		
		public BBParameter<BotDifficulty> difficulty;

		
		public BBParameter<int> masterySetPoint;
	}
}
