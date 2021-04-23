using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


public class CharacterVoiceData
{
	
	[JsonConstructor]
	public CharacterVoiceData(int code, CharacterVoiceType useCase, bool battlePlaying, int globalCoolTime, int useCaseMinCoolTime, int useCaseMaxCoolTime, bool onceRepeatIgnore, int maxCount, bool spatial3D, ListenerType listener, bool immediatelyPlay, string soundName)
	{
		this.code = code;
		this.useCase = useCase;
		this.battlePlaying = battlePlaying;
		this.globalCoolTime = globalCoolTime;
		this.useCaseMinCoolTime = useCaseMinCoolTime;
		this.useCaseMaxCoolTime = useCaseMaxCoolTime;
		this.onceRepeatIgnore = onceRepeatIgnore;
		this.maxCount = maxCount;
		this.spatial3D = spatial3D;
		this.listener = listener;
		this.immediatelyPlay = immediatelyPlay;
		this.soundName = soundName;
	}

	
	public readonly int code;

	
	[JsonConverter(typeof(StringEnumConverter))]
	public readonly CharacterVoiceType useCase;

	
	public readonly bool battlePlaying;

	
	public readonly int globalCoolTime;

	
	public readonly int useCaseMinCoolTime;

	
	public readonly int useCaseMaxCoolTime;

	
	public readonly bool onceRepeatIgnore;

	
	public readonly int maxCount;

	
	public readonly bool spatial3D;

	
	[JsonConverter(typeof(StringEnumConverter))]
	public readonly ListenerType listener;

	
	public readonly bool immediatelyPlay;

	
	public readonly string soundName;
}
