using Newtonsoft.Json;

namespace Blis.Common
{
	public class EffectAndSoundData
	{
		public readonly bool attachParent;


		public readonly bool childSound;


		public readonly int code;


		public readonly string effectParentName;


		public readonly string effectPrefabName;


		public readonly bool loop;


		public readonly int soundMaxDistance;


		public readonly string soundMixer;


		public readonly string soundName;


		public readonly string soundTag;

		[JsonConstructor]
		public EffectAndSoundData(int code, string effectPrefabName, bool attachParent, string effectParentName,
			string soundMixer, string soundName, int soundMaxDistance, bool childSound)
		{
			this.code = code;
			this.effectPrefabName = effectPrefabName;
			this.effectParentName = effectParentName;
			this.attachParent = attachParent;
			this.soundMixer = soundMixer;
			this.soundName = soundName;
			soundTag = "";
			loop = false;
			this.soundMaxDistance = soundMaxDistance;
			this.childSound = childSound;
		}
	}
}