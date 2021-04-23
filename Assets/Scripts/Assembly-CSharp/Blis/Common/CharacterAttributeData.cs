using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blis.Common
{
	
	public class CharacterAttributeData
	{
		
		[JsonConstructor]
		public CharacterAttributeData(string character, int characterCode, MasteryType mastery, int controlDifficulty, int attack, int defense, int disruptor, int move, int assistance)
		{
			this.character = character;
			this.characterCode = characterCode;
			this.mastery = mastery;
			this.controlDifficulty = controlDifficulty;
			this.attack = attack;
			this.defense = defense;
			this.disruptor = disruptor;
			this.move = move;
			this.assistance = assistance;
		}

		
		public readonly string character;

		
		public readonly int characterCode;

		
		[JsonConverter(typeof(StringEnumConverter))]
		public readonly MasteryType mastery;

		
		public readonly int controlDifficulty;

		
		public readonly int attack;

		
		public readonly int defense;

		
		public readonly int disruptor;

		
		public readonly int move;

		
		public readonly int assistance;
	}
}
