using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blis.Common
{
	
	public class WeaponAnimatorLayersData
	{
		
		[JsonConstructor]
		public WeaponAnimatorLayersData(int code, int characterCode, WeaponType weaponType, string layerName, float layerWeight)
		{
			this.code = code;
			this.characterCode = characterCode;
			this.weaponType = weaponType;
			this.layerName = layerName;
			this.layerWeight = layerWeight;
		}

		
		public readonly int code;

		
		public readonly int characterCode;

		
		[JsonConverter(typeof(StringEnumConverter))]
		public readonly WeaponType weaponType;

		
		public readonly string layerName;

		
		public readonly float layerWeight;
	}
}
