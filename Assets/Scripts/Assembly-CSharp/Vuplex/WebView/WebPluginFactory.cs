using UnityEngine;

namespace Vuplex.WebView
{
	
	internal class WebPluginFactory
	{
		
		public virtual IWebPlugin GetPlugin()
		{
			return this.GetPlugin(null);
		}

		
		public virtual IWebPlugin GetPlugin(WebPluginType[] preferredPlugins)
		{
			if (WebPluginFactory._windowsPlugin != null)
			{
				return WebPluginFactory._windowsPlugin;
			}
			throw new WebViewUnavailableException("The 3D WebView for Windows plugin is not currently installed. For more info, please visit https://developer.vuplex.com");
		}

		
		public static void RegisterAndroidPlugin(IWebPlugin plugin)
		{
			WebPluginFactory._androidPlugin = plugin;
		}

		
		public static void RegisterAndroidGeckoPlugin(IWebPlugin plugin)
		{
			WebPluginFactory._androidGeckoPlugin = plugin;
		}

		
		public static void RegisterIOSPlugin(IWebPlugin plugin)
		{
			WebPluginFactory._iosPlugin = plugin;
		}

		
		public static void RegisterMacPlugin(IWebPlugin plugin)
		{
			WebPluginFactory._macPlugin = plugin;
		}

		
		public static void RegisterMockPlugin(IWebPlugin plugin)
		{
			WebPluginFactory._mockPlugin = plugin;
		}

		
		public static void RegisterUwpPlugin(IWebPlugin plugin)
		{
			WebPluginFactory._uwpPlugin = plugin;
		}

		
		public static void RegisterWindowsPlugin(IWebPlugin plugin)
		{
			WebPluginFactory._windowsPlugin = plugin;
		}

		
		private void _logMockWarningOnce(string warning)
		{
			if (!this._mockWarningLogged)
			{
				this._mockWarningLogged = true;
				Debug.LogWarning(warning);
			}
		}

		
		protected static IWebPlugin _androidPlugin;

		
		protected static IWebPlugin _androidGeckoPlugin;

		
		protected static IWebPlugin _iosPlugin;

		
		protected static IWebPlugin _macPlugin;

		
		protected static IWebPlugin _mockPlugin = MockWebPlugin.Instance;

		
		private bool _mockWarningLogged;

		
		private const string MORE_INFO_TEXT = " For more info, please visit https://developer.vuplex.com";

		
		protected static IWebPlugin _uwpPlugin;

		
		protected static IWebPlugin _windowsPlugin;
	}
}
