using System;
using UnityEngine;

namespace Vuplex.WebView
{
	
	[Serializable]
	public class BridgeMessage
	{
		
		public static string ParseType(string serializedMessage)
		{
			string result;
			try
			{
				result = JsonUtility.FromJson<BridgeMessage>(serializedMessage).type;
			}
			catch (Exception)
			{
				result = null;
			}
			return result;
		}

		
		public string type;
	}
}
