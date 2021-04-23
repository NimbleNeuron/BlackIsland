using System;
using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class ObserverViewTeamSlot : BaseUI
	{
		private Image bg;


		private Action<int, int> onClickEvent;


		private GameObject selectObj;


		private List<ObserverViewCharacterSlot> slots;


		private int teamNumber;


		public int TeamNumber => teamNumber;


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			slots = GetComponentsInChildren<ObserverViewCharacterSlot>().ToList<ObserverViewCharacterSlot>();
			bg = GameUtil.Bind<Image>(gameObject, "ListBg");
			bg.enabled = 1 < slots.Count;
			selectObj = GameUtil.Bind<Transform>(gameObject, "SelectBg").gameObject;
		}


		public void SetClickEvent(Action<int, int> action)
		{
			onClickEvent = action;
		}


		public void SetTeam(int teamNumber, Dictionary<long, MatchingService.MatchingUser> matchingTeamMembers)
		{
			this.teamNumber = teamNumber;
			int num = 0;
			foreach (MatchingService.MatchingUser matchingUser in from x in matchingTeamMembers.Values
				orderby x.UserNum
				select x)
			{
				if (slots.Count <= num)
				{
					return;
				}

				slots[num].SetMatchingUser(matchingUser);
				slots[num].SetClickEvent(OnClickEvent);
				num++;
			}

			for (int i = num; i < slots.Count; i++)
			{
				slots[i].Blank();
			}
		}


		public void Select()
		{
			if (!selectObj.activeSelf)
			{
				selectObj.SetActive(true);
			}

			for (int i = 0; i < slots.Count; i++)
			{
				slots[i].Select();
			}
		}


		public void Deselect()
		{
			if (selectObj.activeSelf)
			{
				selectObj.SetActive(false);
			}

			for (int i = 0; i < slots.Count; i++)
			{
				slots[i].Deselect();
			}
		}


		public void Blank()
		{
			for (int i = 0; i < slots.Count; i++)
			{
				slots[i].Blank();
			}

			onClickEvent = null;
		}


		public ObserverViewCharacterSlot GetSlot(long userNum)
		{
			return slots.Find(x => x.UserNum == userNum);
		}


		private void OnClickEvent(int teamSlot)
		{
			Action<int, int> action = onClickEvent;
			if (action == null)
			{
				return;
			}

			action(teamNumber, teamSlot);
		}
	}
}