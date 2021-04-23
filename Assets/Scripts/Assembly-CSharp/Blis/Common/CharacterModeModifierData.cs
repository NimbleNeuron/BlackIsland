using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blis.Common
{
	
	public class CharacterModeModifierData
	{
		
		[JsonConstructor]
		public CharacterModeModifierData(int code, int characterCode, WeaponType weaponType, float soloIncreaseModeDamageRatio, float soloPreventModeDamageRatio, float soloIncreaseModeHealRatio, float soloIncreaseModeShieldRatio, float duoIncreaseModeDamageRatio, float duoPreventModeDamageRatio, float duoIncreaseModeHealRatio, float duoIncreaseModeShieldRatio, float squadIncreaseModeDamageRatio, float squadPreventModeDamageRatio, float squadIncreaseModeHealRatio, float squadIncreaseModeShieldRatio)
		{
			this.code = code;
			this.characterCode = characterCode;
			this.weaponType = weaponType;
			this.soloIncreaseModeDamageRatio = soloIncreaseModeDamageRatio;
			this.soloPreventModeDamageRatio = soloPreventModeDamageRatio;
			this.soloIncreaseModeHealRatio = soloIncreaseModeHealRatio;
			this.soloIncreaseModeShieldRatio = soloIncreaseModeShieldRatio;
			this.duoIncreaseModeDamageRatio = duoIncreaseModeDamageRatio;
			this.duoPreventModeDamageRatio = duoPreventModeDamageRatio;
			this.duoIncreaseModeHealRatio = duoIncreaseModeHealRatio;
			this.duoIncreaseModeShieldRatio = duoIncreaseModeShieldRatio;
			this.squadIncreaseModeDamageRatio = squadIncreaseModeDamageRatio;
			this.squadPreventModeDamageRatio = squadPreventModeDamageRatio;
			this.squadIncreaseModeHealRatio = squadIncreaseModeHealRatio;
			this.squadIncreaseModeShieldRatio = squadIncreaseModeShieldRatio;
		}

		
		public bool AnySololModifier()
		{
			return this.soloIncreaseModeDamageRatio == 0f && this.soloPreventModeDamageRatio == 0f && this.soloIncreaseModeHealRatio == 0f && this.soloIncreaseModeShieldRatio == 0f;
		}

		
		public bool AnyDuoModifier()
		{
			return this.duoIncreaseModeDamageRatio == 0f && this.duoPreventModeDamageRatio == 0f && this.duoIncreaseModeHealRatio == 0f && this.duoIncreaseModeShieldRatio == 0f;
		}

		
		public bool AnyTrioModifier()
		{
			return this.squadIncreaseModeDamageRatio == 0f && this.squadPreventModeDamageRatio == 0f && this.squadIncreaseModeHealRatio == 0f && this.squadIncreaseModeShieldRatio == 0f;
		}

		
		public readonly int code;

		
		public readonly int characterCode;

		
		[JsonConverter(typeof(StringEnumConverter))]
		public readonly WeaponType weaponType;

		
		public readonly float soloIncreaseModeDamageRatio;

		
		public readonly float soloPreventModeDamageRatio;

		
		public readonly float soloIncreaseModeHealRatio;

		
		public readonly float soloIncreaseModeShieldRatio;

		
		public readonly float duoIncreaseModeDamageRatio;

		
		public readonly float duoPreventModeDamageRatio;

		
		public readonly float duoIncreaseModeHealRatio;

		
		public readonly float duoIncreaseModeShieldRatio;

		
		public readonly float squadIncreaseModeDamageRatio;

		
		public readonly float squadPreventModeDamageRatio;

		
		public readonly float squadIncreaseModeHealRatio;

		
		public readonly float squadIncreaseModeShieldRatio;
	}
}
