using System;
using Newtonsoft.Json;

namespace Blis.Common
{
	
	public class MonsterSpawnLevelData
	{
		
		[JsonConstructor]
		public MonsterSpawnLevelData(int playerLevel, int chicken, int bat, int boar, int wildDog, int wolf, int bear, int wickline)
		{
			this.playerLevel = playerLevel;
			this.chicken = chicken;
			this.bat = bat;
			this.boar = boar;
			this.wildDog = wildDog;
			this.wolf = wolf;
			this.bear = bear;
			this.wickline = wickline;
		}

		
		public int GetSpawnLevel(MonsterType monsterType)
		{
			switch (monsterType)
			{
			case MonsterType.Chicken:
				return this.chicken;
			case MonsterType.Bat:
				return this.bat;
			case MonsterType.Boar:
				return this.boar;
			case MonsterType.WildDog:
				return this.wildDog;
			case MonsterType.Wolf:
				return this.wolf;
			case MonsterType.Bear:
				return this.bear;
			case MonsterType.Wickline:
				return this.wickline;
			default:
				throw new ArgumentOutOfRangeException("monsterType", monsterType, null);
			}
		}

		
		public readonly int playerLevel;

		
		public readonly int chicken;

		
		public readonly int bat;

		
		public readonly int boar;

		
		public readonly int wildDog;

		
		public readonly int wolf;

		
		public readonly int bear;

		
		public readonly int wickline;
	}
}
