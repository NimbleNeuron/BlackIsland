using UnityEngine;

namespace Vuplex.WebView
{
	
	internal class MacWebPluginRegistrant
	{
		
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void _registerPlugin()
		{
		}
	}
}
