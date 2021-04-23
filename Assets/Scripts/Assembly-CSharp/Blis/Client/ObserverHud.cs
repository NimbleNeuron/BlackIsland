using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Client
{
	public class ObserverHud : BaseUI
	{
		private ObserverPlayerUI playerUI;


		private ObserverSetting settings;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			settings = GameUtil.Bind<ObserverSetting>(gameObject, "Settings");
			settings.gameObject.SetActive(false);
			playerUI = GameUtil.Bind<ObserverPlayerUI>(gameObject, "ObserverPlayerUI");
		}


		public void Open(LocalPlayerCharacter selectPlayer = null)
		{
			if (MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				return;
			}

			Show();
			playerUI.SetTeams();
			if (selectPlayer != null)
			{
				playerUI.SelectCard(selectPlayer.TeamNumber, selectPlayer.TeamSlot);
				return;
			}

			ObserverHostileAgent hostileAgent =
				MonoBehaviourInstance<ClientService>.inst.MyObserver.Observer.HostileAgent;
			if (hostileAgent.SelectedTeamNumber == 0)
			{
				LocalPlayerCharacter firstPlayer = MonoBehaviourInstance<ClientService>.inst.GetFirstPlayer();
				if (firstPlayer != null)
				{
					playerUI.SelectCard(firstPlayer.TeamNumber, firstPlayer.TeamSlot);
				}
			}
			else
			{
				playerUI.SelectCard(hostileAgent.SelectedTeamNumber, hostileAgent.SelectedTeamSlot);
			}
		}


		public void Close()
		{
			Hide();
		}


		private void Show()
		{
			gameObject.SetActive(true);
		}


		private void Hide()
		{
			gameObject.SetActive(false);
		}


		public void SetDyingCondition(int objectId, bool isDyingCondition)
		{
			if (MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				return;
			}

			playerUI.SetDyingCondition(objectId, isDyingCondition);
		}


		public void SetKillCount(int objectId, int killCount)
		{
			if (MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				return;
			}

			playerUI.SetKillCount(objectId, killCount);
		}


		public void Dead(int objectId)
		{
			if (MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				return;
			}

			playerUI.Dead(objectId);
		}


		public void SetInBattle(int objectId, bool isCombat)
		{
			if (MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				return;
			}

			playerUI.SetInBattle(objectId, isCombat);
		}


		public void SetInBattleByPlayer(int objectId)
		{
			if (MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				return;
			}

			playerUI.SetInBattleCharacter(objectId);
		}


		public void SetLevel(int objectId, int level)
		{
			if (MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				return;
			}

			playerUI.SetLevel(objectId, level);
		}


		public void SetHp(int objectId, int hp, int maxHp)
		{
			if (MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				return;
			}

			playerUI.SetHp(objectId, hp, maxHp);
		}


		public void SetSp(int objectId, int sp, int maxSp)
		{
			if (MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				return;
			}

			playerUI.SetSp(objectId, sp, maxSp);
		}


		public void StartCooldownUltimateSkill(int objectId, float cooldown, float maxCooldown)
		{
			if (MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				return;
			}

			playerUI.StartCooldownUltimateSkill(objectId, cooldown, maxCooldown);
		}


		public void ModifyCooldownUltimateSkill(int objectId, float ultimateSkillCooldown)
		{
			if (MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				return;
			}

			playerUI.ModifyCooldownUltimateSkill(objectId, ultimateSkillCooldown);
		}


		public void HoldCooldownUltimateSkill(int objectId)
		{
			if (MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				return;
			}

			playerUI.HoldCooldownUltimateSkill(objectId);
		}


		public void DrawSkillIcon(LocalPlayerCharacter character)
		{
			if (MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				return;
			}

			playerUI.DrawSkillIcon(character);
		}


		public void SelectTeam(int teamIndex)
		{
			if (MonoBehaviourInstance<ClientService>.inst.IsPlayer ||
			    !MonoBehaviourInstance<ClientService>.inst.IsTeamMode)
			{
				return;
			}

			playerUI.SelectTeam(teamIndex);
		}


		public void SelectTarget(int teamNumber, int teamSlot)
		{
			if (MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				return;
			}

			playerUI.SelectCard(teamNumber, teamSlot);
		}


		public void SelectTarget(int teamSlot)
		{
			if (MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				return;
			}

			playerUI.SelectCard(teamSlot);
		}
	}
}