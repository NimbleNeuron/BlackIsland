using System.Collections.Generic;

namespace Blis.Common
{
	
	public class LukeSkillPassiveData : Singleton<LukeSkillPassiveData>
	{
		
		public readonly int AddRecoveryHpIncrease = 1;

		
		public readonly int AddRecoveryHpIncreaseStackUnit = 10;

		
		public readonly Dictionary<ItemGrade, int> AirSurpplyStack = new Dictionary<ItemGrade, int>();

		
		public readonly int BuffState = 1022101;

		
		public readonly int CleaningCompletedStackMax = 50;

		
		public readonly Dictionary<MonsterType, int> MonsterStack = new Dictionary<MonsterType, int>();

		
		public readonly int PlayerKillStack = 5;

		
		public readonly int RecoveryHpEffectAndSoundCode = 1022102;

		
		public readonly Dictionary<int, float> ReocveryLostHPRatioPlayerKill = new Dictionary<int, float>();

		
		public readonly Dictionary<int, float> ReocveryLostHPRatioWicklineKill = new Dictionary<int, float>();

		
		public LukeSkillPassiveData()
		{
			MonsterStack.Add(MonsterType.Chicken, 1);
			MonsterStack.Add(MonsterType.Bat, 1);
			MonsterStack.Add(MonsterType.Boar, 3);
			MonsterStack.Add(MonsterType.WildDog, 3);
			MonsterStack.Add(MonsterType.Wolf, 3);
			MonsterStack.Add(MonsterType.Bear, 3);
			MonsterStack.Add(MonsterType.Wickline, 10);
			AirSurpplyStack.Add(ItemGrade.Uncommon, 1);
			AirSurpplyStack.Add(ItemGrade.Rare, 1);
			AirSurpplyStack.Add(ItemGrade.Epic, 2);
			AirSurpplyStack.Add(ItemGrade.Legend, 5);
			ReocveryLostHPRatioPlayerKill.Add(1, 0.05f);
			ReocveryLostHPRatioPlayerKill.Add(2, 0.08f);
			ReocveryLostHPRatioPlayerKill.Add(3, 0.12f);
			ReocveryLostHPRatioWicklineKill.Add(1, 0.2f);
			ReocveryLostHPRatioWicklineKill.Add(2, 0.25f);
			ReocveryLostHPRatioWicklineKill.Add(3, 0.3f);
		}

		
		public int GetAddRecoveryHpAmount(int currentStack)
		{
			int cleaningCompletedStackMax = CleaningCompletedStackMax;
			if (currentStack > cleaningCompletedStackMax)
			{
				return (currentStack - cleaningCompletedStackMax) / AddRecoveryHpIncreaseStackUnit;
			}

			return 0;
		}
	}
}