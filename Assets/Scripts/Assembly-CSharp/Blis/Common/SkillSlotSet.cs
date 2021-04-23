using System;

namespace Blis.Common
{
	[Flags]
	public enum SkillSlotSet
	{
		None = 0,

		Attack_1 = 1,

		Attack_2 = 2,

		Attack_3 = 4,

		Attack_4 = 8,

		Attack_5 = 16,

		Passive_1 = 32,

		Passive_2 = 64,

		Passive_3 = 128,

		Active1_1 = 256,

		Active1_2 = 512,

		Active1_3 = 1024,

		Active1_4 = 2048,

		Active1_5 = 4096,

		Active1_6 = 8192,

		Active2_1 = 16384,

		Active2_2 = 32768,

		Active2_3 = 65536,

		Active2_4 = 131072,

		Active2_5 = 262144,

		Active3_1 = 524288,

		Active3_2 = 1048576,

		Active3_3 = 2097152,

		Active3_4 = 4194304,

		Active3_5 = 8388608,

		Active4_1 = 16777216,

		Active4_2 = 33554432,

		Active4_3 = 67108864,

		Active4_4 = 134217728,

		Active4_5 = 268435456,

		WeaponSkill = 1073741824,

		SpecialSkill = -2147483648
	}
}