using System;
using Blis.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class CharacterSelectWeaponSelectSlot : BaseUI
	{
		private Button btn;


		private CanvasAlphaTweener canvasAlphaTweener;


		private CanvasGroup canvasGroup;


		private ColorTweener colorTweener;


		private Coroutine coroutine;


		private RectTransform linkBottom;


		private RectTransform linkMiddle;


		private RectTransform linkTop;


		private ICharacterSelectWeaponListener listener;


		private Image lockImg;


		private Image select;


		private int startingDataCode;


		private Text weaponDesc;


		private Image weaponIcon;


		private Text weaponName;


		public int StartingDataCode => startingDataCode;


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			canvasGroup = GetComponent<CanvasGroup>();
			canvasGroup.alpha = 0f;
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
			canvasAlphaTweener = GetComponent<CanvasAlphaTweener>();
			canvasAlphaTweener.StopAnimation();
			colorTweener = GameUtil.Bind<ColorTweener>(gameObject, "WeaponBg");
			colorTweener.StopAnimation();
			weaponName = GameUtil.Bind<Text>(gameObject, "WeaponName");
			weaponDesc = GameUtil.Bind<Text>(gameObject, "WeaponDesc");
			weaponIcon = GameUtil.Bind<Image>(gameObject, "Weapon");
			lockImg = GameUtil.Bind<Image>(gameObject, "Lock");
			lockImg.enabled = false;
			btn = GameUtil.Bind<Button>(gameObject, "WeaponCardButton");
			btn.onClick.AddListener(OnClickWeapon);
			select = GameUtil.Bind<Image>(gameObject, "Select");
			select.enabled = false;
			linkTop = GameUtil.Bind<RectTransform>(gameObject, "LinkLine_1");
			linkMiddle = GameUtil.Bind<RectTransform>(gameObject, "LinkLine_2");
			linkBottom = GameUtil.Bind<RectTransform>(gameObject, "LinkLine_3");
		}


		public void SetWeapon(int startingDataCode, string weaponName, string weaponDesc, Sprite weaponSprite)
		{
			this.startingDataCode = startingDataCode;
			this.weaponName.text = weaponName;
			this.weaponDesc.text = weaponDesc;
			weaponIcon.sprite = weaponSprite;
			lockImg.enabled = false;
			btn.interactable = true;
		}


		public void SetListener(ICharacterSelectWeaponListener listener)
		{
			this.listener = listener;
		}


		public void Show(float delay)
		{
			Hide();
			coroutine = this.StartThrowingCoroutine(CoroutineUtil.DelayedAction(delay, delegate
				{
					canvasGroup.interactable = true;
					canvasGroup.blocksRaycasts = true;
					canvasAlphaTweener.StopAnimation();
					canvasAlphaTweener.PlayAnimation();
				}),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][CharacterWeaponSelectSlot] Message:" + exception.Message + ", StackTrace:" +
					      exception.StackTrace);
				});
		}


		public void Hide()
		{
			if (coroutine != null)
			{
				StopCoroutine(coroutine);
				coroutine = null;
			}

			canvasAlphaTweener.StopAnimation();
			canvasGroup.alpha = 0f;
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
		}


		public void Select()
		{
			select.enabled = true;
			colorTweener.StopAnimation();
			colorTweener.PlayAnimation();
		}


		public void Deselect()
		{
			select.enabled = false;
			colorTweener.StopAnimation();
		}


		public void SetLink(int index)
		{
			linkTop.transform.localScale = index == 0 ? Vector3.one : Vector3.zero;
			linkMiddle.transform.localScale = index == 1 ? Vector3.one : Vector3.zero;
			linkBottom.transform.localScale = index == 2 ? Vector3.one : Vector3.zero;
		}


		public void OnClickWeapon()
		{
			ICharacterSelectWeaponListener characterSelectWeaponListener = listener;
			if (characterSelectWeaponListener == null)
			{
				return;
			}

			characterSelectWeaponListener.OnClickWeapon(startingDataCode);
		}


		public void Lock()
		{
			if (select.enabled)
			{
				return;
			}

			btn.interactable = false;
			lockImg.enabled = true;
		}


		public void UnLock()
		{
			if (select.enabled)
			{
				return;
			}

			btn.interactable = true;
			lockImg.enabled = false;
		}
	}
}