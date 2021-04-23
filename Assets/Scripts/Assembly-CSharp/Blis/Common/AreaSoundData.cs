using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blis.Common
{
	public class AreaSoundData
	{
		public readonly string AbmiSoundGroupRoom;


		public readonly string AmbiSoundGroupDay;


		public readonly string AmbiSoundGroupNight;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly DefaultMapAreaType area;


		public readonly string BGMSound;


		public readonly int code;

		[JsonConstructor]
		public AreaSoundData(int code, DefaultMapAreaType area, string bgmSound, string abmiSoundGroupRoom,
			string ambiSoundGroupDay, string ambiSoundGroupNight)
		{
			this.code = code;
			this.area = area;
			BGMSound = bgmSound;
			AbmiSoundGroupRoom = abmiSoundGroupRoom;
			AmbiSoundGroupDay = ambiSoundGroupDay;
			AmbiSoundGroupNight = ambiSoundGroupNight;
		}
	}
}