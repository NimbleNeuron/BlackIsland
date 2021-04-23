using System;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class TutorialPage : BasePage, ILnEventHander
	{
		[SerializeField] private Sprite selectSprite = default;


		[SerializeField] private Sprite unSelectSprite = default;


		[SerializeField] private Sprite rolloverSprite = default;


		[SerializeField] private Text txtTutorialTitle = default;


		[SerializeField] private Text txtTutorialDesc = default;


		[SerializeField] private GameObject reward = default;


		[SerializeField] private Image character = default;


		[SerializeField] private Image coin = default;


		[SerializeField] private Text txt_RewardType = default;


		[SerializeField] private Text txt_RewardValue = default;


		[SerializeField] private List<TutorialTabButton> tutorialTabButtons = new List<TutorialTabButton>();


		[SerializeField] private GameObject guard = default;


		[SerializeField] private LobbyBannerItem playGuideBanner = default;


		private TutorialType currentTutorialType = TutorialType.BasicGuide;


		private LnText txtCharacterName;


		private LnText txtTutorialStart;


		public void OnLnDataChange()
		{
			SetTutorialBoard();
		}

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			using (List<TutorialTabButton>.Enumerator enumerator = tutorialTabButtons.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TutorialTabButton tt = enumerator.Current;
					TutorialTabButton tt4 = tt;
					tt4.onClickTutorialTab = (TutorialTabButton.OnClickTutorialTab) Delegate.Combine(
						tt4.onClickTutorialTab,
						new TutorialTabButton.OnClickTutorialTab(delegate(TutorialType type)
						{
							ClickedTutorialTab(type);
						}));
					TutorialTabButton tt2 = tt;
					tt2.onEnterTutorialTab = (TutorialTabButton.OnHoverTutorialTab) Delegate.Combine(
						tt2.onEnterTutorialTab, new TutorialTabButton.OnHoverTutorialTab(delegate(TutorialType type)
						{
							EnterTutorialTab(type);
							if (currentTutorialType == type)
							{
								return;
							}

							tt.Rollover(rolloverSprite);
						}));
					TutorialTabButton tt3 = tt;
					tt3.onExitTutorialTab = (TutorialTabButton.OnHoverTutorialTab) Delegate.Combine(
						tt3.onExitTutorialTab, new TutorialTabButton.OnHoverTutorialTab(delegate(TutorialType type)
						{
							ExitTutorialTab(type);
							if (currentTutorialType == type)
							{
								return;
							}

							tt.UnSelect(unSelectSprite);
						}));
				}
			}

			txtCharacterName = GameUtil.Bind<LnText>(gameObject, "Board/Reward/Character/TXT_CharacterName");
			txtTutorialStart = GameUtil.Bind<LnText>(gameObject, "Board/TutorialStart/TXT_TutorialStart");
		}


		protected override void OnOpenPage()
		{
			base.OnOpenPage();
			BlockStartButton(CommunityService.HasLobby());
			guard.SetActive(false);
			SetTutorialTabs();
			SetFocusTutorialType();
			SetTutorialBoard();
			SetTutorialReward();
			SetPlayBanner();
			foreach (TutorialTabButton tutorialTabButton in tutorialTabButtons)
			{
				if (tutorialTabButton.type == currentTutorialType)
				{
					tutorialTabButton.Select(selectSprite);
					tutorialTabButton.ScaleUp();
				}
				else
				{
					tutorialTabButton.UnSelect(unSelectSprite);
					tutorialTabButton.ScaleDown();
				}
			}
		}


		private void SetTutorialTabs()
		{
			foreach (TutorialTabButton tutorialTabButton in tutorialTabButtons)
			{
				SetTutoriaClear(tutorialTabButton);
				if (tutorialTabButton.type == TutorialType.BasicGuide)
				{
					tutorialTabButton.img_completed.gameObject.SetActive(tutorialTabButton.clear);
					tutorialTabButton.img_Dimmed.gameObject.SetActive(false);
				}
				else
				{
					tutorialTabButton.img_completed.gameObject.SetActive(tutorialTabButton.clear);
					bool tutorialClearState = Lobby.inst.User.GetTutorialClearState(TutorialType.BasicGuide);
					tutorialTabButton.img_Dimmed.gameObject.SetActive(!tutorialClearState);
				}
			}
		}


		private void SetTutoriaClear(TutorialTabButton tt)
		{
			tt.clear = Lobby.inst.User.GetTutorialClearState(tt.type);
		}


		private void SetTutorialBoard()
		{
			int num = (int) currentTutorialType;
			txtTutorialTitle.text = Ln.Get(string.Format("Tutorial/Title/{0}", num));
			txtTutorialDesc.text = Ln.Get(string.Format("Tutorial/Desc/{0}", num));
		}


		private void SetTutorialReward()
		{
			if (tutorialTabButtons.Find(x => x.type == currentTutorialType).clear)
			{
				reward.SetActive(false);
				return;
			}

			if (!reward.activeSelf)
			{
				reward.SetActive(true);
			}

			TutorialRewardData tutorialRewardData = GameDB.tutorial.GetTutorialRewardData((int) currentTutorialType);
			string goodsType = tutorialRewardData.goodsType;
			if (goodsType == "CHARACTER")
			{
				character.gameObject.SetActive(true);
				coin.gameObject.SetActive(false);
				character.sprite =
					SingletonMonoBehaviour<ResourceManager>.inst.GetCharacterFullSprite(tutorialRewardData
						.collectionCode);
				txtCharacterName.text = Ln.Get(LnType.Character_Name, tutorialRewardData.collectionCode.ToString());
				return;
			}

			if (!(goodsType == "ASSET"))
			{
				return;
			}

			character.gameObject.SetActive(false);
			coin.gameObject.SetActive(true);
			txt_RewardType.text = "A-Coin";
			txt_RewardValue.text = tutorialRewardData.amount.ToString();
		}


		private void SetPlayBanner()
		{
			bool clear = tutorialTabButtons.Find(x => x.type == currentTutorialType).clear;
			if (playGuideBanner != null)
			{
				bool active = clear && BannerService.ApplyPlayBannerUI(playGuideBanner);
				playGuideBanner.gameObject.SetActive(active);
			}
		}


		private bool EnableTutorial()
		{
			return tutorialTabButtons.Find(x => x.type == TutorialType.BasicGuide).clear;
		}


		private void SetFocusTutorialType()
		{
			int count = tutorialTabButtons.Count;
			int num = 0;
			using (List<TutorialTabButton>.Enumerator enumerator = tutorialTabButtons.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.clear)
					{
						num++;
					}
				}
			}

			if (count == num)
			{
				currentTutorialType = TutorialType.FinalSurvival;
				return;
			}

			if (!tutorialTabButtons.Find(x => x.type == currentTutorialType).clear)
			{
				return;
			}

			foreach (TutorialTabButton tutorialTabButton in tutorialTabButtons)
			{
				if (!tutorialTabButton.clear)
				{
					currentTutorialType = tutorialTabButton.type;
					break;
				}
			}
		}


		private void ClickedTutorialTab(TutorialType tutorialType)
		{
			if (Lobby.inst.LobbyContext.lobbyState == LobbyState.TutorialStart)
			{
				return;
			}

			if (currentTutorialType == tutorialType)
			{
				return;
			}

			if (tutorialType != TutorialType.BasicGuide && tutorialType - TutorialType.Trace <= 3 && !EnableTutorial())
			{
				return;
			}

			currentTutorialType = tutorialType;
			SetTutorialBoard();
			SetTutorialReward();
			SetPlayBanner();
			foreach (TutorialTabButton tutorialTabButton in tutorialTabButtons)
			{
				if (tutorialTabButton.type == tutorialType)
				{
					tutorialTabButton.Select(selectSprite);
					tutorialTabButton.ScaleUp();
				}
				else
				{
					tutorialTabButton.UnSelect(unSelectSprite);
					tutorialTabButton.ScaleDown();
				}
			}
		}


		private void EnterTutorialTab(TutorialType tutorialType)
		{
			tutorialTabButtons.Find(x => x.type == tutorialType).ScaleUp();
		}


		private void ExitTutorialTab(TutorialType tutorialType)
		{
			if (currentTutorialType == tutorialType)
			{
				return;
			}

			tutorialTabButtons.Find(x => x.type == tutorialType).ScaleDown();
		}


		public void ClickedTutorialStart()
		{
			if (Lobby.inst.LobbyContext.lobbyState == LobbyState.TutorialStart)
			{
				return;
			}

			BlockStartButton(CommunityService.HasLobby());
			if (CommunityService.HasLobby())
			{
				return;
			}

			txtTutorialStart.color = new Color(0.251f, 0.937f, 1f, 1f);
			guard.SetActive(true);
			MonoBehaviourInstance<MatchingService>.inst.StartTutorial(currentTutorialType);
			Lobby.inst.LobbyContext.lobbyState = LobbyState.TutorialStart;
		}


		private void BlockStartButton(bool block)
		{
			txtTutorialStart.color =
				block ? GameConstants.UIColor.disableButtonTextColor : GameConstants.UIColor.enableButtonTextColor;
		}
	}
}