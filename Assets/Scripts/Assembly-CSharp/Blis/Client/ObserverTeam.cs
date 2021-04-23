using System;
using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class ObserverTeam : BaseUI
	{
		public Action<ObserverPlayerCard> OnClickEvent;


		private List<ObserverPlayerCard> playerCards = new List<ObserverPlayerCard>();


		private Image teamLine;


		private DOTweenAnimation teamTween;


		private LnText textTeamNumber;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			playerCards = GetComponentsInChildren<ObserverPlayerCard>().ToList<ObserverPlayerCard>();
			Transform transform = this.transform.Find("TeamInfo/Line");
			if (transform != null)
			{
				teamLine = transform.GetComponent<Image>();
				textTeamNumber = GameUtil.Bind<LnText>(gameObject, "TeamInfo/Team/Txt_Number");
			}

			teamTween = GameUtil.Bind<DOTweenAnimation>(gameObject, "TeamInfo");
		}


		public void SetPlayers(List<PlayerContext> playerContexts)
		{
			int num = 0;
			int num2 = 0;
			if (playerContexts != null)
			{
				foreach (PlayerContext playerContext in from x in playerContexts
					orderby x.GetTeamSlot()
					select x)
				{
					if (playerCards.Count > num)
					{
						playerCards[num].Init(playerContext.Character);
						playerCards[num].SetClickEvent(OnClickCard);
						playerCards[num].Show();
						num++;
						if (num2 == 0)
						{
							num2 = playerContext.Character.TeamNumber;
						}
					}
				}
			}

			for (int i = num; i < playerCards.Count; i++)
			{
				playerCards[i].Init(null);
				playerCards[i].Hide();
			}

			if (teamLine != null && MonoBehaviourInstance<ClientService>.inst.IsTeamMode)
			{
				int num3 = MonoBehaviourInstance<ClientService>.inst.GetTeams().Keys.ToList<int>().IndexOf(num2);
				textTeamNumber.text = (num3 + 1).ToString();
				teamLine.color = UIUtility.ObserverTeamColor(num3);
			}
		}


		public void SelectCard(int teamNumber, int teamSlot)
		{
			bool flag = false;
			for (int i = 0; i < playerCards.Count; i++)
			{
				if (playerCards[i].TeamNumber == teamNumber)
				{
					flag = true;
					playerCards[i].SelectTeam(true);
					if (playerCards[i].TeamSlot == teamSlot)
					{
						SingletonMonoBehaviour<ObserverController>.inst.SetTarget(playerCards[i].ObjectId);
						playerCards[i].Select();
					}
					else
					{
						playerCards[i].Deselect();
					}
				}
				else
				{
					playerCards[i].SelectTeam(false);
					playerCards[i].Deselect();
				}
			}

			if (flag)
			{
				teamTween.DOPlay();
				return;
			}

			teamTween.DORewind();
		}


		public ObserverPlayerCard FindCard(int objectId)
		{
			for (int i = 0; i < playerCards.Count; i++)
			{
				if (playerCards[i].ObjectId == objectId)
				{
					return playerCards[i];
				}
			}

			return null;
		}


		public ObserverPlayerCard FindCard(int teamNumber, int slotNumber)
		{
			for (int i = 0; i < playerCards.Count; i++)
			{
				if (playerCards[i].TeamNumber == teamNumber && playerCards[i].TeamSlot == slotNumber)
				{
					return playerCards[i];
				}
			}

			return null;
		}


		public ObserverPlayerCard FindFirstCard()
		{
			for (int i = 0; i < playerCards.Count; i++)
			{
				if (!playerCards[i].IsDead)
				{
					return playerCards[i];
				}
			}

			return null;
		}


		public void Show()
		{
			gameObject.SetActive(true);
		}


		public void Hide()
		{
			for (int i = 0; i < playerCards.Count; i++)
			{
				playerCards[i].Init(null);
				playerCards[i].Hide();
			}

			gameObject.SetActive(false);
		}


		private void OnClickCard(ObserverPlayerCard slot)
		{
			Action<ObserverPlayerCard> onClickEvent = OnClickEvent;
			if (onClickEvent == null)
			{
				return;
			}

			onClickEvent(slot);
		}
	}
}