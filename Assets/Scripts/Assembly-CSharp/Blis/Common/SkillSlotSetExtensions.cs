using System;

namespace Blis.Common
{
	
	public static class SkillSlotSetExtensions
	{
		
		public static bool IsValidRange(this SkillSlotSet skillSlotSet, SkillSlotIndex skillSlotIndex)
		{
			return skillSlotSet.SlotSet2Index() == skillSlotIndex;
		}

		
		public static bool IsNormalAttack(this SkillSlotSet skillSlotSet)
		{
			if (skillSlotSet <= SkillSlotSet.Attack_3)
			{
				if (skillSlotSet - SkillSlotSet.Attack_1 > 1 && skillSlotSet != SkillSlotSet.Attack_3)
				{
					return false;
				}
			}
			else if (skillSlotSet != SkillSlotSet.Attack_4 && skillSlotSet != SkillSlotSet.Attack_5)
			{
				return false;
			}

			return true;
		}

		
		public static bool IsActiveSkill(this SkillSlotSet skillSlotSet)
		{
			SkillSlotIndex skillSlotIndex = skillSlotSet.SlotSet2Index();
			return skillSlotIndex - SkillSlotIndex.Active1 <= 3;
		}

		
		public static SkillSlotIndex SlotSet2Index(this SkillSlotSet skillSlotSet)
		{
			if (skillSlotSet <= SkillSlotSet.Active2_1)
			{
				if (skillSlotSet <= SkillSlotSet.Passive_2)
				{
					if (skillSlotSet <= SkillSlotSet.Attack_3)
					{
						if (skillSlotSet == SkillSlotSet.SpecialSkill)
						{
							return SkillSlotIndex.SpecialSkill;
						}

						if (skillSlotSet - SkillSlotSet.Attack_1 > 1 && skillSlotSet != SkillSlotSet.Attack_3)
						{
							goto IL_1BE;
						}
					}
					else if (skillSlotSet <= SkillSlotSet.Attack_5)
					{
						if (skillSlotSet != SkillSlotSet.Attack_4 && skillSlotSet != SkillSlotSet.Attack_5)
						{
							goto IL_1BE;
						}
					}
					else
					{
						if (skillSlotSet != SkillSlotSet.Passive_1 && skillSlotSet != SkillSlotSet.Passive_2)
						{
							goto IL_1BE;
						}

						return SkillSlotIndex.Passive;
					}

					return SkillSlotIndex.Attack;
				}

				if (skillSlotSet <= SkillSlotSet.Active1_3)
				{
					if (skillSlotSet <= SkillSlotSet.Active1_1)
					{
						if (skillSlotSet == SkillSlotSet.Passive_3)
						{
							return SkillSlotIndex.Passive;
						}

						if (skillSlotSet != SkillSlotSet.Active1_1)
						{
							goto IL_1BE;
						}
					}
					else if (skillSlotSet != SkillSlotSet.Active1_2 && skillSlotSet != SkillSlotSet.Active1_3)
					{
						goto IL_1BE;
					}
				}
				else if (skillSlotSet <= SkillSlotSet.Active1_5)
				{
					if (skillSlotSet != SkillSlotSet.Active1_4 && skillSlotSet != SkillSlotSet.Active1_5)
					{
						goto IL_1BE;
					}
				}
				else if (skillSlotSet != SkillSlotSet.Active1_6)
				{
					if (skillSlotSet != SkillSlotSet.Active2_1)
					{
						goto IL_1BE;
					}

					return SkillSlotIndex.Active2;
				}

				return SkillSlotIndex.Active1;
			}

			if (skillSlotSet > SkillSlotSet.Active3_3)
			{
				if (skillSlotSet <= SkillSlotSet.Active4_2)
				{
					if (skillSlotSet <= SkillSlotSet.Active3_5)
					{
						if (skillSlotSet != SkillSlotSet.Active3_4 && skillSlotSet != SkillSlotSet.Active3_5)
						{
							goto IL_1BE;
						}

						return SkillSlotIndex.Active3;
					}

					if (skillSlotSet != SkillSlotSet.Active4_1 && skillSlotSet != SkillSlotSet.Active4_2)
					{
						goto IL_1BE;
					}
				}
				else if (skillSlotSet <= SkillSlotSet.Active4_4)
				{
					if (skillSlotSet != SkillSlotSet.Active4_3 && skillSlotSet != SkillSlotSet.Active4_4)
					{
						goto IL_1BE;
					}
				}
				else if (skillSlotSet != SkillSlotSet.Active4_5)
				{
					if (skillSlotSet != SkillSlotSet.WeaponSkill)
					{
						goto IL_1BE;
					}

					return SkillSlotIndex.WeaponSkill;
				}

				return SkillSlotIndex.Active4;
			}

			if (skillSlotSet <= SkillSlotSet.Active2_4)
			{
				if (skillSlotSet != SkillSlotSet.Active2_2 && skillSlotSet != SkillSlotSet.Active2_3 &&
				    skillSlotSet != SkillSlotSet.Active2_4)
				{
					goto IL_1BE;
				}

				return SkillSlotIndex.Active2;
			}

			if (skillSlotSet <= SkillSlotSet.Active3_1)
			{
				if (skillSlotSet == SkillSlotSet.Active2_5)
				{
					return SkillSlotIndex.Active2;
				}

				if (skillSlotSet != SkillSlotSet.Active3_1)
				{
					goto IL_1BE;
				}
			}
			else if (skillSlotSet != SkillSlotSet.Active3_2 && skillSlotSet != SkillSlotSet.Active3_3)
			{
				goto IL_1BE;
			}

			return SkillSlotIndex.Active3;
			IL_1BE:
			Exception ex = new Exception("[SlotSet2Index] Wrong Param : " + skillSlotSet);
			Log.Exception(ex);
			throw ex;
		}
	}
}