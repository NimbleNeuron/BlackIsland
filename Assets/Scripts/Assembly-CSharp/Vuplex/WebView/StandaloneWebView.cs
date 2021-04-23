using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using UnityEngine;

namespace Vuplex.WebView
{
	
	public abstract class StandaloneWebView : BaseWebView, IWithKeyDownAndUp, IWithKeyModifiers, IWithMovablePointer, IWithPointerDownAndUp, IWithPopups
	{
		
		
		
		public event EventHandler<AuthRequestedEventArgs> AuthRequested
		{
			add
			{
				if (this._authRequestedHandler != null)
				{
					throw new InvalidOperationException("AuthRequested supports only one event handler. Please remove the existing handler before adding a new one.");
				}
				this._authRequestedHandler = value;
				StandaloneWebView.WebView_setAuthEnabled(this._nativeWebViewPtr, true);
			}
			remove
			{
				if (this._authRequestedHandler == value)
				{
					this._authRequestedHandler = null;
					StandaloneWebView.WebView_setAuthEnabled(this._nativeWebViewPtr, false);
				}
			}
		}

		
		
		
		public event EventHandler<PopupRequestedEventArgs> PopupRequested;

		
		public new static void ClearAllData()
		{
			if (StandaloneWebView.WebView_pluginIsInitialized())
			{
				StandaloneWebView._throwAlreadyInitializedException("clear the browser data", "ClearAllData");
			}
			string path = StandaloneWebView._getCachePath();
			if (Directory.Exists(path))
			{
				Directory.Delete(path, true);
			}
		}

		
		public override void Copy()
		{
			base._assertValidState();
			StandaloneWebView.WebView_copy(this._nativeWebViewPtr);
		}

		
		public override void Cut()
		{
			base._assertValidState();
			StandaloneWebView.WebView_cut(this._nativeWebViewPtr);
		}

		
		public static void EnableRemoteDebugging(int portNumber)
		{
			if (1024 > portNumber || portNumber > 65535)
			{
				throw new ArgumentException(string.Format("The given port number ({0}) is not in the range from 1024 to 65535.", portNumber));
			}
			if (!StandaloneWebView.WebView_enableRemoteDebugging(portNumber))
			{
				StandaloneWebView._throwAlreadyInitializedException("enable remote debugging", "EnableRemoteDebugging");
			}
		}

		
		public static Task<Cookie> GetCookie(string url, string cookieName)
		{
			TaskCompletionSource<Cookie> taskCompletionSource = new TaskCompletionSource<Cookie>();
			StandaloneWebView.GetCookie(url, cookieName, new Action<Cookie>(taskCompletionSource.SetResult));
			return taskCompletionSource.Task;
		}

		
		public static void GetCookie(string url, string cookieName, Action<Cookie> callback)
		{
			if (!StandaloneWebView.WebView_pluginIsInitialized())
			{
				throw new InvalidOperationException("On Windows and macOS, GetCookie() can only be called when the Chromium process is running (i.e. after a webview is initialized).");
			}
			string text = Guid.NewGuid().ToString();
			StandaloneWebView._pendingGetCookieResultCallbacks[text] = callback;
			StandaloneWebView.WebView_getCookie(url, cookieName, text);
		}

		
		[Obsolete("The IWithKeyModifiers interface is now deprecated. Please use the IWithKeyDownAndUp interface instead.")]
		public void HandleKeyboardInput(string key, KeyModifier modifiers)
		{
			this.KeyDown(key, modifiers);
			this.KeyUp(key, modifiers);
		}

		
		public override void Init(Texture2D viewportTexture, float width, float height, Texture2D videoTexture)
		{
			base.Init(viewportTexture, width, height, videoTexture);
			this._nativeWebViewPtr = StandaloneWebView.WebView_new(base.gameObject.name, base._nativeWidth, base._nativeHeight, null);
			if (this._nativeWebViewPtr == IntPtr.Zero)
			{
				throw new WebViewUnavailableException("Failed to instantiate a new webview. This could indicate that you're using an expired trial version of 3D WebView.");
			}
		}

		
		public void KeyDown(string key, KeyModifier modifiers)
		{
			base._assertValidState();
			StandaloneWebView.WebView_keyDown(this._nativeWebViewPtr, key, (int)modifiers);
		}

		
		public void KeyUp(string key, KeyModifier modifiers)
		{
			base._assertValidState();
			StandaloneWebView.WebView_keyUp(this._nativeWebViewPtr, key, (int)modifiers);
		}

		
		public void MovePointer(Vector2 point)
		{
			base._assertValidState();
			int x = (int)(point.x * (float)base._nativeWidth);
			int y = (int)(point.y * (float)base._nativeHeight);
			StandaloneWebView.WebView_movePointer(this._nativeWebViewPtr, x, y);
		}

		
		public override void Paste()
		{
			base._assertValidState();
			StandaloneWebView.WebView_paste(this._nativeWebViewPtr);
		}

		
		public void PointerDown(Vector2 point)
		{
			this._pointerDown(point, MouseButton.Left, 1);
		}

		
		public void PointerDown(Vector2 point, PointerOptions options)
		{
			if (options == null)
			{
				options = new PointerOptions();
			}
			this._pointerDown(point, options.Button, options.ClickCount);
		}

		
		public void PointerUp(Vector2 point)
		{
			this._pointerUp(point, MouseButton.Left, 1);
		}

		
		public void PointerUp(Vector2 point, PointerOptions options)
		{
			if (options == null)
			{
				options = new PointerOptions();
			}
			this._pointerUp(point, options.Button, options.ClickCount);
		}

		
		public override void SelectAll()
		{
			base._assertValidState();
			StandaloneWebView.WebView_selectAll(this._nativeWebViewPtr);
		}

		
		public static void SetAudioAndVideoCaptureEnabled(bool enabled)
		{
			if (!StandaloneWebView.WebView_setAudioAndVideoCaptureEnabled(enabled))
			{
				StandaloneWebView._throwAlreadyInitializedException("enable audio and video capture", "SetAudioAndVideoCaptureEnabled");
			}
		}

		
		public static void SetCommandLineArguments(string args)
		{
			if (!StandaloneWebView.WebView_setCommandLineArguments(args))
			{
				StandaloneWebView._throwAlreadyInitializedException("set command line arguments", "SetCommandLineArguments");
			}
		}

		
		public static Task<bool> SetCookie(Cookie cookie)
		{
			TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
			StandaloneWebView.SetCookie(cookie, new Action<bool>(taskCompletionSource.SetResult));
			return taskCompletionSource.Task;
		}

		
		public static void SetCookie(Cookie cookie, Action<bool> callback)
		{
			if (!StandaloneWebView.WebView_pluginIsInitialized())
			{
				throw new InvalidOperationException("On Windows and macOS, SetCookie() can only be called when the Chromium process is running (i.e. after a webview is initialized).");
			}
			if (cookie == null)
			{
				throw new ArgumentException("Cookie cannot be null.");
			}
			if (!cookie.IsValid)
			{
				throw new ArgumentException("Cannot set invalid cookie: " + cookie);
			}
			string text = Guid.NewGuid().ToString();
			StandaloneWebView._pendingSetCookieResultCallbacks[text] = callback;
			StandaloneWebView.WebView_setCookie(cookie.ToJson(), text);
		}

		
		public static void SetIgnoreCertificateErrors(bool ignore)
		{
			if (!StandaloneWebView.WebView_setIgnoreCertificateErrors(ignore))
			{
				StandaloneWebView._throwAlreadyInitializedException("ignore certificate errors", "SetIgnoreCertificateErrors");
			}
		}

		
		public void SetNativeFileDialogEnabled(bool enabled)
		{
			base._assertValidState();
			StandaloneWebView.WebView_setNativeFileDialogEnabled(this._nativeWebViewPtr, enabled);
		}

		
		public void SetPopupMode(PopupMode popupMode)
		{
			StandaloneWebView.WebView_setPopupMode(this._nativeWebViewPtr, (int)popupMode);
		}

		
		public static void SetScreenSharingEnabled(bool enabled)
		{
			if (!StandaloneWebView.WebView_setScreenSharingEnabled(enabled))
			{
				StandaloneWebView._throwAlreadyInitializedException("enable or disable screen sharing", "SetScreenSharingEnabled");
			}
		}

		
		public new static void SetStorageEnabled(bool enabled)
		{
			if (!StandaloneWebView.WebView_setCachePath(enabled ? StandaloneWebView._getCachePath() : ""))
			{
				StandaloneWebView._throwAlreadyInitializedException("enable or disable storage", "SetStorageEnabled");
			}
		}

		
		public static void SetTargetFrameRate(uint targetFrameRate)
		{
			if (!StandaloneWebView.WebView_setTargetFrameRate(targetFrameRate))
			{
				StandaloneWebView._throwAlreadyInitializedException("set the target frame rate", "SetTargetFrameRate");
			}
		}

		
		public void SetZoomLevel(float zoomLevel)
		{
			base._assertValidState();
			StandaloneWebView.WebView_setZoomLevel(this._nativeWebViewPtr, zoomLevel);
		}

		
		public static void TerminatePlugin()
		{
			StandaloneWebView.WebView_terminatePlugin();
		}

		
		protected static string _getCachePath()
		{
			return Path.Combine(Application.persistentDataPath, Path.Combine("Vuplex.WebView", "chromium-cache"));
		}

		
		private void HandleAuthRequested(string host)
		{
			EventHandler<AuthRequestedEventArgs> authRequestedHandler = this._authRequestedHandler;
			if (authRequestedHandler == null)
			{
				Debug.LogWarning("The native webview sent an auth request, but no event handler is attached to AuthRequested.");
				StandaloneWebView.WebView_cancelAuth(this._nativeWebViewPtr);
				return;
			}
			AuthRequestedEventArgs e = new AuthRequestedEventArgs(host, delegate(string username, string password)
			{
				StandaloneWebView.WebView_continueAuth(this._nativeWebViewPtr, username, password);
			}, delegate()
			{
				StandaloneWebView.WebView_cancelAuth(this._nativeWebViewPtr);
			});
			authRequestedHandler(this, e);
		}

		
		[MonoPInvokeCallback(typeof(StandaloneWebView.GetCookieCallback))]
		private static void _handleGetCookieResult(string resultCallbackId, string serializedCookie)
		{
			Action<Cookie> action = StandaloneWebView._pendingGetCookieResultCallbacks[resultCallbackId];
			StandaloneWebView._pendingGetCookieResultCallbacks.Remove(resultCallbackId);
			if (action != null)
			{
				Cookie obj = Cookie.FromJson(serializedCookie);
				action(obj);
			}
		}

		
		private void HandlePopup(string message)
		{
			EventHandler<PopupRequestedEventArgs> handler = this.PopupRequested;
			if (handler == null)
				return;
			string[] strArray = message.Split(new char[1]{ ',' }, 2);
			string url = strArray[0];
			string popupBrowserId = strArray[1];
			if (popupBrowserId.Length == 0)
			{
				handler((object) this, new PopupRequestedEventArgs(url, (IWebView) null));
			}
			else
			{
				StandaloneWebView popupWebView = this._instantiate();
				Dispatcher.RunOnMainThread((Action) (() => Web.CreateTexture(1f, 1f, (Action<Texture2D>) (texture =>
				{
					popupWebView.SetResolution(this._numberOfPixelsPerUnityUnit);
					popupWebView._initPopup(texture, this._width, this._height, popupBrowserId);
					handler((object) this, new PopupRequestedEventArgs(url, popupWebView as IWebView));
				}))));
			}
			
			// co: dotPeek
			// EventHandler<PopupRequestedEventArgs> handler = this.PopupRequested;
			// if (handler == null)
			// {
			// 	return;
			// }
			// string[] array = message.Split(new char[]
			// {
			// 	','
			// }, 2);
			// string url = array[0];
			// string popupBrowserId = array[1];
			// if (popupBrowserId.Length == 0)
			// {
			// 	handler(this, new PopupRequestedEventArgs(url, null));
			// 	return;
			// }
			// StandaloneWebView popupWebView = this._instantiate();
			// Action<Texture2D> <>9__1;
			// Dispatcher.RunOnMainThread(delegate
			// {
			// 	float width = 1f;
			// 	float height = 1f;
			// 	Action<Texture2D> callback;
			// 	if ((callback = <>9__1) == null)
			// 	{
			// 		callback = (<>9__1 = delegate(Texture2D texture)
			// 		{
			// 			popupWebView.SetResolution(this._numberOfPixelsPerUnityUnit);
			// 			popupWebView._initPopup(texture, this._width, this._height, popupBrowserId);
			// 			handler(this, new PopupRequestedEventArgs(url, popupWebView as IWebView));
			// 		});
			// 	}
			// 	Web.CreateTexture(width, height, callback);
			// });
		}

		
		[MonoPInvokeCallback(typeof(StandaloneWebView.SetCookieCallback))]
		private static void _handleSetCookieResult(string resultCallbackId, bool success)
		{
			Action<bool> action = StandaloneWebView._pendingSetCookieResultCallbacks[resultCallbackId];
			StandaloneWebView._pendingSetCookieResultCallbacks.Remove(resultCallbackId);
			if (action != null)
			{
				action(success);
			}
		}

		
		private void _initPopup(Texture2D viewportTexture, float width, float height, string popupId)
		{
			base.Init(viewportTexture, width, height, null);
			this._nativeWebViewPtr = StandaloneWebView.WebView_new(base.gameObject.name, base._nativeWidth, base._nativeHeight, popupId);
		}

		
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void _initializePlugin()
		{
			StandaloneWebView.WebView_setSendMessageFunction(Marshal.GetFunctionPointerForDelegate<StandaloneWebView.UnitySendMessageFunction>(new StandaloneWebView.UnitySendMessageFunction(StandaloneWebView._unitySendMessage)));
			StandaloneWebView.WebView_setCookieCallbacks(Marshal.GetFunctionPointerForDelegate<StandaloneWebView.GetCookieCallback>(new StandaloneWebView.GetCookieCallback(StandaloneWebView._handleGetCookieResult)), Marshal.GetFunctionPointerForDelegate<StandaloneWebView.SetCookieCallback>(new StandaloneWebView.SetCookieCallback(StandaloneWebView._handleSetCookieResult)));
			StandaloneWebView.SetStorageEnabled(true);
		}

		
		protected abstract StandaloneWebView _instantiate();

		
		private static void _throwAlreadyInitializedException(string action, string methodName)
		{
			throw new InvalidOperationException(string.Format("Unable to {0} because a webview has already been created. On Windows and macOS, {1}() can only be called prior to initializing any webviews.", action, methodName));
		}

		
		private void _pointerDown(Vector2 point, MouseButton mouseButton, int clickCount)
		{
			base._assertValidState();
			int x = (int)(point.x * (float)base._nativeWidth);
			int y = (int)(point.y * (float)base._nativeHeight);
			StandaloneWebView.WebView_pointerDown(this._nativeWebViewPtr, x, y, (int)mouseButton, clickCount);
		}

		
		private void _pointerUp(Vector2 point, MouseButton mouseButton, int clickCount)
		{
			base._assertValidState();
			int x = (int)(point.x * (float)base._nativeWidth);
			int y = (int)(point.y * (float)base._nativeHeight);
			StandaloneWebView.WebView_pointerUp(this._nativeWebViewPtr, x, y, (int)mouseButton, clickCount);
		}

		
		[MonoPInvokeCallback(typeof(StandaloneWebView.UnitySendMessageFunction))]
		private static void _unitySendMessage(string gameObjectName, string methodName, string message)
		{
			Dispatcher.RunOnMainThread(delegate
			{
				GameObject gameObject = GameObject.Find(gameObjectName);
				if (gameObject == null)
				{
					Debug.LogErrorFormat("Unable to send the message, because there is no GameObject named '{0}'", new object[]
					{
						gameObjectName
					});
					return;
				}
				gameObject.SendMessage(methodName, message);
			});
		}

		
		[DllImport("VuplexWebViewWindows")]
		private static extern void WebView_cancelAuth(IntPtr webViewPtr);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern void WebView_continueAuth(IntPtr webViewPtr, string username, string password);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern void WebView_copy(IntPtr webViewPtr);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern void WebView_cut(IntPtr webViewPtr);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern bool WebView_enableRemoteDebugging(int portNumber);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern void WebView_getCookie(string url, string name, string resultCallbackId);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern void WebView_keyDown(IntPtr webViewPtr, string key, int modifiers);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern void WebView_keyUp(IntPtr webViewPtr, string key, int modifiers);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern void WebView_movePointer(IntPtr webViewPtr, int x, int y);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern IntPtr WebView_new(string gameObjectName, int width, int height, string popupBrowserId);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern void WebView_paste(IntPtr webViewPtr);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern bool WebView_pluginIsInitialized();

		
		[DllImport("VuplexWebViewWindows")]
		private static extern void WebView_pointerDown(IntPtr webViewPtr, int x, int y, int mouseButton, int clickCount);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern void WebView_pointerUp(IntPtr webViewPtr, int x, int y, int mouseButton, int clickCount);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern void WebView_selectAll(IntPtr webViewPtr);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern bool WebView_setAudioAndVideoCaptureEnabled(bool enabled);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern void WebView_setAuthEnabled(IntPtr webViewPtr, bool enabled);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern bool WebView_setCachePath(string cachePath);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern bool WebView_setCommandLineArguments(string args);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern void WebView_setCookie(string serializedCookie, string resultCallbackId);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern int WebView_setCookieCallbacks(IntPtr getCookieCallback, IntPtr setCookieCallback);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern bool WebView_setIgnoreCertificateErrors(bool ignore);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern void WebView_setNativeFileDialogEnabled(IntPtr webViewPtr, bool enabled);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern void WebView_setPopupMode(IntPtr webViewPtr, int popupMode);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern bool WebView_setScreenSharingEnabled(bool enabled);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern int WebView_setSendMessageFunction(IntPtr sendMessageFunction);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern bool WebView_setTargetFrameRate(uint targetFrameRate);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern void WebView_setZoomLevel(IntPtr webViewPtr, float zoomLevel);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern void WebView_terminatePlugin();

		
		private EventHandler<AuthRequestedEventArgs> _authRequestedHandler;

		
		private static Dictionary<string, Action<Cookie>> _pendingGetCookieResultCallbacks = new Dictionary<string, Action<Cookie>>();

		
		private static Dictionary<string, Action<bool>> _pendingSetCookieResultCallbacks = new Dictionary<string, Action<bool>>();

		
		private delegate void GetCookieCallback(string requestId, string serializedCookie);

		
		private delegate void SetCookieCallback(string requestId, bool success);

		
		private delegate void UnitySendMessageFunction(string gameObjectName, string methodName, string message);
	}
}
