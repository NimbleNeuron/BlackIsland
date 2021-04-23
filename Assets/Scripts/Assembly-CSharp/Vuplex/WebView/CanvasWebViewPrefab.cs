using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Vuplex.WebView
{
	
	[HelpURL("https://developer.vuplex.com/webview/CanvasWebViewPrefab")]
	public class CanvasWebViewPrefab : MonoBehaviour
	{
		
		
		
		public event EventHandler<ClickedEventArgs> Clicked
		{
			add
			{
				this._instantiateWebViewPrefabIfNeeded();
				this._webViewPrefab.Clicked += value;
			}
			remove
			{
				this._instantiateWebViewPrefabIfNeeded();
				this._webViewPrefab.Clicked -= value;
			}
		}

		
		
		
		public event EventHandler<ScrolledEventArgs> Scrolled
		{
			add
			{
				this._instantiateWebViewPrefabIfNeeded();
				this._webViewPrefab.Scrolled += value;
			}
			remove
			{
				this._instantiateWebViewPrefabIfNeeded();
				this._webViewPrefab.Scrolled -= value;
			}
		}

		
		
		
		[Obsolete("The CanvasWebViewPrefab.DragToScrollThreshold property is obsolete. Please use DragThreshold instead.")]
		public float DragToScrollThreshold { get; set; }

		
		
		
		public Material Material
		{
			get
			{
				if (!(this._webViewPrefab == null))
				{
					return this._webViewPrefab.Material;
				}
				return null;
			}
			set
			{
				this._instantiateWebViewPrefabIfNeeded();
				this._webViewPrefab.Material = value;
			}
		}

		
		
		
		public bool Visible
		{
			get
			{
				return this._webViewPrefab == null || this._webViewPrefab.Visible;
			}
			set
			{
				this._instantiateWebViewPrefabIfNeeded();
				this._webViewPrefab.Visible = value;
			}
		}

		
		
		public IWebView WebView
		{
			get
			{
				if (!(this._webViewPrefab == null))
				{
					return this._webViewPrefab.WebView;
				}
				return null;
			}
		}

		
		public void Destroy()
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}

		
		public static CanvasWebViewPrefab Instantiate()
		{
			return CanvasWebViewPrefab.Instantiate(default(WebViewOptions));
		}

		
		public static CanvasWebViewPrefab Instantiate(WebViewOptions options)
		{
			CanvasWebViewPrefab component = UnityEngine.Object.Instantiate<GameObject>((GameObject)Resources.Load("CanvasWebViewPrefab")).GetComponent<CanvasWebViewPrefab>();
			component.Init(options);
			return component;
		}

		
		public static CanvasWebViewPrefab Instantiate(IWebView webView)
		{
			CanvasWebViewPrefab component = UnityEngine.Object.Instantiate<GameObject>((GameObject)Resources.Load("CanvasWebViewPrefab")).GetComponent<CanvasWebViewPrefab>();
			component.Init(webView);
			return component;
		}

		
		public void Init()
		{
			this.Init(default(WebViewOptions));
		}

		
		public void Init(WebViewOptions options)
		{
			this._init(options, null);
		}

		
		public void Init(IWebView webView)
		{
			this._init(default(WebViewOptions), webView);
		}

		
		public void SetPointerInputDetector(IPointerInputDetector pointerInputDetector)
		{
			this._instantiateWebViewPrefabIfNeeded();
			this._setCustomPointerInputDetector = true;
			this._webViewPrefab.SetPointerInputDetector(pointerInputDetector);
		}

		
		public Task WaitUntilInitialized()
		{
			TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
			if (this._webViewPrefab != null && this._webViewPrefab.WebView != null)
			{
				taskCompletionSource.SetResult(true);
			}
			else
			{
				this.Initialized = (EventHandler)Delegate.Combine(this.Initialized, new EventHandler(delegate(object sender, EventArgs e)
				{
					taskCompletionSource.SetResult(true);
				}));
			}
			return taskCompletionSource.Task;
		}

		
		
		public IWebView WebViewPrefab
		{
			get
			{
				return this._webViewPrefab.WebView;
			}
		}

		
		private void Awake()
		{
			this.Init();
		}

		
		private Rect _getRect()
		{
			return base.GetComponent<RectTransform>().rect;
		}

		
		private void _init(WebViewOptions options, IWebView initializedWebView = null)
		{
			this._instantiateWebViewPrefabIfNeeded();
			this._webViewPrefab.Initialized += this.WebViewPrefab_Initialized;
			this._webViewPrefab.transform.SetParent(base.transform, false);
			this._webViewPrefab.transform.localPosition = new Vector3(0f, this._getRect().height / 2f, 0f);
			this._webViewPrefab.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
			if (!this._setCustomPointerInputDetector)
			{
				IPointerInputDetector component = base.GetComponent<IPointerInputDetector>();
				if (component == null)
				{
					Debug.LogWarning("The CanvasWebViewPrefab instance has no CanvasPointerInputDetector, so pointer events will not be detected.");
				}
				else
				{
					this._webViewPrefab.SetPointerInputDetector(component);
				}
			}
			this._webViewPrefab.Collider.enabled = false;
			if (initializedWebView == null)
			{
				this._webViewPrefab.Init(1f, 1f, options);
			}
			else
			{
				this._webViewPrefab.Init(initializedWebView);
			}
			this._webViewPrefab.Visible = true;
		}

		
		private void _instantiateWebViewPrefabIfNeeded()
		{
			if (this._webViewPrefab == null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.prefab);
				this._webViewPrefab = gameObject.GetComponent<WebViewPrefab>();
				this._webViewPrefab.Visible = false;
				this._webViewPrefab.SetView(base.GetComponent<ViewportMaterialView>());
			}
		}

		
		private void _resizeWebViewPrefabIfNeeded()
		{
			if (this._webViewPrefab == null || this._webViewPrefab.WebView == null)
			{
				return;
			}
			Rect rect = this._getRect();
			Vector2 size = this._webViewPrefab.WebView.Size;
			if (rect.width != size.x || rect.height != size.y)
			{
				this._webViewPrefab.Resize(rect.width, rect.height);
			}
			Vector3 vector = new Vector3(0f, rect.height / 2f, 0f);
			if (this._webViewPrefab.transform.localPosition != vector)
			{
				this._webViewPrefab.transform.localPosition = vector;
			}
		}

		
		private void Update()
		{
			this._resizeWebViewPrefabIfNeeded();
			this._updateFieldsIfNeeded();
		}

		
		private void _updateFieldsIfNeeded()
		{
			if (this._webViewPrefab == null)
			{
				return;
			}
			if (this._webViewPrefab.ClickingEnabled != this.ClickingEnabled)
			{
				this._webViewPrefab.ClickingEnabled = this.ClickingEnabled;
			}
			if (this._webViewPrefab.DragMode != this.DragMode)
			{
				this._webViewPrefab.DragMode = this.DragMode;
			}
			if (this._webViewPrefab.DragThreshold != this.DragThreshold)
			{
				this._webViewPrefab.DragThreshold = this.DragThreshold;
			}
			if (this._webViewPrefab.HoveringEnabled != this.HoveringEnabled)
			{
				this._webViewPrefab.HoveringEnabled = this.HoveringEnabled;
			}
			if (this._webViewPrefab.ScrollingEnabled != this.ScrollingEnabled)
			{
				this._webViewPrefab.ScrollingEnabled = this.ScrollingEnabled;
			}
			if (this._webViewPrefab.ScrollingSensitivity != this.ScrollingSensitivity)
			{
				this._webViewPrefab.ScrollingSensitivity = this.ScrollingSensitivity;
			}
		}

		
		private void WebViewPrefab_Initialized(object sender, EventArgs e)
		{
			float resolution = this.InitialResolution;
			if (this.InitialResolution <= 0f)
			{
				Debug.LogWarningFormat("Invalid value for CanvasWebViewPrefab.InitialResolution: {0}. The resolution will instead default to 1.", new object[]
				{
					this.InitialResolution
				});
				resolution = 1f;
			}
			this._webViewPrefab.WebView.SetResolution(resolution);
			Rect rect = this._getRect();
			this._webViewPrefab.Resize(rect.width, rect.height);
			EventHandler initialized = this.Initialized;
			if (initialized != null)
			{
				initialized(this, EventArgs.Empty);
			}
			string initialUrl = this.InitialUrl;
			if (!string.IsNullOrEmpty(initialUrl))
			{
				string text = initialUrl.Trim();
				if (!text.Contains(":"))
				{
					text = "http://" + text;
				}
				this._webViewPrefab.WebView.LoadUrl(text);
			}
		}

		
		public EventHandler Initialized;

		
		[Label("Initial URL to Load (Optional)")]
		[Tooltip("Or you can leave the Initial URL blank if you want to initialize the prefab programmatically by calling Init().")]
		public string InitialUrl;

		
		[Label("Initial Resolution (px / Unity unit)")]
		[Tooltip("You can change this to make web content appear larger or smaller.")]
		public float InitialResolution = 1f;

		
		[Tooltip("Note: \"Drag Within Page\" is not supported on iOS or UWP.")]
		public DragMode DragMode;

		
		[Header("Other Settings")]
		public bool ClickingEnabled = true;

		
		[Tooltip("Note: Hovering is not supported on iOS or UWP.")]
		public bool HoveringEnabled = true;

		
		public bool ScrollingEnabled = true;

		
		[Label("Drag Threshold (px)")]
		[Tooltip("Determines the threshold (in web pixels) for triggering a drag.")]
		public float DragThreshold = 20f;

		
		public float ScrollingSensitivity = 15f;

		
		private bool _setCustomPointerInputDetector;

		
		private WebViewPrefab _webViewPrefab;

		
		[SerializeField]
		private GameObject prefab = default;
	}
}
