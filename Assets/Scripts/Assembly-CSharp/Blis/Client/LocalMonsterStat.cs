using Blis.Common;

namespace Blis.Client
{
	public class LocalMonsterStat : LocalCharacterStat
	{
		public override float SightRange => GetValue(StatType.SightRange, false);
		public override float SightRangeRatio => GetValue(StatType.SightRangeRatio, false);
		public override int SightAngle => GetIntValue(StatType.SightAngle, false);
		public override float AttackRange => GetValue(StatType.AttackRange, false);
	}
}