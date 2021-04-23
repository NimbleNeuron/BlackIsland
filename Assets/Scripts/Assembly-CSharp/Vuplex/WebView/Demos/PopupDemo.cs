using System;
using UnityEngine;

namespace Vuplex.WebView.Demos
{
	
	internal class PopupDemo : MonoBehaviour
	{
		
		private void Start()
		{
			WebViewPrefab mainWebViewPrefab = WebViewPrefab.Instantiate(0.6f, 0.3f);
			mainWebViewPrefab.transform.parent = this.transform;
			mainWebViewPrefab.transform.localPosition = new Vector3(0.0f, 0.0f, 0.4f);
			mainWebViewPrefab.transform.localEulerAngles = new Vector3(0.0f, 180f, 0.0f);
			this._focusedPrefab = mainWebViewPrefab;
			mainWebViewPrefab.Initialized += (EventHandler) ((s, e) =>
			{
				if (!(mainWebViewPrefab.WebView is IWithPopups webView))
				{
					mainWebViewPrefab.WebView.LoadHtml(
						"\n            <body>\n                <style>\n                    body {\n                        font-family: sans-serif;\n                        display: flex;\n                        justify-content: center;\n                        align-items: center;\n                        line-height: 1.25;\n                    }\n                    div {\n                        max-width: 80%;\n                    }\n                    li {\n                        margin: 10px 0;\n                    }\n                </style>\n                <div>\n                    <p>\n                        Sorry, but this 3D WebView package doesn't support yet the <a href='https://developer.vuplex.com/webview/IWithPopups'>IWithPopups</a> interface. Current packages that support popups:\n                    </p>\n                    <ul>\n                        <li>\n                            <a href='https://developer.vuplex.com/webview/StandaloneWebView'>3D WebView for Windows and macOS</a>\n                        </li>\n                        <li>\n                            <a href='https://developer.vuplex.com/webview/AndroidGeckoWebView'>3D WebView for Android with Gecko Engine</a>\n                        </li>\n                    </ul>\n                </div>\n            </body>\n        ");
				}
				else
				{
					Debug.Log(
						(object)
						"Loading Pinterest as an example because it uses popups for third party login. Click 'Login', then select Facebook or Google to open a popup for authentication.");
					mainWebViewPrefab.WebView.LoadUrl("https://pinterest.com");
					webView.SetPopupMode(PopupMode.LoadInNewWebView);
					webView.PopupRequested += (EventHandler<PopupRequestedEventArgs>) ((webView2, eventArgs) =>
					{
						// ISSUE: variable of a compiler-generated type
						PopupDemo cDisplayClass20 = this;
						Debug.Log((object) ("Popup opened with URL: " + eventArgs.Url));
						WebViewPrefab popupPrefab = WebViewPrefab.Instantiate(eventArgs.WebView);
						this._focusedPrefab = popupPrefab;
						popupPrefab.transform.parent = this.transform;
						popupPrefab.transform.localPosition = new Vector3(0.0f, 0.0f, 0.39f);
						popupPrefab.transform.localEulerAngles = new Vector3(0.0f, 180f, 0.0f);
						popupPrefab.Initialized += (EventHandler) ((sender, initializedEventArgs) =>
							popupPrefab.WebView.CloseRequested += (EventHandler) ((popupWebView, closeEventArgs) =>
							{
								Debug.Log((object) "Closing the popup");
								// ISSUE: reference to a compiler-generated field
								// ISSUE: reference to a compiler-generated field
								cDisplayClass20._focusedPrefab = mainWebViewPrefab;
								popupPrefab.Destroy();
							}));
					});
				}
			});
			this._setUpKeyboards();
			// co: dotPeek
			// WebViewPrefab mainWebViewPrefab = WebViewPrefab.Instantiate(0.6f, 0.3f);
			// mainWebViewPrefab.transform.parent = base.transform;
			// mainWebViewPrefab.transform.localPosition = new Vector3(0f, 0f, 0.4f);
			// mainWebViewPrefab.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
			// this._focusedPrefab = mainWebViewPrefab;
			// EventHandler<PopupRequestedEventArgs> <>9__1;
			// mainWebViewPrefab.Initialized += delegate(object s, EventArgs e)
			// {
			// 	IWithPopups withPopups = mainWebViewPrefab.WebView as IWithPopups;
			// 	if (withPopups == null)
			// 	{
			// 		mainWebViewPrefab.WebView.LoadHtml("\n            <body>\n                <style>\n                    body {\n                        font-family: sans-serif;\n                        display: flex;\n                        justify-content: center;\n                        align-items: center;\n                        line-height: 1.25;\n                    }\n                    div {\n                        max-width: 80%;\n                    }\n                    li {\n                        margin: 10px 0;\n                    }\n                </style>\n                <div>\n                    <p>\n                        Sorry, but this 3D WebView package doesn't support yet the <a href='https://developer.vuplex.com/webview/IWithPopups'>IWithPopups</a> interface. Current packages that support popups:\n                    </p>\n                    <ul>\n                        <li>\n                            <a href='https://developer.vuplex.com/webview/StandaloneWebView'>3D WebView for Windows and macOS</a>\n                        </li>\n                        <li>\n                            <a href='https://developer.vuplex.com/webview/AndroidGeckoWebView'>3D WebView for Android with Gecko Engine</a>\n                        </li>\n                    </ul>\n                </div>\n            </body>\n        ");
			// 		return;
			// 	}
			// 	Debug.Log("Loading Pinterest as an example because it uses popups for third party login. Click 'Login', then select Facebook or Google to open a popup for authentication.");
			// 	mainWebViewPrefab.WebView.LoadUrl("https://pinterest.com");
			// 	withPopups.SetPopupMode(PopupMode.LoadInNewWebView);
			// 	IWithPopups withPopups2 = withPopups;
			// 	EventHandler<PopupRequestedEventArgs> value;
			// 	if ((value = <>9__1) == null)
			// 	{
			// 		value = (<>9__1 = delegate(object webView, PopupRequestedEventArgs eventArgs)
			// 		{
			// 			Debug.Log("Popup opened with URL: " + eventArgs.Url);
			// 			WebViewPrefab popupPrefab = WebViewPrefab.Instantiate(eventArgs.WebView);
			// 			this._focusedPrefab = popupPrefab;
			// 			popupPrefab.transform.parent = this.transform;
			// 			popupPrefab.transform.localPosition = new Vector3(0f, 0f, 0.39f);
			// 			popupPrefab.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
			// 			EventHandler <>9__3;
			// 			popupPrefab.Initialized += delegate(object sender, EventArgs initializedEventArgs)
			// 			{
			// 				IWebView webView = popupPrefab.WebView;
			// 				EventHandler value2;
			// 				if ((value2 = <>9__3) == null)
			// 				{
			// 					value2 = (<>9__3 = delegate(object popupWebView, EventArgs closeEventArgs)
			// 					{
			// 						Debug.Log("Closing the popup");
			// 						this._focusedPrefab = mainWebViewPrefab;
			// 						popupPrefab.Destroy();
			// 					});
			// 				}
			// 				webView.CloseRequested += value2;
			// 			};
			// 		});
			// 	}
			// 	withPopups2.PopupRequested += value;
			// };
			// this._setUpKeyboards();
		}

		
		private void _setUpKeyboards()
		{
			this._hardwareKeyboardListener = HardwareKeyboardListener.Instantiate();
			this._hardwareKeyboardListener.KeyDownReceived += delegate(object sender, KeyboardInputEventArgs eventArgs)
			{
				IWithKeyDownAndUp withKeyDownAndUp = this._focusedPrefab.WebView as IWithKeyDownAndUp;
				if (withKeyDownAndUp == null)
				{
					this._focusedPrefab.WebView.HandleKeyboardInput(eventArgs.Value);
					return;
				}
				withKeyDownAndUp.KeyDown(eventArgs.Value, eventArgs.Modifiers);
			};
			this._hardwareKeyboardListener.KeyUpReceived += delegate(object sender, KeyboardInputEventArgs eventArgs)
			{
				IWithKeyDownAndUp withKeyDownAndUp = this._focusedPrefab.WebView as IWithKeyDownAndUp;
				if (withKeyDownAndUp != null)
				{
					withKeyDownAndUp.KeyUp(eventArgs.Value, eventArgs.Modifiers);
				}
			};
			Keyboard keyboard = Keyboard.Instantiate();
			keyboard.transform.parent = this._focusedPrefab.transform;
			keyboard.transform.localPosition = new Vector3(0f, -0.31f, 0f);
			keyboard.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
			keyboard.InputReceived += delegate(object sender, EventArgs<string> eventArgs)
			{
				this._focusedPrefab.WebView.HandleKeyboardInput(eventArgs.Value);
			};
		}

		
		private WebViewPrefab _focusedPrefab;

		
		private HardwareKeyboardListener _hardwareKeyboardListener;

		
		private const string NOT_SUPPORTED_HTML = "\n            <body>\n                <style>\n                    body {\n                        font-family: sans-serif;\n                        display: flex;\n                        justify-content: center;\n                        align-items: center;\n                        line-height: 1.25;\n                    }\n                    div {\n                        max-width: 80%;\n                    }\n                    li {\n                        margin: 10px 0;\n                    }\n                </style>\n                <div>\n                    <p>\n                        Sorry, but this 3D WebView package doesn't support yet the <a href='https://developer.vuplex.com/webview/IWithPopups'>IWithPopups</a> interface. Current packages that support popups:\n                    </p>\n                    <ul>\n                        <li>\n                            <a href='https://developer.vuplex.com/webview/StandaloneWebView'>3D WebView for Windows and macOS</a>\n                        </li>\n                        <li>\n                            <a href='https://developer.vuplex.com/webview/AndroidGeckoWebView'>3D WebView for Android with Gecko Engine</a>\n                        </li>\n                    </ul>\n                </div>\n            </body>\n        ";
	}
}
