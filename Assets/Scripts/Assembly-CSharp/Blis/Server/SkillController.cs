using System;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	public class SkillController : MonoBehaviour
	{
		
		public void Init(WorldMovableCharacter wmc)
		{
			this.worldMovableCharacter = wmc;
		}

		
		public SkillControllerSnapshot CreateSnapshot()
		{
			List<SkillScriptSnapshot> list = new List<SkillScriptSnapshot>();
			List<SkillScriptSnapshot> list2 = new List<SkillScriptSnapshot>();
			List<SkillScriptSnapshot> list3 = new List<SkillScriptSnapshot>();
			foreach (SkillScript skillScript in this.playingScripts.Values)
			{
				SkillUseInfo info = skillScript.Info;
				list.Add(new SkillScriptSnapshot
				{
					skillId = info.skillData.SkillId,
					skillCode = info.skillData.code,
					skillEvolutionLevel = info.skillEvolutionLevel,
					cursorPosition = new BlisVector(info.cursorPosition),
					releasePosition = new BlisVector(info.releasePosition),
					targetObjectId = ((info.target != null) ? info.target.ObjectId : 0)
				});
			}
			foreach (SkillScript skillScript2 in this.passiveScripts.Values)
			{
				SkillUseInfo info2 = skillScript2.Info;
				list2.Add(new SkillScriptSnapshot
				{
					skillId = info2.skillData.PassiveSkillId,
					skillCode = info2.skillData.code,
					skillEvolutionLevel = info2.skillEvolutionLevel,
					cursorPosition = new BlisVector(info2.cursorPosition),
					releasePosition = new BlisVector(info2.releasePosition),
					targetObjectId = ((info2.target != null) ? info2.target.ObjectId : 0)
				});
			}
			foreach (Dictionary<int, SkillScript> dictionary in this.playingStateScripts.Values)
			{
				foreach (SkillScript skillScript3 in dictionary.Values)
				{
					SkillUseInfo info3 = skillScript3.Info;
					list3.Add(new SkillScriptSnapshot
					{
						skillId = info3.stateData.GroupData.skillId,
						skillCode = info3.skillData.code,
						skillEvolutionLevel = info3.skillEvolutionLevel,
						cursorPosition = new BlisVector(info3.cursorPosition),
						releasePosition = new BlisVector(info3.releasePosition),
						casterId = skillScript3.Caster.ObjectId
					});
				}
			}
			if (list.Count > 0 || list2.Count > 0 || list3.Count > 0)
			{
				return new SkillControllerSnapshot
				{
					playingScripts = list,
					passiveScripts = list2,
					playingStateScripts = list3
				};
			}
			return null;
		}

		
		public void SetActionOnPlaySkill(Action<SkillUseInfo> action)
		{
			this.OnPlay = action;
		}

		
		public void SetActionOnPlayPassiveSkill(Action<SkillUseInfo> action)
		{
			this.OnPlayPassive = action;
		}

		
		public void SetActionOnConsumeSkillResources(Action<SkillData> action)
		{
			this.OnConsumeSkillResources = action;
		}

		
		public void SetActionOnStartSkill(Action<SkillUseInfo> action)
		{
			this.OnStart = action;
		}

		
		public void SetActionOnStartPassiveSkill(Action<SkillUseInfo> action)
		{
			this.OnStartPassive = action;
		}

		
		public void SetActionOnStartStateSkill(Action<SkillUseInfo, int> action)
		{
			this.OnStartState = action;
		}

		
		public void SetActionOnFinishSkill(Action<SkillSlotSet, MasteryType, SkillId, bool, bool, bool> action)
		{
			this.OnFinish = action;
		}

		
		public void SetActionOnFinishStateSkill(Action<SkillSlotSet, MasteryType, SkillId, bool, bool> action)
		{
			this.OnFinishState = action;
		}

		
		public void SetActionOnFinishPassiveSkill(Action<SkillSlotSet, SkillId, bool, bool> action)
		{
			this.OnFinishPassive = action;
		}

		
		private SkillScript GetSkillScript(SkillId skillId, int sequence)
		{
			if (!this.skillScripts.ContainsKey(skillId))
			{
				this.skillScripts.Add(skillId, new Dictionary<int, SkillScript>());
			}
			if (!this.skillScripts[skillId].ContainsKey(sequence))
			{
				this.skillScripts[skillId].Add(sequence, this.skillScriptManager.Create(skillId));
			}
			return this.skillScripts[skillId][sequence];
		}

		
		private Dictionary<int, SkillScript> GetSkillScripts(SkillId skillId)
		{
			if (!this.skillScripts.ContainsKey(skillId))
			{
				this.skillScripts.Add(skillId, new Dictionary<int, SkillScript>());
			}
			return this.skillScripts[skillId];
		}

		
		public void ResetCooldown()
		{
			this.CancelAll(this.playingScripts);
			this.CancelAll(this.playingStateScripts);
		}

		
		public void Inject(SkillUseInfo skillUseInfo)
		{
			if (skillUseInfo.skillSlotSet.IsNormalAttack())
			{
				return;
			}
			if (skillUseInfo.skillData == null)
			{
				return;
			}
			this.Cast(skillUseInfo, 0);
		}

		
		public void PlayAgain(SkillData skillData, int sequence, Vector3 cursorPosition)
		{
			SkillScript skillScript = this.GetSkillScript(skillData.SkillId, sequence);
			if (skillScript == null)
			{
				return;
			}
			skillScript.PlayAgain(cursorPosition);
		}

		
		public void PlayAgain(SkillData skillData, int sequence, WorldCharacter hitTarget)
		{
			SkillScript skillScript = this.GetSkillScript(skillData.SkillId, sequence);
			if (skillScript == null)
			{
				return;
			}
			skillScript.PlayAgain(hitTarget);
		}

		
		public void OnSelect(WorldCharacter hitTarget, Vector3 hitPosition, Vector3 releasePosition)
		{
			foreach (KeyValuePair<SkillId, SkillScript> keyValuePair in this.playingScripts)
			{
				if (keyValuePair.Value.canAdditionalAction)
				{
					keyValuePair.Value.OnSelect(hitTarget, hitPosition, releasePosition);
				}
			}
		}

		
		public void CastPassiveSkill(SkillUseInfo skillUseInfo)
		{
			if (skillUseInfo.skillData.PassiveSkillId == SkillId.None)
			{
				return;
			}
			Action<SkillUseInfo> onPlayPassive = this.OnPlayPassive;
			if (onPlayPassive == null)
			{
				return;
			}
			onPlayPassive(skillUseInfo);
		}

		
		public void OverwritePassiveSkill(SkillUseInfo skillUseInfo)
		{
			SkillId passiveSkillId = skillUseInfo.skillData.PassiveSkillId;
			if (passiveSkillId == SkillId.None)
			{
				return;
			}
			if (this.passiveScripts.ContainsKey(passiveSkillId))
			{
				this.passiveScripts[passiveSkillId].OverwriteSkillUseInfo(skillUseInfo);
				this.passiveScripts[passiveSkillId].OnUpgradePassiveSkill();
			}
		}

		
		public void Cast(SkillUseInfo skillUseInfo, int sequence)
		{
			SingletonMonoBehaviour<BattleEventCollector>.inst.OnBeforeSkillActive(this.worldMovableCharacter, skillUseInfo.skillData, skillUseInfo.skillSlotSet);
			if (skillUseInfo.injected)
			{
				SkillId skillId = (skillUseInfo.stateData != null) ? skillUseInfo.stateData.GroupData.skillId : skillUseInfo.skillData.SkillId;
				if (skillId == SkillId.None)
				{
					return;
				}
				if (this.IsPlaying(skillId))
				{
					return;
				}
				SkillScript skillScript = this.skillScriptManager.Create(skillId);
				skillScript.Setup(skillUseInfo, this);
				this.CastInjectSkill(skillScript);
			}
			else
			{
				SkillScript skillScript2 = this.GetSkillScript(skillUseInfo.skillData.SkillId, sequence);
				skillScript2.Setup(skillUseInfo, this);
				this.CastMySkill(skillScript2, sequence);
			}
			WorldPlayerCharacter worldPlayerCharacter = this.worldMovableCharacter as WorldPlayerCharacter;
			MasteryType weaponType = (worldPlayerCharacter == null) ? MasteryType.None : worldPlayerCharacter.GetEquipWeaponMasteryType();
			this.worldMovableCharacter.CharacterSkill.UseSkillStack(skillUseInfo.skillSlotSet, weaponType, skillUseInfo.skillData, MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime);
			this.CastReserveOverlapSkill();
		}

		
		private void CastReserveOverlapSkill()
		{
			if (0 < this.reserveSkillInfos.Count)
			{
				SkillController.ReserveSkillInfo reserveSkillInfo = this.reserveSkillInfos[0];
				if (reserveSkillInfo.skillUseInfo.skillData.PlayType == SkillPlayType.Overlap)
				{
					this.reserveSkillInfos.RemoveAt(0);
					this.Cast(reserveSkillInfo.skillUseInfo, reserveSkillInfo.sequence);
				}
			}
		}

		
		private void CastInjectSkill(SkillScript skillScript)
		{
			if (this.Internal_AnyPlayingSkill())
			{
				SkillInterruptType interruptType = skillScript.Info.skillData.InterruptType;
				if (interruptType != SkillInterruptType.None)
				{
					if (interruptType == SkillInterruptType.Cancel)
					{
						if (this.CanActionDuringSkillPlaying())
						{
							this.Cancel();
						}
						else if (skillScript.Info.skillData.PlayType == SkillPlayType.Alone)
						{
							SkillController.ReserveSkillInfo item = new SkillController.ReserveSkillInfo
							{
								skillUseInfo = skillScript.Info,
								sequence = 0
							};
							this.reserveSkillInfos.Add(item);
							return;
						}
					}
				}
				else if (skillScript.Info.skillData.PlayType == SkillPlayType.Alone)
				{
					SkillController.ReserveSkillInfo item2 = new SkillController.ReserveSkillInfo
					{
						skillUseInfo = skillScript.Info,
						sequence = 0
					};
					this.reserveSkillInfos.Add(item2);
					return;
				}
			}
			Action<SkillUseInfo> onPlay = this.OnPlay;
			if (onPlay == null)
			{
				return;
			}
			onPlay(skillScript.Info);
		}

		
		public void PlayStateSkill(SkillUseInfo skillUseInfo, CharacterState state)
		{
			SkillId skillId = skillUseInfo.stateData.GroupData.skillId;
			if (skillId == SkillId.None)
			{
				return;
			}
			int objectId = skillUseInfo.caster.ObjectId;
			Dictionary<int, Coroutine> dictionary = null;
			Coroutine coroutine;
			if (this.playingStateSkills.TryGetValue(skillId, out dictionary) && dictionary.TryGetValue(objectId, out coroutine))
			{
				if (coroutine != null)
				{
					base.StopCoroutine(coroutine);
				}
				dictionary.Remove(objectId);
				if (dictionary.Count == 0)
				{
					this.playingStateSkills.Remove(skillId);
				}
			}
			Dictionary<int, SkillScript> dictionary2 = null;
			SkillScript skillScript = null;
			if (this.playingStateScripts.TryGetValue(skillId, out dictionary2))
			{
				if (dictionary2.TryGetValue(objectId, out skillScript))
				{
					skillScript.OverwriteSkillUseInfo(skillUseInfo);
				}
				else
				{
					skillScript = this.skillScriptManager.Create(skillId);
					skillScript.StartAction(new Action<SkillScript>(this.OnStartStateEvent));
					skillScript.FinishAction(new Action<SkillScript, bool, bool>(this.OnFinishStateEvent));
					skillScript.Setup(skillUseInfo, this);
					dictionary2.Add(objectId, skillScript);
				}
			}
			else
			{
				dictionary2 = new Dictionary<int, SkillScript>();
				skillScript = this.skillScriptManager.Create(skillId);
				skillScript.StartAction(new Action<SkillScript>(this.OnStartStateEvent));
				skillScript.FinishAction(new Action<SkillScript, bool, bool>(this.OnFinishStateEvent));
				skillScript.Setup(skillUseInfo, this);
				dictionary2.Add(objectId, skillScript);
				this.playingStateScripts.Add(skillId, dictionary2);
			}
			if (!this.playingStateSkills.TryGetValue(skillId, out dictionary))
			{
				Coroutine value = this.StartThrowingCoroutine(skillScript.Play(state), delegate(Exception exception)
				{
					Log.E(string.Format("[EXCEPTION][SKILL][{0}] Message:{1}, StackTrace:{2}", skillId, exception.Message, exception.StackTrace));
				});
				this.playingStateSkills.Add(skillId, new Dictionary<int, Coroutine>
				{
					{
						objectId,
						value
					}
				});
				return;
			}
			if (dictionary == null)
			{
				Coroutine value2 = this.StartThrowingCoroutine(skillScript.Play(state), delegate(Exception exception)
				{
					Log.E(string.Format("[EXCEPTION][SKILL][{0}] Message:{1}, StackTrace:{2}", skillId, exception.Message, exception.StackTrace));
				});
				dictionary = new Dictionary<int, Coroutine>
				{
					{
						objectId,
						value2
					}
				};
				this.playingStateSkills[skillId] = dictionary;
				return;
			}
			Coroutine value3 = this.StartThrowingCoroutine(skillScript.Play(state), delegate(Exception exception)
			{
				Log.E(string.Format("[EXCEPTION][SKILL][{0}] Message:{1}, StackTrace:{2}", skillId, exception.Message, exception.StackTrace));
			});
			dictionary[objectId] = value3;
		}

		
		public void PlayPassive(SkillUseInfo skillUseInfo)
		{
			SkillId skillId = skillUseInfo.skillData.PassiveSkillId;
			if (skillId == SkillId.None)
			{
				return;
			}
			if (this.passiveScripts.ContainsKey(skillId))
			{
				if (this.passiveScripts[skillId].Info.SkillLevel == skillUseInfo.SkillLevel)
				{
					return;
				}
				this.passiveScripts[skillId].Stop(false);
				this.passiveScripts.Remove(skillId);
			}
			if (this.passiveSkills.ContainsKey(skillId))
			{
				if (this.passiveSkills[skillId] != null)
				{
					base.StopCoroutine(this.passiveSkills[skillId]);
				}
				this.passiveSkills.Remove(skillId);
			}
			SkillScript skillScript = skillUseInfo.injected ? this.skillScriptManager.Create(skillId) : this.GetSkillScript(skillId, 0);
			this.passiveScripts.Add(skillId, skillScript);
			skillScript.Setup(skillUseInfo, this);
			skillScript.StartAction(new Action<SkillScript>(this.OnStartPassiveEvent));
			skillScript.FinishAction(new Action<SkillScript, bool, bool>(this.OnFinishPassiveEvent));
			Coroutine value = this.StartThrowingCoroutine(skillScript.Play(null), delegate(Exception exception)
			{
				Log.E(string.Format("[SKILL EXCEPTION][{0}] Message:{1}, StackTrace:{2}", skillId, exception.Message, exception.StackTrace));
			});
			this.passiveSkills.Add(skillId, value);
		}

		
		public void Play(SkillUseInfo skillUseInfo, int sequence)
		{
			SkillId skillId = skillUseInfo.skillData.SkillId;
			if (skillId == SkillId.None)
			{
				return;
			}
			SkillScript skillScript = skillUseInfo.injected ? this.skillScriptManager.Create(skillId) : this.GetSkillScript(skillId, sequence);
			skillScript.Setup(skillUseInfo, this);
			skillScript.StartAction(new Action<SkillScript>(this.OnStartEvent));
			skillScript.FinishAction(new Action<SkillScript, bool, bool>(this.OnFinishEvent));
			NormalAttackScript normalAttackScript = skillScript as NormalAttackScript;
			if (normalAttackScript != null)
			{
				normalAttackScript.FinishNormalAttackAction(new Action<SkillScript>(this.OnFinishNormalAttackEvent));
			}
			this.playingScripts.Add(skillId, skillScript);
			this.playingSkills.Add(skillId, null);
			Coroutine coroutine = this.StartThrowingCoroutine(skillScript.Play(null), delegate(Exception exception)
			{
				Log.E(string.Format("[SKILL EXCEPTION][{0}] Message:{1}, StackTrace:{2}", skillId, exception.Message, exception.StackTrace));
			});
			if (this.playingSkills.ContainsKey(skillId) && coroutine != null)
			{
				this.playingSkills[skillId] = coroutine;
			}
			if (skillScript.Info.skillSlotSet.IsNormalAttack())
			{
				this.StartNormalAttack();
			}
		}

		
		private void CastMySkill(SkillScript skillScript, int sequence)
		{
			switch (skillScript.SkillCastType)
			{
			case SkillCastType.Casting:
				this.CastCastingSkill(skillScript, sequence);
				return;
			case SkillCastType.Concentration:
				this.CastConcentrationSkill(skillScript, sequence);
				return;
			case SkillCastType.Channeling:
				this.CastChannelingSkill(skillScript, sequence);
				return;
			default:
				return;
			}
		}

		
		private bool IsPlayingSkill(SkillScript skillScript)
		{
			return this.playingScripts.ContainsKey(skillScript.SkillId);
		}

		
		private bool IsPlayingMySkill(int casterId, SkillSlotSet skillSlotSet)
		{
			foreach (KeyValuePair<SkillId, SkillScript> keyValuePair in this.playingScripts)
			{
				if (keyValuePair.Value.Info.skillSlotSet == skillSlotSet && keyValuePair.Value.Caster.ObjectId == casterId)
				{
					return true;
				}
			}
			return false;
		}

		
		private void CastCastingSkill(SkillScript skillScript, int sequence)
		{
			if (this.IsPlayingSkill(skillScript))
			{
				return;
			}
			if (this.Internal_AnyPlayingSkill())
			{
				if (skillScript.Info.skillSlotSet.IsNormalAttack())
				{
					return;
				}
				if (!skillScript.Info.skillData.InstantCast())
				{
					this.CancelNormalAttack();
				}
				SkillInterruptType interruptType = skillScript.Info.skillData.InterruptType;
				if (interruptType != SkillInterruptType.None)
				{
					if (interruptType == SkillInterruptType.Cancel)
					{
						if (this.CanActionDuringSkillPlaying())
						{
							this.Cancel();
						}
						else if (skillScript.Info.skillData.PlayType == SkillPlayType.Alone)
						{
							bool flag = true;
							foreach (SkillController.ReserveSkillInfo reserveSkillInfo in this.reserveSkillInfos)
							{
								if (!reserveSkillInfo.skillUseInfo.injected && reserveSkillInfo.skillUseInfo.SkillCode == skillScript.SkillCode)
								{
									flag = false;
									break;
								}
							}
							if (flag)
							{
								SkillController.ReserveSkillInfo item = new SkillController.ReserveSkillInfo
								{
									skillUseInfo = skillScript.Info,
									sequence = sequence
								};
								this.reserveSkillInfos.Add(item);
							}
							return;
						}
					}
				}
				else if (skillScript.Info.skillData.PlayType == SkillPlayType.Alone)
				{
					bool flag2 = true;
					foreach (SkillController.ReserveSkillInfo reserveSkillInfo2 in this.reserveSkillInfos)
					{
						if (!reserveSkillInfo2.skillUseInfo.injected && reserveSkillInfo2.skillUseInfo.SkillCode == skillScript.SkillCode)
						{
							flag2 = false;
							break;
						}
					}
					if (flag2)
					{
						SkillController.ReserveSkillInfo item2 = new SkillController.ReserveSkillInfo
						{
							skillUseInfo = skillScript.Info,
							sequence = sequence
						};
						this.reserveSkillInfos.Add(item2);
					}
					return;
				}
			}
			if (skillScript.Info.skillSlotSet.IsNormalAttack() && 0 < this.mountedNormalAttackSkillCode)
			{
				skillScript.Info.skillData = GameDB.skill.GetSkillData(this.mountedNormalAttackSkillCode);
				skillScript.Info.injected = true;
			}
			Action<SkillUseInfo> onPlay = this.OnPlay;
			if (onPlay != null)
			{
				onPlay(skillScript.Info);
			}
			if (!skillScript.Info.skillSlotSet.IsNormalAttack())
			{
				Action<SkillData> onConsumeSkillResources = this.OnConsumeSkillResources;
				if (onConsumeSkillResources == null)
				{
					return;
				}
				onConsumeSkillResources(skillScript.Info.skillData);
			}
		}

		
		private void CastConcentrationSkill(SkillScript skillScript, int sequence)
		{
			if (this.IsPlayingSkill(skillScript))
			{
				return;
			}
			if (this.Internal_AnyPlayingSkill())
			{
				if (skillScript.Info.skillSlotSet.IsNormalAttack())
				{
					return;
				}
				if (!skillScript.Info.skillData.InstantCast())
				{
					this.CancelNormalAttack();
				}
				SkillInterruptType interruptType = skillScript.Info.skillData.InterruptType;
				if (interruptType == SkillInterruptType.None)
				{
					return;
				}
				if (interruptType == SkillInterruptType.Cancel)
				{
					if (!this.CanActionDuringSkillPlaying())
					{
						return;
					}
					this.Cancel();
				}
			}
			Action<SkillUseInfo> onPlay = this.OnPlay;
			if (onPlay != null)
			{
				onPlay(skillScript.Info);
			}
			Action<SkillData> onConsumeSkillResources = this.OnConsumeSkillResources;
			if (onConsumeSkillResources == null)
			{
				return;
			}
			onConsumeSkillResources(skillScript.Info.skillData);
		}

		
		private void CastChannelingSkill(SkillScript skillScript, int sequence)
		{
		}

		
		private void OnStartEvent(SkillScript skillScript)
		{
			Action<SkillUseInfo> onStart = this.OnStart;
			if (onStart == null)
			{
				return;
			}
			onStart(skillScript.Info);
		}

		
		private void OnStartPassiveEvent(SkillScript skillScript)
		{
			Action<SkillUseInfo> onStartPassive = this.OnStartPassive;
			if (onStartPassive == null)
			{
				return;
			}
			onStartPassive(skillScript.Info);
		}

		
		private void OnStartStateEvent(SkillScript skillScript)
		{
			Action<SkillUseInfo, int> onStartState = this.OnStartState;
			if (onStartState == null)
			{
				return;
			}
			onStartState(skillScript.Info, skillScript.Caster.ObjectId);
		}

		
		private void OnFinishEvent(SkillScript skillScript, bool toNextSequence, bool cancel)
		{
			SingletonMonoBehaviour<BattleEventCollector>.inst.OnBeforeActiveSkillFinish(this.worldMovableCharacter, skillScript.Info.skillData, skillScript.Info.skillSlotSet, cancel);
			this.playingScripts.Remove(skillScript.SkillId);
			Coroutine coroutine;
			if (this.playingSkills.TryGetValue(skillScript.SkillId, out coroutine))
			{
				if (coroutine != null)
				{
					base.StopCoroutine(coroutine);
				}
				this.playingSkills.Remove(skillScript.SkillId);
			}
			SkillData skillData = skillScript.Info.skillData;
			bool flag = skillData.CooldownType == SkillCooldownType.SkillFinish;
			if (!flag && cancel && skillData.CooldownType == SkillCooldownType.StateEffectFinish && !this.worldMovableCharacter.StateEffector.IsHaveStateByGroup(skillData.CooldownStateFinish, skillScript.Caster.ObjectId))
			{
				flag = true;
			}
			Action<SkillSlotSet, MasteryType, SkillId, bool, bool, bool> onFinish = this.OnFinish;
			if (onFinish != null)
			{
				onFinish(skillScript.SkillSlotSet, skillScript.MasteryType, skillScript.SkillId, toNextSequence, cancel, flag);
			}
			if (0 < this.reserveSkillInfos.Count)
			{
				SkillController.ReserveSkillInfo reserveSkillInfo = this.reserveSkillInfos[0];
				this.reserveSkillInfos.RemoveAt(0);
				this.Cast(reserveSkillInfo.skillUseInfo, reserveSkillInfo.sequence);
			}
		}

		
		public void OnCrowdControl(StateType stateType, EffectType effectType)
		{
			bool flag = stateType.CanUseSkill();
			bool flag2 = stateType.CanUseAggressiveSkill();
			bool flag3 = stateType.CanUseMovementSkill();
			if (stateType.CancelNormalAttack())
			{
				this.CancelNormalAttack();
			}
			if (stateType.CancelAggressiveSkill())
			{
				this.CancelAggressiveSkill();
			}
			if (stateType.CancelMovementSkill())
			{
				this.CancelMovementSkill();
			}
			if (effectType.Equals(EffectType.Debuff) && !flag)
			{
				this.CancelConcentrationSkill(null);
			}
			for (int i = 0; i < this.reserveSkillInfos.Count; i++)
			{
				if (!this.reserveSkillInfos[i].skillUseInfo.injected)
				{
					if (!flag)
					{
						this.reserveSkillInfos.RemoveAt(i);
						i--;
					}
					else if (!flag2 || !flag3)
					{
						SkillGroupData skillGroupData = GameDB.skill.GetSkillGroupData(this.reserveSkillInfos[i].skillUseInfo.skillData.group);
						if ((!flag2 && skillGroupData.aggressiveSkill) || (!flag3 && skillGroupData.movementSkill))
						{
							this.reserveSkillInfos.RemoveAt(i);
							i--;
						}
					}
				}
			}
		}

		
		private void OnFinishNormalAttackEvent(SkillScript skillScript)
		{
			Action<SkillData> onConsumeSkillResources = this.OnConsumeSkillResources;
			if (onConsumeSkillResources == null)
			{
				return;
			}
			onConsumeSkillResources(skillScript.Info.skillData);
		}

		
		private void OnFinishStateEvent(SkillScript skillScript, bool toNextSequence, bool cancel)
		{
			SkillId skillId = skillScript.Info.stateData.GroupData.skillId;
			int objectId = skillScript.Caster.ObjectId;
			Dictionary<int, SkillScript> dictionary = null;
			if (this.playingStateScripts.TryGetValue(skillId, out dictionary) && dictionary.Remove(objectId) && dictionary.Count == 0)
			{
				this.playingStateScripts.Remove(skillId);
			}
			Dictionary<int, Coroutine> dictionary2 = null;
			Coroutine coroutine;
			if (this.playingStateSkills.TryGetValue(skillId, out dictionary2) && dictionary2.TryGetValue(objectId, out coroutine))
			{
				if (coroutine != null)
				{
					base.StopCoroutine(coroutine);
					dictionary2.Remove(objectId);
				}
				if (dictionary2.Count == 0)
				{
					this.playingStateSkills.Remove(skillId);
				}
			}
			bool arg = false;
			if (skillScript.Caster != null && skillScript.Target != null && objectId == skillScript.Target.ObjectId && skillScript.Info.skillData.CooldownStateFinish == skillScript.StateGroup)
			{
				arg = (skillScript.Info.skillData.CooldownType == SkillCooldownType.StateEffectFinish);
			}
			Action<SkillSlotSet, MasteryType, SkillId, bool, bool> onFinishState = this.OnFinishState;
			if (onFinishState == null)
			{
				return;
			}
			onFinishState(skillScript.SkillSlotSet, skillScript.MasteryType, skillId, cancel, arg);
		}

		
		private void OnFinishPassiveEvent(SkillScript skillScript, bool toNextSequence, bool cancel)
		{
			SkillData skillData = skillScript.Info.skillData;
			SkillId passiveSkillId = skillData.PassiveSkillId;
			this.passiveScripts.Remove(passiveSkillId);
			Coroutine coroutine;
			if (this.passiveSkills.TryGetValue(passiveSkillId, out coroutine))
			{
				if (coroutine != null)
				{
					base.StopCoroutine(coroutine);
				}
				this.passiveSkills.Remove(passiveSkillId);
			}
			bool flag = false;
			if (skillScript.Caster != null && skillScript.Target != null && skillScript.Caster.ObjectId == skillScript.Target.ObjectId && skillData.CooldownStateFinish == skillScript.StateGroup)
			{
				flag = (skillData.CooldownType == SkillCooldownType.SkillFinish);
			}
			if (!flag && cancel && skillData.CooldownType == SkillCooldownType.StateEffectFinish && !this.worldMovableCharacter.StateEffector.IsHaveStateByGroup(skillData.CooldownStateFinish, skillScript.Caster.ObjectId))
			{
				flag = true;
			}
			Action<SkillSlotSet, SkillId, bool, bool> onFinishPassive = this.OnFinishPassive;
			if (onFinishPassive == null)
			{
				return;
			}
			onFinishPassive(skillScript.SkillSlotSet, passiveSkillId, cancel, flag);
		}

		
		public bool IsPlayingStateSkill(SkillId skillId)
		{
			foreach (KeyValuePair<SkillId, Dictionary<int, SkillScript>> keyValuePair in this.playingStateScripts)
			{
				if (keyValuePair.Key == skillId)
				{
					return true;
				}
			}
			return false;
		}

		
		public bool IsPlaying(SkillId SkillId)
		{
			foreach (KeyValuePair<SkillId, SkillScript> keyValuePair in this.playingScripts)
			{
				if (keyValuePair.Value.Info.skillData.SkillId == SkillId)
				{
					return true;
				}
			}
			return false;
		}

		
		public bool IsPlaying(SkillSlotSet skillSlotSlot)
		{
			foreach (KeyValuePair<SkillId, SkillScript> keyValuePair in this.playingScripts)
			{
				if (keyValuePair.Value.Info.skillSlotSet == skillSlotSlot)
				{
					return true;
				}
			}
			return false;
		}

		
		public bool IsPlaying(SkillSlotIndex skillSlotIndex)
		{
			foreach (SkillSlotSet skillSlotSlot in skillSlotIndex.Index2SlotList())
			{
				if (this.IsPlaying(skillSlotSlot))
				{
					return true;
				}
			}
			return false;
		}

		
		public bool AnyPlayingSkill()
		{
			return 0 < this.playingScripts.Count;
		}

		
		public bool SkillScriptOnMove(Vector3 destination)
		{
			if (!this.worldMovableCharacter.CanMove())
			{
				return false;
			}
			foreach (SkillScript skillScript in this.playingScripts.Values)
			{
				if (skillScript.UseOnMove())
				{
					skillScript.OnMove(destination);
					return true;
				}
			}
			foreach (Dictionary<int, SkillScript> dictionary in this.playingStateScripts.Values)
			{
				foreach (SkillScript skillScript2 in dictionary.Values)
				{
					if (skillScript2.UseOnMove())
					{
						skillScript2.OnMove(destination);
						return true;
					}
				}
			}
			return false;
		}

		
		public bool SkillScriptOnTargetOn(WorldObject target)
		{
			foreach (SkillScript skillScript in this.playingScripts.Values)
			{
				if (skillScript.UseOnTargetOn())
				{
					skillScript.OnTargetOn(target);
					return true;
				}
			}
			foreach (Dictionary<int, SkillScript> dictionary in this.playingStateScripts.Values)
			{
				foreach (SkillScript skillScript2 in dictionary.Values)
				{
					if (skillScript2.UseOnTargetOn())
					{
						skillScript2.OnTargetOn(target);
						return true;
					}
				}
			}
			return false;
		}

		
		private bool Internal_AnyPlayingSkill()
		{
			foreach (KeyValuePair<SkillId, SkillScript> keyValuePair in this.playingScripts)
			{
				if (keyValuePair.Value.Info.skillData.PlayType == SkillPlayType.Alone)
				{
					return true;
				}
				if (keyValuePair.Value.Info.skillData.CooldownType != SkillCooldownType.SkillStart)
				{
					return true;
				}
			}
			return false;
		}

		
		public bool CanMoveDuringSkillPlaying()
		{
			foreach (KeyValuePair<SkillId, SkillScript> keyValuePair in this.playingScripts)
			{
				if (!keyValuePair.Value.CanMoveDuringSkillPlaying)
				{
					return false;
				}
			}
			return true;
		}

		
		public bool CanStopDuringSkillPlaying()
		{
			foreach (KeyValuePair<SkillId, SkillScript> keyValuePair in this.playingScripts)
			{
				if (!keyValuePair.Value.CanStopDuringSkillPlaying)
				{
					return false;
				}
			}
			return true;
		}

		
		public bool CanCastDuringSkillPlaying(int casterId, SkillSlotSet skillSlotSet, SkillData skillData)
		{
			if (this.Internal_AnyPlayingSkill())
			{
				if (skillSlotSet.IsNormalAttack() && this.IsPlayingMySkill(casterId, skillSlotSet))
				{
					return false;
				}
				if (skillData.PlayType == SkillPlayType.Overlap)
				{
					return true;
				}
				if (skillData.CastType == SkillCastType.Concentration && this.IsPlayingMySkill(casterId, skillSlotSet))
				{
					return true;
				}
				SkillInterruptType interruptType = skillData.InterruptType;
				if (interruptType == SkillInterruptType.None)
				{
					return false;
				}
				if (interruptType == SkillInterruptType.Cancel)
				{
					return this.CanActionDuringSkillPlaying();
				}
			}
			return true;
		}

		
		public bool CanActionDuringSkillPlaying()
		{
			foreach (SkillScript skillScript in this.playingScripts.Values)
			{
				if (skillScript.StateCode == 0 && skillScript.Info.skillData.PlayType == SkillPlayType.Alone && skillScript.Info.skillData.InterruptHandlingType == SkillInterruptHandlingType.Ignore)
				{
					return false;
				}
			}
			return true;
		}

		
		public bool IsReadyNormalAttack(float attackDelay)
		{
			return MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - this.lastNormalAttackTime > attackDelay;
		}

		
		public void ReadyNormalAttack()
		{
			this.lastNormalAttackTime = 0f;
		}

		
		private void StartNormalAttack()
		{
			this.lastNormalAttackTime = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
		}

		
		private void Cancel()
		{
			this.cancelTempScripts.Clear();
			foreach (KeyValuePair<SkillId, SkillScript> keyValuePair in this.playingScripts)
			{
				if (keyValuePair.Value.Info.skillData.InterruptHandlingType == SkillInterruptHandlingType.Accept)
				{
					this.cancelTempScripts.Add(keyValuePair.Value);
				}
			}
			this.StopTargetSkillScripts(this.cancelTempScripts);
		}

		
		private void StopScript(SkillScript skillScript, bool cancel)
		{
			if (cancel && skillScript.Info.skillSlotSet.IsNormalAttack())
			{
				NormalAttackScript normalAttackScript = skillScript as NormalAttackScript;
				if (normalAttackScript != null && normalAttackScript.IsFinishNormalAttack())
				{
					cancel = false;
				}
				else
				{
					this.ReadyNormalAttack();
				}
			}
			skillScript.Stop(cancel);
		}

		
		public void CancelAll()
		{
			this.CancelAll(this.playingScripts);
			this.CancelAll(this.passiveScripts);
		}

		
		private void CancelAll(Dictionary<SkillId, SkillScript> scripts)
		{
			this.cancelAllTempScripts_1.Clear();
			foreach (SkillScript item in scripts.Values)
			{
				this.cancelAllTempScripts_1.Add(item);
			}
			this.StopTargetSkillScripts(this.cancelAllTempScripts_1);
		}

		
		private void CancelAll(Dictionary<SkillId, Dictionary<int, SkillScript>> scripts)
		{
			this.cancelAllTempScripts_2.Clear();
			foreach (Dictionary<int, SkillScript> dictionary in scripts.Values)
			{
				foreach (SkillScript item in dictionary.Values)
				{
					this.cancelAllTempScripts_2.Add(item);
				}
			}
			this.StopTargetSkillScripts(this.cancelAllTempScripts_2);
		}

		
		public void CancelPassiveSkill(SkillId skillId)
		{
			SkillScript skillScript = null;
			if (this.passiveScripts.TryGetValue(skillId, out skillScript))
			{
				this.StopScript(skillScript, true);
			}
		}

		
		public void CancelStateSkill(SkillId skillId, int casterId, bool cancel)
		{
			SkillScript skillScript = null;
			foreach (Dictionary<int, SkillScript> dictionary in this.playingStateScripts.Values)
			{
				foreach (SkillScript skillScript2 in dictionary.Values)
				{
					if (skillScript2.Info.stateData.GroupData.skillId == skillId && skillScript2.Caster.ObjectId == casterId)
					{
						skillScript = skillScript2;
						break;
					}
				}
				if (skillScript != null)
				{
					break;
				}
			}
			if (skillScript != null)
			{
				this.StopScript(skillScript, cancel);
			}
		}

		
		public void Cancel(SkillId skillId)
		{
			SkillScript skillScript = null;
			if (this.playingScripts.TryGetValue(skillId, out skillScript))
			{
				this.StopScript(skillScript, true);
			}
		}

		
		public bool CancelNormalAttack()
		{
			this.cancelNormalAttackTempScripts.Clear();
			foreach (SkillScript skillScript in this.playingScripts.Values)
			{
				if (skillScript.Info.skillSlotSet.IsNormalAttack())
				{
					this.cancelNormalAttackTempScripts.Add(skillScript);
				}
			}
			if (this.cancelNormalAttackTempScripts.Count > 0)
			{
				this.StopTargetSkillScripts(this.cancelNormalAttackTempScripts);
				return true;
			}
			return false;
		}

		
		public bool CancelNormalAttack(int exceptTargetId)
		{
			foreach (SkillScript skillScript in this.playingScripts.Values)
			{
				if (skillScript.Info.skillSlotSet.IsNormalAttack() && skillScript.Info.target.ObjectId != exceptTargetId)
				{
					this.StopScript(skillScript, true);
					return true;
				}
			}
			return false;
		}

		
		public void CancelAggressiveSkill()
		{
			this.cancelAggressiveSkillTempScripts.Clear();
			foreach (SkillScript skillScript in this.playingScripts.Values)
			{
				if (skillScript.Info.skillData.AggressiveSkill)
				{
					switch (skillScript.Info.skillData.CastType)
					{
					case SkillCastType.Concentration:
						if (skillScript.IsConcentrating)
						{
							this.cancelAggressiveSkillTempScripts.Add(skillScript);
						}
						break;
					case SkillCastType.Channeling:
						this.cancelAggressiveSkillTempScripts.Add(skillScript);
						break;
					}
				}
			}
			this.StopTargetSkillScripts(this.cancelAggressiveSkillTempScripts);
		}

		
		public void CancelMovementSkill()
		{
			this.cancelMovementSkillTempScripts.Clear();
			foreach (SkillScript skillScript in this.playingScripts.Values)
			{
				if (skillScript.Info.skillData.MovementSkill)
				{
					switch (skillScript.Info.skillData.CastType)
					{
					case SkillCastType.Concentration:
						if (skillScript.IsConcentrating)
						{
							this.cancelMovementSkillTempScripts.Add(skillScript);
						}
						break;
					case SkillCastType.Channeling:
						this.cancelMovementSkillTempScripts.Add(skillScript);
						break;
					}
				}
			}
			this.StopTargetSkillScripts(this.cancelMovementSkillTempScripts);
		}

		
		public void CancelConcentrationSkill(Func<SkillScript, bool> condition = null)
		{
			this.cancelConcentrationSkillTempScripts.Clear();
			foreach (SkillScript skillScript in this.playingScripts.Values)
			{
				if (skillScript.IsConcentrating && (condition == null || condition(skillScript)))
				{
					this.cancelConcentrationSkillTempScripts.Add(skillScript);
				}
			}
			this.StopTargetSkillScripts(this.cancelConcentrationSkillTempScripts);
		}

		
		private void StopTargetSkillScripts(List<SkillScript> targetSkillScripts)
		{
			int num = targetSkillScripts.Count - 1;
			while (num >= 0 && targetSkillScripts.Count > num)
			{
				this.StopScript(targetSkillScripts[num], true);
				num++;
			}
		}

		
		public bool IsLockSlot(SkillSlotSet skillSlotSet)
		{
			if (this.lockedSkillSet.Contains(skillSlotSet))
			{
				return true;
			}
			if (skillSlotSet == SkillSlotSet.SpecialSkill)
			{
				SpecialSkillId specialSkillId = this.worldMovableCharacter.CharacterSkill.SpecialSkillId;
				return this.lockedSpecialSkill.Contains(specialSkillId);
			}
			return false;
		}

		
		public void LockSkillSlot(SpecialSkillId specialSkillId, bool isLock)
		{
			if (isLock)
			{
				this.lockedSpecialSkill.Add(specialSkillId);
				return;
			}
			this.lockedSpecialSkill.Remove(specialSkillId);
		}

		
		public void LockSkillSlot(SkillSlotSet skillSlotSet, bool isLock)
		{
			if (isLock)
			{
				this.lockedSkillSet.Add(skillSlotSet);
				return;
			}
			this.lockedSkillSet.Remove(skillSlotSet);
		}

		
		public SkillScriptParameterCollection GetPlayingScriptParameterCollection(WorldCharacter target, DamageType type, DamageSubType subType, int damageId)
		{
			SkillScriptParameterCollection skillScriptParameterCollection = SkillScriptParameterCollection.Create();
			foreach (Dictionary<int, SkillScript> dictionary in this.playingStateScripts.Values)
			{
				foreach (KeyValuePair<int, SkillScript> keyValuePair in dictionary)
				{
					skillScriptParameterCollection.Merge(keyValuePair.Value.GetParameters(target.SkillAgent, type, subType, damageId));
				}
			}
			foreach (SkillScript skillScript in this.passiveScripts.Values)
			{
				skillScriptParameterCollection.Merge(skillScript.GetParameters(target.SkillAgent, type, subType, damageId));
			}
			return skillScriptParameterCollection;
		}

		
		public void MountNormalAttack(int skillCode)
		{
			this.mountedNormalAttackSkillCode = skillCode;
		}

		
		public void UnmountNormalAttack()
		{
			this.mountedNormalAttackSkillCode = 0;
		}

		
		public UseSkillErrorCode IsCanUseSkill(SkillId skillId, int sequence, WorldCharacter hitTarget, Vector3? cursorPosition)
		{
			return this.GetSkillScript(skillId, sequence).IsCanUseSkill(hitTarget, cursorPosition, this.worldMovableCharacter);
		}

		
		public void UpgradeSkill(SkillSlotSet? skillSlotSet, int characterCode, ObjectType objectType)
		{
			if (skillSlotSet == null)
			{
				return;
			}
			int num = 1;
			SkillSlotSet value = skillSlotSet.Value;
			SkillSet skillSetData = GameDB.skill.GetSkillSetData(characterCode, objectType, value);
			num = ((skillSetData != null) ? skillSetData.GetActiveMaxSequence() : num);
			for (int i = 0; i < num; i++)
			{
				SkillData skillData = GameDB.skill.GetSkillData(characterCode, objectType, value, 1, i);
				SkillId skillId = skillData.SkillId.Equals(SkillId.None) ? skillData.PassiveSkillId : skillData.SkillId;
				SkillScript skillScript = this.GetSkillScript(skillId, i);
				if (skillScript != null)
				{
					skillScript.UpgradeSkill();
				}
			}
		}

		
		public void UpgradeWeaponSkill(MasteryType masteryType)
		{
			int num = 1;
			SkillSet skillSetData = GameDB.skill.GetSkillSetData(masteryType);
			num = ((skillSetData != null) ? skillSetData.GetActiveMaxSequence() : num);
			for (int i = 0; i < num; i++)
			{
				SkillData skillData = GameDB.skill.GetSkillData(masteryType, 1, i);
				SkillScript skillScript = this.GetSkillScript(skillData.SkillId, i);
				if (skillScript != null)
				{
					skillScript.UpgradeSkill();
				}
			}
		}

		
		public bool OnlyMoveInputWhileSkillPlaying()
		{
			foreach (KeyValuePair<SkillId, SkillScript> keyValuePair in this.playingScripts)
			{
				if (keyValuePair.Value.Info.skillData.OnlyMoveInputWhileSkillPlaying)
				{
					return true;
				}
			}
			return false;
		}

		
		private readonly Dictionary<SkillId, Dictionary<int, SkillScript>> skillScripts = new Dictionary<SkillId, Dictionary<int, SkillScript>>(SingletonComparerEnum<SkillIdComparer, SkillId>.Instance);

		
		private readonly SkillScriptManager skillScriptManager = SkillScriptManager.inst;

		
		private readonly List<SkillController.ReserveSkillInfo> reserveSkillInfos = new List<SkillController.ReserveSkillInfo>();

		
		public readonly Dictionary<SkillId, SkillScript> playingScripts = new Dictionary<SkillId, SkillScript>(SingletonComparerEnum<SkillIdComparer, SkillId>.Instance);

		
		public readonly Dictionary<SkillId, Coroutine> playingSkills = new Dictionary<SkillId, Coroutine>(SingletonComparerEnum<SkillIdComparer, SkillId>.Instance);

		
		private WorldMovableCharacter worldMovableCharacter;

		
		public readonly Dictionary<SkillId, SkillScript> passiveScripts = new Dictionary<SkillId, SkillScript>(SingletonComparerEnum<SkillIdComparer, SkillId>.Instance);

		
		private Dictionary<SkillId, Coroutine> passiveSkills = new Dictionary<SkillId, Coroutine>(SingletonComparerEnum<SkillIdComparer, SkillId>.Instance);

		
		public readonly Dictionary<SkillId, Dictionary<int, SkillScript>> playingStateScripts = new Dictionary<SkillId, Dictionary<int, SkillScript>>(SingletonComparerEnum<SkillIdComparer, SkillId>.Instance);

		
		public readonly Dictionary<SkillId, Dictionary<int, Coroutine>> playingStateSkills = new Dictionary<SkillId, Dictionary<int, Coroutine>>(SingletonComparerEnum<SkillIdComparer, SkillId>.Instance);

		
		private readonly HashSet<SkillSlotSet> lockedSkillSet = new HashSet<SkillSlotSet>(SingletonComparerEnum<SkillSlotSetComparer, SkillSlotSet>.Instance);

		
		private readonly HashSet<SpecialSkillId> lockedSpecialSkill = new HashSet<SpecialSkillId>(SingletonComparerEnum<SpecialSkillIdComparer, SpecialSkillId>.Instance);

		
		private float lastNormalAttackTime;

		
		private int mountedNormalAttackSkillCode;

		
		private Action<SkillUseInfo> OnPlay;

		
		private Action<SkillUseInfo> OnPlayPassive;

		
		private Action<SkillUseInfo> OnStart;

		
		private Action<SkillUseInfo> OnStartPassive;

		
		private Action<SkillUseInfo, int> OnStartState;

		
		private Action<SkillData> OnConsumeSkillResources;

		
		private Action<SkillSlotSet, MasteryType, SkillId, bool, bool, bool> OnFinish;

		
		private Action<SkillSlotSet, MasteryType, SkillId, bool, bool> OnFinishState;

		
		private Action<SkillSlotSet, SkillId, bool, bool> OnFinishPassive;

		
		private readonly List<SkillScript> cancelTempScripts = new List<SkillScript>();

		
		private readonly List<SkillScript> cancelAllTempScripts_1 = new List<SkillScript>();

		
		private readonly List<SkillScript> cancelAllTempScripts_2 = new List<SkillScript>();

		
		private readonly List<SkillScript> cancelNormalAttackTempScripts = new List<SkillScript>();

		
		private readonly List<SkillScript> cancelAggressiveSkillTempScripts = new List<SkillScript>();

		
		private readonly List<SkillScript> cancelMovementSkillTempScripts = new List<SkillScript>();

		
		private readonly List<SkillScript> cancelConcentrationSkillTempScripts = new List<SkillScript>();

		
		public class ReserveSkillInfo
		{
			
			public SkillUseInfo skillUseInfo;

			
			public int sequence;
		}
	}
}
