using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blis.Common
{
	public class BotMastery
	{
		public readonly int easy;


		public readonly int hard;


		public readonly int masterySetPoint;


		public readonly int normal;


		public readonly int pvp;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly MasteryType type;

		[JsonConstructor]
		public BotMastery(int masterySetPoint, MasteryType type, int easy, int normal, int hard, int pvp)
		{
			this.masterySetPoint = masterySetPoint;
			this.type = type;
			this.easy = easy;
			this.normal = normal;
			this.hard = hard;
			this.pvp = pvp;
		}


		public int GetMasteryLevel(BotDifficulty difficulty)
		{
			switch (difficulty)
			{
				case BotDifficulty.EASY:
					return easy;
				case BotDifficulty.NORMAL:
					return normal;
				case BotDifficulty.HARD:
					return hard;
				case BotDifficulty.PVP:
					return pvp;
				default:
					throw new ArgumentOutOfRangeException("difficulty", difficulty, null);
			}
		}
	}
}