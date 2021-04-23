using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Blis.Client
{
	public class LobbyShopTab : LobbyTabBaseUI, ILobbyTab
	{
		[SerializeField] private ShopCharacter shopCharacter = default;


		[FormerlySerializedAs("shopMoney")] [SerializeField]
		private ShopAsset shopAsset = default;


		private ShopState currentShopState = default;


		private Toggle menuCharacter = default;


		private Toggle menuDlc = default;


		private Toggle menuEtc = default;


		private Toggle menuMoney = default;


		private Toggle menuSkin = default;


		private Toggle menuTwitchDrops = default;


		private ShopDLC shopDlc = default;


		private ShopEtc shopEtc = default;


		private ShopSkin shopSkin = default;


		private ShopTwitchDrops shopTwitchDrops = default;


		public void OnOpen(LobbyTab from)
		{
			EnableCanvas(true);
			OpenShop(currentShopState);
		}


		public TabCloseResult OnClose(LobbyTab to)
		{
			EnableCanvas(false);
			return TabCloseResult.Success;
		}

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			menuCharacter = GameUtil.Bind<Toggle>(gameObject, "Submenu/MenuCharacter");
			menuSkin = GameUtil.Bind<Toggle>(gameObject, "Submenu/MenuSkin");
			menuMoney = GameUtil.Bind<Toggle>(gameObject, "Submenu/MenuMoney");
			menuEtc = GameUtil.Bind<Toggle>(gameObject, "Submenu/MenuEtc");
			menuDlc = GameUtil.Bind<Toggle>(gameObject, "Submenu/MenuDlc");
			menuTwitchDrops = GameUtil.Bind<Toggle>(gameObject, "Submenu/MenuTwitchDrops");
			shopSkin = GameUtil.Bind<ShopSkin>(gameObject, "ShopSkin");
			shopEtc = GameUtil.Bind<ShopEtc>(gameObject, "ShopEtc");
			shopDlc = GameUtil.Bind<ShopDLC>(gameObject, "ShopDlc");
			shopTwitchDrops = GameUtil.Bind<ShopTwitchDrops>(gameObject, "ShopTwitchDrops");
			menuCharacter.onValueChanged.AddListener(delegate(bool isOn)
			{
				OnToggleChange(isOn, ShopState.CHARACTER);
			});
			menuSkin.onValueChanged.AddListener(delegate(bool isOn) { OnToggleChange(isOn, ShopState.SKIN); });
			menuMoney.onValueChanged.AddListener(delegate(bool isOn) { OnToggleChange(isOn, ShopState.MONEY); });
			menuEtc.onValueChanged.AddListener(delegate(bool isOn) { OnToggleChange(isOn, ShopState.ETC); });
			menuDlc.onValueChanged.AddListener(delegate(bool isOn) { OnToggleChange(isOn, ShopState.DLC); });
			menuTwitchDrops.onValueChanged.AddListener(delegate(bool isOn)
			{
				OnToggleChange(isOn, ShopState.TWITCH_DROPS);
			});
			shopCharacter.noMoneyOpenShopCallback = NoMoneyOpenShopCallback;
			shopSkin.noMoneyOpenShopCallback = NoMoneyOpenShopCallback;
			shopEtc.lackOfAssetCallback = NoMoneyOpenShopCallback;
		}


		private void OnToggleChange(bool isOn, ShopState shopState)
		{
			if (isOn)
			{
				OpenShop(shopState);
			}
		}


		private void OpenShop(ShopState currentShopState)
		{
			if (MonoBehaviourInstance<LobbyUI>.inst.CurrentTab != LobbyTab.ShopTab)
			{
				MonoBehaviourInstance<LobbyUI>.inst.SetLobbyTab(LobbyTab.ShopTab);
			}

			shopCharacter.ClosePage();
			shopSkin.ClosePage();
			shopAsset.ClosePage();
			shopEtc.ClosePage();
			shopDlc.ClosePage();
			shopTwitchDrops.ClosePage();
			this.currentShopState = currentShopState;
			switch (currentShopState)
			{
				case ShopState.CHARACTER:
					shopCharacter.OpenPage();
					return;
				case ShopState.SKIN:
					shopSkin.OpenPage();
					return;
				case ShopState.MONEY:
					shopAsset.OpenPage();
					return;
				case ShopState.ETC:
					shopEtc.OpenPage();
					return;
				case ShopState.DLC:
					shopDlc.OpenPage();
					return;
				case ShopState.TWITCH_DROPS:
					shopTwitchDrops.OpenPage();
					return;
				default:
					return;
			}
		}


		public void UpdateDlcProducts(bool resetPos)
		{
			shopDlc.UpdateScrollView(resetPos);
		}


		public void UpdateTwitchDrops(bool resetPos)
		{
			shopTwitchDrops.UpdateScrollView(resetPos);
		}


		public void NoMoneyOpenShopCallback()
		{
			if (menuMoney.isOn)
			{
				OnToggleChange(true, ShopState.MONEY);
			}
			else
			{
				menuMoney.isOn = true;
			}

			MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("ServerError/2100"), new Popup.Button
			{
				text = Ln.Get("확인")
			});
		}


		public void OnClickShowTwitchDrops()
		{
			menuTwitchDrops.isOn = true;
		}


		private enum ShopState
		{
			CHARACTER,

			SKIN,

			MONEY,

			ETC,

			DLC,

			TWITCH_DROPS
		}
	}
}