using System.Collections.Generic;
using UnityEngine;

namespace Blis.Common
{
	public class AuthToken_Guest : AuthToken
	{
		public readonly string deviceId;


		public AuthToken_Guest()
		{
			deviceId = SystemInfo.deviceUniqueIdentifier;
		}


		public string GetPlayerId()
		{
			return deviceId;
		}


		public Dictionary<string, string> GetAttributesMap()
		{
			return new Dictionary<string, string>
			{
				{
					"authorizationCode",
					deviceId
				}
			};
		}
	}
}