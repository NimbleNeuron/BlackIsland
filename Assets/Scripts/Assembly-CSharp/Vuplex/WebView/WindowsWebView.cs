using System;
using System.Collections;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;
using UnityEngine.Rendering;

namespace Vuplex.WebView
{
	
	public class WindowsWebView : StandaloneWebView, IWebView
	{
		
		
		public WebPluginType PluginType
		{
			get
			{
				return WebPluginType.Windows;
			}
		}

		
		public static WindowsWebView Instantiate()
		{
			return new GameObject().AddComponent<WindowsWebView>();
		}

		
		public override void Dispose()
		{
			WindowsWebView.WebView_removePointer(this._nativeWebViewPtr);
			base.Dispose();
		}

		
		public static bool ValidateGraphicsApi()
		{
			bool flag = SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D11;
			if (!flag)
			{
				Debug.LogError("Unsupported graphics API: 3D WebView for Windows requires Direct3D11. Please go to Player Settings and set \"Graphics APIs for Windows\" to Direct3D11.");
			}
			return flag;
		}

		
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void _initializeWindowsPlugin()
		{
			IntPtr functionPointerForDelegate = Marshal.GetFunctionPointerForDelegate<WindowsWebView.LogFunction>(new WindowsWebView.LogFunction(WindowsWebView._logInfo));
			IntPtr functionPointerForDelegate2 = Marshal.GetFunctionPointerForDelegate<WindowsWebView.LogFunction>(new WindowsWebView.LogFunction(WindowsWebView._logWarning));
			IntPtr functionPointerForDelegate3 = Marshal.GetFunctionPointerForDelegate<WindowsWebView.LogFunction>(new WindowsWebView.LogFunction(WindowsWebView._logError));
			WindowsWebView.WebView_setLogFunctions(functionPointerForDelegate, functionPointerForDelegate2, functionPointerForDelegate3);
			WindowsWebView.WebView_logVersionInfo();
		}

		
		protected override StandaloneWebView _instantiate()
		{
			return WindowsWebView.Instantiate();
		}

		
		[MonoPInvokeCallback(typeof(WindowsWebView.LogFunction))]
		private static void _logInfo(string message)
		{
			Debug.Log(message);
		}

		
		[MonoPInvokeCallback(typeof(WindowsWebView.LogFunction))]
		private static void _logWarning(string message)
		{
			Debug.LogWarning(message);
		}

		
		[MonoPInvokeCallback(typeof(WindowsWebView.LogFunction))]
		private static void _logError(string message)
		{
			Debug.LogError(message);
		}

		
		private void OnEnable()
		{
			base.StartCoroutine(this._renderPluginOncePerFrame());
		}

		
		private IEnumerator _renderPluginOncePerFrame()
		{
			for (;;)
			{
				if (Application.isBatchMode)
				{
					yield return null;
				}
				else
				{
					yield return new WaitForEndOfFrame();
				}
				if (this._nativeWebViewPtr != IntPtr.Zero && !base.IsDisposed)
				{
					int eventID = WindowsWebView.WebView_depositPointer(this._nativeWebViewPtr);
					GL.IssuePluginEvent(WindowsWebView.WebView_getRenderFunction(), eventID);
				}
			}
		}

		
		[DllImport("VuplexWebViewWindows")]
		private static extern int WebView_depositPointer(IntPtr pointer);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern IntPtr WebView_getRenderFunction();

		
		[DllImport("VuplexWebViewWindows")]
		private static extern void WebView_logVersionInfo();

		
		[DllImport("VuplexWebViewWindows")]
		private static extern void WebView_removePointer(IntPtr pointer);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern int WebView_setLogFunctions(IntPtr logInfoFunction, IntPtr logWarningFunction, IntPtr logErrorFunction);

		
		private delegate void LogFunction(string message);
	}
}
