using Blis.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class TutorialTabButton : BaseControl
	{
		public delegate void OnClickTutorialTab(TutorialType tutorialType);


		public delegate void OnHoverTutorialTab(TutorialType tutorialType);


		public TutorialType type;


		public bool clear;


		public Button button;


		public Image img_completed;


		public Image img_Dimmed;


		private Image btnImage;


		public OnClickTutorialTab onClickTutorialTab;


		public OnHoverTutorialTab onEnterTutorialTab;


		public OnHoverTutorialTab onExitTutorialTab;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			btnImage = transform.GetComponent<Image>();
		}


		public void Select(Sprite selectSprite)
		{
			btnImage.sprite = selectSprite;
		}


		public void UnSelect(Sprite UnSelectSprite)
		{
			btnImage.sprite = UnSelectSprite;
		}


		public void Rollover(Sprite rolloverSprite)
		{
			btnImage.sprite = rolloverSprite;
		}


		public void ScaleUp()
		{
			button.transform.localScale = Vector3.one;
		}


		public void ScaleDown()
		{
			button.transform.localScale = new Vector3(0.95f, 0.95f, 1f);
		}


		public override void OnPointerClick(PointerEventData eventData)
		{
			base.OnPointerClick(eventData);
			onClickTutorialTab(type);
		}


		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);
			onEnterTutorialTab(type);
		}


		public override void OnPointerExit(PointerEventData eventData)
		{
			base.OnPointerExit(eventData);
			onExitTutorialTab(type);
		}
	}
}