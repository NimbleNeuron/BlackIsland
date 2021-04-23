using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Server
{
	
	public class TutorialService : ServiceBase
	{
		
		public void Init(TutorialType tutorialType)
		{
			this.currentTutorialSettingData = GameDB.tutorial.GetTutorialSettingData(tutorialType);
			this.sequence = 0;
		}

		
		public void OnSpawnBotPlayerCharacter(WorldBotPlayerCharacter botPlayerCharacter)
		{
			if (this.currentTutorialSettingData.type == TutorialType.Trace)
			{
				botPlayerCharacter.StopAI();
				return;
			}
			if (this.currentTutorialSettingData.type == TutorialType.FinalSurvival)
			{
				botPlayerCharacter.StopAI();
			}
		}

		
		public void Next()
		{
			this.sequence++;
			switch (this.currentTutorialSettingData.type)
			{
			case TutorialType.BasicGuide:
			case TutorialType.Hunt:
			case TutorialType.PowerUp:
				return;
			case TutorialType.Trace:
				if (this.sequence != 1)
				{
					return;
				}
				using (List<WorldBotPlayerCharacter>.Enumerator enumerator = MonoBehaviourInstance<GameService>.inst.World.FindAll<WorldBotPlayerCharacter>().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						WorldBotPlayerCharacter worldBotPlayerCharacter = enumerator.Current;
						CharacterSpawnPoint characterSpawnPoint = null;
						if (worldBotPlayerCharacter.CharacterSettingCode == this.TraceBotCharacterSettingCode1)
						{
							characterSpawnPoint = this.game.Level.GetPlayerSpawnPoint(1, 2);
						}
						else if (worldBotPlayerCharacter.CharacterSettingCode == this.TraceBotCharacterSettingCode2)
						{
							characterSpawnPoint = this.game.Level.GetPlayerSpawnPoint(1, 3);
						}
						else if (worldBotPlayerCharacter.CharacterSettingCode == this.TraceBotCharacterSettingCode3)
						{
							characterSpawnPoint = this.game.Level.GetPlayerSpawnPoint(1, 4);
						}
						if (characterSpawnPoint != null)
						{
							worldBotPlayerCharacter.WarpTo(characterSpawnPoint.transform.position, true);
						}
						worldBotPlayerCharacter.StartAI();
					}
					return;
				}
			case TutorialType.FinalSurvival:
				break;
			default:
				return;
			}
			if (this.sequence == 1)
			{
				foreach (WorldBotPlayerCharacter worldBotPlayerCharacter2 in MonoBehaviourInstance<GameService>.inst.World.FindAll<WorldBotPlayerCharacter>())
				{
					CharacterSpawnPoint characterSpawnPoint2 = null;
					if (worldBotPlayerCharacter2.CharacterSettingCode == this.FinalSurvivalBotCharacterSettingCode1)
					{
						characterSpawnPoint2 = this.game.Level.GetPlayerSpawnPoint(1, 1);
					}
					else if (worldBotPlayerCharacter2.CharacterSettingCode == this.FinalSurvivalBotCharacterSettingCode2)
					{
						characterSpawnPoint2 = this.game.Level.GetPlayerSpawnPoint(2, 1);
					}
					else if (worldBotPlayerCharacter2.CharacterSettingCode == this.FinalSurvivalBotCharacterSettingCode3)
					{
						characterSpawnPoint2 = this.game.Level.GetPlayerSpawnPoint(3, 1);
					}
					else if (worldBotPlayerCharacter2.CharacterSettingCode == this.FinalSurvivalBotCharacterSettingCode4)
					{
						characterSpawnPoint2 = this.game.Level.GetPlayerSpawnPoint(4, 1);
					}
					else if (worldBotPlayerCharacter2.CharacterSettingCode == this.FinalSurvivalBotCharacterSettingCode5)
					{
						characterSpawnPoint2 = this.game.Level.GetPlayerSpawnPoint(5, 1);
					}
					else if (worldBotPlayerCharacter2.CharacterSettingCode == this.FinalSurvivalBotCharacterSettingCode6)
					{
						characterSpawnPoint2 = this.game.Level.GetPlayerSpawnPoint(6, 1);
					}
					else if (worldBotPlayerCharacter2.CharacterSettingCode == this.FinalSurvivalBotCharacterSettingCode7)
					{
						characterSpawnPoint2 = this.game.Level.GetPlayerSpawnPoint(7, 1);
					}
					else if (worldBotPlayerCharacter2.CharacterSettingCode == this.FinalSurvivalBotCharacterSettingCode8)
					{
						characterSpawnPoint2 = this.game.Level.GetPlayerSpawnPoint(8, 1);
					}
					else if (worldBotPlayerCharacter2.CharacterSettingCode == this.FinalSurvivalBotCharacterSettingCode9)
					{
						characterSpawnPoint2 = this.game.Level.GetPlayerSpawnPoint(9, 1);
					}
					else if (worldBotPlayerCharacter2.CharacterSettingCode == this.FinalSurvivalBotCharacterSettingCode10)
					{
						characterSpawnPoint2 = this.game.Level.GetPlayerSpawnPoint(10, 1);
					}
					else if (worldBotPlayerCharacter2.CharacterSettingCode == this.FinalSurvivalBotCharacterSettingCode11)
					{
						characterSpawnPoint2 = this.game.Level.GetPlayerSpawnPoint(11, 2);
					}
					else if (worldBotPlayerCharacter2.CharacterSettingCode == this.FinalSurvivalBotCharacterSettingCode12)
					{
						characterSpawnPoint2 = this.game.Level.GetPlayerSpawnPoint(12, 1);
					}
					else if (worldBotPlayerCharacter2.CharacterSettingCode == this.FinalSurvivalBotCharacterSettingCode13)
					{
						characterSpawnPoint2 = this.game.Level.GetPlayerSpawnPoint(14, 1);
					}
					else if (worldBotPlayerCharacter2.CharacterSettingCode == this.FinalSurvivalBotCharacterSettingCode14)
					{
						characterSpawnPoint2 = this.game.Level.GetPlayerSpawnPoint(15, 1);
					}
					if (characterSpawnPoint2 != null)
					{
						worldBotPlayerCharacter2.WarpTo(characterSpawnPoint2.transform.position, true);
					}
					worldBotPlayerCharacter2.StartAI();
				}
				MonoBehaviourInstance<GameService>.inst.Area.StartRestriction(DayNight.Day);
			}
		}

		
		private TutorialSettingData currentTutorialSettingData;

		
		private int sequence;

		
		public readonly int TraceBotCharacterSettingCode1 = 20;

		
		public readonly int TraceBotCharacterSettingCode2 = 21;

		
		public readonly int TraceBotCharacterSettingCode3 = 22;

		
		public readonly int FinalSurvivalBotCharacterSettingCode1 = 50;

		
		public readonly int FinalSurvivalBotCharacterSettingCode2 = 51;

		
		public readonly int FinalSurvivalBotCharacterSettingCode3 = 52;

		
		public readonly int FinalSurvivalBotCharacterSettingCode4 = 53;

		
		public readonly int FinalSurvivalBotCharacterSettingCode5 = 54;

		
		public readonly int FinalSurvivalBotCharacterSettingCode6 = 55;

		
		public readonly int FinalSurvivalBotCharacterSettingCode7 = 56;

		
		public readonly int FinalSurvivalBotCharacterSettingCode8 = 57;

		
		public readonly int FinalSurvivalBotCharacterSettingCode9 = 58;

		
		public readonly int FinalSurvivalBotCharacterSettingCode10 = 59;

		
		public readonly int FinalSurvivalBotCharacterSettingCode11 = 60;

		
		public readonly int FinalSurvivalBotCharacterSettingCode12 = 61;

		
		public readonly int FinalSurvivalBotCharacterSettingCode13 = 62;

		
		public readonly int FinalSurvivalBotCharacterSettingCode14 = 63;
	}
}
