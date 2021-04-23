using System.Collections.Generic;
using Blis.Client.UI;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class PlayerContext : UserContext
	{
		public readonly string nickname;
		public readonly int startingWeaponCode;
		private LocalCharacterSkill characterSkill;
		private bool disconnected;
		private bool isUseTempNickname;
		private bool observing;
		private LocalPlayerCharacter playerCharacter { get; set; }
		public PlayerContext(long userId, string nickname, int startingWeaponCode) : base(userId)
		{
			this.nickname = nickname;
			this.startingWeaponCode = startingWeaponCode;
		}
		public LocalPlayerCharacter Character => playerCharacter;
		public LocalCharacterSkill CharacterSkill => characterSkill;
		public bool IsObserving => observing;
		public bool IsDisconnected => disconnected;
		public bool IsUseTempNickname => isUseTempNickname;

		public override int GetTeamSlot()
		{
			if (!(playerCharacter != null))
			{
				return 0;
			}

			return playerCharacter.TeamSlot;
		}

		public void SetPlayerCharacter(LocalPlayerCharacter playerCharacter)
		{
			Log.V(string.Format("userId: {0}  nickname: {1}  playerCharacter.ObjectId: {2}", userId, nickname,
				playerCharacter.ObjectId));

			if (this is MyPlayerContext mctx)
			{
				Debug.Log(playerCharacter);
			}
			
			this.playerCharacter = playerCharacter;
		}


		public virtual void Init(byte[] playerSnapshot)
		{
			if (playerSnapshot != null)
			{
				PlayerSnapshot playerSnapshot2 = Serializer.Default.Deserialize<PlayerSnapshot>(playerSnapshot);
				observing = playerSnapshot2.observing;
				disconnected = playerSnapshot2.disconnected;
				characterSkill = new LocalCharacterSkill(playerCharacter.CharacterCode, playerCharacter.ObjectType);
				characterSkill.Init(playerSnapshot2.characterSkillSnapshot);
				if (Character.IsConcentrating && !characterSkill.IsConcentrating())
				{
					SkillData skillData = Character.GetSkillData(Character.ConcentrateSkillSet);
					Character.EndConcentration(skillData, true);
				}

				int rank = playerSnapshot2.rank == 0 ? -1 : playerSnapshot2.rank;
				if (!playerCharacter.IsAlive)
				{
					MonoBehaviourInstance<ClientService>.inst.AddUserRank(playerCharacter.ObjectId,
						playerCharacter.TeamNumber, rank);
				}
			}

			UISystem.Action(new UpdatePlayerInfo(playerCharacter));
		}


		public void Observing()
		{
			observing = true;
			MonoBehaviourInstance<GameUI>.inst.TeamHud.Observing(Character.ObjectId);
		}


		public void Connected()
		{
			disconnected = false;
			MonoBehaviourInstance<GameUI>.inst.TeamHud.Connected(Character.ObjectId);
		}


		public void Disconnected()
		{
			disconnected = true;
			MonoBehaviourInstance<GameUI>.inst.TeamHud.Disconnected(Character.ObjectId);
		}


		public virtual void UpdateSkills(Dictionary<SkillSlotIndex, int> characterSkillLevels, int skillPoint)
		{
			if (skillPoint < 0)
			{
				return;
			}

			foreach (KeyValuePair<SkillSlotIndex, int> keyValuePair in characterSkillLevels)
			{
				if (keyValuePair.Key != SkillSlotIndex.WeaponSkill)
				{
					characterSkill.SetSkillLevel(keyValuePair.Key, keyValuePair.Value);
				}
			}
		}


		public virtual void UpgradeSkill(SkillSlotIndex skillSlotIndex)
		{
			if (skillSlotIndex == SkillSlotIndex.WeaponSkill)
			{
				characterSkill.UpgradeSkill(Character.GetEquipWeaponMasteryType());
				return;
			}

			characterSkill.UpgradeSkill(skillSlotIndex);
		}


		public virtual void EvolutionSkill(SkillSlotIndex skillSlotIndex)
		{
			if (skillSlotIndex == SkillSlotIndex.WeaponSkill)
			{
				characterSkill.EvolutionSkill(Character.GetEquipWeaponMasteryType());
				return;
			}

			characterSkill.EvolutionSkill(skillSlotIndex);
		}


		public virtual void ResetSkillCooldown()
		{
			characterSkill.ResetCooldown();
			MonoBehaviourInstance<GameUI>.inst.HudUpdateStartUltimateSkillCooldown(Character.ObjectId, 0f, 0f);
		}


		public virtual void StartSkillCooldown(SkillSlotSet skillSlotSet, MasteryType masteryType, float cooldown)
		{
			if (skillSlotSet == SkillSlotSet.WeaponSkill)
			{
				characterSkill.StartCooldown(masteryType, MonoBehaviourInstance<ClientService>.inst.CurrentSkillTime,
					cooldown);
			}
			else
			{
				characterSkill.StartCooldown(skillSlotSet, MonoBehaviourInstance<ClientService>.inst.CurrentSkillTime,
					cooldown);
			}

			SkillSlotIndex? skillSlotIndex = characterSkill.GetSkillSlotIndex(skillSlotSet);
			if (skillSlotIndex == null)
			{
				return;
			}

			if (skillSlotIndex.Value == SkillSlotIndex.Active4)
			{
				MonoBehaviourInstance<GameUI>.inst.HudUpdateStartUltimateSkillCooldown(Character.ObjectId, cooldown,
					cooldown);
			}
		}


		public virtual bool SwitchSkillSet(SkillSlotIndex skillSlotIndex, SkillSlotSet skillSlotSet)
		{
			bool result = characterSkill.SwitchSkillSet(skillSlotIndex, skillSlotSet);
			if (skillSlotIndex == SkillSlotIndex.Active4)
			{
				float cooldown = characterSkill.GetCooldown(skillSlotSet,
					MonoBehaviourInstance<ClientService>.inst.CurrentServerFrameTime);
				MonoBehaviourInstance<GameUI>.inst.HudUpdateStartUltimateSkillCooldown(Character.ObjectId, cooldown,
					characterSkill.GetMaxCooldown(skillSlotSet));
				MonoBehaviourInstance<GameUI>.inst.HudUpdateTeamUltimateSkillIcon(Character);
			}

			return result;
		}


		public virtual void ModifySkillCooldown(SkillSlotSet skillSlotSet, float modifyValue)
		{
			if (skillSlotSet == SkillSlotSet.WeaponSkill)
			{
				characterSkill.ModifyCooldown(Character.GetEquipWeaponMasteryType(),
					MonoBehaviourInstance<ClientService>.inst.CurrentSkillTime, modifyValue);
			}
			else
			{
				characterSkill.ModifyCooldown(skillSlotSet, MonoBehaviourInstance<ClientService>.inst.CurrentSkillTime,
					modifyValue);
			}

			if (skillSlotSet.SlotSet2Index() == SkillSlotIndex.Active4)
			{
				float cooldown = characterSkill.GetCooldown(skillSlotSet,
					MonoBehaviourInstance<ClientService>.inst.CurrentSkillTime);
				MonoBehaviourInstance<GameUI>.inst.HudUpdateModifyUltimateSkillCooldown(Character.ObjectId, cooldown);
			}
		}


		public virtual void HoldSkillCooldown(SkillSlotSet skillSlotSet, MasteryType masteryType, bool isHold)
		{
			if (skillSlotSet == SkillSlotSet.WeaponSkill)
			{
				CharacterSkill.SetHoldCooldown(masteryType, isHold);
			}
			else
			{
				CharacterSkill.SetHoldCooldown(skillSlotSet, isHold);
			}

			SkillSlotIndex? skillSlotIndex = characterSkill.GetSkillSlotIndex(skillSlotSet);
			if (skillSlotIndex != null && skillSlotIndex.Value == SkillSlotIndex.Active4)
			{
				MonoBehaviourInstance<GameUI>.inst.HudUpdateHoldUltimateSkillCooldown(Character.ObjectId);
			}
		}


		public virtual void ResetSkillSequence(SkillSlotSet skillSlotSet)
		{
			characterSkill.ResetSkillSequence(skillSlotSet);
		}


		public virtual void SetSkillSequence(SkillSlotSet skillSlotSet, MasteryType masteryType, int sequence,
			float duration, float sequenceCooldown)
		{
			characterSkill.SetSkillSequence(skillSlotSet, sequence);
			characterSkill.SetSequenceDuration(skillSlotSet, masteryType, duration, sequenceCooldown,
				MonoBehaviourInstance<ClientService>.inst.CurrentSkillTime);
		}


		public int GetLocalSkillSequence(SkillSlotSet skillSlotSet)
		{
			return characterSkill.GetLocalSkillSequence(skillSlotSet);
		}


		public int GetServerSkillSequence(SkillSlotSet skillSlotSet)
		{
			return characterSkill.GetSkillSequence(skillSlotSet);
		}


		public virtual void UpdateSkillEvolutionPoint(SkillEvolutionPointType pointType, int evolutionPoint)
		{
			characterSkill.UpdateSkillEvolutionPoint(pointType, evolutionPoint);
		}


		public bool IsHaveSkillEvolutionPoint(SkillSlotIndex skillSlotIndex)
		{
			return characterSkill.SkillEvolution.IsHavePoint(skillSlotIndex);
		}


		public void SetSkillSequence(SkillSlotSet skillSlotSet, int seq)
		{
			characterSkill.SetSkillSequence(skillSlotSet, seq);
		}


		public SkillSlotSet? GetSkillSlotSet(SkillSlotIndex skillSlotIndex)
		{
			return characterSkill.GetSkillSlotSet(skillSlotIndex);
		}


		public void SetIsUseTempNickname(bool isUse)
		{
			isUseTempNickname = isUse;
		}
	}
}