using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blis.Common
{
	public class BotCraft
	{
		public readonly int craftSetPoint;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly ItemGrade easy;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly ItemGrade hard;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly ItemGrade normal;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly ItemGrade pvp;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly BotCraftType type;

		[JsonConstructor]
		public BotCraft(int craftSetPoint, BotCraftType type, ItemGrade easy, ItemGrade normal, ItemGrade hard,
			ItemGrade pvp)
		{
			this.craftSetPoint = craftSetPoint;
			this.type = type;
			this.easy = easy;
			this.normal = normal;
			this.hard = hard;
			this.pvp = pvp;
		}


		public ItemGrade GetItemGrade(BotDifficulty difficulty)
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
					return ItemGrade.None;
			}
		}
	}
}