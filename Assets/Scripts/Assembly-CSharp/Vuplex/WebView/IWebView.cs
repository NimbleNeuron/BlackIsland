using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Vuplex.WebView
{
	
	public interface IWebView
	{
		
		
		
		event EventHandler CloseRequested;

		
		
		
		event EventHandler<ProgressChangedEventArgs> LoadProgressChanged;

		
		
		
		event EventHandler<EventArgs<string>> MessageEmitted;

		
		
		
		event EventHandler PageLoadFailed;

		
		
		
		event EventHandler<EventArgs<string>> TitleChanged;

		
		
		
		event EventHandler<UrlChangedEventArgs> UrlChanged;

		
		
		
		event EventHandler<EventArgs<Rect>> VideoRectChanged;

		
		
		bool IsDisposed { get; }

		
		
		bool IsInitialized { get; }

		
		
		List<string> PageLoadScripts { get; }

		
		
		WebPluginType PluginType { get; }

		
		
		float Resolution { get; }

		
		
		Vector2 Size { get; }

		
		
		Vector2 SizeInPixels { get; }

		
		
		Texture2D Texture { get; }

		
		
		string Url { get; }

		
		
		Texture2D VideoTexture { get; }

		
		void Init(Texture2D viewportTexture, float width, float height, Texture2D videoTexture);

		
		void Init(Texture2D viewportTexture, float width, float height);

		
		void Blur();

		
		Task<bool> CanGoBack();

		
		Task<bool> CanGoForward();

		
		void CanGoBack(Action<bool> callback);

		
		void CanGoForward(Action<bool> callback);

		
		Task<byte[]> CaptureScreenshot();

		
		void CaptureScreenshot(Action<byte[]> callback);

		
		void Click(Vector2 point);

		
		void Click(Vector2 point, bool preventStealingFocus);

		
		void Copy();

		
		void Cut();

		
		void DisableViewUpdates();

		
		void Dispose();

		
		void EnableViewUpdates();

		
		Task<string> ExecuteJavaScript(string javaScript);

		
		void ExecuteJavaScript(string javaScript, Action<string> callback);

		
		void Focus();

		
		Task<byte[]> GetRawTextureData();

		
		void GetRawTextureData(Action<byte[]> callback);

		
		void GoBack();

		
		void GoForward();

		
		void HandleKeyboardInput(string key);

		
		void LoadHtml(string html);

		
		void LoadUrl(string url);

		
		void LoadUrl(string url, Dictionary<string, string> additionalHttpHeaders);

		
		void Paste();

		
		void PostMessage(string data);

		
		void Reload();

		
		void Resize(float width, float height);

		
		void Scroll(Vector2 scrollDelta);

		
		void Scroll(Vector2 scrollDelta, Vector2 point);

		
		void SelectAll();

		
		void SetResolution(float pixelsPerUnityUnit);

		
		void ZoomIn();

		
		void ZoomOut();
	}
}
