using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Vuplex.WebView
{
	
	[HelpURL("https://developer.vuplex.com/webview/WebViewPrefab")]
	public class WebViewPrefab : MonoBehaviour
	{
		
		
		
		public event EventHandler<ClickedEventArgs> Clicked;

		
		
		
		public event EventHandler Initialized;

		
		
		
		public event EventHandler<ScrolledEventArgs> Scrolled;

		
		
		
		[Obsolete("The WebViewPrefab.DragToScrollThreshold property is obsolete. Please use DragThreshold instead.")]
		public float DragToScrollThreshold { get; set; }

		
		
		
		[Obsolete("The static WebViewPrefab.ScrollSensitivity property is obsolete. Please use one of the following instance properties instead: WebViewPrefab.ScrollingSensitivity or CanvasWebViewPrefab.ScrollingSensitivity.")]
		public static float ScrollSensitivity { get; set; }

		
		
		public Collider Collider
		{
			get
			{
				return this._getDefaultView().GetComponent<Collider>();
			}
		}

		
		
		
		public Material Material
		{
			get
			{
				if (!(this._view == null))
				{
					return this._view.Material;
				}
				return null;
			}
			set
			{
				this._view.Material = value;
			}
		}

		
		
		
		public bool Visible
		{
			get
			{
				return this._getView().Visible || this._getVideoLayer().Visible;
			}
			set
			{
				this._getView().Visible = value;
				this._getVideoLayer().Visible = value;
			}
		}

		
		
		public IWebView WebView
		{
			get
			{
				return this._webView;
			}
		}

		
		public static WebViewPrefab Instantiate(float width, float height)
		{
			return WebViewPrefab.Instantiate(width, height, default(WebViewOptions));
		}

		
		public static WebViewPrefab Instantiate(float width, float height, WebViewOptions options)
		{
			WebViewPrefab component = UnityEngine.Object.Instantiate<GameObject>((GameObject)Resources.Load("WebViewPrefab")).GetComponent<WebViewPrefab>();
			component.Init(width, height, options);
			return component;
		}

		
		public static WebViewPrefab Instantiate(IWebView webView)
		{
			WebViewPrefab component = UnityEngine.Object.Instantiate<GameObject>((GameObject)Resources.Load("WebViewPrefab")).GetComponent<WebViewPrefab>();
			component.Init(webView);
			return component;
		}

		
		public void Init()
		{
			this._init(base.transform.localScale.x, base.transform.localScale.y, default(WebViewOptions), null);
		}

		
		public virtual void Init(float width, float height)
		{
			this._init(width, height, default(WebViewOptions), null);
		}

		
		public virtual void Init(float width, float height, WebViewOptions options)
		{
			this._init(width, height, options, null);
		}

		
		public void Init(IWebView webView)
		{
			if (!webView.IsInitialized)
			{
				throw new ArgumentException("WebViewPrefab.Init(IWebView) was called with an uninitialized webview, but an initialized webview is required.");
			}
			this._init(webView.Size.x, webView.Size.y, default(WebViewOptions), webView);
		}

		
		public Vector2 ConvertToScreenPoint(Vector3 worldPosition)
		{
			Vector3 vector = this._viewResizer.transform.InverseTransformPoint(worldPosition);
			return new Vector2(1f - vector.x, -1f * vector.y);
		}

		
		public void Destroy()
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}

		
		public void Resize(float width, float height)
		{
			if (this._webView != null)
			{
				this._webView.Resize(width, height);
			}
			this._setViewSize(width, height);
		}

		
		public void SetCutoutRect(Rect rect)
		{
			this._view.SetCutoutRect(rect);
		}

		
		public void SetPointerInputDetector(IPointerInputDetector pointerInputDetector)
		{
			IPointerInputDetector pointerInputDetector2 = this._pointerInputDetector;
			this._pointerInputDetector = pointerInputDetector;
			if (this._webView != null)
			{
				this._initPointerInputDetector(this._webView, pointerInputDetector2);
			}
		}

		
		public void SetView(ViewportMaterialView view)
		{
			this._viewOverride = view;
			this._getDefaultView().enabled = (view == null);
		}

		
		public Task WaitUntilInitialized()
		{
			TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
			if (this._webView != null)
			{
				taskCompletionSource.SetResult(true);
			}
			else
			{
				this.Initialized += delegate(object sender, EventArgs e)
				{
					taskCompletionSource.SetResult(true);
				};
			}
			return taskCompletionSource.Task;
		}

		
		private void _attachWebViewEventHandlers(IWebView webView)
		{
			if (!this._options.disableVideo)
			{
				webView.VideoRectChanged += delegate(object sender, EventArgs<Rect> e)
				{
					this._setVideoRect(e.Value);
				};
			}
		}

		
		private void Awake()
		{
			if (!string.IsNullOrEmpty(this.InitialUrl))
			{
				this.Init();
			}
		}

		
		private Vector2 _convertRatioPointToUnityUnits(Vector2 point)
		{
			float x = this._viewResizer.transform.localScale.x * point.x;
			float y = this._viewResizer.transform.localScale.y * point.y;
			return new Vector2(x, y);
		}

		
		private ViewportMaterialView _getDefaultView()
		{
			return base.transform.Find("WebViewPrefabResizer/WebViewPrefabView").GetComponent<ViewportMaterialView>();
		}

		
		private ViewportMaterialView _getVideoLayer()
		{
			return this._getVideoRectPositioner().GetComponentInChildren<ViewportMaterialView>();
		}

		
		private Transform _getVideoRectPositioner()
		{
			return base.transform.GetChild(0).Find("VideoRectPositioner");
		}

		
		protected virtual ViewportMaterialView _getView()
		{
			if (this._viewOverride != null)
			{
				return this._viewOverride;
			}
			return this._getDefaultView();
		}

		
		private void _init(float width, float height, WebViewOptions options = default(WebViewOptions), IWebView initializedWebView = null)
		{
			this._throwExceptionIfInitialized();
			this._options = options;
			this._resetLocalScale();
			IWebView webView = (initializedWebView == null) ? Web.CreateWebView(this._options.preferredPlugins) : initializedWebView;
			MonoBehaviour monoBehaviour = webView as MonoBehaviour;
			if (monoBehaviour != null)
			{
				this._webViewGameObject = monoBehaviour.gameObject;
			}
			this._attachWebViewEventHandlers(webView);
			if (this.InitialResolution <= 0f)
			{
				Debug.LogWarningFormat("Invalid value for InitialResolution ({0}) will be ignored.", new object[]
				{
					this.InitialResolution
				});
			}
			else if (this.InitialResolution != 1300f)
			{
				if (webView.IsInitialized)
				{
					Debug.LogWarning("Custom InitialResolution setting will be ignored because an initialized IWebView was provided.");
				}
				else
				{
					webView.SetResolution(this.InitialResolution);
				}
			}
			this._viewResizer = base.transform.GetChild(0);
			this._setViewSize(width, height);
			this._initView();
			Web.CreateMaterial(delegate(Material viewMaterial)
			{
				this._viewMaterial = viewMaterial;
				this._view.Material = viewMaterial;
				this._initWebViewIfReady(webView);
			});
			this._videoRectPositioner = this._getVideoRectPositioner();
			this._initVideoLayer();
			if (options.disableVideo)
			{
				this._videoLayerDisabled = true;
				this._videoRectPositioner.gameObject.SetActive(false);
				this._initWebViewIfReady(webView);
				return;
			}
			Web.CreateVideoMaterial(delegate(Material videoMaterial)
			{
				if (videoMaterial == null)
				{
					this._videoLayerDisabled = true;
					this._videoRectPositioner.gameObject.SetActive(false);
				}
				else
				{
					this._videoMaterial = videoMaterial;
					this._videoLayer.Material = videoMaterial;
					this._setVideoRect(new Rect(0f, 0f, 0f, 0f));
				}
				this._initWebViewIfReady(webView);
			});
		}

		
		private void _initPointerInputDetector(IWebView webView, IPointerInputDetector previousPointerInputDetector = null)
		{
			if (previousPointerInputDetector != null)
			{
				previousPointerInputDetector.BeganDrag -= this.InputDetector_BeganDrag;
				previousPointerInputDetector.Dragged -= this.InputDetector_Dragged;
				previousPointerInputDetector.PointerDown -= this.InputDetector_PointerDown;
				previousPointerInputDetector.PointerExited -= this.InputDetector_PointerExited;
				previousPointerInputDetector.PointerMoved -= this.InputDetector_PointerMoved;
				previousPointerInputDetector.PointerUp -= this.InputDetector_PointerUp;
				previousPointerInputDetector.Scrolled -= this.InputDetector_Scrolled;
			}
			if (this._pointerInputDetector == null)
			{
				this._pointerInputDetector = this._viewResizer.GetComponentInChildren<IPointerInputDetector>();
			}
			this._pointerInputDetector.PointerMovedEnabled = (webView is IWithMovablePointer);
			this._pointerInputDetector.BeganDrag += this.InputDetector_BeganDrag;
			this._pointerInputDetector.Dragged += this.InputDetector_Dragged;
			this._pointerInputDetector.PointerDown += this.InputDetector_PointerDown;
			this._pointerInputDetector.PointerExited += this.InputDetector_PointerExited;
			this._pointerInputDetector.PointerMoved += this.InputDetector_PointerMoved;
			this._pointerInputDetector.PointerUp += this.InputDetector_PointerUp;
			this._pointerInputDetector.Scrolled += this.InputDetector_Scrolled;
		}

		
		private void _initVideoLayer()
		{
			this._videoLayer = this._getVideoLayer();
		}

		
		private void _initView()
		{
			this._view = this._getView();
		}

		
		private void _initWebViewIfReady(IWebView webView)
		{
			if (this._view.Texture == null || (!this._videoLayerDisabled && this._videoLayer.Texture == null))
			{
				return;
			}
			bool isInitialized = webView.IsInitialized;
			if (isInitialized)
			{
				this._view.Texture = webView.Texture;
				this._videoLayer.Texture = webView.VideoTexture;
			}
			else
			{
				webView.Init(this._view.Texture, this._viewResizer.localScale.x, this._viewResizer.localScale.y, this._videoLayer.Texture);
			}
			this._initPointerInputDetector(webView, null);
			this._webView = webView;
			EventHandler initialized = this.Initialized;
			if (initialized != null)
			{
				initialized(this, EventArgs.Empty);
			}
			if (!string.IsNullOrEmpty(this.InitialUrl))
			{
				if (isInitialized)
				{
					Debug.LogWarning("Custom InitialUrl value will be ignored because an initialized webview was provided.");
					return;
				}
				string text = this.InitialUrl.Trim();
				if (!text.Contains(":"))
				{
					text = "http://" + text;
				}
				webView.LoadUrl(text);
			}
		}

		
		private void InputDetector_BeganDrag(object sender, EventArgs<Vector2> eventArgs)
		{
			this._previousDragPoint = this._convertRatioPointToUnityUnits(this._pointerDownRatioPoint);
		}

		
		private void InputDetector_Dragged(object sender, EventArgs<Vector2> eventArgs)
		{
			if (this.DragMode == DragMode.Disabled || this._webView == null)
			{
				return;
			}
			Vector2 value = eventArgs.Value;
			Vector2 previousDragPoint = this._previousDragPoint;
			Vector2 vector = this._convertRatioPointToUnityUnits(value);
			this._previousDragPoint = vector;
			Vector2 vector2 = this._convertRatioPointToUnityUnits(this._pointerDownRatioPoint) - vector;
			if (this.DragMode == DragMode.DragWithinPage)
			{
				if (vector2.magnitude * this._webView.Resolution > this.DragThreshold)
				{
					this._movePointerIfNeeded(value);
				}
				return;
			}
			Vector2 scrollDelta = previousDragPoint - vector;
			this._scrollIfNeeded(scrollDelta, this._pointerDownRatioPoint);
			if (this._clickIsPending && vector2.magnitude * this._webView.Resolution > this.DragThreshold)
			{
				this._clickIsPending = false;
			}
		}

		
		protected virtual void InputDetector_PointerDown(object sender, PointerEventArgs eventArgs)
		{
			this._pointerIsDown = true;
			this._pointerDownRatioPoint = eventArgs.Point;
			if (!this.ClickingEnabled || this._webView == null)
			{
				return;
			}
			if (this.DragMode == DragMode.DragWithinPage)
			{
				IWithPointerDownAndUp withPointerDownAndUp = this._webView as IWithPointerDownAndUp;
				if (withPointerDownAndUp != null)
				{
					withPointerDownAndUp.PointerDown(eventArgs.Point, eventArgs.ToPointerOptions());
					return;
				}
				if (!this._loggedDragWarning)
				{
					this._loggedDragWarning = true;
					Debug.LogWarningFormat("The WebViewPrefab's DragMode is set to DragWithinPage, but the webview implementation for this platform ({0}) doesn't support the PointerDown and PointerUp methods needed for dragging within a page. For more info, see https://developer.vuplex.com/webview/IWithPointerDownAndUp .", new object[]
					{
						this._webView.PluginType
					});
				}
			}
			this._clickIsPending = true;
		}

		
		private void InputDetector_PointerExited(object sender, EventArgs eventArgs)
		{
			if (this.HoveringEnabled)
			{
				this._movePointerIfNeeded(Vector2.zero);
			}
		}

		
		private void InputDetector_PointerMoved(object sender, EventArgs<Vector2> eventArgs)
		{
			if (this._pointerIsDown || !this.HoveringEnabled)
			{
				return;
			}
			this._movePointerIfNeeded(eventArgs.Value);
		}

		
		protected virtual void InputDetector_PointerUp(object sender, PointerEventArgs eventArgs)
		{
			this._pointerIsDown = false;
			if (!this.ClickingEnabled || this._webView == null)
			{
				return;
			}
			IWithPointerDownAndUp withPointerDownAndUp = this._webView as IWithPointerDownAndUp;
			if (this.DragMode == DragMode.DragWithinPage && withPointerDownAndUp != null)
			{
				Vector2 point = ((this._convertRatioPointToUnityUnits(this._pointerDownRatioPoint) - this._convertRatioPointToUnityUnits(eventArgs.Point)).magnitude * this._webView.Resolution > this.DragThreshold) ? eventArgs.Point : this._pointerDownRatioPoint;
				withPointerDownAndUp.PointerUp(point, eventArgs.ToPointerOptions());
			}
			else
			{
				if (!this._clickIsPending)
				{
					return;
				}
				this._clickIsPending = false;
				if (withPointerDownAndUp == null || this._options.clickWithoutStealingFocus)
				{
					this._webView.Click(eventArgs.Point, this._options.clickWithoutStealingFocus);
				}
				else
				{
					PointerOptions options = eventArgs.ToPointerOptions();
					withPointerDownAndUp.PointerDown(eventArgs.Point, options);
					withPointerDownAndUp.PointerUp(eventArgs.Point, options);
				}
			}
			EventHandler<ClickedEventArgs> clicked = this.Clicked;
			if (clicked != null)
			{
				clicked(this, new ClickedEventArgs(eventArgs.Point));
			}
		}

		
		private void InputDetector_Scrolled(object sender, ScrolledEventArgs eventArgs)
		{
			Vector2 scrollDelta = new Vector2(eventArgs.ScrollDelta.x * this.ScrollingSensitivity, eventArgs.ScrollDelta.y * this.ScrollingSensitivity);
			this._scrollIfNeeded(scrollDelta, eventArgs.Point);
		}

		
		private void _movePointerIfNeeded(Vector2 point)
		{
			IWithMovablePointer withMovablePointer = this._webView as IWithMovablePointer;
			if (withMovablePointer == null)
			{
				return;
			}
			if (point != this._previousMovePointerPoint)
			{
				this._previousMovePointerPoint = point;
				withMovablePointer.MovePointer(point);
			}
		}

		
		private void OnDestroy()
		{
			if (this._webView != null && !this._webView.IsDisposed)
			{
				this._webView.Dispose();
			}
			this.Destroy();
			if (this._viewMaterial != null)
			{
				UnityEngine.Object.Destroy(this._viewMaterial.mainTexture);
				UnityEngine.Object.Destroy(this._viewMaterial);
			}
			if (this._videoMaterial != null)
			{
				UnityEngine.Object.Destroy(this._videoMaterial.mainTexture);
				UnityEngine.Object.Destroy(this._videoMaterial);
			}
		}

		
		private void _reinitAfterScriptsReloaded()
		{
			if (this._webViewGameObject == null)
			{
				return;
			}
			IWebView componentInChildren = this._webViewGameObject.GetComponentInChildren<IWebView>();
			this._attachWebViewEventHandlers(componentInChildren);
			this._initView();
			this._initVideoLayer();
			this._initPointerInputDetector(componentInChildren, null);
			this._webView = componentInChildren;
		}

		
		private void _resetLocalScale()
		{
			Vector3 localScale = base.transform.localScale;
			Vector3 localPosition = base.transform.localPosition;
			base.transform.localScale = new Vector3(1f, 1f, localScale.z);
			float x = 0.5f * localScale.x;
			base.transform.localPosition = base.transform.localPosition + Quaternion.Euler(base.transform.localEulerAngles) * new Vector3(x, 0f, 0f);
		}

		
		private void _scrollIfNeeded(Vector2 scrollDelta, Vector2 point)
		{
			if (!this.ScrollingEnabled || this._webView == null || scrollDelta == Vector2.zero)
			{
				return;
			}
			this._webView.Scroll(scrollDelta, point);
			EventHandler<ScrolledEventArgs> scrolled = this.Scrolled;
			if (scrolled != null)
			{
				scrolled(this, new ScrolledEventArgs(scrollDelta, point));
			}
		}

		
		private void _setVideoRect(Rect videoRect)
		{
			this._view.SetCutoutRect(videoRect);
			this._videoRectPositioner.localPosition = new Vector3(1f - (videoRect.x + videoRect.width), -1f * videoRect.y, this._videoRectPositioner.localPosition.z);
			this._videoRectPositioner.localScale = new Vector3(videoRect.width, videoRect.height, this._videoRectPositioner.localScale.z);
			float xmin = Math.Max(0f, -1f * videoRect.x / videoRect.width);
			float ymin = Math.Max(0f, -1f * videoRect.y / videoRect.height);
			float xmax = Math.Min(1f, (1f - videoRect.x) / videoRect.width);
			float ymax = Math.Min(1f, (1f - videoRect.y) / videoRect.height);
			Rect rect = Rect.MinMaxRect(xmin, ymin, xmax, ymax);
			if (rect == new Rect(0f, 0f, 1f, 1f))
			{
				rect = new Rect(0f, 0f, 0f, 0f);
			}
			this._videoLayer.SetCropRect(rect);
		}

		
		private void _setViewSize(float width, float height)
		{
			float z = this._viewResizer.localScale.z;
			this._viewResizer.localScale = new Vector3(width, height, z);
			Vector3 localPosition = this._viewResizer.localPosition;
			localPosition.x = width * -0.5f;
			this._viewResizer.localPosition = localPosition;
		}

		
		private void _throwExceptionIfInitialized()
		{
			if (this._webView != null)
			{
				throw new InvalidOperationException("Init() cannot be called on a WebViewPrefab that has already been initialized.");
			}
		}

		
		[Label("Initial URL to load (optional)")]
		[Tooltip("Or you can leave the Initial URL blank if you want to initialize the prefab programmatically by calling Init().")]
		public string InitialUrl;

		
		[Label("Initial Resolution (px / Unity unit)")]
		[Tooltip("You can change this to make web content appear larger or smaller.")]
		public float InitialResolution = 1300f;

		
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

		
		public float ScrollingSensitivity = 0.005f;

		
		private Vector2 _pointerDownRatioPoint;

		
		private bool _clickIsPending;

		
		private bool _loggedDragWarning;

		
		private WebViewOptions _options;

		
		private IPointerInputDetector _pointerInputDetector;

		
		private bool _pointerIsDown;

		
		private Vector2 _previousDragPoint;

		
		private Vector2 _previousMovePointerPoint;

		
		private ViewportMaterialView _videoLayer;

		
		private bool _videoLayerDisabled;

		
		private Material _videoMaterial;

		
		private Transform _videoRectPositioner;

		
		protected ViewportMaterialView _view;

		
		private Material _viewMaterial;

		
		private ViewportMaterialView _viewOverride;

		
		private Transform _viewResizer;

		
		protected IWebView _webView;

		
		private GameObject _webViewGameObject;
	}
}
