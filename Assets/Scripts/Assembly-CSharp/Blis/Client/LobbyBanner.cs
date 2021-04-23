using System;
using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class LobbyBanner : BaseUI
	{
		private const int SMALL_BANNER_MAX = 4;


		private const float TURN_OVER_TIME = 10f;


		public int currentBigBanner;


		public float turnOverCountdown = 10f;


		private readonly List<Image> dotFocusList = new List<Image>();


		private readonly List<LobbyBannerItem> smallBanners = new List<LobbyBannerItem>();


		private LobbyBannerItem bigBanner;


		private CanvasAlphaTweener canvasAlphaTweener;


		private CanvasGroup canvasGroup;


		private List<Button> dotButtons;


		private GameObject dots;


		private GameObject sub;


		private void Update()
		{
			if (MonoBehaviourInstance<LobbyUI>.inst.CurrentTab == LobbyTab.MainTab && dotFocusList.Count > 0)
			{
				turnOverCountdown -= Time.deltaTime;
				if (turnOverCountdown <= 0f)
				{
					turnOverCountdown = 10f;
					SetInteractable(false);
					currentBigBanner++;
					if (currentBigBanner >= dotFocusList.Count)
					{
						currentBigBanner = 0;
					}

					SetBigBanner(currentBigBanner);
					FocusDot(currentBigBanner);
					SetInteractable(true);
				}
			}
		}

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			GameUtil.Bind<CanvasGroup>(gameObject, ref canvasGroup);
			SetInteractable(false);
			bigBanner = GameUtil.Bind<LobbyBannerItem>(gameObject, "BannerFrame/Main");
			dots = GameUtil.Bind<RectTransform>(gameObject, "BannerFrame/Main/Dots").gameObject;
			dotButtons = dots.GetComponentsInChildren<Button>(true).ToList<Button>();
			sub = GameUtil.Bind<RectTransform>(gameObject, "BannerFrame/Sub").gameObject;
			sub.GetComponentsInChildren<LobbyBannerItem>(smallBanners);
			GameUtil.Bind<CanvasAlphaTweener>(gameObject, ref canvasAlphaTweener);
		}


		public void InitBanners()
		{
			int loadedBigBannerCount = BannerService.GetLoadedBigBannerCount();
			int num = BannerService.GetLoadedSmallBannerCount();
			num = num > 4 ? 4 : num;
			SetInteractable(false);
			int num2 = loadedBigBannerCount - dotButtons.Count;
			for (int i = 0; i < num2; i++)
			{
				Button button = Instantiate<Button>(dotButtons[0], dots.transform);
				dotButtons.Add(button);
				button.name = dotButtons.Count.ToString();
			}

			int num3 = 0;
			using (List<Button>.Enumerator enumerator = dotButtons.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Button btn = enumerator.Current;
					if (num3 < loadedBigBannerCount)
					{
						dotButtons[num3].onClick.AddListener(delegate { DotButtonClicked(btn); });
						dotFocusList.Add(GameUtil.Bind<Image>(dotButtons[num3++].gameObject, "Focus"));
					}
					else
					{
						dotButtons[num3++].gameObject.SetActive(false);
					}
				}
			}

			if (loadedBigBannerCount > 0)
			{
				SetBigBanner(0);
				FocusDot(0);
				SetInteractable(true);
			}
			else
			{
				bigBanner.Init("", "", null, "");
			}

			if (num > 0)
			{
				sub.gameObject.SetActive(true);
				SetSmallBanner(num);
				return;
			}

			sub.gameObject.SetActive(false);
		}


		public void DotButtonClicked(Button selected)
		{
			int num = Convert.ToInt32(selected.name);
			SetBigBanner(num - 1);
			FocusDot(num - 1);
		}


		public void SetBigBanner(int selectedIndex)
		{
			currentBigBanner = selectedIndex;
			turnOverCountdown = 10f;
			LobbyApi.LobbyNotice lobbyNotice;
			Texture2D bannerImage;
			BannerService.GetBigBanner(selectedIndex, out lobbyNotice, out bannerImage);
			bigBanner.Init(lobbyNotice.title, lobbyNotice.message, bannerImage, lobbyNotice.targetUrl);
		}


		private void SetSmallBanner(int bannerCount)
		{
			for (int i = 0; i < smallBanners.Count; i++)
			{
				if (i < bannerCount)
				{
					LobbyApi.LobbyNotice lobbyNotice;
					Texture2D bannerImage;
					BannerService.GetSmallBanner(i, out lobbyNotice, out bannerImage);
					smallBanners[i].Init(lobbyNotice.title, lobbyNotice.message, bannerImage, lobbyNotice.targetUrl);
				}
				else
				{
					smallBanners[i].gameObject.SetActive(false);
				}
			}
		}


		private void FocusDot(int selectedIndex)
		{
			for (int i = 0; i < dotFocusList.Count; i++)
			{
				dotFocusList[i].enabled = i == selectedIndex;
			}
		}


		public void SetInteractable(bool active)
		{
			canvasGroup.interactable = active;
		}
	}
}