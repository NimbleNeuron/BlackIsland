using System;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class LobbyPortraitCharacterCard : LobbyCharacterCard
	{
		[SerializeField] private Button btn = default;


		[SerializeField] private CanvasGroup characterCanvasGroup = default;


		[SerializeField] private Text characterName = default;


		[SerializeField] private Transform objFocus = default;


		[SerializeField] private GameObject banMark = default;


		[SerializeField] private Text focusedCharacterName = default;


		[SerializeField] private Image characterSprite = default;


		[SerializeField] private Image focusedCharacterSprite = default;


		[SerializeField] private Transform[] freemMarks = default;


		[SerializeField] private Transform objBlank = default;


		[SerializeField] private Transform[] kakaoPcMarks = default;


		private readonly Color disableColor = new Color(0.2f, 0.2f, 0.2f);


		private bool ban = default;


		private int characterCode = default;


		private Image focusBg = default;


		private Image focusNameBg = default;


		private float focusScale = 1.05f;


		private Image focusSelect = default;


		private bool freeRotation = default;


		private bool have = default;


		private bool kakaoPc = default;


		private ICharacterSelectCardListener listener;


		public int CharacterCode => characterCode;


		private void Awake()
		{
			focusBg = objFocus.GetComponent<Image>();
			focusNameBg = GameUtil.Bind<Image>(objFocus.gameObject, "Name");
			focusSelect = GameUtil.Bind<Image>(objFocus.gameObject, "Image");
			btn.onClick.AddListener(OnClickEvent);
		}


		public void SetCharacterCode(int characterCode, bool have, bool freeRotation, bool ban, bool kakaoPc)
		{
			if (characterCode <= 0)
			{
				Blank();
				return;
			}

			this.ban = ban;
			this.kakaoPc = kakaoPc;
			SetCharacterCode(characterCode, have, freeRotation);
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


		public override void SetCharacterCode(int characterCode, bool have, bool freeRotation)
		{
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
		}


		public void SetListener(ICharacterSelectCardListener listener)
		{
			this.listener = listener;
		}


		public void SetFocusScale(float scale)
		{
			focusScale = scale;
		}


		private void Refresh()
		{
			UnBlank();
			if (ban)
			{
				SetBan(true);
				SetFreeRotation(false);
				SetKakaoPcMark(false);
				return;
			}

			bool flag = kakaoPc && !have;
			SetBan(false);
			SetKakaoPcMark(flag);
			SetFreeRotation(freeRotation && !flag);
		}


		public void SetBan(bool isBan)
		{
			ban = isBan;
			banMark.SetActive(isBan);
			btn.enabled = !isBan;
		}


		private void SetFreeRotation(bool enable)
		{
			Transform[] array = freemMarks;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].localScale = enable ? Vector3.one : Vector3.zero;
			}
		}


		private void SetKakaoPcMark(bool enable)
		{
			Transform[] array = kakaoPcMarks;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].localScale = enable ? Vector3.one : Vector3.zero;
			}
		}


		private void Blank()
		{
			transform.localScale = Vector3.zero;
			if (objBlank != null)
			{
				objBlank.localScale = Vector3.one;
			}

			characterCanvasGroup.blocksRaycasts = false;
			characterCanvasGroup.alpha = 0f;
		}


		private void UnBlank()
		{
			transform.localScale = Vector3.one;
			if (objBlank != null)
			{
				objBlank.localScale = Vector3.zero;
			}

			characterCanvasGroup.blocksRaycasts = true;
			characterCanvasGroup.alpha = 1f;
		}


		public void Focus(bool focus)
		{
			objFocus.transform.localScale = focus ? Vector3.one * focusScale : Vector3.zero;
		}


		public void SetFocusColor(int pickTeamSlot, bool isPicked)
		{
			Color teamColor = GameConstants.TeamMode.GetTeamColor(pickTeamSlot);
			Color color = Color.white;
			color = isPicked ? (teamColor + disableColor) * 0.5f : teamColor;
			focusNameBg.color = color;
			focusSelect.color = color;
			focusBg.color = isPicked ? disableColor : Color.white;
			focusedCharacterSprite.color = isPicked ? disableColor : Color.white;
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


		public void OnPointerClickBankMark(BaseEventData eventData)
		{
			MonoBehaviourInstance<LobbyUI>.inst.ToastMessage.ShowMessage(Ln.Get("시스템 벤 캐릭터"));
		}


		public override void OnLnDataChange()
		{
			SetCharacterCode(characterCode, have, freeRotation);
		}
	}
}