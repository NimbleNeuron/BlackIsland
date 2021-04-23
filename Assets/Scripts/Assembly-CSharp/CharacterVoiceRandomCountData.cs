using System;
using Blis.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


public class CharacterVoiceRandomCountData
{
	
	[JsonConstructor]
	public CharacterVoiceRandomCountData(int code, int characterCode, int skinIndex, CharacterVoiceType useCase, string skillSet, int randomCount)
	{
		this.code = code;
		this.characterCode = characterCode;
		this.skinIndex = skinIndex;
		this.useCase = useCase;
		SkillSlotSet skillSlotSet;
		this.skillSet = (Enum.TryParse<SkillSlotSet>(skillSet, true, out skillSlotSet) ? skillSlotSet : SkillSlotSet.None);
		this.randomCount = randomCount;
	}

	
	public readonly int code;

	
	public readonly int characterCode;

	
	public readonly int skinIndex;

	
	[JsonConverter(typeof(StringEnumConverter))]
	public readonly CharacterVoiceType useCase;

	
	[JsonConverter(typeof(StringEnumConverter))]
	public readonly SkillSlotSet skillSet;

	
	public readonly int randomCount;
}
