using System.Collections.Generic;
using Blis.Common.Utils;
using UnityEngine.UI;

namespace Blis.Client
{
	public class CharacterSelectTeamCharacterView : BaseUI
	{
		private readonly List<CharacterSelectTeamSlot> teamSlots = new List<CharacterSelectTeamSlot>();


		private LayoutElement layoutElement;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			layoutElement = GetComponent<LayoutElement>();
			GetComponentsInChildren<CharacterSelectTeamSlot>(teamSlots);
			HideSlots();
		}


		public void SetTeamSlot(MatchingService.MatchingUser userInfo)
		{
			if (teamSlots == null)
			{
				return;
			}

			int slotIndex = MonoBehaviourInstance<MatchingService>.inst.GetSlotIndex(userInfo);
			if (slotIndex < 0 || teamSlots.Count <= slotIndex)
			{
				return;
			}

			teamSlots[slotIndex].SetSlot(userInfo);
		}


		public void PickCharacter(MatchingService.MatchingUser userInfo)
		{
			if (teamSlots == null)
			{
				return;
			}

			for (int i = 0; i < teamSlots.Count; i++)
			{
				if (teamSlots[i].TeamSlot == userInfo.TeamSlot)
				{
					teamSlots[i].PlayPickEffect();
					return;
				}
			}
		}


		public void Clear()
		{
			if (MonoBehaviourInstance<MatchingService>.inst.TeamMemberCount == 1)
			{
				layoutElement.ignoreLayout = true;
				HideSlots();
				return;
			}

			layoutElement.ignoreLayout = false;
			for (int i = 0; i < teamSlots.Count; i++)
			{
				if (i < MonoBehaviourInstance<MatchingService>.inst.TeamMemberCount)
				{
					teamSlots[i].SetSlot(MonoBehaviourInstance<MatchingService>.inst.GetTeamUser(i));
				}
				else
				{
					teamSlots[i].Hide();
				}
			}
		}


		private void HideSlots()
		{
			for (int i = 0; i < teamSlots.Count; i++)
			{
				teamSlots[i].Hide();
			}
		}
	}
}