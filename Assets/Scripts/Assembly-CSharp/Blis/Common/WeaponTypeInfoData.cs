using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blis.Common
{
	
	public class WeaponTypeInfoData
	{
		
		[JsonConstructor]
		public WeaponTypeInfoData(WeaponType type, float attackSpeed, float attackRange, int shopFilter, int summonObjectHitDamage)
		{
			this.type = type;
			this.attackSpeed = attackSpeed;
			this.attackRange = attackRange;
			this.shopFilter = shopFilter;
			this.summonObjectHitDamage = summonObjectHitDamage;
		}

		
		[JsonConverter(typeof(StringEnumConverter))]
		public readonly WeaponType type;

		
		public readonly float attackSpeed;

		
		public readonly float attackRange;

		
		public readonly int shopFilter;

		
		public readonly int summonObjectHitDamage;
	}
}
