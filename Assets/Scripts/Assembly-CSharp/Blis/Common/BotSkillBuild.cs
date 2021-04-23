using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blis.Common
{
	public class BotSkillBuild
	{
		public readonly int characterCode;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly SkillSlotIndex easy;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly SkillSlotIndex hard;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly SkillSlotIndex normal;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly SkillSlotIndex pvp;

		[JsonConstructor]
		public BotSkillBuild(int characterCode, SkillSlotIndex easy, SkillSlotIndex normal, SkillSlotIndex hard,
			SkillSlotIndex pvp)
		{
			this.characterCode = characterCode;
			this.easy = easy;
			this.normal = normal;
			this.hard = hard;
			this.pvp = pvp;
		}


		public SkillSlotIndex GetSkillSlotIndex(BotDifficulty difficulty)
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
					return easy;
			}
		}
	}
}