using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace Vuplex.WebView
{
	
	public abstract class BaseWebView : MonoBehaviour
	{
		
		
		
		public event EventHandler CloseRequested;

		
		
		
		public event EventHandler<ProgressChangedEventArgs> LoadProgressChanged;

		
		
		
		public event EventHandler<EventArgs<string>> MessageEmitted
		{
			add
			{
				this._messageHandler.MessageEmitted += value;
			}
			remove
			{
				this._messageHandler.MessageEmitted -= value;
			}
		}

		
		
		
		public event EventHandler PageLoadFailed;

		
		
		
		public event EventHandler<EventArgs<string>> TitleChanged
		{
			add
			{
				this._messageHandler.TitleChanged += value;
			}
			remove
			{
				this._messageHandler.TitleChanged -= value;
			}
		}

		
		
		
		public event EventHandler<UrlChangedEventArgs> UrlChanged;

		
		
		
		public event EventHandler<EventArgs<Rect>> VideoRectChanged
		{
			add
			{
				this._messageHandler.VideoRectChanged += value;
			}
			remove
			{
				this._messageHandler.VideoRectChanged -= value;
			}
		}

		
		
		
		public bool IsDisposed { get; protected set; }

		
		
		
		public bool IsInitialized { get; private set; }

		
		
		public List<string> PageLoadScripts
		{
			get
			{
				return this._pageLoadScripts;
			}
		}

		
		
		public float Resolution
		{
			get
			{
				return this._numberOfPixelsPerUnityUnit;
			}
		}

		
		
		public Vector2 Size
		{
			get
			{
				return new Vector2(this._width, this._height);
			}
		}

		
		
		public Vector2 SizeInPixels
		{
			get
			{
				return new Vector2((float)this._nativeWidth, (float)this._nativeHeight);
			}
		}

		
		
		public Texture2D Texture
		{
			get
			{
				return this._viewportTexture;
			}
		}

		
		
		
		public string Url { get; private set; }

		
		
		public Texture2D VideoTexture
		{
			get
			{
				return this._videoTexture;
			}
		}

		
		public void Init(Texture2D texture, float width, float height)
		{
			this.Init(texture, width, height, null);
		}

		
		public virtual void Init(Texture2D viewportTexture, float width, float height, Texture2D videoTexture)
		{
			if (this.IsInitialized)
			{
				throw new InvalidOperationException("Init() cannot be called on a webview that has already been initialized.");
			}
			this._viewportTexture = viewportTexture;
			this._videoTexture = videoTexture;
			base.gameObject.name = string.Format("WebView-{0}", Guid.NewGuid().ToString());
			this._width = width;
			this._height = height;
			Utils.ThrowExceptionIfAbnormallyLarge(this._nativeWidth, this._nativeHeight);
			this._messageHandler.JavaScriptResultReceived += this.MessageHandler_JavaScriptResultReceived;
			this._messageHandler.UrlChanged += this.MessageHandler_UrlChanged;
			this.IsInitialized = true;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}

		
		public virtual void Blur()
		{
			this._assertValidState();
			BaseWebView.WebView_blur(this._nativeWebViewPtr);
		}

		
		public Task<bool> CanGoBack()
		{
			TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
			this.CanGoBack(new Action<bool>(taskCompletionSource.SetResult));
			return taskCompletionSource.Task;
		}

		
		public Task<bool> CanGoForward()
		{
			TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
			this.CanGoForward(new Action<bool>(taskCompletionSource.SetResult));
			return taskCompletionSource.Task;
		}

		
		public virtual void CanGoBack(Action<bool> callback)
		{
			this._assertValidState();
			this._pendingCanGoBackCallbacks.Add(callback);
			BaseWebView.WebView_canGoBack(this._nativeWebViewPtr);
		}

		
		public virtual void CanGoForward(Action<bool> callback)
		{
			this._assertValidState();
			this._pendingCanGoForwardCallbacks.Add(callback);
			BaseWebView.WebView_canGoForward(this._nativeWebViewPtr);
		}

		
		public Task<byte[]> CaptureScreenshot()
		{
			TaskCompletionSource<byte[]> taskCompletionSource = new TaskCompletionSource<byte[]>();
			this.CaptureScreenshot(new Action<byte[]>(taskCompletionSource.SetResult));
			return taskCompletionSource.Task;
		}

		
		public virtual void CaptureScreenshot(Action<byte[]> callback)
		{
			byte[] obj = new byte[0];
			try
			{
				Texture2D texture2D = this._getReadableTexture();
				obj = texture2D.EncodeToPNG();
				UnityEngine.Object.Destroy(texture2D);
			}
			catch (Exception arg)
			{
				Debug.LogError("An exception occurred while capturing the screenshot: " + arg);
			}
			callback(obj);
		}

		
		public virtual void Click(Vector2 point)
		{
			this._assertValidState();
			int x = (int)(point.x * (float)this._nativeWidth);
			int y = (int)(point.y * (float)this._nativeHeight);
			BaseWebView.WebView_click(this._nativeWebViewPtr, x, y);
		}

		
		public virtual void Click(Vector2 point, bool preventStealingFocus)
		{
			this.Click(point);
		}

		
		public static void ClearAllData()
		{
			BaseWebView.WebView_clearAllData();
		}

		
		public static void CreateTexture(float width, float height, Action<Texture2D> callback)
		{
			int width2 = (int)(width * 1300f);
			int height2 = (int)(height * 1300f);
			Utils.ThrowExceptionIfAbnormallyLarge(width2, height2);
			Texture2D texture = new Texture2D(width2, height2, TextureFormat.RGBA32, false, false);
			Dispatcher.RunOnMainThread(delegate
			{
				callback(texture);
			});
		}

		
		public virtual void Copy()
		{
			this._assertValidState();
			this._getSelectedText(delegate(string text)
			{
				GUIUtility.systemCopyBuffer = text;
			});
		}

		
		public virtual void Cut()
		{
			this._assertValidState();
			this._getSelectedText(delegate(string text)
			{
				GUIUtility.systemCopyBuffer = text;
				this.HandleKeyboardInput("Backspace");
			});
		}

		
		public virtual void DisableViewUpdates()
		{
			this._assertValidState();
			BaseWebView.WebView_disableViewUpdates(this._nativeWebViewPtr);
			this._viewUpdatesAreEnabled = false;
		}

		
		public virtual void Dispose()
		{
			this._assertValidState();
			this.IsDisposed = true;
			BaseWebView.WebView_destroy(this._nativeWebViewPtr);
			this._nativeWebViewPtr = IntPtr.Zero;
			if (this != null)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		
		public virtual void EnableViewUpdates()
		{
			this._assertValidState();
			if (this._currentViewportNativeTexture != IntPtr.Zero)
			{
				this._viewportTexture.UpdateExternalTexture(this._currentViewportNativeTexture);
			}
			BaseWebView.WebView_enableViewUpdates(this._nativeWebViewPtr);
			this._viewUpdatesAreEnabled = true;
		}

		
		public Task<string> ExecuteJavaScript(string javaScript)
		{
			TaskCompletionSource<string> taskCompletionSource = new TaskCompletionSource<string>();
			this.ExecuteJavaScript(javaScript, new Action<string>(taskCompletionSource.SetResult));
			return taskCompletionSource.Task;
		}

		
		public virtual void ExecuteJavaScript(string javaScript, Action<string> callback)
		{
			this._assertValidState();
			string text = null;
			if (callback != null)
			{
				text = Guid.NewGuid().ToString();
				this._pendingJavaScriptResultCallbacks[text] = callback;
			}
			BaseWebView.WebView_executeJavaScript(this._nativeWebViewPtr, javaScript, text);
		}

		
		public virtual void Focus()
		{
			this._assertValidState();
			BaseWebView.WebView_focus(this._nativeWebViewPtr);
		}

		
		public Task<byte[]> GetRawTextureData()
		{
			TaskCompletionSource<byte[]> taskCompletionSource = new TaskCompletionSource<byte[]>();
			this.GetRawTextureData(new Action<byte[]>(taskCompletionSource.SetResult));
			return taskCompletionSource.Task;
		}

		
		public virtual void GetRawTextureData(Action<byte[]> callback)
		{
			byte[] obj = new byte[0];
			try
			{
				Texture2D texture2D = this._getReadableTexture();
				obj = texture2D.GetRawTextureData();
				UnityEngine.Object.Destroy(texture2D);
			}
			catch (Exception arg)
			{
				Debug.LogError("An exception occurred while getting the raw texture data: " + arg);
			}
			callback(obj);
		}

		
		public static void GloballySetUserAgent(bool mobile)
		{
			if (!BaseWebView.WebView_globallySetUserAgentToMobile(mobile))
			{
				throw new InvalidOperationException("Unable to set the User-Agent string, because a webview has already been created with the default User-Agent. On Windows and macOS, SetUserAgent() can only be called prior to creating any webviews.");
			}
		}

		
		public static void GloballySetUserAgent(string userAgent)
		{
			if (!BaseWebView.WebView_globallySetUserAgent(userAgent))
			{
				throw new InvalidOperationException("Unable to set the User-Agent string, because a webview has already been created with the default User-Agent. On Windows and macOS, SetUserAgent() can only be called prior to creating any webviews.");
			}
		}

		
		public virtual void GoBack()
		{
			this._assertValidState();
			BaseWebView.WebView_goBack(this._nativeWebViewPtr);
		}

		
		public virtual void GoForward()
		{
			this._assertValidState();
			BaseWebView.WebView_goForward(this._nativeWebViewPtr);
		}

		
		public virtual void HandleKeyboardInput(string input)
		{
			this._assertValidState();
			BaseWebView.WebView_handleKeyboardInput(this._nativeWebViewPtr, input);
		}

		
		public virtual void LoadHtml(string html)
		{
			this._assertValidState();
			BaseWebView.WebView_loadHtml(this._nativeWebViewPtr, html);
		}

		
		public virtual void LoadUrl(string url)
		{
			this._assertValidState();
			BaseWebView.WebView_loadUrl(this._nativeWebViewPtr, this._transformStreamingAssetsUrlIfNeeded(url));
		}

		
		public virtual void LoadUrl(string url, Dictionary<string, string> additionalHttpHeaders)
		{
			this._assertValidState();
			if (additionalHttpHeaders == null)
			{
				this.LoadUrl(url);
				return;
			}
			string[] value = (from key in additionalHttpHeaders.Keys
			select string.Format("{0}: {1}", key, additionalHttpHeaders[key])).ToArray<string>();
			string newlineDelimitedHttpHeaders = string.Join("\n", value);
			BaseWebView.WebView_loadUrlWithHeaders(this._nativeWebViewPtr, url, newlineDelimitedHttpHeaders);
		}

		
		public virtual void Paste()
		{
			this._assertValidState();
			foreach (char c in GUIUtility.systemCopyBuffer)
			{
				this.HandleKeyboardInput(char.ToString(c));
			}
		}

		
		public void PostMessage(string data)
		{
			string arg = data.Replace("'", "\\'").Replace("\n", "\\n");
			this.ExecuteJavaScript(string.Format("vuplex._emit('message', {{ data: '{0}' }})", arg));
		}

		
		public virtual void Reload()
		{
			this._assertValidState();
			BaseWebView.WebView_reload(this._nativeWebViewPtr);
		}

		
		public virtual void Resize(float width, float height)
		{
			if (this.IsDisposed || (width == this._width && height == this._height))
			{
				return;
			}
			this._width = width;
			this._height = height;
			this._resize();
		}

		
		public virtual void Scroll(Vector2 delta)
		{
			this._assertValidState();
			int deltaX = (int)(delta.x * this._numberOfPixelsPerUnityUnit);
			int deltaY = (int)(delta.y * this._numberOfPixelsPerUnityUnit);
			BaseWebView.WebView_scroll(this._nativeWebViewPtr, deltaX, deltaY);
		}

		
		public virtual void Scroll(Vector2 scrollDelta, Vector2 point)
		{
			this._assertValidState();
			int deltaX = (int)(scrollDelta.x * this._numberOfPixelsPerUnityUnit);
			int deltaY = (int)(scrollDelta.y * this._numberOfPixelsPerUnityUnit);
			int pointerX = (int)(point.x * (float)this._nativeWidth);
			int pointerY = (int)(point.y * (float)this._nativeHeight);
			BaseWebView.WebView_scrollAtPoint(this._nativeWebViewPtr, deltaX, deltaY, pointerX, pointerY);
		}

		
		public virtual void SelectAll()
		{
			this._assertValidState();
			this.ExecuteJavaScript("(function() {\n                    var element = document.activeElement || document.body;\n                    while (!(element === document.body || element.getAttribute('contenteditable') === 'true')) {\n                        if (typeof element.select === 'function') {\n                            element.select();\n                            return;\n                        }\n                        element = element.parentElement;\n                    }\n                    var range = document.createRange();\n                    range.selectNodeContents(element);\n                    var selection = window.getSelection();\n                    selection.removeAllRanges();\n                    selection.addRange(range);\n                })();", null);
		}

		
		public void SetResolution(float pixelsPerUnityUnit)
		{
			this._numberOfPixelsPerUnityUnit = pixelsPerUnityUnit;
			this._resize();
		}

		
		public static void SetStorageEnabled(bool enabled)
		{
			BaseWebView.WebView_setStorageEnabled(enabled);
		}

		
		public virtual void ZoomIn()
		{
			this._assertValidState();
			BaseWebView.WebView_zoomIn(this._nativeWebViewPtr);
		}

		
		public virtual void ZoomOut()
		{
			this._assertValidState();
			BaseWebView.WebView_zoomOut(this._nativeWebViewPtr);
		}

		
		
		protected int _nativeHeight
		{
			get
			{
				return Math.Max(1, (int)(this._height * this._numberOfPixelsPerUnityUnit));
			}
		}

		
		
		protected int _nativeWidth
		{
			get
			{
				return Math.Max(1, (int)(this._width * this._numberOfPixelsPerUnityUnit));
			}
		}

		
		protected void _assertValidState()
		{
			if (!this.IsInitialized)
			{
				throw new InvalidOperationException("Methods cannot be called on an uninitialized webview. Please initialize it first with IWebView.Init().");
			}
			if (this.IsDisposed)
			{
				throw new InvalidOperationException("Methods cannot be called on a disposed webview.");
			}
		}

		
		protected virtual Material _createMaterialForBlitting()
		{
			return Utils.CreateDefaultMaterial();
		}

		
		private Texture2D _getReadableTexture()
		{
			RenderTexture temporary = RenderTexture.GetTemporary(this._nativeWidth, this._nativeHeight, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
			if (this._materialForBlitting == null)
			{
				this._materialForBlitting = this._createMaterialForBlitting();
			}
			Graphics.Blit(this.Texture, temporary, this._materialForBlitting);
			RenderTexture active = RenderTexture.active;
			RenderTexture.active = temporary;
			Texture2D texture2D = new Texture2D(this._nativeWidth, this._nativeHeight);
			texture2D.ReadPixels(new Rect(0f, 0f, (float)temporary.width, (float)temporary.height), 0, 0);
			texture2D.Apply();
			RenderTexture.active = active;
			RenderTexture.ReleaseTemporary(temporary);
			return texture2D;
		}

		
		private void _getSelectedText(Action<string> callback)
		{
			this.ExecuteJavaScript("var element = document.activeElement;\n                if (element instanceof HTMLInputElement || element instanceof HTMLTextAreaElement) {\n                    element.value.substring(element.selectionStart, element.selectionEnd);\n                } else {\n                    window.getSelection().toString();\n                }", callback);
		}

		
		private void HandleCanGoBackResult(string message)
		{
			bool obj = bool.Parse(message);
			List<Action<bool>> list = new List<Action<bool>>(this._pendingCanGoBackCallbacks);
			this._pendingCanGoBackCallbacks.Clear();
			foreach (Action<bool> action in list)
			{
				try
				{
					action(obj);
				}
				catch (Exception arg)
				{
					Debug.LogError("An exception occurred while calling the callback for CanGoBack: " + arg);
				}
			}
		}

		
		private void HandleCanGoForwardResult(string message)
		{
			bool obj = bool.Parse(message);
			List<Action<bool>> list = new List<Action<bool>>(this._pendingCanGoForwardCallbacks);
			this._pendingCanGoForwardCallbacks.Clear();
			foreach (Action<bool> action in list)
			{
				try
				{
					action(obj);
				}
				catch (Exception arg)
				{
					Debug.LogError("An exception occurred while calling the callForward for CanGoForward: " + arg);
				}
			}
		}

		
		private void HandleCloseRequested(string message)
		{
			if (this.CloseRequested != null)
			{
				this.CloseRequested(this, EventArgs.Empty);
			}
		}

		
		private void HandleJavaScriptResult(string message)
		{
			string[] array = message.Split(new char[]
			{
				','
			}, 2);
			string key = array[0];
			string obj = array[1];
			Action<string> action = this._pendingJavaScriptResultCallbacks[key];
			this._pendingJavaScriptResultCallbacks.Remove(key);
			action(obj);
		}

		
		private void HandleLoadFailed(string unusedParam)
		{
			if (this.PageLoadFailed != null)
			{
				this.PageLoadFailed(this, EventArgs.Empty);
			}
			ProgressChangedEventArgs e = new ProgressChangedEventArgs(ProgressChangeType.Failed, 1f);
			this.OnLoadProgressChanged(e);
		}

		
		private void HandleLoadFinished(string unusedParam)
		{
			ProgressChangedEventArgs e = new ProgressChangedEventArgs(ProgressChangeType.Finished, 1f);
			this.OnLoadProgressChanged(e);
			foreach (string javaScript in this._pageLoadScripts)
			{
				this.ExecuteJavaScript(javaScript, null);
			}
		}

		
		private void HandleLoadStarted(string unusedParam)
		{
			ProgressChangedEventArgs e = new ProgressChangedEventArgs(ProgressChangeType.Started, 0f);
			this.OnLoadProgressChanged(e);
		}

		
		private void HandleLoadProgressUpdate(string progressString)
		{
			float progress = float.Parse(progressString);
			ProgressChangedEventArgs e = new ProgressChangedEventArgs(ProgressChangeType.Updated, progress);
			this.OnLoadProgressChanged(e);
		}

		
		private void HandleMessageEmitted(string serializedMessage)
		{
			this._messageHandler.HandleMessage(serializedMessage);
		}

		
		private void HandleTextureChanged(string textureString)
		{
			IntPtr intPtr = new IntPtr(long.Parse(textureString));
			if (intPtr == this._currentViewportNativeTexture)
			{
				return;
			}
			IntPtr currentViewportNativeTexture = this._currentViewportNativeTexture;
			this._currentViewportNativeTexture = intPtr;
			if (this._viewUpdatesAreEnabled)
			{
				this._viewportTexture.UpdateExternalTexture(this._currentViewportNativeTexture);
			}
			if (currentViewportNativeTexture != IntPtr.Zero && currentViewportNativeTexture != this._currentViewportNativeTexture)
			{
				BaseWebView.WebView_destroyTexture(currentViewportNativeTexture, SystemInfo.graphicsDeviceType.ToString());
			}
		}

		
		private void MessageHandler_JavaScriptResultReceived(object sender, EventArgs<StringWithIdBridgeMessage> e)
		{
			string id = e.Value.id;
			string value = e.Value.value;
			Action<string> action = this._pendingJavaScriptResultCallbacks[id];
			this._pendingJavaScriptResultCallbacks.Remove(id);
			action(value);
		}

		
		protected void MessageHandler_UrlChanged(object sender, UrlChangedEventArgs e)
		{
			this.OnUrlChanged(e);
		}

		
		protected virtual void OnLoadProgressChanged(ProgressChangedEventArgs e)
		{
			if (this.LoadProgressChanged != null)
			{
				this.LoadProgressChanged(this, e);
			}
		}

		
		protected virtual void OnUrlChanged(UrlChangedEventArgs e)
		{
			if (this.Url == e.Url)
			{
				return;
			}
			this.Url = e.Url;
			if (this.UrlChanged != null)
			{
				this.UrlChanged(this, e);
			}
		}

		
		protected virtual void _resize()
		{
			if (this._viewportTexture)
			{
				this._assertValidState();
				Utils.ThrowExceptionIfAbnormallyLarge(this._nativeWidth, this._nativeHeight);
				BaseWebView.WebView_resize(this._nativeWebViewPtr, this._nativeWidth, this._nativeHeight);
			}
		}

		
		protected string _transformStreamingAssetsUrlIfNeeded(string originalUrl)
		{
			if (originalUrl == null)
			{
				throw new ArgumentException("URL cannot be null.");
			}
			Match match = BaseWebView.STREAMING_ASSETS_URL_REGEX.Match(originalUrl);
			if (!match.Success)
			{
				return originalUrl;
			}
			string value = match.Groups[2].Captures[0].Value;
			return "file://" + Path.Combine(Application.streamingAssetsPath, value);
		}

		
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
		private static extern void WebView_blur(IntPtr webViewPtr);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern void WebView_canGoBack(IntPtr webViewPtr);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern void WebView_canGoForward(IntPtr webViewPtr);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern void WebView_clearAllData();

		
		[DllImport("VuplexWebViewWindows")]
		private static extern void WebView_click(IntPtr webViewPtr, int x, int y);

		
		[DllImport("VuplexWebViewWindows")]
		protected static extern void WebView_destroyTexture(IntPtr texture, string graphicsApi);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern void WebView_destroy(IntPtr webViewPtr);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern void WebView_disableViewUpdates(IntPtr webViewPtr);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern void WebView_enableViewUpdates(IntPtr webViewPtr);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern void WebView_executeJavaScript(IntPtr webViewPtr, string javaScript, string resultCallbackId);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern void WebView_focus(IntPtr webViewPtr);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern bool WebView_globallySetUserAgentToMobile(bool mobile);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern bool WebView_globallySetUserAgent(string userAgent);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern void WebView_goBack(IntPtr webViewPtr);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern void WebView_goForward(IntPtr webViewPtr);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern void WebView_handleKeyboardInput(IntPtr webViewPtr, string input);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern void WebView_loadHtml(IntPtr webViewPtr, string html);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern void WebView_loadUrl(IntPtr webViewPtr, string url);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern void WebView_loadUrlWithHeaders(IntPtr webViewPtr, string url, string newlineDelimitedHttpHeaders);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern void WebView_reload(IntPtr webViewPtr);

		
		[DllImport("VuplexWebViewWindows")]
		protected static extern void WebView_resize(IntPtr webViewPtr, int width, int height);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern void WebView_scroll(IntPtr webViewPtr, int deltaX, int deltaY);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern void WebView_scrollAtPoint(IntPtr webViewPtr, int deltaX, int deltaY, int pointerX, int pointerY);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern void WebView_setStorageEnabled(bool enabled);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern void WebView_zoomIn(IntPtr webViewPtr);

		
		[DllImport("VuplexWebViewWindows")]
		private static extern void WebView_zoomOut(IntPtr webViewPtr);

		
		protected IntPtr _currentViewportNativeTexture;

		
		protected const string _dllName = "VuplexWebViewWindows";

		
		protected float _height;

		
		private Material _materialForBlitting;

		
		protected BridgeMessageHandler _messageHandler = new BridgeMessageHandler();

		
		protected IntPtr _nativeWebViewPtr = IntPtr.Zero;

		
		protected float _numberOfPixelsPerUnityUnit = 1300f;

		
		private List<string> _pageLoadScripts = new List<string>();

		
		private List<Action<bool>> _pendingCanGoBackCallbacks = new List<Action<bool>>();

		
		private List<Action<bool>> _pendingCanGoForwardCallbacks = new List<Action<bool>>();

		
		protected Dictionary<string, Action<string>> _pendingJavaScriptResultCallbacks = new Dictionary<string, Action<string>>();

		
		private static readonly Regex STREAMING_ASSETS_URL_REGEX = new Regex("^streaming-assets:(//)?(.*)$", RegexOptions.IgnoreCase);

		
		private const string USER_AGENT_EXCEPTION_MESSAGE = "Unable to set the User-Agent string, because a webview has already been created with the default User-Agent. On Windows and macOS, SetUserAgent() can only be called prior to creating any webviews.";

		
		protected Texture2D _videoTexture;

		
		protected bool _viewUpdatesAreEnabled = true;

		
		protected Texture2D _viewportTexture;

		
		protected float _width;
	}
}
