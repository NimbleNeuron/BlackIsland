using System;
using Blis.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UIName : BaseTrackUI, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler,
		IPointerExitHandler
	{
		private readonly int CCFontSize = 18;


		private readonly Color clickableColor = new Color(0f, 0f, 0f, 0.8f);


		private readonly Color defaultColor = new Color(0f, 0f, 0f, 0f);


		private readonly Color hoverColor = Color.black;


		private int defaultFontSize;


		private Image dropItemRolloverBg;


		private Image image;


		private bool isItemUIName;


		private Text label;


		private Action leftClickCallback;


		private Image needMark;


		private Outline outline;


		private Action rightClickCallback;


		private Vector3 stickToOffset = Vector3.zero;


		private BaseTrackUI stickToTrackUI;

		protected override void Awake()
		{
			base.Awake();
			image = GetComponent<Image>();
			label = GameUtil.Bind<Text>(gameObject, "Text");
			outline = label.GetComponent<Outline>();
			outline.enabled = false;
			defaultFontSize = label.fontSize;
			dropItemRolloverBg = GameUtil.Bind<Image>(gameObject, "Selection");
			needMark = GameUtil.Bind<Image>(gameObject, "NeedMark");
		}


		public void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
			{
				if (eventData.button == PointerEventData.InputButton.Right)
				{
					Action action = rightClickCallback;
					if (action == null)
					{
						return;
					}

					action();
				}

				return;
			}

			Action action2 = leftClickCallback;
			if (action2 == null)
			{
				return;
			}

			action2();
		}


		public void OnPointerEnter(PointerEventData eventData)
		{
			if (isItemUIName)
			{
				ShowRolloverBg(true);
				return;
			}

			image.color = hoverColor;
		}


		public void OnPointerExit(PointerEventData eventData)
		{
			if (isItemUIName)
			{
				ShowRolloverBg(false);
				return;
			}

			if (image.raycastTarget)
			{
				image.color = clickableColor;
				return;
			}

			image.color = defaultColor;
		}


		public override void ResetUI()
		{
			base.ResetUI();
			SetName(null);
			leftClickCallback = null;
			rightClickCallback = null;
			image.raycastTarget = false;
			image.color = defaultColor;
			dropItemRolloverBg.color = defaultColor;
			needMark.color = defaultColor;
			isItemUIName = false;
			stickToTrackUI = null;
			stickToOffset = Vector3.zero;
			transform.localScale = Vector3.one;
		}


		public void SetLeftClickCallback(Action callback)
		{
			leftClickCallback = callback;
		}


		public void SetRightClickCallback(Action callback)
		{
			rightClickCallback = callback;
		}


		public string GetName()
		{
			return label.text;
		}


		public void SetName(string name)
		{
			label.text = name;
		}


		public void SetCCName(string name)
		{
			label.text = name;
			label.fontSize = CCFontSize;
			outline.enabled = true;
		}


		public void ResetCCName(string name)
		{
			label.text = name;
			label.fontSize = defaultFontSize;
			outline.enabled = false;
		}


		public void SetColor(Color color)
		{
			label.color = color;
		}


		public void EnableClick(bool enable)
		{
			image.raycastTarget = enable;
			if (!isItemUIName)
			{
				ShowBg(enable);
			}
		}


		public void ShowBg(bool enable)
		{
			image.color = enable ? clickableColor : defaultColor;
		}


		public void ShowNeedMark(bool enable)
		{
			needMark.color = enable ? Color.white : defaultColor;
		}


		public void ShowRolloverBg(bool enable)
		{
			dropItemRolloverBg.color = enable ? Color.white : defaultColor;
		}


		public void SetIsItemUIName(bool isItemUIName)
		{
			this.isItemUIName = isItemUIName;
		}


		public void SetStickToTrackUi(BaseTrackUI stickToTrackUI, Vector3 stickToOffset)
		{
			if (stickToTrackUI != null)
			{
				stickToTrackUI.OnUpdateUIFinish += StickToTrackUiOnUpdateUIFinish;
			}

			this.stickToTrackUI = stickToTrackUI;
			this.stickToOffset = stickToOffset;
		}


		private void StickToTrackUiOnUpdateUIFinish()
		{
			if (stickToTrackUI == null)
			{
				return;
			}

			if (!stickToTrackUI.enabled)
			{
				return;
			}

			if (!stickToTrackUI.gameObject.activeSelf)
			{
				return;
			}

			transform.position = stickToTrackUI.transform.position + stickToOffset;
		}


		protected override void UpdateUI()
		{
			if (stickToTrackUI != null)
			{
				return;
			}

			base.UpdateUI();
		}
	}
}