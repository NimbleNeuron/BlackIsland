using System;
using UnityEngine;

namespace Vuplex.WebView.Demos
{
	
	internal class CanvasPopupDemo : MonoBehaviour
	{
		
		private void Start()
		{
			this._canvas = GameObject.Find("Canvas");
			CanvasWebViewPrefab mainWebViewPrefab = CanvasWebViewPrefab.Instantiate();
			mainWebViewPrefab.transform.SetParent(this._canvas.transform, false);
			RectTransform transform1 = mainWebViewPrefab.transform as RectTransform;
			transform1.anchoredPosition3D = Vector3.zero;
			transform1.offsetMin = Vector2.zero;
			transform1.offsetMax = Vector2.zero;
			mainWebViewPrefab.transform.localScale = Vector3.one;
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
						CanvasPopupDemo cDisplayClass30 = this;
						Debug.Log((object) ("Popup opened with URL: " + eventArgs.Url));
						CanvasWebViewPrefab popupPrefab = CanvasWebViewPrefab.Instantiate(eventArgs.WebView);
						this._focusedPrefab = popupPrefab;
						popupPrefab.transform.SetParent(this._canvas.transform, false);
						RectTransform transform2 = popupPrefab.transform as RectTransform;
						transform2.anchoredPosition3D = Vector3.zero;
						transform2.offsetMin = Vector2.zero;
						transform2.offsetMax = Vector2.zero;
						popupPrefab.transform.localScale = Vector3.one;
						Vector3 localPosition = popupPrefab.transform.localPosition;
						localPosition.z = 0.1f;
						popupPrefab.transform.localPosition = localPosition;
						popupPrefab.Initialized += (EventHandler) ((sender, initializedEventArgs) =>
							popupPrefab.WebView.CloseRequested += (EventHandler) ((popupWebView, closeEventArgs) =>
							{
								Debug.Log((object) "Closing the popup");
								// ISSUE: reference to a compiler-generated field
								// ISSUE: reference to a compiler-generated field
								cDisplayClass30._focusedPrefab = mainWebViewPrefab;
								popupPrefab.Destroy();
							}));
					});
				}
			});
			this._setUpKeyboards();
			
			// co: dotPeek
			// this._canvas = GameObject.Find("Canvas");
			// CanvasWebViewPrefab mainWebViewPrefab = CanvasWebViewPrefab.Instantiate();
			// mainWebViewPrefab.transform.SetParent(this._canvas.transform, false);
			// RectTransform rectTransform = mainWebViewPrefab.transform as RectTransform;
			// rectTransform.anchoredPosition3D = Vector3.zero;
			// rectTransform.offsetMin = Vector2.zero;
			// rectTransform.offsetMax = Vector2.zero;
			// mainWebViewPrefab.transform.localScale = Vector3.one;
			// this._focusedPrefab = mainWebViewPrefab;
			// CanvasWebViewPrefab mainWebViewPrefab2 = mainWebViewPrefab;
			// EventHandler<PopupRequestedEventArgs> <>9__1;
			// mainWebViewPrefab2.Initialized = (EventHandler)Delegate.Combine(mainWebViewPrefab2.Initialized, new EventHandler(delegate(object s, EventArgs e)
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
			// 			CanvasWebViewPrefab popupPrefab = CanvasWebViewPrefab.Instantiate(eventArgs.WebView);
			// 			this._focusedPrefab = popupPrefab;
			// 			popupPrefab.transform.SetParent(this._canvas.transform, false);
			// 			RectTransform rectTransform2 = popupPrefab.transform as RectTransform;
			// 			rectTransform2.anchoredPosition3D = Vector3.zero;
			// 			rectTransform2.offsetMin = Vector2.zero;
			// 			rectTransform2.offsetMax = Vector2.zero;
			// 			popupPrefab.transform.localScale = Vector3.one;
			// 			Vector3 localPosition = popupPrefab.transform.localPosition;
			// 			localPosition.z = 0.1f;
			// 			popupPrefab.transform.localPosition = localPosition;
			// 			CanvasWebViewPrefab popupPrefab2 = popupPrefab;
			// 			EventHandler <>9__3;
			// 			popupPrefab2.Initialized = (EventHandler)Delegate.Combine(popupPrefab2.Initialized, new EventHandler(delegate(object sender, EventArgs initializedEventArgs)
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
			// 			}));
			// 		});
			// 	}
			// 	withPopups2.PopupRequested += value;
			// }));
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
		}

		
		private GameObject _canvas;

		
		private CanvasWebViewPrefab _focusedPrefab;

		
		private HardwareKeyboardListener _hardwareKeyboardListener;

		
		private const string NOT_SUPPORTED_HTML = "\n            <body>\n                <style>\n                    body {\n                        font-family: sans-serif;\n                        display: flex;\n                        justify-content: center;\n                        align-items: center;\n                        line-height: 1.25;\n                    }\n                    div {\n                        max-width: 80%;\n                    }\n                    li {\n                        margin: 10px 0;\n                    }\n                </style>\n                <div>\n                    <p>\n                        Sorry, but this 3D WebView package doesn't support yet the <a href='https://developer.vuplex.com/webview/IWithPopups'>IWithPopups</a> interface. Current packages that support popups:\n                    </p>\n                    <ul>\n                        <li>\n                            <a href='https://developer.vuplex.com/webview/StandaloneWebView'>3D WebView for Windows and macOS</a>\n                        </li>\n                        <li>\n                            <a href='https://developer.vuplex.com/webview/AndroidGeckoWebView'>3D WebView for Android with Gecko Engine</a>\n                        </li>\n                    </ul>\n                </div>\n            </body>\n        ";
	}
}
