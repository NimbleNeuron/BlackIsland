using System;
using UnityEngine;

namespace Vuplex.WebView
{
	
	[Serializable]
	internal class StringBridgeMessage : BridgeMessage
	{
		
		public static string ParseValue(string serializedMessage)
		{
			return JsonUtility.FromJson<StringBridgeMessage>(serializedMessage).value;
		}

		
		public string value;
	}
}
