using System;
using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	public class StateEffector : MonoBehaviour
	{
		
		public List<CharacterStateValue> CreateSnapshot()
		{
			List<CharacterStateValue> list = new List<CharacterStateValue>();
			foreach (KeyValuePair<StateType, List<CharacterState>> keyValuePair in this.characterStates)
			{
				for (int i = 0; i < keyValuePair.Value.Count; i++)
				{
					CharacterState characterState = keyValuePair.Value[i];
					list.Add(new CharacterStateValue(characterState.StateData.code, characterState.CreatedTime, characterState.StateData.duration, characterState.StackCount, characterState.ReserveCount, characterState.Caster.ObjectId, characterState.OriginalDuration));
				}
			}
			return list;
		}

		
		public void Init()
		{
			this.characterStates.Clear();
			this.cachedStats.Clear();
			this.isChangedStats = false;
			this.isCalculatedStats = false;
		}

		
		public bool AnyHaveStateByGroup(int stateGroup)
		{
			foreach (KeyValuePair<StateType, List<CharacterState>> keyValuePair in this.characterStates)
			{
				using (List<CharacterState>.Enumerator enumerator2 = keyValuePair.Value.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (enumerator2.Current.Group == stateGroup)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		
		public bool IsHaveStateByGroup(int stateGroup, int casterId)
		{
			foreach (List<CharacterState> list in this.characterStates.Values)
			{
				foreach (CharacterState characterState in list)
				{
					if (characterState.Group == stateGroup && (casterId == 0 || casterId == characterState.Caster.ObjectId))
					{
						return true;
					}
				}
			}
			return false;
		}

		
		public bool IsHaveStateByType(StateType stateType)
		{
			return this.characterStates.ContainsKey(stateType) && this.characterStates[stateType].Count != 0;
		}

		
		public void Overwrite(int stateCode, int casterId)
		{
			CharacterStateData data = GameDB.characterState.GetData(stateCode);
			CharacterState characterState = this.FindStateByGroup(data.group, casterId);
			if (characterState != null)
			{
				characterState.UpdateData(data);
				characterState.CalculateStats();
			}
		}

		
		public void AddWithoutCallback(CharacterState state, int casterId)
		{
			if (this.IsImmune(state))
			{
				return;
			}
			if (state.StateGroupData.stateType.IsCrowdControl())
			{
				this.AddCrowdControl(state, casterId, false);
				return;
			}
			this.AddState(state, casterId, false);
		}

		
		public void AddModeModifierStat(CharacterState state, int casterId)
		{
			this.AddState(state, casterId, false);
		}

		
		public void StatUpdateInstantly()
		{
			this.isChangedStats = false;
			this.isCalculatedStats = false;
			this.MergeStats();
			StateEffector.ChangedStat onChangedStat = this.OnChangedStat;
			if (onChangedStat == null)
			{
				return;
			}
			onChangedStat();
		}

		
		public void Add(CharacterState state, int casterId)
		{
			if (this.IsImmune(state))
			{
				if (state.Group == 5000000)
				{
					Log.V("[WICKLINE DEAD] Not Have KillBuff!!! : IsImmune is true");
				}
				return;
			}
			if (state.StateGroupData.stateType.IsCrowdControl())
			{
				this.AddCrowdControl(state, casterId, true);
				return;
			}
			this.AddState(state, casterId, true);
		}

		
		private void AddState(CharacterState state, int casterId, bool useAddCallback)
		{
			CharacterState characterState;
			if (state.StateGroupData.notCheckCasterId)
			{
				characterState = this.FindStateByGroup(state.Group, 0);
			}
			else
			{
				characterState = this.FindStateByGroup(state.Group, casterId);
			}
			if (characterState != null)
			{
				bool flag = false;
				if (characterState.MaxStack >= 2)
				{
					if (characterState.Level != state.Level || state.Caster.ObjectId != characterState.Caster.ObjectId)
					{
						state.AddStack(characterState.StackCount + ((state.StackCount == 0) ? state.StateGroupData.defaultStack : 0));
						this.Terminate(characterState, true);
						characterState = null;
						this.AddNewState(state);
						flag = true;
					}
					else
					{
						characterState.AddStack(state.StackCount);
						characterState.ResetCreateTime();
						this.SortStates(characterState.StateGroupData.stateType);
					}
				}
				else if (characterState.CanReserve)
				{
					characterState.Reserve(state);
				}
				else if (characterState.Level != state.Level || state.Caster.ObjectId != characterState.Caster.ObjectId)
				{
					this.Terminate(characterState, true);
					if (state.StateGroupData.notCheckCasterId)
					{
						StateEffector.ModifyStateWithCmd completeRemoveState = this.CompleteRemoveState;
						if (completeRemoveState != null)
						{
							completeRemoveState(characterState, true);
						}
					}
					characterState = null;
					this.AddNewState(state);
					flag = true;
				}
				else
				{
					characterState.ResetCreateTime();
					this.SortStates(characterState.StateGroupData.stateType);
					if (useAddCallback)
					{
						StateEffector.ModifyState completeResetCreateTimeState = this.CompleteResetCreateTimeState;
						if (completeResetCreateTimeState != null)
						{
							completeResetCreateTimeState(characterState);
						}
						useAddCallback = false;
					}
				}
				if (useAddCallback && !flag)
				{
					StateEffector.ModifyState completeChangedState = this.CompleteChangedState;
					if (completeChangedState != null)
					{
						completeChangedState(characterState);
					}
					useAddCallback = false;
				}
			}
			else
			{
				this.AddNewState(state);
			}
			if (state.StateGroupData.stateType == StateType.Unstoppable)
			{
				this.RemoveAllByType(StateType.Fetter);
			}
			if (characterState != null)
			{
				this.UpdateStats(characterState);
				if (useAddCallback)
				{
					StateEffector.ModifyState completeAddState = this.CompleteAddState;
					if (completeAddState != null)
					{
						completeAddState(characterState);
					}
				}
				characterState.UseSkill();
				return;
			}
			this.UpdateStats(state);
			if (useAddCallback)
			{
				StateEffector.ModifyState completeAddState2 = this.CompleteAddState;
				if (completeAddState2 != null)
				{
					completeAddState2(state);
				}
			}
			state.UseSkill();
		}

		
		private void AddCrowdControl(CharacterState state, int casterId, bool useAddCallback)
		{
			if (this.IsBlockedByCrowdControl(state))
			{
				return;
			}
			this.CancelPlayingCrowdControl(state);
			this.PausePlayingCrowdControl(state);
			ValueTuple<bool, float> valueTuple = this.IsReduceCrowdControlDuration(state);
			bool item = valueTuple.Item1;
			float item2 = valueTuple.Item2;
			if (item)
			{
				float num = 1f - item2;
				if (num < 0f)
				{
					num = 0f;
				}
				float num2 = state.Duration * num;
				Log.H(string.Format("[DebuffReduce] {0} Duration : {1} -> {2}", state.StateGroupData.stateType, state.Duration, num2));
				state.SetDuration(num2);
			}
			this.AddState(state, casterId, useAddCallback);
		}

		
		private bool IsBlockedByCrowdControl(CharacterState state)
		{
			StateType stateType = state.StateGroupData.stateType;
			foreach (StateType playingType in this.characterStates.Keys)
			{
				if (stateType.IsBlockedByPlayingState(playingType))
				{
					return true;
				}
			}
			return false;
		}

		
		private void CancelPlayingCrowdControl(CharacterState addState)
		{
			StateType stateType = addState.StateGroupData.stateType;
			for (int i = 0; i < this.characterStateKeys.Count; i++)
			{
				StateType stateType2 = this.characterStateKeys[i];
				if (stateType2.IsCrowdControl() && stateType.IsCancelPlayingSkill(stateType2))
				{
					for (int j = 0; j < this.characterStates[stateType2].Count; j++)
					{
						CharacterState state = this.characterStates[stateType2][j];
						this.Terminate(state, true);
						this.UpdateStats(state);
						StateEffector.ModifyStateWithCmd completeRemoveState = this.CompleteRemoveState;
						if (completeRemoveState != null)
						{
							completeRemoveState(state, true);
						}
						j--;
					}
				}
			}
		}

		
		private void PausePlayingCrowdControl(CharacterState addState)
		{
			StateType stateType = addState.StateGroupData.stateType;
			foreach (KeyValuePair<StateType, List<CharacterState>> keyValuePair in this.characterStates)
			{
				if (keyValuePair.Key.IsCrowdControl())
				{
					if (stateType.IsPausePlayingSkill(keyValuePair.Key))
					{
						foreach (CharacterState characterState in keyValuePair.Value)
						{
							characterState.SkillPause();
						}
					}
					if (keyValuePair.Key.IsPausePlayingSkill(stateType))
					{
						addState.SkillPause();
					}
				}
			}
		}

		
		private bool IsImmune(CharacterState state)
		{
			return this.characterStates.ContainsKey(StateType.Unstoppable) && StateType.Unstoppable.IsImmune(state.StateGroupData.stateType);
		}

		
		// [return: TupleElementNames(new string[]
		// {
		// 	"isReduce",
		// 	"fReduceValue"
		// })]
		private (bool, float) IsReduceCrowdControlDuration(CharacterState state)
		{
			if (this.characterStates.ContainsKey(StateType.ReduceCCType01) && StateType.ReduceCCType01.IsReduceDurationType01(state.StateGroupData.stateType))
			{
				float num = 0f;
				foreach (CharacterState characterState in this.characterStates[StateType.ReduceCCType01])
				{
					foreach (StatParameter statParameter in characterState.ExternalStats)
					{
						if (statParameter.statType.IsReduceCrowdControlDurationType(state.StateGroupData.stateType))
						{
							num += statParameter.statValue;
						}
					}
				}
				return new ValueTuple<bool, float>(true, num);
			}
			return new ValueTuple<bool, float>(false, 0f);
		}

		
		public CharacterState FindStateByGroup(int stateGroup, int casterId)
		{
			foreach (KeyValuePair<StateType, List<CharacterState>> characterState1 in this.characterStates)
			{
				CharacterState characterState2 = 0 < casterId ? characterState1.Value.Find((Predicate<CharacterState>) (x => x.Group.Equals(stateGroup) && x.Caster.ObjectId.Equals(casterId))) : characterState1.Value.Find((Predicate<CharacterState>) (x => x.Group.Equals(stateGroup)));
				if (characterState2 != null)
					return characterState2;
			}
			return (CharacterState) null;
			
			// co: dotPeek
			// Predicate<CharacterState> <>9__0;
			// Predicate<CharacterState> <>9__1;
			// foreach (KeyValuePair<StateType, List<CharacterState>> keyValuePair in this.characterStates)
			// {
			// 	CharacterState characterState;
			// 	if (0 >= casterId)
			// 	{
			// 		List<CharacterState> value = keyValuePair.Value;
			// 		Predicate<CharacterState> match;
			// 		if ((match = <>9__1) == null)
			// 		{
			// 			match = (<>9__1 = ((CharacterState x) => x.Group.Equals(stateGroup)));
			// 		}
			// 		characterState = value.Find(match);
			// 	}
			// 	else
			// 	{
			// 		List<CharacterState> value2 = keyValuePair.Value;
			// 		Predicate<CharacterState> match2;
			// 		if ((match2 = <>9__0) == null)
			// 		{
			// 			match2 = (<>9__0 = ((CharacterState x) => x.Group.Equals(stateGroup) && x.Caster.ObjectId.Equals(casterId)));
			// 		}
			// 		characterState = value2.Find(match2);
			// 	}
			// 	CharacterState characterState2 = characterState;
			// 	if (characterState2 != null)
			// 	{
			// 		return characterState2;
			// 	}
			// }
			// return null;
		}

		
		private void AddNewState(CharacterState state)
		{
			if (!this.characterStates.ContainsKey(state.StateGroupData.stateType))
			{
				this.characterStateKeys.Add(state.StateGroupData.stateType);
				this.characterStates.Add(state.StateGroupData.stateType, new List<CharacterState>());
			}
			state.OnCalculateStat = new CharacterState.CalculateStat(this.OnCalculateStat);
			this.characterStates[state.StateGroupData.stateType].Add(state);
			WorldPlayerCharacter worldPlayerCharacter = state.Self as WorldPlayerCharacter;
			if (worldPlayerCharacter != null)
			{
				worldPlayerCharacter.CharacterSkill.SkillEvolution.OnStateStack(worldPlayerCharacter.CharacterCode, state.Group, state.StackCount, new Action<SkillEvolutionPointType, int>(worldPlayerCharacter.UpdateSkillEvolutionPoint));
			}
			state.Start(state.StackCount);
			this.SortStates(state.StateGroupData.stateType);
		}

		
		private void OnCalculateStat()
		{
			this.isCalculatedStats = true;
		}

		
		private void SortStates(StateType stateType)
		{
			if (this.characterStates.ContainsKey(stateType) && 1 < this.characterStates[stateType].Count)
			{
				this.characterStates[stateType].Sort((CharacterState x, CharacterState y) => x.RemainTime().CompareTo(y.RemainTime()));
			}
		}

		
		private void Terminate(CharacterState state, bool cancel)
		{
			state.Terminate(cancel);
			this.RemoveState(state);
		}

		
		private void RemoveState(CharacterState state)
		{
			StateType stateType = state.StateGroupData.stateType;
			if (this.characterStates.ContainsKey(stateType))
			{
				this.characterStates[stateType].Remove(state);
			}
			this.ResumePlayingCrowdControl(state);
		}

		
		private void ResumePlayingCrowdControl(CharacterState removeState)
		{
			StateType stateType = removeState.StateGroupData.stateType;
			foreach (KeyValuePair<StateType, List<CharacterState>> keyValuePair in this.characterStates)
			{
				if (keyValuePair.Key.IsCrowdControl() && stateType.IsPausePlayingSkill(keyValuePair.Key))
				{
					foreach (CharacterState characterState in keyValuePair.Value)
					{
						characterState.SkillResume();
					}
				}
			}
		}

		
		public bool RemoveAllByType(StateType stateType)
		{
			if (this.characterStates.ContainsKey(stateType))
			{
				for (int i = 0; i < this.characterStates[stateType].Count; i++)
				{
					CharacterState state = this.characterStates[stateType][i];
					this.Terminate(state, true);
					this.UpdateStats(state);
					StateEffector.ModifyStateWithCmd completeRemoveState = this.CompleteRemoveState;
					if (completeRemoveState != null)
					{
						completeRemoveState(state, true);
					}
					i--;
				}
				return true;
			}
			return false;
		}

		
		public bool RemoveByGroup(int stateGroup, int casterId)
		{
			CharacterState characterState = this.FindStateByGroup(stateGroup, casterId);
			if (characterState != null)
			{
				this.Terminate(characterState, true);
				this.UpdateStats(characterState);
				StateEffector.ModifyStateWithCmd completeRemoveState = this.CompleteRemoveState;
				if (completeRemoveState != null)
				{
					completeRemoveState(characterState, true);
				}
				return true;
			}
			return false;
		}

		
		public void FrameUpdate()
		{
			this.removeStateTypes.Clear();
			for (int i = 0; i < this.characterStateKeys.Count; i++)
			{
				StateType stateType = this.characterStateKeys[i];
				for (int j = 0; j < this.characterStates[stateType].Count; j++)
				{
					CharacterState characterState = this.characterStates[stateType][j];
					characterState.FrameUpdate();
					if (characterState.IsDone())
					{
						this.Terminate(characterState, false);
						this.UpdateStats(characterState);
						StateEffector.ModifyStateWithCmd completeRemoveState = this.CompleteRemoveState;
						if (completeRemoveState != null)
						{
							completeRemoveState(characterState, true);
						}
						j--;
						if (characterState.ReserveCount > 0)
						{
							characterState.ExecuteReserve();
							this.AddNewState(characterState);
							this.UpdateStats(characterState);
							StateEffector.ModifyState completeAddState = this.CompleteAddState;
							if (completeAddState != null)
							{
								completeAddState(characterState);
							}
							characterState.UseSkill();
						}
					}
				}
				if (this.characterStates[stateType].Count == 0)
				{
					this.removeStateTypes.Add(stateType);
				}
			}
			for (int k = 0; k < this.removeStateTypes.Count; k++)
			{
				this.characterStateKeys.Remove(this.removeStateTypes[k]);
				this.characterStates.Remove(this.removeStateTypes[k]);
			}
			if (this.isCalculatedStats)
			{
				this.isCalculatedStats = false;
				this.MergeStats();
				this.isChangedStats = true;
			}
			if (this.isChangedStats)
			{
				this.isChangedStats = false;
				StateEffector.ChangedStat onChangedStat = this.OnChangedStat;
				if (onChangedStat == null)
				{
					return;
				}
				onChangedStat();
			}
		}

		
		public Dictionary<StatType, float> GetAllStat()
		{
			this.ret.Clear();
			foreach (KeyValuePair<StatType, float> keyValuePair in this.cachedStats)
			{
				float num = keyValuePair.Value;
				if (keyValuePair.Key.IsRatio())
				{
					num /= 100f;
				}
				this.ret.Add(keyValuePair.Key, num);
			}
			return this.ret;
		}

		
		private void MergeStats()
		{
			this.cachedStats.Clear();
			this.minusResult.Clear();
			foreach (KeyValuePair<StateType, List<CharacterState>> keyValuePair in this.characterStates)
			{
				foreach (CharacterState characterState in keyValuePair.Value)
				{
					if (characterState.Group >= 0)
					{
						foreach (KeyValuePair<StatType, float> keyValuePair2 in characterState.GetStats())
						{
							if (keyValuePair2.Value < 0f)
							{
								StatType key = keyValuePair2.Key;
								if ((key == StatType.MoveSpeed || key == StatType.MoveSpeedRatio) && this.characterStates.ContainsKey(StateType.Unstoppable))
								{
									continue;
								}
							}
							if (0f <= keyValuePair2.Value)
							{
								if (this.cachedStats.ContainsKey(keyValuePair2.Key))
								{
									Dictionary<StatType, float> dictionary = this.cachedStats;
									StatType key = keyValuePair2.Key;
									dictionary[key] += keyValuePair2.Value;
								}
								else
								{
									this.cachedStats.Add(keyValuePair2.Key, keyValuePair2.Value);
								}
							}
							else if (this.minusResult.ContainsKey(keyValuePair2.Key))
							{
								if (keyValuePair2.Value < this.minusResult[keyValuePair2.Key])
								{
									this.minusResult[keyValuePair2.Key] = keyValuePair2.Value;
								}
							}
							else
							{
								this.minusResult.Add(keyValuePair2.Key, keyValuePair2.Value);
							}
						}
					}
				}
			}
			foreach (KeyValuePair<StatType, float> keyValuePair3 in this.minusResult)
			{
				if (this.cachedStats.ContainsKey(keyValuePair3.Key))
				{
					Dictionary<StatType, float> dictionary = this.cachedStats;
					StatType key = keyValuePair3.Key;
					dictionary[key] += this.minusResult[keyValuePair3.Key];
				}
				else
				{
					this.cachedStats.Add(keyValuePair3.Key, keyValuePair3.Value);
				}
			}
		}

		
		private void UpdateStats(CharacterState state)
		{
			if (this.IsHaveStat(state))
			{
				this.MergeStats();
				this.isChangedStats = true;
			}
			StateType stateType = state.StateGroupData.stateType;
			if (stateType == StateType.Shield)
			{
				this.isChangedStats = true;
			}
			if (0f <= state.StateData.forcedMoveSpeed)
			{
				this.isChangedStats = true;
			}
		}

		
		public List<CharacterState> GetAllStates()
		{
			List<CharacterState> list = new List<CharacterState>();
			foreach (KeyValuePair<StateType, List<CharacterState>> keyValuePair in this.characterStates)
			{
				if (0 < keyValuePair.Value.Count)
				{
					list.AddRange(keyValuePair.Value);
				}
			}
			return list;
		}

		
		private bool IsHaveStat(CharacterState state)
		{
			return 0 < state.GetStats().Count;
		}

		
		public void RemoveOnDead(bool sendPacket)
		{
			for (int i = 0; i < this.characterStateKeys.Count; i++)
			{
				StateType key = this.characterStateKeys[i];
				for (int j = 0; j < this.characterStates[key].Count; j++)
				{
					CharacterState characterState = this.characterStates[key][j];
					if (characterState.StateGroupData.removeOnDead)
					{
						this.Terminate(characterState, true);
						this.UpdateStats(characterState);
						StateEffector.ModifyStateWithCmd completeRemoveState = this.CompleteRemoveState;
						if (completeRemoveState != null)
						{
							completeRemoveState(characterState, sendPacket);
						}
						j--;
					}
				}
			}
		}

		
		public int GetShield()
		{
			int num = 0;
			if (!this.characterStates.ContainsKey(StateType.Shield))
			{
				return num;
			}
			foreach (CharacterState characterState in this.characterStates[StateType.Shield])
			{
				ShieldState shieldState = characterState as ShieldState;
				if (shieldState != null)
				{
					num += shieldState.CurrentShieldAmount;
				}
			}
			return num;
		}

		
		public int Damage(DamageInfo damageInfo, out int blockedDamage)
		{
			int damage = damageInfo.Damage;
			this.BlockDamage(damageInfo, ref damage, out blockedDamage);
			return damage;
		}

		
		private void BlockDamage(DamageInfo damageInfo, ref int damage, out int blockedDamage)
		{
			blockedDamage = 0;
			if (damageInfo.DamageType == DamageType.RedZone)
			{
				return;
			}
			if (damage <= 0)
			{
				return;
			}
			for (int i = 0; i < this.characterStateKeys.Count; i++)
			{
				StateType key = this.characterStateKeys[i];
				for (int j = 0; j < this.characterStates[key].Count; j++)
				{
					CharacterState characterState = this.characterStates[key][j];
					IBlockDamage blockDamage = characterState as IBlockDamage;
					if (blockDamage != null)
					{
						int num = damage;
						damage = blockDamage.BlockDamage(damageInfo.DamagePoint, damageInfo.DamageSubType, damage);
						if (damage < num)
						{
							SingletonMonoBehaviour<BattleEventCollector>.inst.OnBlockDamage(characterState.Self, characterState.Caster.ObjectId, damageInfo.Attacker, damageInfo.UndefendedDamage, num - damage, damageInfo.DamagePoint, damageInfo.DamageSubType);
							blockedDamage += Mathf.Max(0, num - damage);
							this.isChangedStats = true;
						}
						if (damage == 0)
						{
							return;
						}
					}
				}
			}
		}

		
		public bool CanAction(ActionType actionType)
		{
			foreach (KeyValuePair<StateType, List<CharacterState>> keyValuePair in this.characterStates)
			{
				if (keyValuePair.Value.Count > 0 && keyValuePair.Key.IsCrowdControl() && keyValuePair.Key.CanNotAction().IsCanNotAction(actionType))
				{
					return false;
				}
			}
			return true;
		}

		
		public bool CanAction(CastingActionType castingActionType)
		{
			foreach (KeyValuePair<StateType, List<CharacterState>> keyValuePair in this.characterStates)
			{
				if (keyValuePair.Value.Count > 0 && keyValuePair.Key.IsCrowdControl() && keyValuePair.Key.CanNotAction().IsCanNotAction(castingActionType))
				{
					return false;
				}
			}
			return true;
		}

		
		public bool CanCharacterControl()
		{
			foreach (KeyValuePair<StateType, List<CharacterState>> keyValuePair in this.characterStates)
			{
				if (keyValuePair.Value.Count > 0 && keyValuePair.Key.IsCrowdControl() && !keyValuePair.Key.CanControl())
				{
					return false;
				}
			}
			return true;
		}

		
		public bool CanNormalAttack()
		{
			foreach (KeyValuePair<StateType, List<CharacterState>> keyValuePair in this.characterStates)
			{
				if (keyValuePair.Value.Count > 0 && keyValuePair.Key.IsCrowdControl() && !keyValuePair.Key.CanNormalAttack())
				{
					return false;
				}
			}
			return true;
		}

		
		public bool CanUseSkill(SkillData skillData)
		{
			if (skillData.CanCastingWhileCCState)
			{
				return true;
			}
			if (!this.CanUseSkill())
			{
				return false;
			}
			if (skillData.AggressiveSkill && skillData.MovementSkill)
			{
				return this.CanUseAggressiveSkill() && this.CanUseMovementSkill();
			}
			if (skillData.AggressiveSkill)
			{
				return this.CanUseAggressiveSkill();
			}
			return !skillData.MovementSkill || this.CanUseMovementSkill();
		}

		
		private bool CanUseSkill()
		{
			foreach (KeyValuePair<StateType, List<CharacterState>> keyValuePair in this.characterStates)
			{
				if (keyValuePair.Value.Count > 0 && keyValuePair.Key.IsCrowdControl() && !keyValuePair.Key.CanUseSkill())
				{
					return false;
				}
			}
			return true;
		}

		
		private bool CanUseAggressiveSkill()
		{
			foreach (KeyValuePair<StateType, List<CharacterState>> keyValuePair in this.characterStates)
			{
				if (keyValuePair.Value.Count > 0 && keyValuePair.Key.IsCrowdControl() && !keyValuePair.Key.CanUseAggressiveSkill())
				{
					return false;
				}
			}
			return true;
		}

		
		private bool CanUseMovementSkill()
		{
			foreach (KeyValuePair<StateType, List<CharacterState>> keyValuePair in this.characterStates)
			{
				if (keyValuePair.Value.Count > 0 && keyValuePair.Key.IsCrowdControl() && !keyValuePair.Key.CanUseMovementSkill())
				{
					return false;
				}
			}
			return true;
		}

		
		public bool CanMove()
		{
			foreach (KeyValuePair<StateType, List<CharacterState>> keyValuePair in this.characterStates)
			{
				if (keyValuePair.Value.Count > 0 && keyValuePair.Key.IsCrowdControl() && !keyValuePair.Key.CanMove())
				{
					return false;
				}
			}
			return true;
		}

		
		public int GetStackByGroup(int stateGroup, int casterId)
		{
			CharacterState characterState = this.FindStateByGroup(stateGroup, casterId);
			if (characterState == null)
			{
				return 0;
			}
			return characterState.StackCount;
		}

		
		public void ModifyStateValue(int stateGroup, int casterId, float durationChangeAmount, int changeStackCount, bool isResetCreateedTime)
		{
			CharacterState characterState = this.FindStateByGroup(stateGroup, casterId);
			if (characterState == null || (durationChangeAmount == 0f && changeStackCount == 0 && !isResetCreateedTime))
			{
				return;
			}
			if (isResetCreateedTime)
			{
				characterState.ResetCreateTime();
			}
			if (changeStackCount > 0)
			{
				characterState.AddStack(changeStackCount);
			}
			else if (changeStackCount < 0)
			{
				characterState.RemoveStack(-changeStackCount);
			}
			if (characterState.StackCount == 0 && characterState.StateGroupData.defaultStack != 0)
			{
				this.RemoveByGroup(characterState.Group, casterId);
				return;
			}
			if (durationChangeAmount != 0f)
			{
				if (durationChangeAmount < 0f && characterState.Duration <= -durationChangeAmount)
				{
					this.RemoveByGroup(stateGroup, casterId);
					return;
				}
				characterState.ModifyDuration(durationChangeAmount);
			}
			StateEffector.ModifyState completeChangedState = this.CompleteChangedState;
			if (completeChangedState == null)
			{
				return;
			}
			completeChangedState(characterState);
		}

		
		public void DurationPauseState(int stateGroup, int casterId, float deltaPauseTime)
		{
			CharacterState characterState = this.FindStateByGroup(stateGroup, casterId);
			if (characterState == null)
			{
				return;
			}
			if (deltaPauseTime <= 0f)
			{
				return;
			}
			characterState.DurationPause(deltaPauseTime);
			StateEffector.ModifyState completePauseState = this.CompletePauseState;
			if (completePauseState == null)
			{
				return;
			}
			completePauseState(characterState);
		}

		
		public bool AnyCrowdControlState()
		{
			foreach (KeyValuePair<StateType, List<CharacterState>> keyValuePair in this.characterStates)
			{
				if (keyValuePair.Key.IsNegativelyCrowdControl())
				{
					return true;
				}
			}
			return false;
		}

		
		public bool AnyNegativelyAffectsMovementState()
		{
			foreach (KeyValuePair<StateType, List<CharacterState>> keyValuePair in this.characterStates)
			{
				if (keyValuePair.Key.IsNegativelyAffectMovement())
				{
					return true;
				}
			}
			return false;
		}

		
		public bool AnyForcedMoveSpeedState()
		{
			foreach (KeyValuePair<StateType, List<CharacterState>> keyValuePair in this.characterStates)
			{
				using (List<CharacterState>.Enumerator enumerator2 = keyValuePair.Value.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (enumerator2.Current.StateData.forcedMoveSpeed >= 0f)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		
		public float GetForcedMoveSpeed()
		{
			ForcedMoveSpeedType forcedMoveSpeedType = ForcedMoveSpeedType.None;
			float num = 0f;
			foreach (KeyValuePair<StateType, List<CharacterState>> keyValuePair in this.characterStates)
			{
				foreach (CharacterState characterState in keyValuePair.Value)
				{
					float forcedMoveSpeed = characterState.StateData.forcedMoveSpeed;
					ForcedMoveSpeedType forcedMoveSpeedType2 = characterState.StateData.GroupData.forcedMoveSpeedType;
					if (forcedMoveSpeed > 0f && forcedMoveSpeedType2 >= forcedMoveSpeedType)
					{
						if (forcedMoveSpeedType < forcedMoveSpeedType2)
						{
							forcedMoveSpeedType = forcedMoveSpeedType2;
							num = forcedMoveSpeed;
						}
						else if (num < forcedMoveSpeed)
						{
							num = forcedMoveSpeed;
						}
					}
				}
			}
			return num;
		}

		
		public void SetExternalNonCalculateStatValue(int stateGroup, int casterId, float statValue)
		{
			CharacterState characterState = this.FindStateByGroup(stateGroup, casterId);
			if (characterState == null)
			{
				return;
			}
			characterState.SetExternalNonCalculateStatValue(statValue);
		}

		
		public void ActivateStatCalculator(int stateGroup, int casterId, bool activate)
		{
			CharacterState characterState = this.FindStateByGroup(stateGroup, casterId);
			if (characterState == null)
			{
				return;
			}
			characterState.ActivateCalculator(activate);
		}

		
		public List<KeyValuePair<int, int>> GetBuffers(int myObjectId)
		{
			this.supporters.Clear();
			foreach (KeyValuePair<StateType, List<CharacterState>> keyValuePair in this.characterStates)
			{
				for (int i = 0; i < keyValuePair.Value.Count; i++)
				{
					CharacterState state = keyValuePair.Value[i];
					if (state.Caster.ObjectId != myObjectId && state.StateGroupData.effectType == EffectType.Buff && !this.supporters.Any((KeyValuePair<int, int> x) => x.Key == state.Caster.ObjectId))
					{
						this.supporters.Add(new KeyValuePair<int, int>(state.Caster.ObjectId, state.Caster.TeamNumber));
					}
				}
			}
			return this.supporters;
		}

		
		public List<KeyValuePair<int, int>> GetDebuffers(int myObjectId)
		{
			this.assistants.Clear();
			foreach (KeyValuePair<StateType, List<CharacterState>> keyValuePair in this.characterStates)
			{
				for (int i = 0; i < keyValuePair.Value.Count; i++)
				{
					CharacterState state = keyValuePair.Value[i];
					if (state.Caster.ObjectId != myObjectId && state.StateGroupData.effectType == EffectType.Debuff && !this.assistants.Any((KeyValuePair<int, int> x) => x.Key == state.Caster.ObjectId))
					{
						this.assistants.Add(new KeyValuePair<int, int>(state.Caster.ObjectId, state.Caster.TeamNumber));
					}
				}
			}
			return this.assistants;
		}

		
		public void OnChangeHp(int hp)
		{
			foreach (KeyValuePair<StateType, List<CharacterState>> keyValuePair in this.characterStates)
			{
				for (int i = 0; i < keyValuePair.Value.Count; i++)
				{
					CharacterState characterState = keyValuePair.Value[i];
					if (characterState.StateData.GroupData.statCalculationType == StateStatCalculationType.LostHpRate)
					{
						characterState.CalculateStats();
					}
				}
			}
		}

		
		public void OnChangeMaxHp(int maxHp)
		{
			foreach (KeyValuePair<StateType, List<CharacterState>> keyValuePair in this.characterStates)
			{
				for (int i = 0; i < keyValuePair.Value.Count; i++)
				{
					CharacterState characterState = keyValuePair.Value[i];
					if (characterState.StateData.GroupData.statCalculationType == StateStatCalculationType.LostHpRate)
					{
						characterState.CalculateStats();
					}
				}
			}
		}

		
		private readonly Dictionary<StateType, List<CharacterState>> characterStates = new Dictionary<StateType, List<CharacterState>>(SingletonComparerEnum<StateTypeComparer, StateType>.Instance);

		
		private readonly List<StateType> characterStateKeys = new List<StateType>();

		
		private readonly Dictionary<StatType, float> cachedStats = new Dictionary<StatType, float>(SingletonComparerEnum<StatTypeComparer, StatType>.Instance);

		
		private readonly Dictionary<StatType, float> minusResult = new Dictionary<StatType, float>(SingletonComparerEnum<StatTypeComparer, StatType>.Instance);

		
		private readonly Dictionary<StatType, float> ret = new Dictionary<StatType, float>(SingletonComparerEnum<StatTypeComparer, StatType>.Instance);

		
		public StateEffector.ModifyState CompleteAddState = delegate(CharacterState p0)
		{
		};

		
		public StateEffector.ModifyStateWithCmd CompleteRemoveState = delegate(CharacterState p0, bool p1)
		{
		};

		
		public StateEffector.ModifyState CompleteChangedState = delegate(CharacterState p0)
		{
		};

		
		public StateEffector.ModifyState CompleteResetCreateTimeState = delegate(CharacterState p0)
		{
		};

		
		public StateEffector.ModifyState CompletePauseState = delegate(CharacterState p0)
		{
		};

		
		public StateEffector.ChangedStat OnChangedStat = delegate()
		{
		};

		
		private bool isChangedStats;

		
		private bool isCalculatedStats;

		
		private readonly List<StateType> removeStateTypes = new List<StateType>();

		
		private readonly List<KeyValuePair<int, int>> supporters = new List<KeyValuePair<int, int>>();

		
		private readonly List<KeyValuePair<int, int>> assistants = new List<KeyValuePair<int, int>>();

		
		public delegate void ChangedStat();

		
		public delegate void ModifyState(CharacterState state);

		
		public delegate void ModifyStateWithCmd(CharacterState state, bool sendPacket);
	}
}
