using MessagePack;

namespace Blis.Common
{
	[MessagePackObject()]
	public class MasteryValue
	{
		[Key(4)] public readonly int AddExp;


		[Key(2)] public readonly int MasteryExp;


		[Key(1)] public readonly int MasteryLevel;


		[Key(0)] public readonly MasteryType MasteryType;


		[Key(3)] public readonly int WeaponSkillPoint;

		[SerializationConstructor]
		public MasteryValue(MasteryType masteryType, int level, int exp, int weaponSkillPoint, int addExp)
		{
			MasteryType = masteryType;
			MasteryLevel = level;
			MasteryExp = exp;
			WeaponSkillPoint = weaponSkillPoint;
			AddExp = addExp;
		}
	}
}