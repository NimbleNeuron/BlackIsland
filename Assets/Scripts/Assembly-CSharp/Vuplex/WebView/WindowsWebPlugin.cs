using UnityEngine;

namespace Vuplex.WebView
{
	
	internal class WindowsWebPlugin : StandaloneWebPlugin, IWebPlugin
	{
		
		
		public static WindowsWebPlugin Instance
		{
			get
			{
				if (WindowsWebPlugin._instance == null)
				{
					WindowsWebPlugin._instance = new GameObject("WindowsWebPlugin").AddComponent<WindowsWebPlugin>();
					UnityEngine.Object.DontDestroyOnLoad(WindowsWebPlugin._instance.gameObject);
				}
				return WindowsWebPlugin._instance;
			}
		}

		
		public virtual IWebView CreateWebView()
		{
			return WindowsWebView.Instantiate();
		}

		
		private void OnValidate()
		{
			WindowsWebView.ValidateGraphicsApi();
		}

		
		private static WindowsWebPlugin _instance;
	}
}
