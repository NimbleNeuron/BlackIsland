using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blis.Common
{
	public class WeaponMountData
	{
		public readonly string animationController;


		public readonly int characterCode;


		public readonly int code;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly WeaponMountType mountType;


		public readonly string prefab;


		public readonly float scale;


		public readonly int skinIndex;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly WeaponType weaponType;

		[JsonConstructor]
		public WeaponMountData(int code, int characterCode, int skinIndex, WeaponType weaponType, string prefab,
			string animationController, WeaponMountType mountType, float scale)
		{
			this.code = code;
			this.characterCode = characterCode;
			this.skinIndex = skinIndex;
			this.weaponType = weaponType;
			this.prefab = prefab;
			this.animationController = animationController;
			this.mountType = mountType;
			this.scale = scale;
		}
	}
}