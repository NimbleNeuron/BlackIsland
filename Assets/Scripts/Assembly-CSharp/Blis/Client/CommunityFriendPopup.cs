using Blis.Common;
using Blis.Common.Utils;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class CommunityFriendPopup : BaseUI
	{
		private readonly Color disableColor = Color.gray;


		private readonly Color enableColor = new Color(0.95f, 0.85f, 0.65f);


		private RectTransform anchor;


		private Button button;


		private Text buttonText;


		private CanvasGroup canvasGroup;


		private bool isOpen;


		private CSteamID steamID;


		private CommunityUserSlot userSlot;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			GameUtil.Bind<CanvasGroup>(gameObject, ref canvasGroup);
			anchor = GameUtil.Bind<RectTransform>(gameObject, "Anchor");
			userSlot = GameUtil.Bind<CommunityUserSlot>(anchor.gameObject, "UserInfo");
			button = GameUtil.Bind<Button>(anchor.gameObject, "Buttons/Btn1");
			button.onClick.AddListener(OnClickButton1);
			buttonText = GameUtil.Bind<Text>(anchor.gameObject, "Buttons/Btn1/Text");
			Close();
		}


		public void Open(CSteamID steamID)
		{
			this.steamID = steamID;
			isOpen = true;
			UpdatePopup(steamID);
			Vector2 vector = Input.mousePosition;
			vector += GameUtil.ConvertPositionOnScreenResolution(50f, 30f);
			anchor.transform.position = vector;
			canvasGroup.alpha = 1f;
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;
		}


		public void Close()
		{
			isOpen = false;
			steamID = CSteamID.Nil;
			canvasGroup.alpha = 0f;
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
		}


		public void UpdatePopup(CSteamID steamID)
		{
			if (!isOpen)
			{
				return;
			}

			if (!this.steamID.Equals(steamID))
			{
				return;
			}

			CommunityUserSlot communityUserSlot = userSlot;
			if (communityUserSlot != null)
			{
				communityUserSlot.SetSlot(steamID);
			}

			if (button != null && buttonText != null)
			{
				if (CommunityService.HasLobby())
				{
					if (CommunityService.IsLobbyOwner() && !CommunityService.IsMe(steamID))
					{
						button.interactable = true;
						buttonText.color = enableColor;
					}
					else
					{
						button.interactable = false;
						buttonText.color = disableColor;
					}
				}
				else
				{
					button.interactable = true;
					buttonText.color = enableColor;
				}

				buttonText.text = Ln.Get(CommunityService.IsLobbyMember(steamID) ? "추방하기" : "초대하기");
			}
		}


		private void OnClickButton1()
		{
			if (CommunityService.HasLobby() && !CommunityService.IsLobbyOwner())
			{
				return;
			}

			if (CommunityService.IsMe(steamID))
			{
				return;
			}

			if (CommunityService.IsLobbyMember(steamID))
			{
				KickLobby();
			}
			else
			{
				InviteLobby();
			}

			Close();
		}


		private void InviteLobby()
		{
			if (CommunityService.HasLobby() && !CommunityService.IsLobbyOwner())
			{
				return;
			}

			if (CommunityService.IsMe(steamID))
			{
				return;
			}

			if (CommunityService.IsLobbyMember(steamID))
			{
				return;
			}

			CommunityService.InviteMyGroup(steamID);
		}


		private void KickLobby()
		{
			if (CommunityService.HasLobby() && !CommunityService.IsLobbyOwner())
			{
				return;
			}

			if (CommunityService.IsMe(steamID))
			{
				return;
			}

			if (!CommunityService.IsLobbyMember(steamID))
			{
				return;
			}

			CSteamID kickSteamID = new CSteamID(steamID.m_SteamID);
			Popup inst = MonoBehaviourInstance<Popup>.inst;
			string msg = Ln.Format("{0}님을 추방하시겠습니까?", CommunityService.GetFriendSteamName(kickSteamID));
			Popup.Button[] array = new Popup.Button[2];
			array[0] = new Popup.Button
			{
				type = Popup.ButtonType.Confirm,
				text = Ln.Get("확인"),
				callback = delegate { CommunityService.KickLobby(kickSteamID); }
			};
			int num = 1;
			Popup.Button button = new Popup.Button();
			button.type = Popup.ButtonType.Cancel;
			button.text = Ln.Get("취소");
			button.callback = delegate { };
			array[num] = button;
			inst.Message(msg, array);
		}
	}
}