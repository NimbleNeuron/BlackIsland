using System;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class CharacterDetailSkin : BasePage
	{
		[SerializeField] private GameObject cloneTarget = default;


		private readonly List<CharacterSelectSkinSlot> slot = new List<CharacterSelectSkinSlot>();


		private int characterCode;


		private Button purchaseButton = default;


		private Text purchaseInfo = default;


		private List<CharacterSkinData> resultData = new List<CharacterSkinData>();


		private ScrollRect scrollRect = default;


		private RectTransform scrollViewParent = default;


		private CharacterSkinData selectData;


		private int skinCharacterCode;


		private int skinCode;


		private Text skinDesc = default;


		private int skinIndex;


		private Text skinTitle = default;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			Bind();
			AddEvent();
		}


		private void Bind()
		{
			purchaseInfo = GameUtil.Bind<Text>(gameObject, "Buy/TXT_BuyInfo");
			purchaseButton = GameUtil.Bind<Button>(gameObject, "Buy/Btn_SkinBuy");
			scrollRect = GameUtil.Bind<ScrollRect>(gameObject, "SkinList/SkinScrollView");
			scrollViewParent = GameUtil.Bind<RectTransform>(gameObject, "SkinList/SkinScrollView/Viewport/Content");
			skinTitle = GameUtil.Bind<Text>(gameObject, "SkinName");
			skinDesc = GameUtil.Bind<Text>(gameObject, "StoryDesc/Viewport/Content/Desc");
		}


		private void AddEvent()
		{
			purchaseButton.onClick.AddListener(OnClickPurchase);
		}


		protected override void OnOpenPage()
		{
			base.OnOpenPage();
			Refresh();
		}


		protected override void OnClosePage()
		{
			base.OnClosePage();
		}


		public void SetData(int characterCode, int skinCode)
		{
			this.characterCode = characterCode;
			this.skinCode = skinCode;
		}


		public void Refresh()
		{
			InitData();
			InitInfo();
			InitModel();
			RefreshCell();
		}


		private void InitData()
		{
			if (skinCode < 0)
			{
				List<CharacterSkinData> skinDataList = GameDB.character.GetSkinDataList(characterCode);
				if (skinDataList.Count > 0)
				{
					skinCode = skinDataList[0].code;
				}
			}

			selectData = GameDB.character.GetSkinData(skinCode);
		}


		private void InitInfo()
		{
			switch (selectData.purchaseType)
			{
				case SkinPurchaseType.FREE:
					purchaseInfo.gameObject.SetActive(true);
					purchaseInfo.text = Ln.Get("기본으로 지급되는 스킨");
					goto IL_AF;
				case SkinPurchaseType.EVENT_REWARD:
					purchaseInfo.gameObject.SetActive(true);
					purchaseInfo.text = Ln.Get("스킨을 이벤트로 획득 가능");
					goto IL_AF;
				case SkinPurchaseType.BATTLE_PASS:
					purchaseInfo.gameObject.SetActive(true);
					purchaseInfo.text = Ln.Get("스킨을 배틀패스 구매 가능");
					goto IL_AF;
			}

			purchaseInfo.gameObject.SetActive(false);
			IL_AF:
			purchaseButton.gameObject.SetActive(!Lobby.inst.IsHaveSkin(skinCode));
			skinTitle.text = LnUtil.GetSkinName(skinCode);
			skinDesc.text = LnUtil.GetSkinDesc(skinCode);
		}


		private void InitModel()
		{
			if (skinCharacterCode == selectData.characterCode && skinIndex == selectData.index)
			{
				return;
			}

			SingletonMonoBehaviour<LobbyCharacterStation>.inst.LoadCharacter(skinCharacterCode = characterCode,
				skinIndex = selectData.index);
		}


		private void RefreshCell()
		{
			resultData = GameDB.character.GetSkinDataList(characterCode);
			if (resultData.Count > slot.Count)
			{
				int num = resultData.Count - slot.Count;
				for (int i = 0; i < num; i++)
				{
					GameObject gameObject = Instantiate<GameObject>(cloneTarget, scrollViewParent);
					CharacterSelectSkinSlot characterSelectSkinSlot =
						gameObject != null ? gameObject.GetComponent<CharacterSelectSkinSlot>() : null;
					if (characterSelectSkinSlot != null)
					{
						characterSelectSkinSlot.SetScrollRect(scrollRect);
						CharacterSelectSkinSlot characterSelectSkinSlot2 = characterSelectSkinSlot;
						characterSelectSkinSlot2.selectCallback =
							(CharacterSelectSkinSlot.SelectCallback) Delegate.Combine(
								characterSelectSkinSlot2.selectCallback,
								new CharacterSelectSkinSlot.SelectCallback(OnClickCell));
						slot.Add(characterSelectSkinSlot);
					}
				}
			}

			for (int j = 0; j < slot.Count; j++)
			{
				bool flag = j < resultData.Count;
				CharacterSelectSkinSlot characterSelectSkinSlot3 = slot[j];
				characterSelectSkinSlot3.gameObject.SetActive(flag);
				if (flag)
				{
					CharacterSkinData characterSkinData = resultData[j];
					int index = j;
					characterSelectSkinSlot3.SetSlot(characterSkinData, index);
					characterSelectSkinSlot3.SetNotReleaseableLock(!Lobby.inst.IsHaveSkin(characterSkinData.code));
					characterSelectSkinSlot3.OnSelectSlot(skinCode == characterSkinData.code);
				}
			}
		}


		private void OnClickCell(int slotIndex)
		{
			SetData(characterCode, resultData[slotIndex].code);
			Refresh();
		}


		private void OnClickPurchase()
		{
			ShopProductService.RequestShopSkinList(delegate
			{
				ShopProduct shopSkin = ShopProductService.GetShopSkin(skinCode);
				if (shopSkin != null)
				{
					MonoBehaviourInstance<LobbyUI>.inst.ShopProductWindow.Open();
					MonoBehaviourInstance<LobbyUI>.inst.ShopProductWindow.SetProduct(shopSkin);
					ShopProductWindow shopProductWindow = MonoBehaviourInstance<LobbyUI>.inst.ShopProductWindow;
					shopProductWindow.buySuccessCallback = (ShopProductWindow.BuySuccessCallback) Delegate.Combine(
						shopProductWindow.buySuccessCallback,
						new ShopProductWindow.BuySuccessCallback(delegate { Refresh(); }));
					MonoBehaviourInstance<LobbyUI>.inst.ShopProductWindow.noMoneyOpenShopCallback = delegate
					{
						MonoBehaviourInstance<LobbyUI>.inst.GetLobbyTab<LobbyShopTab>(LobbyTab.ShopTab)
							.NoMoneyOpenShopCallback();
					};
				}
			});
		}
	}
}