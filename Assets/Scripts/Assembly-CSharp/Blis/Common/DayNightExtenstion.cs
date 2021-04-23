using UnityEngine;

namespace Blis.Common
{
	public static class DayNightExtenstion
	{
		public static DayNight Next(this DayNight dayNight)
		{
			if (dayNight == DayNight.Day)
			{
				return DayNight.Night;
			}

			return DayNight.Day;
		}


		public static Sprite GetSprite(this DayNight dayNight)
		{
			if (dayNight == DayNight.Day)
			{
				return SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Ico_DaySun");
			}

			if (dayNight != DayNight.Night)
			{
				return null;
			}

			return SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Ico_NightMoon");
		}


		public static string GetSoundGroupName(this DayNight dayNight)
		{
			if (dayNight == DayNight.Day)
			{
				return "Ambie_A_Day";
			}

			if (dayNight != DayNight.Night)
			{
				return "";
			}

			return "Ambie_A_Night";
		}
	}
}