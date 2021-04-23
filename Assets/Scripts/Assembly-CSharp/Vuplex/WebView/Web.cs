using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Vuplex.WebView
{
	
	public static class Web
	{
		
		public static void ClearAllData()
		{
			Web._pluginFactory.GetPlugin().ClearAllData();
		}

		
		public static Task<Material> CreateMaterial()
		{
			TaskCompletionSource<Material> taskCompletionSource = new TaskCompletionSource<Material>();
			Web.CreateMaterial(new Action<Material>(taskCompletionSource.SetResult));
			return taskCompletionSource.Task;
		}

		
		public static void CreateMaterial(Action<Material> callback)
		{
			Web._pluginFactory.GetPlugin().CreateMaterial(callback);
		}

		
		public static void CreateVideoMaterial(Action<Material> callback)
		{
			Web._pluginFactory.GetPlugin().CreateVideoMaterial(callback);
		}

		
		public static Task<Texture2D> CreateTexture(float width, float height)
		{
			TaskCompletionSource<Texture2D> taskCompletionSource = new TaskCompletionSource<Texture2D>();
			Web.CreateTexture(width, height, new Action<Texture2D>(taskCompletionSource.SetResult));
			return taskCompletionSource.Task;
		}

		
		public static void CreateTexture(float width, float height, Action<Texture2D> callback)
		{
			Web._pluginFactory.GetPlugin().CreateTexture(width, height, callback);
		}

		
		public static IWebView CreateWebView()
		{
			return Web._pluginFactory.GetPlugin().CreateWebView();
		}

		
		public static IWebView CreateWebView(WebPluginType[] preferredPlugins)
		{
			return Web._pluginFactory.GetPlugin(preferredPlugins).CreateWebView();
		}

		
		public static void SetIgnoreCertificateErrors(bool ignore)
		{
			Web._pluginFactory.GetPlugin().SetIgnoreCertificateErrors(ignore);
		}

		
		public static void SetTouchScreenKeyboardEnabled(bool enabled)
		{
			IPluginWithTouchScreenKeyboard pluginWithTouchScreenKeyboard = Web._pluginFactory.GetPlugin() as IPluginWithTouchScreenKeyboard;
			if (pluginWithTouchScreenKeyboard != null)
			{
				pluginWithTouchScreenKeyboard.SetTouchScreenKeyboardEnabled(enabled);
			}
		}

		
		public static void SetStorageEnabled(bool enabled)
		{
			Web._pluginFactory.GetPlugin().SetStorageEnabled(enabled);
		}

		
		public static void SetUserAgent(bool mobile)
		{
			Web._pluginFactory.GetPlugin().SetUserAgent(mobile);
		}

		
		public static void SetUserAgent(string userAgent)
		{
			Web._pluginFactory.GetPlugin().SetUserAgent(userAgent);
		}

		
		internal static void SetPluginFactory(WebPluginFactory pluginFactory)
		{
			Web._pluginFactory = pluginFactory;
		}

		
		private static WebPluginFactory _pluginFactory = new WebPluginFactory();
	}
}
