using System;

namespace Blis.Server
{
	
	public static class DeployRegionEx
	{
		
		public static DeploySetting FromString(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return DeploySetting.Local;
			}

			string text = value.ToLower();
			foreach (object obj in Enum.GetValues(typeof(DeploySetting)))
			{
				DeploySetting result = (DeploySetting) obj;
				if (text.Equals(result.ToString().ToLower()))
				{
					return result;
				}
			}

			return DeploySetting.Local;
		}
	}
}