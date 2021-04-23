using System;

namespace Blis.Common
{
	[Flags]
	public enum ActionCategoryType
	{
		None = 0,


		NoCastAction = 1,


		CastAction = 2,


		ResurrectAction = 4,


		ItemUseAction = 8,


		ItemEquipOrUnequipAction = 16,


		All = -1
	}
}