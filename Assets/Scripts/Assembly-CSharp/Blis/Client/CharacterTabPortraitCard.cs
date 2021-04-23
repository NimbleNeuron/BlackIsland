using System;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class CharacterTabPortraitCard : LobbyCharacterCard, IPointerEnterHandler, IEventSystemHandler,
		IPointerExitHandler
	{
		private readonly Color disableColor = new Color(0.2f, 0.2f, 0.2f);


		private Button characterButton;


		private CanvasGroup characterCanvasGroup;


		private int characterCode;


		private Text characterName;


		private Image characterSprite;


		private Image focusBg;


		private Text focusedCharacterName;


		private Image focusedCharacterSprite;


		private Image focusNameBg;


		private Transform[] freemMarks;


		private bool freeRotation;


		private bool have;


		private ICharacterSelectCardListener listener;


		private Transform objFocus;


		private Transform[] objLocks;


		public int CharacterCode => characterCode;


		private void Awake()
		{
			characterButton = GameUtil.Bind<Button>(gameObject, "CharacterButton");
			characterName = GameUtil.Bind<Text>(characterButton.gameObject, "NameBg/Label");
			characterSprite = GameUtil.Bind<Image>(characterButton.gameObject, "Cha");
			GameUtil.Bind<CanvasGroup>(characterButton.gameObject, ref characterCanvasGroup);
			objFocus = GameUtil.Bind<Transform>(characterButton.gameObject, "OBJ_Focus");
			focusNameBg = GameUtil.Bind<Image>(objFocus.gameObject, "Name");
			focusedCharacterName = GameUtil.Bind<Text>(focusNameBg.gameObject, "Label");
			focusedCharacterSprite = GameUtil.Bind<Image>(objFocus.gameObject, "Cha");
			GameUtil.Bind<Image>(objFocus.gameObject, ref focusBg);
			objLocks = new Transform[2];
			objLocks[0] = GameUtil.Bind<Transform>(characterButton.gameObject, "OBJ_Lock");
			objLocks[1] = GameUtil.Bind<Transform>(objFocus.gameObject, "OBJ_Lock");
			freemMarks = new Transform[2];
			freemMarks[0] = GameUtil.Bind<Transform>(characterButton.gameObject, "FreeMark");
			freemMarks[1] = GameUtil.Bind<Transform>(objFocus.gameObject, "FreeMark");
			characterButton.onClick.AddListener(OnClickEvent);
		}


		private void OnDisable()
		{
			Focus(false);
		}


		public void OnPointerEnter(PointerEventData eventData)
		{
			Focus(true);
		}


		public void OnPointerExit(PointerEventData eventData)
		{
			Focus(false);
		}


		public override void SetCharacterCode(int characterCode, bool have, bool freeRotation)
		{
			if (characterCode <= 0)
			{
				Blank();
				return;
			}

			this.characterCode = characterCode;
			this.have = have;
			this.freeRotation = freeRotation;
			string text = Ln.Get(LnType.Character_Name, characterCode.ToString());
			Sprite characterLobbyPortraitSprite =
				SingletonMonoBehaviour<ResourceManager>.inst.GetCharacterLobbyPortraitSprite(characterCode);
			characterName.text = text;
			focusedCharacterName.text = text;
			characterSprite.sprite = characterLobbyPortraitSprite;
			focusedCharacterSprite.sprite = characterLobbyPortraitSprite;
			Refresh();
			ShopProductWindow shopProductWindow = MonoBehaviourInstance<LobbyUI>.inst.ShopProductWindow;
			shopProductWindow.buySuccessCallback = (ShopProductWindow.BuySuccessCallback) Delegate.Combine(
				shopProductWindow.buySuccessCallback, new ShopProductWindow.BuySuccessCallback(delegate(object data)
				{
					if ((int) data != characterCode)
					{
						return;
					}

					this.have = true;
					Refresh();
				}));
		}


		public void SetListener(ICharacterSelectCardListener listener)
		{
			this.listener = listener;
		}


		public override void OnLnDataChange()
		{
			SetCharacterCode(characterCode, have, freeRotation);
		}


		private void Refresh()
		{
			UnBlank();
			transform.localScale = Vector3.one;
			characterCanvasGroup.blocksRaycasts = true;
			characterCanvasGroup.alpha = 1f;
			SetFreeRotation();
			SetLockState();
		}


		private void SetFreeRotation()
		{
			Transform[] array = freemMarks;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].localScale = freeRotation ? Vector3.one : Vector3.zero;
			}
		}


		private void SetLockState()
		{
			bool flag = freeRotation || have;
			Transform[] array = objLocks;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].localScale = flag ? Vector3.zero : Vector3.one;
			}
		}


		private void Blank()
		{
			transform.localScale = Vector3.zero;
			characterCanvasGroup.blocksRaycasts = false;
			characterCanvasGroup.alpha = 0f;
		}


		private void UnBlank()
		{
			transform.localScale = Vector3.one;
			characterCanvasGroup.blocksRaycasts = true;
			characterCanvasGroup.alpha = 1f;
		}


		private void OnClickEvent()
		{
			ICharacterSelectCardListener characterSelectCardListener = listener;
			if (characterSelectCardListener == null)
			{
				return;
			}

			characterSelectCardListener.OnClickCharacterCard(characterCode);
		}


		public void SetFocusColor(int pickTeamSlot, bool isPicked)
		{
			Color teamColor = GameConstants.TeamMode.GetTeamColor(pickTeamSlot);
			focusNameBg.color = isPicked ? (teamColor + disableColor) * 0.5f : teamColor;
			focusBg.color = isPicked ? disableColor : Color.white;
			focusedCharacterSprite.color = isPicked ? disableColor : Color.white;
		}


		private void Focus(bool focus)
		{
			objFocus.gameObject.SetActive(focus);
		}
	}
}