using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blis.Common
{
	public class RecommendStarting
	{
		public readonly int characterCode;
		public readonly int code;
		public readonly int defaultWeapon;
		public readonly string Description;

		[JsonConverter(typeof(StringEnumConverter))]
		public readonly MasteryType mastery;


		public readonly int startItemGroupCode;


		public readonly int startWeapon;

		[JsonConstructor]
		public RecommendStarting(int code, int characterCode, MasteryType mastery, int startWeapon,
			int startItemGroupCode, int defaultWeapon, string description)
		{
			this.code = code;
			this.characterCode = characterCode;
			this.mastery = mastery;
			this.startWeapon = startWeapon;
			this.startItemGroupCode = startItemGroupCode;
			this.defaultWeapon = defaultWeapon;
			Description = description;
		}
	}
}