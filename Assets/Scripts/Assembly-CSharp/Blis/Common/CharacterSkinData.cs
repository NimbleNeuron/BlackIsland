using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blis.Common
{
	public class CharacterSkinData
	{
		public readonly int characterCode;


		public readonly int code;


		public readonly string effectsPath;


		public readonly string fxSoundPath;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly SkinGrade grade;


		public readonly int index;


		public readonly string objectPath;


		public readonly string projectilesPath;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly SkinPurchaseType purchaseType;


		public readonly string voicePath;


		public readonly string weaponMountCommonPath;


		public readonly string weaponMountPath;


		[JsonConstructor]
		public CharacterSkinData(int code, int characterCode, int index, SkinGrade grade, SkinPurchaseType purchaseType,
			string effectsPath, string projectilesPath, string objectPath, string fxSoundPath, string voicePath,
			string weaponMountPath, string weaponMountCommonPath)
		{
			this.code = code;
			this.characterCode = characterCode;
			this.index = index;
			this.grade = grade;
			this.purchaseType = purchaseType;
			this.effectsPath = effectsPath;
			this.projectilesPath = projectilesPath;
			this.objectPath = objectPath;
			this.fxSoundPath = fxSoundPath;
			this.voicePath = voicePath;
			this.weaponMountPath = weaponMountPath;
			this.weaponMountCommonPath = weaponMountCommonPath;
			SkinSuffix = string.Format("S{0:D3}", index);
		}


		[JsonIgnore] public string SkinSuffix { get; }
	}
}