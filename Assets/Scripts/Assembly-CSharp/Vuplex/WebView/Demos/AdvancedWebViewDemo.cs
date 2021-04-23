using System;
using System.Timers;
using UnityEngine;

namespace Vuplex.WebView.Demos
{
	
	internal class AdvancedWebViewDemo : MonoBehaviour
	{
		
		private void Start()
		{
			this._mainWebViewPrefab = WebViewPrefab.Instantiate(0.6f, 0.3f);
			this._mainWebViewPrefab.transform.parent = base.transform;
			this._mainWebViewPrefab.transform.localPosition = new Vector3(0f, -0.05f, 0.4f);
			this._mainWebViewPrefab.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
			this._mainWebViewPrefab.Initialized += delegate(object initializedSender, EventArgs initializedEventArgs)
			{
				this._mainWebViewPrefab.WebView.UrlChanged += this.MainWebView_UrlChanged;
				this._mainWebViewPrefab.WebView.LoadUrl("https://www.google.com");
			};
			this._setUpKeyboards();
			this._controlsWebViewPrefab = WebViewPrefab.Instantiate(0.6f, 0.05f);
			this._controlsWebViewPrefab.transform.parent = this._mainWebViewPrefab.transform;
			this._controlsWebViewPrefab.transform.localPosition = new Vector3(0f, 0.06f, 0f);
			this._controlsWebViewPrefab.transform.localEulerAngles = Vector3.zero;
			this._controlsWebViewPrefab.Initialized += delegate(object sender, EventArgs e)
			{
				this._controlsWebViewPrefab.WebView.MessageEmitted += delegate(object webView, EventArgs<string> eventArgs)
				{
					if (eventArgs.Value == "CONTROLS_INITIALIZED")
					{
						this._setDisplayedUrl(this._mainWebViewPrefab.WebView.Url);
					}
				};
				this._controlsWebViewPrefab.WebView.MessageEmitted += this.Controls_MessageEmitted;
				this._controlsWebViewPrefab.WebView.LoadHtml("\n            <!DOCTYPE html>\n            <html>\n                <head>\n                    <!-- This transparent meta tag instructs 3D WebView to allow the page to be transparent. -->\n                    <meta name='transparent' content='true'>\n                    <meta charset='UTF-8'>\n                    <style>\n                        body {\n                            font-family: Helvetica, Arial, Sans-Serif;\n                            margin: 0;\n                            height: 100vh;\n                            color: white;\n                        }\n                        .controls {\n                            display: flex;\n                            justify-content: space-between;\n                            align-items: center;\n                            height: 100%;\n                        }\n                        .controls > div {\n                            background-color: #283237;\n                            border-radius: 8px;\n                            height: 100%;\n                        }\n                        .url-display {\n                            flex: 0 0 75%;\n                            width: 75%;\n                            display: flex;\n                            align-items: center;\n                            overflow: hidden;\n                        }\n                        #url {\n                            width: 100%;\n                            white-space: nowrap;\n                            overflow: hidden;\n                            text-overflow: ellipsis;\n                            padding: 0 15px;\n                            font-size: 18px;\n                        }\n                        .buttons {\n                            flex: 0 0 20%;\n                            width: 20%;\n                            display: flex;\n                            justify-content: space-around;\n                            align-items: center;\n                        }\n                        .buttons > button {\n                            font-size: 40px;\n                            background: none;\n                            border: none;\n                            outline: none;\n                            color: white;\n                            margin: 0;\n                            padding: 0;\n                        }\n                        .buttons > button:disabled {\n                            color: rgba(255, 255, 255, 0.3);\n                        }\n                        .buttons > button:last-child {\n                            transform: scaleX(-1);\n                        }\n                        /* For Gecko only, set the background color\n                        to black so that the shader's cutout rect\n                        can translate the black pixels to transparent.*/\n                        @supports (-moz-appearance:none) {\n                            body {\n                                background-color: black;\n                            }\n                        }\n                    </style>\n                </head>\n                <body>\n                    <div class='controls'>\n                        <div class='url-display'>\n                            <div id='url'></div>\n                        </div>\n                        <div class='buttons'>\n                            <button id='back-button' disabled='true' onclick='vuplex.postMessage({ type: \"GO_BACK\" })'>←</button>\n                            <button id='forward-button' disabled='true' onclick='vuplex.postMessage({ type: \"GO_FORWARD\" })'>←</button>\n                        </div>\n                    </div>\n                    <script>\n                        // Handle messages sent from C#\n                        function handleMessage(message) {\n                            var data = JSON.parse(message.data);\n                            if (data.type === 'SET_URL') {\n                                document.getElementById('url').innerText = data.url;\n                            } else if (data.type === 'SET_BUTTONS') {\n                                document.getElementById('back-button').disabled = !data.canGoBack;\n                                document.getElementById('forward-button').disabled = !data.canGoForward;\n                            }\n                        }\n\n                        function attachMessageListener() {\n                            window.vuplex.addEventListener('message', handleMessage);\n                            window.vuplex.postMessage('CONTROLS_INITIALIZED');\n                        }\n\n                        if (window.vuplex) {\n                            attachMessageListener();\n                        } else {\n                            window.addEventListener('vuplexready', attachMessageListener);\n                        }\n                    </script>\n                </body>\n            </html>\n        ");
				WebPluginType pluginType = this._controlsWebViewPrefab.WebView.PluginType;
				if (pluginType == WebPluginType.AndroidGecko || pluginType == WebPluginType.UniversalWindowsPlatform)
				{
					this._controlsWebViewPrefab.SetCutoutRect(new Rect(0f, 0f, 1f, 1f));
				}
			};
			this._buttonRefreshTimer.AutoReset = false;
			this._buttonRefreshTimer.Interval = 1000.0;
			this._buttonRefreshTimer.Elapsed += this.ButtonRefreshTimer_Elapsed;
		}

		
		private void ButtonRefreshTimer_Elapsed(object sender, ElapsedEventArgs eventArgs)
		{
			Dispatcher.RunOnMainThread(delegate
			{
				this._mainWebViewPrefab.WebView.CanGoBack(delegate(bool canGoBack)
				{
					this._mainWebViewPrefab.WebView.CanGoForward(delegate(bool canGoForward)
					{
						string data = string.Format("{{ \"type\": \"SET_BUTTONS\", \"canGoBack\": {0}, \"canGoForward\": {1} }}", canGoBack.ToString().ToLower(), canGoForward.ToString().ToLower());
						this._controlsWebViewPrefab.WebView.PostMessage(data);
					});
				});
			});
		}

		
		private void Controls_MessageEmitted(object sender, EventArgs<string> eventArgs)
		{
			string a = BridgeMessage.ParseType(eventArgs.Value);
			if (a == "GO_BACK")
			{
				this._mainWebViewPrefab.WebView.GoBack();
				return;
			}
			if (a == "GO_FORWARD")
			{
				this._mainWebViewPrefab.WebView.GoForward();
			}
		}

		
		private void MainWebView_UrlChanged(object sender, UrlChangedEventArgs eventArgs)
		{
			this._setDisplayedUrl(eventArgs.Url);
			this._buttonRefreshTimer.Start();
		}

		
		private void _setDisplayedUrl(string url)
		{
			if (this._controlsWebViewPrefab.WebView != null)
			{
				string data = string.Format("{{ \"type\": \"SET_URL\", \"url\": \"{0}\" }}", url);
				this._controlsWebViewPrefab.WebView.PostMessage(data);
			}
		}

		
		private void _setUpKeyboards()
		{
			this._hardwareKeyboardListener = HardwareKeyboardListener.Instantiate();
			this._hardwareKeyboardListener.KeyDownReceived += delegate(object sender, KeyboardInputEventArgs eventArgs)
			{
				IWithKeyDownAndUp withKeyDownAndUp = this._mainWebViewPrefab.WebView as IWithKeyDownAndUp;
				if (withKeyDownAndUp == null)
				{
					this._mainWebViewPrefab.WebView.HandleKeyboardInput(eventArgs.Value);
					return;
				}
				withKeyDownAndUp.KeyDown(eventArgs.Value, eventArgs.Modifiers);
			};
			this._hardwareKeyboardListener.KeyUpReceived += delegate(object sender, KeyboardInputEventArgs eventArgs)
			{
				IWithKeyDownAndUp withKeyDownAndUp = this._mainWebViewPrefab.WebView as IWithKeyDownAndUp;
				if (withKeyDownAndUp != null)
				{
					withKeyDownAndUp.KeyUp(eventArgs.Value, eventArgs.Modifiers);
				}
			};
			Keyboard keyboard = Keyboard.Instantiate();
			keyboard.transform.parent = this._mainWebViewPrefab.transform;
			keyboard.transform.localPosition = new Vector3(0f, -0.31f, 0f);
			keyboard.transform.localEulerAngles = Vector3.zero;
			keyboard.InputReceived += delegate(object sender, EventArgs<string> eventArgs)
			{
				this._mainWebViewPrefab.WebView.HandleKeyboardInput(eventArgs.Value);
			};
		}

		
		private Timer _buttonRefreshTimer = new Timer();

		
		private WebViewPrefab _controlsWebViewPrefab;

		
		private HardwareKeyboardListener _hardwareKeyboardListener;

		
		private WebViewPrefab _mainWebViewPrefab;

		
		private const string CONTROLS_HTML = "\n            <!DOCTYPE html>\n            <html>\n                <head>\n                    <!-- This transparent meta tag instructs 3D WebView to allow the page to be transparent. -->\n                    <meta name='transparent' content='true'>\n                    <meta charset='UTF-8'>\n                    <style>\n                        body {\n                            font-family: Helvetica, Arial, Sans-Serif;\n                            margin: 0;\n                            height: 100vh;\n                            color: white;\n                        }\n                        .controls {\n                            display: flex;\n                            justify-content: space-between;\n                            align-items: center;\n                            height: 100%;\n                        }\n                        .controls > div {\n                            background-color: #283237;\n                            border-radius: 8px;\n                            height: 100%;\n                        }\n                        .url-display {\n                            flex: 0 0 75%;\n                            width: 75%;\n                            display: flex;\n                            align-items: center;\n                            overflow: hidden;\n                        }\n                        #url {\n                            width: 100%;\n                            white-space: nowrap;\n                            overflow: hidden;\n                            text-overflow: ellipsis;\n                            padding: 0 15px;\n                            font-size: 18px;\n                        }\n                        .buttons {\n                            flex: 0 0 20%;\n                            width: 20%;\n                            display: flex;\n                            justify-content: space-around;\n                            align-items: center;\n                        }\n                        .buttons > button {\n                            font-size: 40px;\n                            background: none;\n                            border: none;\n                            outline: none;\n                            color: white;\n                            margin: 0;\n                            padding: 0;\n                        }\n                        .buttons > button:disabled {\n                            color: rgba(255, 255, 255, 0.3);\n                        }\n                        .buttons > button:last-child {\n                            transform: scaleX(-1);\n                        }\n                        /* For Gecko only, set the background color\n                        to black so that the shader's cutout rect\n                        can translate the black pixels to transparent.*/\n                        @supports (-moz-appearance:none) {\n                            body {\n                                background-color: black;\n                            }\n                        }\n                    </style>\n                </head>\n                <body>\n                    <div class='controls'>\n                        <div class='url-display'>\n                            <div id='url'></div>\n                        </div>\n                        <div class='buttons'>\n                            <button id='back-button' disabled='true' onclick='vuplex.postMessage({ type: \"GO_BACK\" })'>←</button>\n                            <button id='forward-button' disabled='true' onclick='vuplex.postMessage({ type: \"GO_FORWARD\" })'>←</button>\n                        </div>\n                    </div>\n                    <script>\n                        // Handle messages sent from C#\n                        function handleMessage(message) {\n                            var data = JSON.parse(message.data);\n                            if (data.type === 'SET_URL') {\n                                document.getElementById('url').innerText = data.url;\n                            } else if (data.type === 'SET_BUTTONS') {\n                                document.getElementById('back-button').disabled = !data.canGoBack;\n                                document.getElementById('forward-button').disabled = !data.canGoForward;\n                            }\n                        }\n\n                        function attachMessageListener() {\n                            window.vuplex.addEventListener('message', handleMessage);\n                            window.vuplex.postMessage('CONTROLS_INITIALIZED');\n                        }\n\n                        if (window.vuplex) {\n                            attachMessageListener();\n                        } else {\n                            window.addEventListener('vuplexready', attachMessageListener);\n                        }\n                    </script>\n                </body>\n            </html>\n        ";
	}
}
