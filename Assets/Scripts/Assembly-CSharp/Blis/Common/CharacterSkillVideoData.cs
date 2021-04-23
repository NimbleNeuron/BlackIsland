using Newtonsoft.Json;

namespace Blis.Common
{
	
	public class CharacterSkillVideoData
	{
		
		public readonly int code;

		
		public readonly string otherPlatFormUrl;

		
		public readonly float volume;

		
		public readonly string youTubeUrl;

		
		[JsonConstructor]
		public CharacterSkillVideoData(int code, string youTubeUrl, string otherPlatFormUrl, float volume)
		{
			this.code = code;
			this.youTubeUrl = youTubeUrl;
			this.otherPlatFormUrl = otherPlatFormUrl;
			this.volume = volume;
		}
	}
}