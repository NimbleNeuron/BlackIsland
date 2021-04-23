using System;
using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class SettingWindow : BaseWindow
	{
		[SerializeField] private ToggleGroup toggleGroup = default;


		[SerializeField] private List<BasePage> pages = default;


		private readonly Dictionary<Toggle, BasePage> toggles = new Dictionary<Toggle, BasePage>();


		private Text txtGiveUp = default;


		private Text txtQuit = default;


		protected override void OnDestroy()
		{
			base.OnDestroy();
			SingletonMonoBehaviour<KoreaGameGradeUtil>.inst.SetGameGradeToast(null);
		}

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			txtQuit = GameUtil.Bind<Text>(gameObject, "Btn/Exit/TXT_Lobby");
			txtGiveUp = GameUtil.Bind<Text>(gameObject, "Btn/Exit/TXT_InGame");
			Transform transform = toggleGroup.transform;
			int num = 0;
			while (num < transform.childCount && num < pages.Count)
			{
				Toggle component = transform.GetChild(num).GetComponent<Toggle>();
				if (component != null)
				{
					toggles.Add(component, pages[num]);
				}

				num++;
			}

			using (Dictionary<Toggle, BasePage>.KeyCollection.Enumerator enumerator = toggles.Keys.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Toggle toggle = enumerator.Current;
					toggle.onValueChanged.AddListener(delegate(bool isOn) { OnToggleChange(toggles[toggle], isOn); });
				}
			}

			if (backShade != null)
			{
				pages.Cast<BasicSettingPage>().First<BasicSettingPage>().onResolution += backShade.Resolution;
			}
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
			SingletonMonoBehaviour<KoreaGameGradeUtil>.inst.SetGameGradeToast(MonoBehaviourInstance<Popup>.inst
				.GameGradeToast);
		}


		private void OnToggleChange(BasePage page, bool isOn)
		{
			if (page.IsOpen && isOn)
			{
				return;
			}

			if (IsOpen)
			{
				if (isOn)
				{
					page.OpenPage();
					return;
				}

				page.ClosePage();
			}
		}


		protected override void OnOpen()
		{
			base.OnOpen();
			foreach (KeyValuePair<Toggle, BasePage> keyValuePair in toggles)
			{
				if (keyValuePair.Key.isOn)
				{
					keyValuePair.Value.OpenPage();
					if (SingletonMonoBehaviour<Bootstrap>.inst.IsLobbyScene)
					{
						txtQuit.gameObject.SetActive(true);
						txtGiveUp.gameObject.SetActive(false);
					}
					else
					{
						txtQuit.gameObject.SetActive(false);
						txtGiveUp.gameObject.SetActive(true);
						ClientService inst = MonoBehaviourInstance<ClientService>.inst;
						if (inst != null)
						{
							txtGiveUp.text = inst.IsPlayer ? Ln.Get("전투 포기") : Ln.Get("관전 종료");
						}
						else
						{
							txtGiveUp.text = Ln.Get("전투 포기");
						}
					}
				}
			}

			SingletonMonoBehaviour<KoreaGameGradeUtil>.inst.GradeToastReStart();
		}


		public void OnLobbyStateUpdate(LobbyState lobbyState)
		{
			if (!SingletonMonoBehaviour<Bootstrap>.inst.IsLobbyScene)
			{
				return;
			}

			LobbyService inst = MonoBehaviourInstance<LobbyService>.inst;
			if (inst == null)
			{
				return;
			}

			foreach (BasePage basePage in pages)
			{
				if (basePage is GameSettingPage)
				{
					(basePage as GameSettingPage).SetHideNameInteractable(inst.LobbyState == LobbyState.Ready);
					break;
				}
			}
		}


		protected override void OnClose()
		{
			base.OnClose();
			MonoBehaviourInstance<Tooltip>.inst.Hide();
			foreach (KeyValuePair<Toggle, BasePage> keyValuePair in toggles)
			{
				if (keyValuePair.Value.IsOpen)
				{
					keyValuePair.Value.ClosePage();
				}
			}

			SingletonMonoBehaviour<KoreaGameGradeUtil>.inst.GradeToastRewind();
		}


		public void QuitButton()
		{
			string msg = string.Empty;
			bool isLobbyScene = SingletonMonoBehaviour<Bootstrap>.inst.IsLobbyScene;
			Action callback;
			if (isLobbyScene)
			{
				msg = Ln.Get("게임을 종료하시겠습니까?");
				callback = Application.Quit;
			}
			else if (MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				if (MonoBehaviourInstance<ClientService>.inst.MyPlayer.Character.IsAlive)
				{
					msg = Ln.Get("게임을 종료하시면 플레이 중인 캐릭터가 즉시 사망합니다. 종료하시겠습니까?");
					callback = delegate
					{
						MonoBehaviourInstance<GameClient>.inst.Request(new ReqExitGame());
						Close();
					};
				}
				else
				{
					msg = Ln.Get("관전을 종료하고 게임을 나가시겠습니까?");
					callback = delegate { MonoBehaviourInstance<GameClient>.inst.Request(new ReqExitGame()); };
				}
			}
			else
			{
				msg = Ln.Get("관전을 종료하고 게임을 나가시겠습니까?");
				callback = delegate
				{
					MonoBehaviourInstance<GameClient>.inst.Request(new ReqExitGame());
					Close();
				};
			}

			string text;
			if (isLobbyScene)
			{
				text = Ln.Get("게임 종료");
			}
			else
			{
				text = MonoBehaviourInstance<ClientService>.inst.IsPlayer ? Ln.Get("전투 포기") : Ln.Get("관전 종료");
			}

			MonoBehaviourInstance<Popup>.inst.Message(msg, new Popup.Button
			{
				type = Popup.ButtonType.Confirm,
				text = text,
				callback = callback
			}, new Popup.Button
			{
				type = Popup.ButtonType.Cancel,
				text = Ln.Get("닫기")
			});
		}
	}
}