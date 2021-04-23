using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Client
{
	public class TeamHud : BaseUI, ITeamMemberSlotEventListener
	{
		public List<TeamMemberSlot> TeamMemberSlots { get; } = new List<TeamMemberSlot>();


		public void OnSlotLeftClick(TeamMemberSlot slot) { }


		public void OnSlotDoubleClick(TeamMemberSlot slot) { }


		public void OnSlotRightClick(TeamMemberSlot slot) { }


		public void OnPointerDown(TeamMemberSlot slot)
		{
			MonoBehaviourInstance<MobaCamera>.inst.StartTeamMoveCamera(slot.TeamIndex == 1 ? 2 : 3);
		}


		public void OnPointerUp(TeamMemberSlot slot)
		{
			MonoBehaviourInstance<MobaCamera>.inst.StopTeamMoveCamera(slot.TeamIndex == 1 ? 2 : 3);
		}


		public void OnPointerEnter(TeamMemberSlot slot) { }


		public void OnPointerExit(TeamMemberSlot slot) { }


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			transform.GetComponentsInChildren<TeamMemberSlot>(false, TeamMemberSlots);
			int num = 1;
			foreach (TeamMemberSlot teamMemberSlot in TeamMemberSlots)
			{
				teamMemberSlot.SetTeamIndex(num);
				teamMemberSlot.SetEventListener(this);
				num++;
			}
		}


		public void Init(List<LocalPlayerCharacter> teamMembers)
		{
			for (int i = 0; i < TeamMemberSlots.Count; i++)
			{
				if (i < teamMembers.Count)
				{
					TeamMemberSlots[i].Init(teamMembers[i]);
					TeamMemberSlots[i].Show();
				}
				else
				{
					TeamMemberSlots[i].Hide();
				}
			}
		}


		public void SetLevel(int objectId, int level)
		{
			if (!MonoBehaviourInstance<ClientService>.inst.IsTeamMode)
			{
				return;
			}

			TeamMemberSlot teamMemberSlot = FindSlot(objectId);
			if (teamMemberSlot != null)
			{
				teamMemberSlot.SetLevel(level);
			}
		}


		public void StartCooldownUltimateSkill(int objectId, float cooldown, float maxCooldown)
		{
			if (!MonoBehaviourInstance<ClientService>.inst.IsTeamMode)
			{
				return;
			}

			TeamMemberSlot teamMemberSlot = FindSlot(objectId);
			if (teamMemberSlot != null)
			{
				teamMemberSlot.SetUltimateSkillReady(cooldown <= 0f);
				teamMemberSlot.SetUltimateSkillCooldown(cooldown, maxCooldown);
			}
		}


		public void ModifyCooldownUltimateSkill(int objectId, float addCooldown)
		{
			if (!MonoBehaviourInstance<ClientService>.inst.IsTeamMode)
			{
				return;
			}

			TeamMemberSlot teamMemberSlot = FindSlot(objectId);
			if (teamMemberSlot != null)
			{
				teamMemberSlot.AddUltimateSkillCooldown(addCooldown);
			}
		}


		public void HoldCooldownUltimateSkill(int objectId)
		{
			if (!MonoBehaviourInstance<ClientService>.inst.IsTeamMode)
			{
				return;
			}

			TeamMemberSlot teamMemberSlot = FindSlot(objectId);
			if (teamMemberSlot != null)
			{
				teamMemberSlot.HoldCooldownUltimateSkill();
			}
		}


		public void DrawSkillIcon(LocalPlayerCharacter character)
		{
			if (!MonoBehaviourInstance<ClientService>.inst.IsTeamMode)
			{
				return;
			}

			TeamMemberSlot teamMemberSlot = FindSlot(character.ObjectId);
			if (teamMemberSlot != null)
			{
				teamMemberSlot.DrawSkillIcon(character);
			}
		}


		public void SetHp(int objectId, int hp, int maxHp)
		{
			if (!MonoBehaviourInstance<ClientService>.inst.IsTeamMode)
			{
				return;
			}

			TeamMemberSlot teamMemberSlot = FindSlot(objectId);
			if (teamMemberSlot != null)
			{
				teamMemberSlot.SetHpBar(hp, maxHp);
			}
		}


		public void SetSp(int objectId, int sp, int maxSp)
		{
			if (!MonoBehaviourInstance<ClientService>.inst.IsTeamMode)
			{
				return;
			}

			TeamMemberSlot teamMemberSlot = FindSlot(objectId);
			if (teamMemberSlot != null)
			{
				teamMemberSlot.SetSpBar(sp, maxSp);
			}
		}


		public void SetDyingCondition(int objectId, bool isDyingCondition)
		{
			if (!MonoBehaviourInstance<ClientService>.inst.IsTeamMode)
			{
				return;
			}

			TeamMemberSlot teamMemberSlot = FindSlot(objectId);
			if (teamMemberSlot != null)
			{
				if (isDyingCondition)
				{
					teamMemberSlot.DyingCondition();
					return;
				}

				teamMemberSlot.Alive();
			}
		}


		public void Dead(int objectId)
		{
			if (!MonoBehaviourInstance<ClientService>.inst.IsTeamMode)
			{
				return;
			}

			TeamMemberSlot teamMemberSlot = FindSlot(objectId);
			if (teamMemberSlot != null)
			{
				teamMemberSlot.Dead();
			}
		}


		public void Connected(int objectId)
		{
			if (!MonoBehaviourInstance<ClientService>.inst.IsTeamMode)
			{
				return;
			}

			TeamMemberSlot teamMemberSlot = FindSlot(objectId);
			if (teamMemberSlot != null)
			{
				teamMemberSlot.Connected();
			}
		}


		public void Disconnected(int objectId)
		{
			if (!MonoBehaviourInstance<ClientService>.inst.IsTeamMode)
			{
				return;
			}

			TeamMemberSlot teamMemberSlot = FindSlot(objectId);
			if (teamMemberSlot != null)
			{
				teamMemberSlot.Disconnected();
			}
		}


		public void Observing(int objectId)
		{
			if (!MonoBehaviourInstance<ClientService>.inst.IsTeamMode)
			{
				return;
			}

			TeamMemberSlot teamMemberSlot = FindSlot(objectId);
			if (teamMemberSlot != null)
			{
				teamMemberSlot.Observing();
			}
		}


		public void OnShowEmotionIcon(int objectId, EmotionIconData emotionIconData)
		{
			if (!MonoBehaviourInstance<ClientService>.inst.IsTeamMode)
			{
				return;
			}

			TeamMemberSlot teamMemberSlot = FindSlot(objectId);
			if (teamMemberSlot != null)
			{
				teamMemberSlot.SetEmotionIcon(emotionIconData);
			}
		}


		private TeamMemberSlot FindSlot(int objectId)
		{
			for (int i = 0; i < TeamMemberSlots.Count; i++)
			{
				if (TeamMemberSlots[i].ObjectId == objectId)
				{
					return TeamMemberSlots[i];
				}
			}

			return null;
		}


		public void SetInBattle(int objectId, bool isCombat)
		{
			if (!MonoBehaviourInstance<ClientService>.inst.IsTeamMode)
			{
				return;
			}

			TeamMemberSlot teamMemberSlot = FindSlot(objectId);
			if (teamMemberSlot != null)
			{
				teamMemberSlot.SetInBattleUI(isCombat);
			}
		}


		public void SetInBattleByPlayer(int objecId)
		{
			if (!MonoBehaviourInstance<ClientService>.inst.IsTeamMode)
			{
				return;
			}

			TeamMemberSlot teamMemberSlot = FindSlot(objecId);
			if (teamMemberSlot != null)
			{
				teamMemberSlot.SetInBattleCharacterUI();
			}
		}
	}
}