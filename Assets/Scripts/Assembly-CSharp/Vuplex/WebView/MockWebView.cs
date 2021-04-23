using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Vuplex.WebView
{
	
	internal class MockWebView : MonoBehaviour, IWebView
	{
		private void Ref()
		{
			Reference.Use(CloseRequested);
			Reference.Use(MessageEmitted);
			Reference.Use(PageLoadFailed);
			Reference.Use(VideoRectChanged);
			Reference.Use(TitleChanged);
		}
		
		
		
		
		public event EventHandler CloseRequested= default;

		
		
		
		public event EventHandler<ProgressChangedEventArgs> LoadProgressChanged= default;

		
		
		
		public event EventHandler<EventArgs<string>> MessageEmitted= default;

		
		
		
		public event EventHandler PageLoadFailed= default;

		
		
		
		public event EventHandler<EventArgs<string>> TitleChanged= default;

		
		
		
		public event EventHandler<UrlChangedEventArgs> UrlChanged= default;

		
		
		
		public event EventHandler<EventArgs<Rect>> VideoRectChanged= default;

		
		
		
		public bool IsDisposed { get; private set; }

		
		
		
		public bool IsInitialized { get; private set; }

		
		
		public List<string> PageLoadScripts
		{
			get
			{
				return this._pageLoadScripts;
			}
		}

		
		
		public WebPluginType PluginType
		{
			get
			{
				return WebPluginType.Mock;
			}
		}

		
		
		public float Resolution
		{
			get
			{
				return this._numberOfPixelsPerUnityUnit;
			}
		}

		
		
		
		public Vector2 Size { get; private set; }

		
		
		public Vector2 SizeInPixels
		{
			get
			{
				return new Vector2(this.Size.x * this._numberOfPixelsPerUnityUnit, this.Size.y * this._numberOfPixelsPerUnityUnit);
			}
		}

		
		
		
		public Texture2D Texture { get; private set; }

		
		
		
		public string Url { get; private set; }

		
		
		
		public Texture2D VideoTexture { get; private set; }

		
		public void Init(Texture2D viewportTexture, float width, float height)
		{
			this.Init(viewportTexture, width, height, null);
		}

		
		public static MockWebView Instantiate()
		{
			return new GameObject("MockWebView").AddComponent<MockWebView>();
		}

		
		public void Init(Texture2D viewportTexture, float width, float height, Texture2D videoTexture)
		{
			this.Texture = viewportTexture;
			this.VideoTexture = videoTexture;
			this.Size = new Vector2(width, height);
			this.IsInitialized = true;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			this._log("Init() width: {0}, height: {1}", new object[]
			{
				width.ToString("n4"),
				height.ToString("n4")
			});
		}

		
		public void Blur()
		{
			this._log("Blur()", Array.Empty<object>());
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

		
		public void CanGoBack(Action<bool> callback)
		{
			this._log("CanGoBack()", Array.Empty<object>());
			callback(false);
		}

		
		public void CanGoForward(Action<bool> callback)
		{
			this._log("CanGoForward()", Array.Empty<object>());
			callback(false);
		}

		
		public Task<byte[]> CaptureScreenshot()
		{
			TaskCompletionSource<byte[]> taskCompletionSource = new TaskCompletionSource<byte[]>();
			this.CaptureScreenshot(new Action<byte[]>(taskCompletionSource.SetResult));
			return taskCompletionSource.Task;
		}

		
		public void CaptureScreenshot(Action<byte[]> callback)
		{
			this._log("CaptureScreenshot()", Array.Empty<object>());
			callback(new byte[0]);
		}

		
		public void Click(Vector2 point)
		{
			this._log("Click({0})", new object[]
			{
				point.ToString("n4")
			});
		}

		
		public void Click(Vector2 point, bool preventStealingFocus)
		{
			this._log("Click({0}, {1})", new object[]
			{
				point.ToString("n4"),
				preventStealingFocus
			});
		}

		
		public void Copy()
		{
			this._log("Copy()", Array.Empty<object>());
		}

		
		public void Cut()
		{
			this._log("Cut()", Array.Empty<object>());
		}

		
		public void DisableViewUpdates()
		{
			this._log("DisableViewUpdates()", Array.Empty<object>());
		}

		
		public void Dispose()
		{
			this.IsDisposed = true;
			this._log("Dispose()", Array.Empty<object>());
			if (this != null)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		
		public void EnableViewUpdates()
		{
			this._log("EnableViewUpdates()", Array.Empty<object>());
		}

		
		public Task<string> ExecuteJavaScript(string javaScript)
		{
			TaskCompletionSource<string> taskCompletionSource = new TaskCompletionSource<string>();
			this.ExecuteJavaScript(javaScript, new Action<string>(taskCompletionSource.SetResult));
			return taskCompletionSource.Task;
		}

		
		public void ExecuteJavaScript(string javaScript, Action<string> callback)
		{
			this._log("ExecuteJavaScript(\"{0}...\")", new object[]
			{
				javaScript.Substring(0, 25)
			});
			callback("");
		}

		
		public void Focus()
		{
			this._log("Focus()", Array.Empty<object>());
		}

		
		public Task<byte[]> GetRawTextureData()
		{
			TaskCompletionSource<byte[]> taskCompletionSource = new TaskCompletionSource<byte[]>();
			this.GetRawTextureData(new Action<byte[]>(taskCompletionSource.SetResult));
			return taskCompletionSource.Task;
		}

		
		public void GetRawTextureData(Action<byte[]> callback)
		{
			this._log("GetRawTextureData()", Array.Empty<object>());
			callback(new byte[0]);
		}

		
		public void GoBack()
		{
			this._log("GoBack()", Array.Empty<object>());
		}

		
		public void GoForward()
		{
			this._log("GoForward()", Array.Empty<object>());
		}

		
		public void HandleKeyboardInput(string input)
		{
			this._log("HandleKeyboardInput(\"{0}\")", new object[]
			{
				input
			});
		}

		
		public virtual void LoadHtml(string html)
		{
			this.Url = html.Substring(0, 25);
			this._log("LoadHtml(\"{0}...\")", new object[]
			{
				this.Url
			});
			if (this.UrlChanged != null)
			{
				this.UrlChanged(this, new UrlChangedEventArgs(this.Url, "Title", UrlActionType.Load));
			}
			if (this.LoadProgressChanged != null)
			{
				this.LoadProgressChanged(this, new ProgressChangedEventArgs(ProgressChangeType.Finished, 1f));
			}
		}

		
		public virtual void LoadUrl(string url)
		{
			this.LoadUrl(url, null);
		}

		
		public virtual void LoadUrl(string url, Dictionary<string, string> additionalHttpHeaders)
		{
			this.Url = url;
			this._log("LoadUrl(\"{0}\")", new object[]
			{
				url
			});
			if (this.UrlChanged != null)
			{
				this.UrlChanged(this, new UrlChangedEventArgs(url, "Title", UrlActionType.Load));
			}
			if (this.LoadProgressChanged != null)
			{
				this.LoadProgressChanged(this, new ProgressChangedEventArgs(ProgressChangeType.Finished, 1f));
			}
		}

		
		public void Paste()
		{
			this._log("Paste()", Array.Empty<object>());
		}

		
		public void PostMessage(string data)
		{
			this._log("PostMessage(\"{0}\")", new object[]
			{
				data
			});
		}

		
		public void Reload()
		{
			this._log("Reload()", Array.Empty<object>());
		}

		
		public void Resize(float width, float height)
		{
			this.Size = new Vector2(width, height);
			this._log("Resize({0}, {1})", new object[]
			{
				width.ToString("n4"),
				height.ToString("n4")
			});
		}

		
		public void Scroll(Vector2 delta)
		{
			this._log("Scroll({0})", new object[]
			{
				delta.ToString("n4")
			});
		}

		
		public void Scroll(Vector2 delta, Vector2 point)
		{
			this._log("Scroll({0}, {1})", new object[]
			{
				delta.ToString("n4"),
				point.ToString("n4")
			});
		}

		
		public void SelectAll()
		{
			this._log("SelectAll()", Array.Empty<object>());
		}

		
		public void SetResolution(float pixelsPerUnityUnit)
		{
			this._numberOfPixelsPerUnityUnit = pixelsPerUnityUnit;
			this._log("SetResolution({0})", new object[]
			{
				pixelsPerUnityUnit
			});
		}

		
		public void ZoomIn()
		{
			this._log("ZoomIn()", Array.Empty<object>());
		}

		
		public void ZoomOut()
		{
			this._log("ZoomOut()", Array.Empty<object>());
		}

		
		private void _log(string message, params object[] args)
		{
			Debug.LogFormat("[MockWebView] " + message, args);
		}

		
		private List<string> _pageLoadScripts = new List<string>();

		
		private float _numberOfPixelsPerUnityUnit = 1300f;
	}
}
