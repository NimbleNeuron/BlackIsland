using UnityEngine;

namespace Vuplex.WebView.Demos
{
	
	internal class SimpleWebViewDemo : MonoBehaviour
	{
		
		private void Start()
		{
			this._webViewPrefab = GameObject.Find("WebViewPrefab").GetComponent<WebViewPrefab>();
			this._setUpKeyboards();
		}

		
		private void _setUpKeyboards()
		{
			this._hardwareKeyboardListener = HardwareKeyboardListener.Instantiate();
			this._hardwareKeyboardListener.KeyDownReceived += delegate(object sender, KeyboardInputEventArgs eventArgs)
			{
				IWithKeyDownAndUp withKeyDownAndUp = this._webViewPrefab.WebView as IWithKeyDownAndUp;
				if (withKeyDownAndUp == null)
				{
					this._webViewPrefab.WebView.HandleKeyboardInput(eventArgs.Value);
					return;
				}
				withKeyDownAndUp.KeyDown(eventArgs.Value, eventArgs.Modifiers);
			};
			this._hardwareKeyboardListener.KeyUpReceived += delegate(object sender, KeyboardInputEventArgs eventArgs)
			{
				IWithKeyDownAndUp withKeyDownAndUp = this._webViewPrefab.WebView as IWithKeyDownAndUp;
				if (withKeyDownAndUp != null)
				{
					withKeyDownAndUp.KeyUp(eventArgs.Value, eventArgs.Modifiers);
				}
			};
			Keyboard keyboard = Keyboard.Instantiate();
			keyboard.transform.parent = this._webViewPrefab.transform;
			keyboard.transform.localPosition = new Vector3(0f, -0.31f, 0f);
			keyboard.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
			keyboard.InputReceived += delegate(object sender, EventArgs<string> eventArgs)
			{
				this._webViewPrefab.WebView.HandleKeyboardInput(eventArgs.Value);
			};
		}

		
		private HardwareKeyboardListener _hardwareKeyboardListener;

		
		private WebViewPrefab _webViewPrefab;
	}
}
