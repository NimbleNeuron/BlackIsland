using System.Collections.Generic;
using MessagePack;

namespace Blis.Common
{
	
	public class TutorialSettingData
	{
		[IgnoreMember]
		public int BotCount
		{
			get
			{
				return this.botSettingDataCode.Count;
			}
		}
		
		public TutorialSettingData(TutorialType type, int playerSettingDataCode, BotDifficulty botDifficulty, List<int> botSettingDataCode, DayNight beginDayNight, bool enableAirSupply, bool enableMonsterReSpawn, bool enableBattleTimer, bool enableHyperloop, bool enableSecurityConsole, bool enableChangeCameraMode, TutorialFinishConditionType finishConditionType, int finishConditionKey, int finishConditionValue, List<int> restrictedAreas)
		{
			this.type = type;
			this.playerSettingDataCode = playerSettingDataCode;
			this.botDifficulty = botDifficulty;
			this.botSettingDataCode = botSettingDataCode;
			this.beginDayNight = beginDayNight;
			this.enableAirSupply = enableAirSupply;
			this.enableMonsterReSpawn = enableMonsterReSpawn;
			this.enableBattleTimer = enableBattleTimer;
			this.enableHyperloop = enableHyperloop;
			this.enableSecurityConsole = enableSecurityConsole;
			this.enableChangeCameraMode = enableChangeCameraMode;
			this.finishConditionType = finishConditionType;
			this.finishConditionKey = finishConditionKey;
			this.finishConditionValue = finishConditionValue;
			this.restrictedAreas = restrictedAreas;
		}

		
		public readonly TutorialType type;
		public readonly int playerSettingDataCode;
		public readonly BotDifficulty botDifficulty;
		public readonly List<int> botSettingDataCode;
		public readonly DayNight beginDayNight;
		public readonly List<int> restrictedAreas;
		public readonly bool enableAirSupply;
		public readonly bool enableMonsterReSpawn;
		public readonly bool enableBattleTimer;
		public readonly bool enableHyperloop;
		public readonly bool enableSecurityConsole;
		public readonly bool enableChangeCameraMode;
		public readonly TutorialFinishConditionType finishConditionType;
		public readonly int finishConditionKey;
		public readonly int finishConditionValue;
	}
}
