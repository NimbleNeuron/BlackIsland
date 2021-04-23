using System;
using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	[RequireComponent(typeof(Canvas))]
	[RequireComponent(typeof(CanvasGroup))]
	[RequireComponent(typeof(GraphicRaycaster))]
	public class BaseWindow : BaseUI, IPointerClickHandler, IEventSystemHandler, IDropHandler, IDragHandler
	{
		private static readonly List<BaseWindow> _windowStack = new List<BaseWindow>();


		[SerializeField] protected UIBackShade backShade;


		public float FadeSpeed;


		private CanvasGroup _canvasGroup;


		private GraphicRaycaster _graphicRaycaster;


		private bool _isOpen;


		private Canvas canvas;


		private Coroutine fadeinCoroutine;


		private Coroutine fadeoutCoroutine;


		public static BaseWindow FocusedWindow => GetFocusedWindow();


		public CanvasGroup CanvasGroup => _canvasGroup;


		protected bool IsFocused => this == FocusedWindow;


		public bool IsOpen => _isOpen;


		public void OnDrag(PointerEventData eventData) { }


		public virtual void OnDrop(PointerEventData eventData)
		{
			PushWindowStack(this);
		}


		public virtual void OnPointerClick(PointerEventData eventData)
		{
			PushWindowStack(this);
		}


		private static BaseWindow GetFocusedWindow()
		{
			if (_windowStack.Count > 0)
			{
				return _windowStack[_windowStack.Count - 1];
			}

			return null;
		}


		private static void PushWindowStack(BaseWindow window)
		{
			if (window != null && GetFocusedWindow() != window)
			{
				_windowStack.Remove(window);
				_windowStack.Add(window);
				window.FocusWindow();
			}
		}


		public virtual bool IgnoreEscapeInputWindow()
		{
			return false;
		}


		private static void RemoveWindowStack(BaseWindow window)
		{
			_windowStack.Remove(window);
		}


		
		
		public event Action OnOpenEvent;


		
		
		public event Action OnCloseEvent;


		protected virtual void OnOpen() { }


		protected virtual void OnClose() { }


		protected void FocusWindow()
		{
			transform.SetSiblingIndex(int.MaxValue);
		}


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			if (backShade != null)
			{
				backShade.gameObject.SetActive(true);
			}

			_canvasGroup = GetComponent<CanvasGroup>();
			_graphicRaycaster = GetComponent<GraphicRaycaster>();
			canvas = GetComponent<Canvas>();
			FadeSpeed = 10f;
			_canvasGroup.alpha = 0f;
			_canvasGroup.interactable = false;
			_canvasGroup.blocksRaycasts = false;
			if (_graphicRaycaster != null)
			{
				_graphicRaycaster.enabled = false;
			}

			_isOpen = false;
			gameObject.SetActive(true);
		}


		public void OnFocused()
		{
			if (fadeoutCoroutine != null)
			{
				return;
			}

			Open();
		}


		public virtual void Open()
		{
			if (!gameObject.activeSelf)
			{
				Log.E("'" + gameObject.name + "' is not active window");
				return;
			}

			if (fadeoutCoroutine != null)
			{
				StopCoroutine(fadeoutCoroutine);
			}

			if (fadeinCoroutine != null)
			{
				StopCoroutine(fadeinCoroutine);
			}

			PushWindowStack(this);
			if (!_isOpen)
			{
				_isOpen = true;
				OnOpen();
				if (backShade != null)
				{
					backShade.Resolution();
				}

				Action onOpenEvent = OnOpenEvent;
				if (onOpenEvent != null)
				{
					onOpenEvent();
				}

				_canvasGroup.alpha = 1f;
				_canvasGroup.interactable = true;
				_canvasGroup.blocksRaycasts = true;
				if (_graphicRaycaster != null)
				{
					_graphicRaycaster.enabled = true;
				}
			}

			fadeinCoroutine = this.StartThrowingCoroutine(FadeIn(delegate { fadeinCoroutine = null; }),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][FadeIn] Message:" + exception.Message + ", StackTrace:" + exception.StackTrace);
				});
		}


		public virtual void Close()
		{
			if (!gameObject.activeSelf)
			{
				Log.E("'" + gameObject.name + "' is not active window");
				return;
			}

			if (fadeoutCoroutine != null)
			{
				StopCoroutine(fadeoutCoroutine);
			}

			if (fadeinCoroutine != null)
			{
				StopCoroutine(fadeinCoroutine);
			}

			if (_isOpen)
			{
				_isOpen = false;
				OnClose();
				Action onCloseEvent = OnCloseEvent;
				if (onCloseEvent != null)
				{
					onCloseEvent();
				}
			}

			RemoveWindowStack(this);
			fadeoutCoroutine = this.StartThrowingCoroutine(FadeOut(delegate
				{
					fadeoutCoroutine = null;
					_canvasGroup.alpha = 0f;
					_canvasGroup.interactable = false;
					_canvasGroup.blocksRaycasts = false;
					if (_graphicRaycaster != null)
					{
						_graphicRaycaster.enabled = false;
					}
				}),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][FadeOut] Message:" + exception.Message + ", StackTrace:" + exception.StackTrace);
				});
		}


		public void CustomWindowClose()
		{
			if (MonoBehaviourInstance<LobbyUI>.inst != null)
			{
				MonoBehaviourInstance<LobbyUI>.inst.CustomModeWindow.Exit();
			}
		}


		public void ToggleWindow()
		{
			if (IsOpen)
			{
				Close();
				return;
			}

			Open();
		}


		private IEnumerator FadeIn(Action callBack = null)
		{
			while (_canvasGroup.alpha < 1f)
			{
				_canvasGroup.alpha += FadeSpeed * Time.deltaTime;
				yield return null;
			}

			if (callBack != null)
			{
				callBack();
			}
		}


		private IEnumerator FadeOut(Action callBack = null)
		{
			while (_canvasGroup.alpha > 0f)
			{
				_canvasGroup.alpha -= FadeSpeed * Time.deltaTime;
				yield return null;
			}

			if (callBack != null)
			{
				callBack();
			}
		}
	}
}