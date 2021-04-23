using Newtonsoft.Json;

namespace Blis.Common
{
	public class CharacterExpData
	{
		public readonly int accumulateLevelUpExp;


		public readonly int level;


		public readonly int levelUpExp;

		[JsonConstructor]
		public CharacterExpData(int level, int levelUpExp, int accumulateLevelUpExp)
		{
			this.level = level;
			this.levelUpExp = levelUpExp;
			this.accumulateLevelUpExp = accumulateLevelUpExp;
		}
	}
}