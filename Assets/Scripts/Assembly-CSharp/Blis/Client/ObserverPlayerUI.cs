using System;
using System.Collections.Generic;
using System.Linq;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class ObserverPlayerUI : BaseUI
	{
		private List<ObserverTeam> observerTeams = new List<ObserverTeam>();


		private int selectTeamNumber;


		private int selectTeamSlot;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			Transform transform = this.transform.Find(GlobalUserData.matchingTeamMode.ToString());
			if (transform != null)
			{
				transform.gameObject.SetActive(true);
				observerTeams = transform.GetComponentsInChildren<ObserverTeam>().ToList<ObserverTeam>();
			}
		}


		public void SetTeams()
		{
			int num = 0;
			foreach (KeyValuePair<int, List<PlayerContext>> keyValuePair in MonoBehaviourInstance<ClientService>.inst
				.GetTeams())
			{
				observerTeams[num].Show();
				observerTeams[num].SetPlayers(keyValuePair.Value);
				ObserverTeam observerTeam = observerTeams[num];
				observerTeam.OnClickEvent = (Action<ObserverPlayerCard>) Delegate.Combine(observerTeam.OnClickEvent,
					new Action<ObserverPlayerCard>(OnClickEvent));
				num++;
			}

			for (int i = num; i < observerTeams.Count; i++)
			{
				observerTeams[i].Hide();
				observerTeams[i].SetPlayers(null);
			}
		}


		public void SelectTeam(int teamIndex)
		{
			if (MonoBehaviourInstance<ClientService>.inst.GetTeams().Count < teamIndex)
			{
				return;
			}

			ObserverPlayerCard observerPlayerCard = observerTeams[teamIndex - 1].FindFirstCard();
			if (observerPlayerCard != null)
			{
				selectTeamNumber = observerPlayerCard.TeamNumber;
				selectTeamSlot = observerPlayerCard.TeamSlot;
			}

			for (int i = 0; i < observerTeams.Count; i++)
			{
				observerTeams[i].SelectCard(selectTeamNumber, selectTeamSlot);
			}
		}


		public void SelectCard(int teamNumber, int teamSlot)
		{
			selectTeamNumber = teamNumber;
			selectTeamSlot = teamSlot;
			for (int i = 0; i < observerTeams.Count; i++)
			{
				observerTeams[i].SelectCard(teamNumber, teamSlot);
			}
		}


		public void SelectCard(int teamSlot)
		{
			selectTeamSlot = teamSlot;
			ObserverPlayerCard observerPlayerCard = FindCard(selectTeamNumber, teamSlot);
			if (observerPlayerCard == null || observerPlayerCard.ObjectId == 0)
			{
				return;
			}

			for (int i = 0; i < observerTeams.Count; i++)
			{
				observerTeams[i].SelectCard(selectTeamNumber, teamSlot);
			}
		}


		public void SetDyingCondition(int objectId, bool isDyingCondition)
		{
			ObserverPlayerCard observerPlayerCard = FindCard(objectId);
			if (observerPlayerCard == null)
			{
				return;
			}

			if (isDyingCondition)
			{
				observerPlayerCard.DyingCondition();
				return;
			}

			observerPlayerCard.Alive();
		}


		public void SetKillCount(int objectId, int killCount)
		{
			ObserverPlayerCard observerPlayerCard = FindCard(objectId);
			if (observerPlayerCard == null)
			{
				return;
			}

			observerPlayerCard.SetKillCount(killCount);
		}


		public void Dead(int objectId)
		{
			ObserverPlayerCard observerPlayerCard = FindCard(objectId);
			if (observerPlayerCard == null)
			{
				return;
			}

			observerPlayerCard.Dead();
		}


		public void SetInBattle(int objectId, bool isCombat)
		{
			ObserverPlayerCard observerPlayerCard = FindCard(objectId);
			if (observerPlayerCard == null)
			{
				return;
			}

			observerPlayerCard.SetInBattle(isCombat);
		}


		public void SetInBattleCharacter(int objectId)
		{
			ObserverPlayerCard observerPlayerCard = FindCard(objectId);
			if (observerPlayerCard == null)
			{
				return;
			}

			observerPlayerCard.SetInBattleCharacter();
		}


		public void SetLevel(int objectId, int level)
		{
			ObserverPlayerCard observerPlayerCard = FindCard(objectId);
			if (observerPlayerCard == null)
			{
				return;
			}

			observerPlayerCard.SetLevel(level);
		}


		public void SetHp(int objectId, int hp, int maxHp)
		{
			ObserverPlayerCard observerPlayerCard = FindCard(objectId);
			if (observerPlayerCard == null)
			{
				return;
			}

			observerPlayerCard.SetHpBar(hp, maxHp);
		}


		public void SetSp(int objectId, int sp, int maxSp)
		{
			ObserverPlayerCard observerPlayerCard = FindCard(objectId);
			if (observerPlayerCard == null)
			{
				return;
			}

			observerPlayerCard.SetSpBar(sp, maxSp);
		}


		public void StartCooldownUltimateSkill(int objectId, float cooldown, float maxCooldown)
		{
			ObserverPlayerCard observerPlayerCard = FindCard(objectId);
			if (observerPlayerCard == null)
			{
				return;
			}

			observerPlayerCard.SetUltimateSkillReady(cooldown <= 0f);
			observerPlayerCard.SetUltimateSkillCooldown(cooldown, maxCooldown);
		}


		public void ModifyCooldownUltimateSkill(int objectId, float ultimateSkillCooldown)
		{
			ObserverPlayerCard observerPlayerCard = FindCard(objectId);
			if (observerPlayerCard == null)
			{
				return;
			}

			observerPlayerCard.AddUltimateSkillCooldown(ultimateSkillCooldown);
		}


		public void HoldCooldownUltimateSkill(int objectId)
		{
			ObserverPlayerCard observerPlayerCard = FindCard(objectId);
			if (observerPlayerCard == null)
			{
				return;
			}

			observerPlayerCard.HoldCooldownUltimateSkill();
		}


		public void DrawSkillIcon(LocalPlayerCharacter character)
		{
			ObserverPlayerCard observerPlayerCard = FindCard(character.ObjectId);
			if (observerPlayerCard == null)
			{
				return;
			}

			observerPlayerCard.DrawUltimateIcon(character);
		}


		private ObserverPlayerCard FindCard(int objectId)
		{
			for (int i = 0; i < observerTeams.Count; i++)
			{
				ObserverPlayerCard observerPlayerCard = observerTeams[i].FindCard(objectId);
				if (observerPlayerCard != null)
				{
					return observerPlayerCard;
				}
			}

			return null;
		}


		private ObserverPlayerCard FindCard(int teamNumber, int teamSlot)
		{
			for (int i = 0; i < observerTeams.Count; i++)
			{
				ObserverPlayerCard observerPlayerCard = observerTeams[i].FindCard(teamNumber, teamSlot);
				if (observerPlayerCard != null)
				{
					return observerPlayerCard;
				}
			}

			return null;
		}


		public void OnClickEvent(ObserverPlayerCard slot)
		{
			selectTeamNumber = slot.TeamNumber;
			selectTeamSlot = slot.TeamSlot;
			SelectCard(slot.TeamNumber, slot.TeamSlot);
		}
	}
}