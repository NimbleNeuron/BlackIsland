using System;
using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class PlayerSkill
	{
		public delegate void ChangeCursorStatus(CursorStatus cursorStatus);


		private static readonly Dictionary<int, string> useSkillErrorMessage = new Dictionary<int, string>
		{
			{
				0,
				"지금은 사용할 수 없습니다!"
			},
			{
				1,
				"아직 배우지 않은 스킬입니다!"
			},
			{
				2,
				"스킬이 아직 준비되지 않았습니다!"
			},
			{
				3,
				"스태미너가 부족합니다!"
			},
			{
				4,
				"자원이 부족합니다!"
			},
			{
				5,
				"휴식 중에는 스킬을 사용할 수 없습니다."
			},
			{
				6,
				"장착된 무기가 없어 공격할 수 없습니다."
			},
			{
				7,
				"InvalidAction"
			},
			{
				8,
				"NotInvalidTarget"
			},
			{
				9999,
				null
			}
		};


		private readonly MyPlayerContext myPlayer;
		private readonly Dictionary<int, int> skillEvolutionOrderMap = new Dictionary<int, int>();
		private readonly Dictionary<GameInputEvent, SkillSlotIndex> skillInputEventMap =
			new Dictionary<GameInputEvent, SkillSlotIndex>
			{
				{
					GameInputEvent.Attack,
					SkillSlotIndex.Attack
				},
				{
					GameInputEvent.Active1,
					SkillSlotIndex.Active1
				},
				{
					GameInputEvent.Active2,
					SkillSlotIndex.Active2
				},
				{
					GameInputEvent.Active3,
					SkillSlotIndex.Active3
				},
				{
					GameInputEvent.Active4,
					SkillSlotIndex.Active4
				},
				{
					GameInputEvent.WeaponSkill,
					SkillSlotIndex.WeaponSkill
				},
				{
					GameInputEvent.LearnPassive,
					SkillSlotIndex.Passive
				},
				{
					GameInputEvent.LearnActive1,
					SkillSlotIndex.Active1
				},
				{
					GameInputEvent.LearnActive2,
					SkillSlotIndex.Active2
				},
				{
					GameInputEvent.LearnActive3,
					SkillSlotIndex.Active3
				},
				{
					GameInputEvent.LearnActive4,
					SkillSlotIndex.Active4
				},
				{
					GameInputEvent.LearnWeaponSkill,
					SkillSlotIndex.WeaponSkill
				}
			};


		private readonly List<SkillInput> skillInputs = new List<SkillInput>();
		private readonly PlayerSkillLock skillLock;
		private readonly Dictionary<int, int> skillUpOrderMap = new Dictionary<int, int>();
		private readonly SplatManager splatManager;

		private readonly List<UnprocessedAdditionalInput> unprocessedAdditionalInputs =
			new List<UnprocessedAdditionalInput>();

		private SkillData currentIndicatorSkillData;
		public SkillSlotSet? currentSkillSlotSet;
		private bool isFixedIndicator;
		private float lastCheckValidateTime;
		public ChangeCursorStatus OnChangeCursorStatus = delegate { };
		private Vector3? pickPointWhenUseSkillPickPointThenDirection;
		private Plane plane = new Plane(Vector3.up, Vector3.zero);
		private int skillEvolutionOrder;
		private int skillUpOrder;

		public PlayerSkill(MyPlayerContext myPlayer, SplatManager splatManager)
		{
			this.splatManager = splatManager;
			this.myPlayer = myPlayer;
			skillLock = new PlayerSkillLock(myPlayer.Character);
		}

		private LocalPlayerCharacter myCharacter => myPlayer.Character;


		private SkillSlotIndex GetSkillSlotIndex(GameInputEvent gameInputEvent)
		{
		#if UNITY_EDITOR
			if (!skillInputEventMap.ContainsKey(gameInputEvent))
			{
				Debug.LogError($"GetSkillSlotIndex::KeyNotFound : {gameInputEvent}");
				return (SkillSlotIndex) 0;
			}
		#endif
			return skillInputEventMap[gameInputEvent];
		}


		private GameInputEvent GetGameInputEvent(SkillSlotIndex skillSlotIndex)
		{
			foreach (KeyValuePair<GameInputEvent, SkillSlotIndex> keyValuePair in skillInputEventMap)
			{
				if (keyValuePair.Value == skillSlotIndex)
				{
					return keyValuePair.Key;
				}
			}

			return GameInputEvent.None;
		}


		private string GetSkillKeyCode(SkillSlotIndex skillSlotIndex)
		{
			string result = "";
			foreach (KeyValuePair<GameInputEvent, SkillSlotIndex> keyValuePair in skillInputEventMap)
			{
				if (keyValuePair.Value == skillSlotIndex)
				{
					result = MonoBehaviourInstance<GameInput>.inst.GetKeyCode(keyValuePair.Key).ToString();
					break;
				}
			}

			return result;
		}


		private bool PossibleUseActiveSkill(SkillSlotSet skillSlotSet, LocalObject hitTarget, Vector3 hitPoint)
		{
			SkillData skillData = GetSkillData(skillSlotSet);
			if (skillData == null)
			{
				return false;
			}

			if (skillData.SkillId == SkillId.None)
			{
				return false;
			}

			int localSkillSequence = myPlayer.GetLocalSkillSequence(skillSlotSet);
			if (!CanReqSkillInput(skillSlotSet, localSkillSequence, skillData))
			{
				return false;
			}

			bool flag = IsAdditionalInput(skillSlotSet, localSkillSequence, skillData);
			UsableSkillCheckList usableSkillCheckList = ~UsableSkillCheckList.InScriptCheck;
			if (flag)
			{
				usableSkillCheckList &= ~UsableSkillCheckList.Resource;
			}

			UseSkillErrorCode useSkillErrorCode =
				CheckUsableSkill(skillSlotSet, usableSkillCheckList, hitTarget, hitPoint, skillData);
			if (useSkillErrorCode != UseSkillErrorCode.None)
			{
				MonoBehaviourInstance<GameUI>.inst.ToastMessage.ShowMessage(
					Ln.Get(useSkillErrorMessage[(int) useSkillErrorCode]));
				return false;
			}

			return true;
		}


		private UseSkillErrorCode CheckUsableSkill(SkillSlotSet skillSlotSet, UsableSkillCheckList usableSkillCheckList,
			LocalObject hitTarget, Vector3? cursorPosition, SkillData skillData)
		{
			if (usableSkillCheckList.HasFlag(UsableSkillCheckList.EquipWeapon) && !myCharacter.IsEquippedWeapon())
			{
				return UseSkillErrorCode.Disarmed;
			}

			SkillSlotIndex? skillSlotIndex = myPlayer.CharacterSkill.GetSkillSlotIndex(skillSlotSet);
			if (skillSlotIndex == null)
			{
				return UseSkillErrorCode.NotReady;
			}

			if (usableSkillCheckList.HasFlag(UsableSkillCheckList.SkillLevel) &&
			    myCharacter.GetSkillLevel(skillSlotIndex.Value) < 1)
			{
				return UseSkillErrorCode.NotLearn;
			}

			if (usableSkillCheckList.HasFlag(UsableSkillCheckList.Cooldown))
			{
				if (!myCharacter.CheckCooldown(skillSlotSet))
				{
					return UseSkillErrorCode.NotReady;
				}

				bool flag = myCharacter.IsLastSequence(skillSlotSet);
				bool flag2 = myCharacter.IsHoldCooldown(skillSlotSet);
				if (flag && flag2)
				{
					return UseSkillErrorCode.NotReady;
				}
			}

			if (usableSkillCheckList.HasFlag(UsableSkillCheckList.Resource))
			{
				UseSkillErrorCode useSkillErrorCode = myCharacter.EnoughMainCost(skillData);
				if (useSkillErrorCode != UseSkillErrorCode.None)
				{
					return useSkillErrorCode;
				}

				useSkillErrorCode = myCharacter.EnoughExCost(skillData);
				if (useSkillErrorCode != UseSkillErrorCode.None)
				{
					return useSkillErrorCode;
				}
			}

			if (usableSkillCheckList.HasFlag(UsableSkillCheckList.Cooldown) && myCharacter.IsRest)
			{
				return UseSkillErrorCode.Resting;
			}

			if (usableSkillCheckList.HasFlag(UsableSkillCheckList.InScriptCheck))
			{
				UseSkillErrorCode useSkillErrorCode2 =
					myCharacter.IsCanUseSkill(skillSlotSet, hitTarget, cursorPosition);
				if (useSkillErrorCode2 != UseSkillErrorCode.None)
				{
					return useSkillErrorCode2;
				}
			}

			if (usableSkillCheckList.HasFlag(UsableSkillCheckList.InScriptSlotEnableCheck))
			{
				UseSkillErrorCode useSkillErrorCode3 = myCharacter.IsSkillSlotCanUseInSkillScript(skillSlotSet);
				if (useSkillErrorCode3 != UseSkillErrorCode.None)
				{
					return useSkillErrorCode3;
				}
			}

			if (myCharacter.ConcentrateSkillSet != SkillSlotSet.None &&
			    myCharacter.ConcentrateSkillSet != skillSlotSet && skillData.PlayType != SkillPlayType.Overlap)
			{
				return UseSkillErrorCode.NotAvailableNow;
			}

			return UseSkillErrorCode.None;
		}


		public void UseActiveSkillExternal(SkillSlotIndex skillSlotIndex)
		{
			SkillData skillData = GetSkillData(skillSlotIndex);
			if (skillData.SkillId == SkillId.None)
			{
				return;
			}

			if (IsLockSkill(skillSlotIndex))
			{
				MonoBehaviourInstance<GameUI>.inst.ToastMessage.ShowMessage(Ln.Get(useSkillErrorMessage[0]));
				return;
			}

			if (myCharacter == null)
			{
				return;
			}

			if (myCharacter.CheckDyingCondition())
			{
				return;
			}

			SkillSlotSet? skillSlotSet = myPlayer.CharacterSkill.GetSkillSlotSet(skillSlotIndex);
			if (skillSlotSet == null)
			{
				return;
			}

			int localSkillSequence = myPlayer.GetLocalSkillSequence(skillSlotSet.Value);
			if (!CanReqSkillInput(skillSlotSet.Value, localSkillSequence, skillData))
			{
				return;
			}

			bool flag = IsAdditionalInput(skillSlotSet.Value, localSkillSequence, skillData);
			UsableSkillCheckList usableSkillCheckList = ~UsableSkillCheckList.InScriptCheck;
			if (flag)
			{
				usableSkillCheckList &= ~UsableSkillCheckList.Resource;
			}

			UseSkillErrorCode useSkillErrorCode =
				CheckUsableSkill(skillSlotSet.Value, usableSkillCheckList, null, null, skillData);
			if (useSkillErrorCode != UseSkillErrorCode.None)
			{
				MonoBehaviourInstance<GameUI>.inst.ToastMessage.ShowMessage(
					Ln.Get(useSkillErrorMessage[(int) useSkillErrorCode]));
				return;
			}

			GameInputEvent gameInputEvent = GetGameInputEvent(skillSlotIndex);
			if (!Singleton<LocalSetting>.inst.setting.quickCastKeys.ContainsKey(gameInputEvent))
			{
				return;
			}

			bool flag2 = Singleton<LocalSetting>.inst.setting.quickCastKeys[gameInputEvent];
			if (!IsPlaying(skillSlotSet.Value) && !flag2 && currentSkillSlotSet != null)
			{
				SkillSlotSet? skillSlotSet2 = skillSlotSet;
				SkillSlotSet value = currentSkillSlotSet.Value;
				if ((skillSlotSet2.GetValueOrDefault() == value) & (skillSlotSet2 != null))
				{
					return;
				}
			}

			if (!PossibleUseActiveSkill(skillSlotSet.Value, myCharacter, Vector3.zero))
			{
				return;
			}

			if (skillData.InstantCast())
			{
				if (!ShouldChangeCursorStatusToSkill(skillData, flag, flag2))
				{
					ChangeCursorStatus onChangeCursorStatus = OnChangeCursorStatus;
					if (onChangeCursorStatus != null)
					{
						onChangeCursorStatus(CursorStatus.Normal);
					}

					UseSkill(skillSlotSet.Value, skillData, localSkillSequence, myCharacter, Vector3.zero, Vector3.zero,
						flag);
					return;
				}

				if (!PossibleShowSkillIndicator(skillSlotSet.Value))
				{
					return;
				}

				ShowSkillIndicator(skillSlotSet.Value);
				ChangeCursorStatus onChangeCursorStatus2 = OnChangeCursorStatus;
				if (onChangeCursorStatus2 == null)
				{
					return;
				}

				onChangeCursorStatus2(CursorStatus.Skill);
			}
			else if (!myCharacter.IsConcentrating && !flag && PossibleShowSkillIndicator(skillSlotSet.Value))
			{
				ShowSkillIndicator(skillSlotSet.Value);
				ChangeCursorStatus onChangeCursorStatus3 = OnChangeCursorStatus;
				if (onChangeCursorStatus3 == null)
				{
					return;
				}

				onChangeCursorStatus3(CursorStatus.Skill);
			}
			else
			{
				if (!ShouldChangeCursorStatusToSkill(skillData, flag, flag2))
				{
					ChangeCursorStatus onChangeCursorStatus4 = OnChangeCursorStatus;
					if (onChangeCursorStatus4 != null)
					{
						onChangeCursorStatus4(CursorStatus.Normal);
					}

					UseSkill(skillSlotSet.Value, skillData, localSkillSequence, myCharacter, Vector3.zero, Vector3.zero,
						flag);
					return;
				}

				if (!PossibleShowSkillIndicator(skillSlotSet.Value))
				{
					return;
				}

				ShowSkillIndicator(skillSlotSet.Value);
				ChangeCursorStatus onChangeCursorStatus5 = OnChangeCursorStatus;
				if (onChangeCursorStatus5 == null)
				{
					return;
				}

				onChangeCursorStatus5(CursorStatus.Skill);
			}
		}


		private bool ShouldChangeCursorStatusToSkill(SkillData skillData, bool isAdditionalInput,
			bool isQuickCast = false)
		{
			if (currentSkillSlotSet == null && !skillData.InstantCast() && !isAdditionalInput)
			{
				if (!isQuickCast)
				{
					if (!skillData.CanAdditionalAction)
					{
						return true;
					}

					if (skillData.NeedInputForCast)
					{
						return true;
					}
				}
				else if (skillData.CastWaysType == SkillCastWaysType.PickPointThenDirection)
				{
					return true;
				}
			}

			return false;
		}


		private bool UseSkill(SkillSlotSet skillSlotSet, SkillData skillData, int skillSeq, LocalObject hitTarget,
			Vector3 hitPoint, Vector3 releasePoint, bool isAdditionalInput)
		{
			UseSkillErrorCode useSkillErrorCode = CheckUsableSkill(skillSlotSet, UsableSkillCheckList.InScriptCheck,
				hitTarget, hitPoint, skillData);
			if (useSkillErrorCode != UseSkillErrorCode.None)
			{
				MonoBehaviourInstance<GameUI>.inst.ToastMessage.ShowMessage(
					Ln.Get(useSkillErrorMessage[(int) useSkillErrorCode]));
				return false;
			}

			if (currentSkillSlotSet.HasValue)
			{
				if (skillSlotSet == myCharacter.ConcentrateSkillSet)
				{
					ReleaseFixedIndicator(true);
				}
				else
				{
					HideIndicator();
				}
			}
			else if (isAdditionalInput && skillData.CastType == SkillCastType.Concentration)
			{
				unprocessedAdditionalInputs.Add(new UnprocessedAdditionalInput(skillSlotSet));
			}

			if (skillData.CastWaysType == SkillCastWaysType.PickTargetEdge ||
			    skillData.CastWaysType == SkillCastWaysType.PickTargetCenter)
			{
				if (hitTarget == null)
				{
					return false;
				}

				LocalCharacter target = hitTarget as LocalCharacter;
				if (target == null || !target.IsAlive)
				{
					return false;
				}

				LocalPlayerCharacter localPlayerCharacter = hitTarget as LocalPlayerCharacter;
				if (localPlayerCharacter != null && localPlayerCharacter.IsDyingCondition &&
				    GameDB.skill.GetSkillGroupData(skillData.group).impossibleDyingConditionTarget)
				{
					return false;
				}

				if (!IsValidTarget(skillData, target))
				{
					MonoBehaviourInstance<GameUI>.inst.ToastMessage.ShowMessage(Ln.Get(useSkillErrorMessage[8]));
					return false;
				}

				if (hitTarget.IsTypeOf<LocalSummonBase>() &&
				    skillData.TargetType != SkillTargetType.NotSpecifiedAndSummonObject)
				{
					MonoBehaviourInstance<GameUI>.inst.ToastMessage.ShowMessage(Ln.Get(useSkillErrorMessage[8]));
					return false;
				}

				GameClient inst = MonoBehaviourInstance<GameClient>.inst;
				ReqUseTargetSkill packet = new ReqUseTargetSkill();
				packet.skillSlotSet = skillSlotSet;
				packet.targetId = hitTarget.ObjectId;
				Action<ResUseSkill> callback = SkillUseServerResponse;
				inst.Request<ReqUseTargetSkill, ResUseSkill>(packet, callback);
			}
			else
			{
				Vector3 skillPoint = GetSkillPoint(hitPoint, skillData);
				GameClient inst = MonoBehaviourInstance<GameClient>.inst;
				ReqUsePointSkill packet = new ReqUsePointSkill();
				packet.skillSlotSet = skillSlotSet;
				packet.hitPosition = skillPoint;
				packet.releasePosition = releasePoint;
				Action<ResUseSkill> callback = SkillUseServerResponse;
				inst.Request<ReqUsePointSkill, ResUseSkill>(packet, callback);
			}

			skillInputs.Add(new SkillInput(skillSlotSet, skillSeq, isAdditionalInput));
			return true;

			void SkillUseServerResponse(ResUseSkill res)
			{
				if (0 > res.errorCode)
				{
					return;
				}

				if (useSkillErrorMessage.ContainsKey(res.errorCode) && useSkillErrorMessage[res.errorCode] != null)
				{
					MonoBehaviourInstance<GameUI>.inst.ToastMessage.ShowMessage(
						Ln.Get(useSkillErrorMessage[res.errorCode]));
				}

				SkillInputsCheckValidate();
				for (int index = skillInputs.Count - 1; index >= 0; --index)
				{
					if (skillInputs[index].SkillSlotSet == skillSlotSet && skillInputs[index].Seq == skillSeq &&
					    skillInputs[index].IsAddinalInput == isAdditionalInput)
					{
						skillInputs.RemoveAt(index);
						break;
					}
				}
			}

			// co: dotPeek
			// PlayerSkill.<>c__DisplayClass31_0 CS$<>8__locals1 = new PlayerSkill.<>c__DisplayClass31_0();
			// CS$<>8__locals1.<>4__this = this;
			// CS$<>8__locals1.skillSlotSet = skillSlotSet;
			// CS$<>8__locals1.skillSeq = skillSeq;
			// CS$<>8__locals1.isAdditionalInput = isAdditionalInput;
			// UseSkillErrorCode useSkillErrorCode = this.CheckUsableSkill(CS$<>8__locals1.skillSlotSet, PlayerSkill.UsableSkillCheckList.InScriptCheck, hitTarget, new Vector3?(hitPoint), skillData);
			// if (useSkillErrorCode != UseSkillErrorCode.None)
			// {
			// 	MonoBehaviourInstance<GameUI>.inst.ToastMessage.ShowMessage(Ln.Get(PlayerSkill.useSkillErrorMessage[(int)useSkillErrorCode]), 3f);
			// 	return false;
			// }
			// if (this.currentSkillSlotSet != null)
			// {
			// 	if (CS$<>8__locals1.skillSlotSet == this.myCharacter.ConcentrateSkillSet)
			// 	{
			// 		this.ReleaseFixedIndicator(true);
			// 	}
			// 	else
			// 	{
			// 		this.HideIndicator();
			// 	}
			// }
			// else if (CS$<>8__locals1.isAdditionalInput && skillData.CastType == SkillCastType.Concentration)
			// {
			// 	this.unprocessedAdditionalInputs.Add(new PlayerSkill.UnprocessedAdditionalInput(CS$<>8__locals1.skillSlotSet));
			// }
			// if (skillData.CastWaysType == SkillCastWaysType.PickTargetEdge || skillData.CastWaysType == SkillCastWaysType.PickTargetCenter)
			// {
			// 	if (hitTarget == null)
			// 	{
			// 		return false;
			// 	}
			// 	LocalCharacter localCharacter = hitTarget as LocalCharacter;
			// 	if (localCharacter == null)
			// 	{
			// 		return false;
			// 	}
			// 	if (!localCharacter.IsAlive)
			// 	{
			// 		return false;
			// 	}
			// 	LocalPlayerCharacter localPlayerCharacter = hitTarget as LocalPlayerCharacter;
			// 	if (localPlayerCharacter != null && localPlayerCharacter.IsDyingCondition && GameDB.skill.GetSkillGroupData(skillData.group).impossibleDyingConditionTarget)
			// 	{
			// 		return false;
			// 	}
			// 	if (!this.IsValidTarget(skillData, localCharacter))
			// 	{
			// 		MonoBehaviourInstance<GameUI>.inst.ToastMessage.ShowMessage(Ln.Get(PlayerSkill.useSkillErrorMessage[8]), 3f);
			// 		return false;
			// 	}
			// 	if (hitTarget.IsTypeOf<LocalSummonBase>() && skillData.TargetType != SkillTargetType.NotSpecifiedAndSummonObject)
			// 	{
			// 		MonoBehaviourInstance<GameUI>.inst.ToastMessage.ShowMessage(Ln.Get(PlayerSkill.useSkillErrorMessage[8]), 3f);
			// 		return false;
			// 	}
			// 	MonoBehaviourInstance<GameClient>.inst.Request<ReqUseTargetSkill, ResUseSkill>(new ReqUseTargetSkill
			// 	{
			// 		skillSlotSet = CS$<>8__locals1.skillSlotSet,
			// 		targetId = hitTarget.ObjectId
			// 	}, new Action<ResUseSkill>(CS$<>8__locals1.<UseSkill>g__SkillUseServerResponse|0));
			// }
			// else
			// {
			// 	Vector3 skillPoint = this.GetSkillPoint(hitPoint, skillData);
			// 	MonoBehaviourInstance<GameClient>.inst.Request<ReqUsePointSkill, ResUseSkill>(new ReqUsePointSkill
			// 	{
			// 		skillSlotSet = CS$<>8__locals1.skillSlotSet,
			// 		hitPosition = skillPoint,
			// 		releasePosition = releasePoint
			// 	}, new Action<ResUseSkill>(CS$<>8__locals1.<UseSkill>g__SkillUseServerResponse|0));
			// }
			// this.skillInputs.Add(new PlayerSkill.SkillInput(CS$<>8__locals1.skillSlotSet, CS$<>8__locals1.skillSeq, CS$<>8__locals1.isAdditionalInput));
			// return true;
		}


		private bool IsValidTarget(SkillData skillData, LocalCharacter target)
		{
			if (target == null || skillData == null)
			{
				return false;
			}

			switch (skillData.TargetType)
			{
				case SkillTargetType.Self:
					return myCharacter.ObjectId == target.ObjectId;
				case SkillTargetType.ExceptSelf:
					return myCharacter.ObjectId != target.ObjectId;
				case SkillTargetType.Ally:
					return myPlayer.GetHostileType(target) == HostileType.Ally;
				case SkillTargetType.Enemy:
					return myPlayer.GetHostileType(target) == HostileType.Enemy;
				case SkillTargetType.NotSpecified:
				case SkillTargetType.NotSpecifiedAndSummonObject:
					return true;
				default:
					return false;
			}
		}


		private bool IsSkillInputExist(SkillSlotSet skillSlotSet, int skillSeq)
		{
			SkillInputsCheckValidate();
			foreach (SkillInput skillInput in skillInputs)
			{
				if (skillInput.SkillSlotSet == skillSlotSet && skillInput.Seq == skillSeq)
				{
					return true;
				}
			}

			return false;
		}


		private bool CanReqSkillInput(SkillSlotSet skillSlotSet, int skillSeq, SkillData skillData)
		{
			if (IsSkillInputExist(skillSlotSet, skillSeq))
			{
				if (skillData.CanAdditionalAction)
				{
					using (List<SkillInput>.Enumerator enumerator = skillInputs.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							SkillInput skillInput = enumerator.Current;
							if (skillInput.SkillSlotSet == skillSlotSet && skillInput.Seq == skillSeq &&
							    skillInput.IsAddinalInput)
							{
								return false;
							}
						}

						return true;
					}
				}

				return false;
			}

			return true;
		}


		private bool IsAdditionalInput(SkillSlotSet skillSlotSet, int skillSeq, SkillData skillData)
		{
			if (skillData.AdditionalAction == AdditionalAction.None)
			{
				return false;
			}

			SkillInputsCheckValidate();
			return IsSkillInputExist(skillSlotSet, skillSeq) ||
			       myCharacter.LocalSkillPlayer.IsPlaying(skillData.SkillId);
		}


		private void SkillInputsCheckValidate()
		{
			if (lastCheckValidateTime == Time.time)
			{
				return;
			}

			lastCheckValidateTime = Time.time;
			for (int i = skillInputs.Count - 1; i >= 0; i--)
			{
				if (!skillInputs[i].CheckValidate())
				{
					skillInputs.RemoveAt(i);
				}
			}
		}


		public void OnSkillStart(SkillId skillId, SkillSlotSet skillSlotSet, SkillData skillData)
		{
			if (skillData.SkillId != skillId)
			{
				return;
			}

			int localSkillSequence = myPlayer.GetLocalSkillSequence(skillSlotSet);
			SkillInputsCheckValidate();
			for (int i = skillInputs.Count - 1; i >= 0; i--)
			{
				if (skillInputs[i].SkillSlotSet == skillSlotSet && skillInputs[i].Seq == localSkillSequence &&
				    !skillInputs[i].IsAddinalInput)
				{
					skillInputs.RemoveAt(i);
					break;
				}
			}

			myPlayer.CharacterSkill.LocalNextSequence(skillSlotSet);
		}


		public void OnSkillReserveCancel(SkillSlotSet skillSlotSet)
		{
			int num = -1;
			int num2 = -1;
			for (int i = skillInputs.Count - 1; i >= 0; i--)
			{
				if (skillInputs[i].SkillSlotSet == skillSlotSet)
				{
					if (skillInputs[i].IsAddinalInput)
					{
						skillInputs.RemoveAt(i);
						if (num != -1)
						{
							break;
						}
					}
					else if (num2 < skillInputs[i].Seq)
					{
						num = i;
						num2 = skillInputs[i].Seq;
					}
				}
			}

			if (num != -1)
			{
				skillInputs.RemoveAt(num);
			}
		}


		public void ShowNextLevelSkillTooltip(SkillSlotIndex skillSlotIndex)
		{
			int skillLevel = myCharacter.GetSkillLevel(skillSlotIndex);
			int num = 0 < skillLevel ? skillLevel : 1;
			SkillSlotSet? skillSlotSet = myPlayer.CharacterSkill.GetSkillSlotSet(skillSlotIndex);
			if (skillSlotSet == null)
			{
				return;
			}

			SkillData skillData;
			SkillData skillData2;
			if (skillSlotIndex == SkillSlotIndex.WeaponSkill)
			{
				skillData = GameDB.skill.GetSkillData(myCharacter.GetEquipWeaponMasteryType(), num,
					myPlayer.GetLocalSkillSequence(skillSlotSet.Value));
				skillData2 = GameDB.skill.GetSkillData(myCharacter.GetEquipWeaponMasteryType(), num + 1,
					myPlayer.GetLocalSkillSequence(skillSlotSet.Value));
			}
			else if (skillSlotIndex == SkillSlotIndex.SpecialSkill)
			{
				skillData = GameDB.skill.GetSkillData(myPlayer.CharacterSkill.SpecialSkillId, 0);
				skillData2 = null;
			}
			else
			{
				skillData = GameDB.skill.GetSkillData(myCharacter.CharacterCode, ObjectType.PlayerCharacter,
					skillSlotSet.Value, num, 0);
				skillData2 = GameDB.skill.GetSkillData(myCharacter.CharacterCode, ObjectType.PlayerCharacter,
					skillSlotSet.Value, num + 1, 0);
			}

			float cooldownReduction =
				skillSlotIndex >= SkillSlotIndex.WeaponSkill ? 0f : myCharacter.Stat.CooldownReduction;
			bool addSkillInfo = 0 < skillLevel && skillData2 != null;
			if (skillData != null)
			{
				MonoBehaviourInstance<Tooltip>.inst.SetSkill(myCharacter, skillData, skillData2,
					myPlayer.CharacterSkill.GetSkillEvolutionLevel(skillSlotIndex), GetSkillKeyCode(skillSlotIndex),
					addSkillInfo, cooldownReduction);
				MonoBehaviourInstance<Tooltip>.inst.ShowFixed(null, Tooltip.TooltipPosition.SkillInfoCenterPosition);
			}
		}


		public void ShowEvolutionSkillTooltip(SkillSlotIndex skillSlotIndex)
		{
			int skillEvolutionLevel = myCharacter.GetSkillEvolutionLevel(skillSlotIndex);
			SkillData skillData = myCharacter.GetSkillData(skillSlotIndex);
			if (!skillData.Evolutionable)
			{
				return;
			}

			SkillEvolutionData evolutionData = GameDB.skill.GetEvolutionData(skillData);
			if (evolutionData == null)
			{
				return;
			}

			float cooldownReduction =
				skillSlotIndex >= SkillSlotIndex.WeaponSkill ? 0f : myCharacter.Stat.CooldownReduction;
			if (skillEvolutionLevel + 1 <= evolutionData.maxEvolutionLevel)
			{
				MonoBehaviourInstance<Tooltip>.inst.SetEvolutionSkill(myCharacter, skillData, skillEvolutionLevel + 1,
					GetSkillKeyCode(skillSlotIndex), cooldownReduction);
				MonoBehaviourInstance<Tooltip>.inst.ShowFixed(null, Tooltip.TooltipPosition.SkillInfoCenterPosition);
			}
		}


		public void ShowSkillTooltip(SkillSlotIndex skillSlotIndex)
		{
			SkillData skillData = GetSkillData(skillSlotIndex);
			if (skillData == null)
			{
				return;
			}

			int skillEvolutionLevel = myPlayer.CharacterSkill.GetSkillEvolutionLevel(skillSlotIndex);
			float cooldownReduction =
				skillSlotIndex >= SkillSlotIndex.WeaponSkill ? 0f : myCharacter.Stat.CooldownReduction;
			MonoBehaviourInstance<Tooltip>.inst.SetSkill(myCharacter, skillData, null, skillEvolutionLevel,
				GetSkillKeyCode(skillSlotIndex), false, cooldownReduction);
			MonoBehaviourInstance<Tooltip>.inst.ShowFixed(null, Tooltip.TooltipPosition.SkillInfoCenterPosition);
		}


		public void UpgradeSkill(SkillSlotIndex skillSlotIndex)
		{
			if (myPlayer.CanSkillUpgrade(skillSlotIndex))
			{
				Singleton<SoundControl>.inst.PlayUISound("SkillLvUpClick",
					ClientService.GamePlayMode.Play | ClientService.GamePlayMode.ObserveTeam);
				MonoBehaviourInstance<GameClient>.inst.Request(new ReqUpgradeSkill
				{
					skillSlotIndex = skillSlotIndex
				});
			}
		}


		public void EvolutionSkill(SkillSlotIndex skillSlotIndex)
		{
			if (myPlayer.CanSkillEvolution(skillSlotIndex))
			{
				Singleton<SoundControl>.inst.PlayUISound("SkillEvoUpClick",
					ClientService.GamePlayMode.Play | ClientService.GamePlayMode.ObserveTeam);
				MonoBehaviourInstance<GameClient>.inst.Request(new ReqEvolutionSkill
				{
					skillSlotIndex = skillSlotIndex
				});
			}
		}


		public bool ShowSeparateSkillIndicator(SkillSlotIndex skillSlotIndex)
		{
			SkillData skillData = GetSkillData(skillSlotIndex);
			if (skillData == null)
			{
				return false;
			}

			if (string.IsNullOrEmpty(skillData.Guideline))
			{
				return false;
			}

			if (myCharacter.IsConcentrating)
			{
				return false;
			}

			if (myPlayer.CharacterSkill.GetSkillSlotSet(skillSlotIndex) == null)
			{
				return false;
			}

			float range;
			Splat spellIndicator = UpdateIndicator(skillData, out range);
			SetSkillIndicator(spellIndicator, range, skillData);
			return true;
		}


		private bool PossibleShowSkillIndicator(SkillSlotSet skillSlotSet)
		{
			if (skillSlotSet.SlotSet2Index() == SkillSlotIndex.Attack)
			{
				return false;
			}

			SkillData skillData = GetSkillData(skillSlotSet);
			return skillData != null && !string.IsNullOrEmpty(skillData.Guideline) &&
			       skillData.CastWaysType != SkillCastWaysType.Instant &&
			       (currentSkillSlotSet == null || currentSkillSlotSet.Value != skillSlotSet);
		}


		private void ShowSkillIndicator(SkillSlotSet skillSlotSet)
		{
			SkillData skillData = GetSkillData(skillSlotSet);
			if (currentSkillSlotSet != null)
			{
				if (skillSlotSet == myCharacter.ConcentrateSkillSet)
				{
					ReleaseFixedIndicator(true);
				}
				else
				{
					HideIndicator();
				}
			}

			currentSkillSlotSet = skillSlotSet;
			float range;
			Splat spellIndicator = UpdateIndicator(skillData, out range);
			SetSkillIndicator(spellIndicator, range, skillData);
		}


		private void SetSkillIndicator(Splat spellIndicator, float range, SkillData skillData)
		{
			currentIndicatorSkillData = skillData;
			SplatManager splatManager = this.splatManager;
			if (splatManager != null)
			{
				splatManager.SetIndicator(spellIndicator);
			}

			MonoBehaviourInstance<GameUI>.inst.Minimap.UIMap.SetIndicator(range);
			myCharacter.LocalSkillPlayer.OnDisplaySkillIndicator(currentIndicatorSkillData, spellIndicator);
		}


		private void SetPickPointThenDirectionIndicator(SkillData skillData, Vector3 lineStartPosition,
			StatePickPointThenDirectionIndicator indicatorState)
		{
			float num;
			PickPointThenDirectionIndicator pickPointThenDirectionIndicator =
				UpdateIndicator(skillData, out num) as PickPointThenDirectionIndicator;
			if (pickPointThenDirectionIndicator != null)
			{
				pickPointThenDirectionIndicator.SetLineStartPosition(lineStartPosition);
				pickPointThenDirectionIndicator.SetIndicatorState(indicatorState);
			}
		}


		public Splat UpdateIndicator(SkillData skillData, out float range)
		{
			if (skillData == null)
			{
				range = 0f;
				return null;
			}

			float num = skillData.UseWeaponRange ? myCharacter.Stat.AttackRange : 0f;
			range = skillData.range + num;
			float num2 = 0f;
			float maxRange = range;
			SkillId skillId = skillData.SkillId != SkillId.None ? skillData.SkillId : skillData.PassiveSkillId;
			if (myCharacter.LocalSkillPlayer.GetSkillRange(skillId, skillData, ref num2, ref maxRange))
			{
				range = num2;
			}

			if (range <= 0f)
			{
				return null;
			}

			SplatManager splatManager = this.splatManager;
			Splat splat = splatManager != null ? splatManager.GetIndicator(skillData.Guideline) : null;
			if (splat == null)
			{
				return null;
			}

			float skillWidth = myCharacter.LocalSkillPlayer.GetSkillWidth(skillId, skillData);
			float skillInnerRange = myCharacter.LocalSkillPlayer.GetSkillInnerRange(skillId, skillData);
			float skillLength = myCharacter.LocalSkillPlayer.GetSkillLength(skillId, skillData);
			float skillAngle = myCharacter.LocalSkillPlayer.GetSkillAngle(skillId, skillData);
			splat.Range = range;
			splat.MaxRange = maxRange;
			splat.Progress = 0f;
			splat.Scale = skillInnerRange;
			splat.Length = skillLength;
			splat.Width = skillWidth;
			splat.Angle = skillAngle;
			splat.Direction = null;
			if (splat is PointIndicator)
			{
				(splat as PointIndicator).RestrictCursorToRange =
					skillData.CastWaysType == SkillCastWaysType.PickPointInArea;
			}

			return splat;
		}


		public void ReleaseFixedIndicator(bool withHideIndicator)
		{
			isFixedIndicator = false;
			if (withHideIndicator)
			{
				HideIndicator();
			}
		}


		public void HideIndicator()
		{
			if (isFixedIndicator)
			{
				return;
			}

			if (currentSkillSlotSet != null)
			{
				ClearUnprocessedAdditionalInputs(currentSkillSlotSet.Value);
			}

			myCharacter.LocalSkillPlayer.OnHideSkillIndicator(currentIndicatorSkillData,
				this.splatManager.CurrentIndicator);
			SplatManager splatManager = this.splatManager;
			if (splatManager != null)
			{
				splatManager.CancelIndicator();
			}

			MonoBehaviourInstance<GameUI>.inst.Minimap.UIMap.HideIndicator();
			currentSkillSlotSet = null;
			currentIndicatorSkillData = null;
		}


		private void ClearUnprocessedAdditionalInputs(SkillSlotSet skillSlotSet)
		{
			for (int i = unprocessedAdditionalInputs.Count - 1; i >= 0; i--)
			{
				if (unprocessedAdditionalInputs[i].SkillSlotSet == skillSlotSet)
				{
					unprocessedAdditionalInputs.RemoveAt(i);
				}
			}
		}


		public void OnChangedSkillPoint(bool increasedPoint)
		{
			UpdateUpgradeSkillSlots(increasedPoint);
			UpdateEvolutionSkilllSlots(false);
			UpdateRemainSkillPoint();
			MonoBehaviourInstance<GameUI>.inst.StatusHud.UpdateBuffPosition();
			if (!myPlayer.IsHaveSkillUpgradePoint())
			{
				MonoBehaviourInstance<Tooltip>.inst.Hide();
			}
		}


		public void OnChangedEvolutionSkillPoint(bool increasedPoint)
		{
			UpdateUpgradeSkillSlots(false, true);
			UpdateEvolutionSkilllSlots(increasedPoint);
			UpdateRemainSkillPoint();
			MonoBehaviourInstance<GameUI>.inst.StatusHud.UpdateBuffPosition();
			if (!myPlayer.IsHaveSkillUpgradePoint())
			{
				MonoBehaviourInstance<Tooltip>.inst.Hide();
			}
		}


		private void UpdateRemainSkillPoint()
		{
			if (myPlayer.CharacterSkill.SkillEvolution.IsHavePoint(SkillEvolutionPointType.CharacterSkill))
			{
				MonoBehaviourInstance<GameUI>.inst.SkillHud.SetRemainSkillPoint(
					myPlayer.CharacterSkill.SkillEvolution.GetPoint(SkillEvolutionPointType.CharacterSkill), true);
				return;
			}

			MonoBehaviourInstance<GameUI>.inst.SkillHud.SetRemainSkillPoint(myPlayer.SkillPoint, false);
		}


		public void OnSkillUpgrade(SkillSlotIndex skillSlotIndex)
		{
			UpdateSkillSlotSet(skillSlotIndex);
			SetAddSkillUpOrder(GetSkillData(skillSlotIndex).code);
			SkillSlot skillSlot = MonoBehaviourInstance<GameUI>.inst.SkillHud.FindSkillSlot(skillSlotIndex);
			if (skillSlot != null)
			{
				skillSlot.PlayEffect();
			}

			SkillSlot skillSlot2 = MonoBehaviourInstance<GameUI>.inst.SkillHud.FindSkillSlot(skillSlotIndex);
			if (skillSlot2 != null)
			{
				skillSlot2.PlayUpgradeEffect();
			}

			SkillUpgradeSlot skillUpgradeSlot =
				MonoBehaviourInstance<GameUI>.inst.SkillHud.FindSkillUpgradeSlot(skillSlotIndex);
			if (skillUpgradeSlot != null)
			{
				skillUpgradeSlot.RefreshTooltip();
			}

			Singleton<SoundControl>.inst.PlayUISound("SkillUp",
				ClientService.GamePlayMode.Play | ClientService.GamePlayMode.ObserveTeam);
		}


		public void OnSkillEvolution(SkillSlotIndex skillSlotIndex)
		{
			UpdateSkillSlotSet(skillSlotIndex);
			SetAddSkillUpOrder(GetSkillData(skillSlotIndex).code);
			SkillSlot skillSlot = MonoBehaviourInstance<GameUI>.inst.SkillHud.FindSkillSlot(skillSlotIndex);
			if (skillSlot != null)
			{
				skillSlot.PlayEffect();
			}

			SkillSlot skillSlot2 = MonoBehaviourInstance<GameUI>.inst.SkillHud.FindSkillSlot(skillSlotIndex);
			if (skillSlot2 != null)
			{
				skillSlot2.PlayEvolutionEffect();
			}

			Singleton<SoundControl>.inst.PlayUISound("SkillUp",
				ClientService.GamePlayMode.Play | ClientService.GamePlayMode.ObserveTeam);
		}


		private void UpdateSkillSlotSet(SkillSlotIndex skillSlotIndex)
		{
			bool isSkillEvolutionUiOpen = MonoBehaviourInstance<GameUI>.inst.SkillHud.IsSkillEvolutionUiOpen;
			SkillData skillData = GetSkillData(skillSlotIndex);
			UpdateSkillHudSlot(skillSlotIndex, skillData);
			UpdateSkillHudSlotCanUseSkill(skillSlotIndex, skillData);
			MonoBehaviourInstance<GameUI>.inst.SkillHud.UpdateSkillUpgradeSlot(skillSlotIndex,
				myPlayer.IsHaveSkillUpgradePoint(), myPlayer.CanSkillUpgrade(skillSlotIndex),
				myPlayer.CharacterSkill.SkillEvolution.AnyPoint(), false);
			MonoBehaviourInstance<GameUI>.inst.SkillHud.UpdateSkillEvolutionSlot(skillSlotIndex,
				GetSkillData(skillSlotIndex), myPlayer.IsHaveSkillEvolutionPoint(skillSlotIndex),
				myPlayer.CanSkillEvolution(skillSlotIndex), false);
			if (isSkillEvolutionUiOpen != MonoBehaviourInstance<GameUI>.inst.SkillHud.IsSkillEvolutionUiOpen)
			{
				if (isSkillEvolutionUiOpen)
				{
					Singleton<SoundControl>.inst.PlayUISound("SkillEvoUpClose",
						ClientService.GamePlayMode.Play | ClientService.GamePlayMode.ObserveTeam);
					return;
				}

				Singleton<SoundControl>.inst.PlayUISound("SkillEvoUpOpen",
					ClientService.GamePlayMode.Play | ClientService.GamePlayMode.ObserveTeam);
			}
		}


		public void SetCrowdControl(CharacterStateValue characterState, bool isAdd)
		{
			if (!characterState.StateType.IsCrowdControl())
			{
				return;
			}

			if (isAdd)
			{
				ActionCategoryType actionCategoryType = characterState.StateType.CanNotAction();
				if (actionCategoryType != ActionCategoryType.None)
				{
					SingletonMonoBehaviour<PlayerController>.inst.ClearNextInteractionObject(actionCategoryType);
				}
			}

			long uniqueCode = characterState.GetUniqueCode();
			bool lockSkill = !characterState.StateType.CanUseSkill();
			bool lockAggressiveSkill = !characterState.StateType.CanUseAggressiveSkill();
			bool lockMovementSkill = !characterState.StateType.CanUseMovementSkill();
			foreach (SkillSlotSet skillSlotSet in GameDB.skill.allSkillSlotSet)
			{
				if (skillSlotSet != SkillSlotSet.None)
				{
					SkillSlotIndex skillSlotIndex = skillSlotSet.SlotSet2Index();
					if (skillSlotIndex >= SkillSlotIndex.Active1 && skillSlotIndex <= SkillSlotIndex.SpecialSkill)
					{
						skillLock.LockByState(skillSlotSet, isAdd, uniqueCode, lockSkill, lockAggressiveSkill,
							lockMovementSkill);
					}
				}
			}

			for (SkillSlotIndex skillSlotIndex2 = SkillSlotIndex.Active1;
				skillSlotIndex2 <= SkillSlotIndex.SpecialSkill;
				skillSlotIndex2++)
			{
				SkillSlot skillSlot = MonoBehaviourInstance<GameUI>.inst.SkillHud.FindSkillSlot(skillSlotIndex2);
				if (skillSlot != null)
				{
					skillSlot.SetLock(IsLockSkill(skillSlotIndex2));
				}
			}
		}


		public void LockSkillSlot(SpecialSkillId specialSkillId, bool isLock)
		{
			skillLock.Lock(specialSkillId, isLock);
			if (specialSkillId == myPlayer.CharacterSkill.SpecialSkillId)
			{
				SkillSlot skillSlot =
					MonoBehaviourInstance<GameUI>.inst.SkillHud.FindSkillSlot(SkillSlotIndex.SpecialSkill);
				if (skillSlot == null)
				{
					return;
				}

				skillSlot.SetLock(IsLockSkill(SkillSlotSet.SpecialSkill));
			}
		}


		public void LockSkillSlot(SkillSlotSet skillSlotSet, bool isLock)
		{
			skillLock.Lock(skillSlotSet, isLock);
			SkillSlot skillSlot = MonoBehaviourInstance<GameUI>.inst.SkillHud.FindSkillSlot(skillSlotSet);
			if (skillSlot == null)
			{
				return;
			}

			skillSlot.SetLock(IsLockSkill(skillSlotSet));
		}


		private bool IsLockSkill(SkillSlotSet skillSlotSet)
		{
			SkillData skillData = GetSkillData(skillSlotSet);
			return skillData == null ||
			       skillSlotSet == SkillSlotSet.SpecialSkill &&
			       skillLock.IsSpecialSkillLock(myPlayer.CharacterSkill.SpecialSkillId) || skillLock.IsLock(
				       skillSlotSet, skillData.CanCastingWhileCCState, skillData.AggressiveSkill,
				       skillData.MovementSkill);
		}


		private bool IsLockSkill(SkillSlotIndex skillSlotIndex)
		{
			SkillSlotSet? skillSlotSet = myPlayer.CharacterSkill.GetSkillSlotSet(skillSlotIndex);
			return skillSlotSet == null || IsLockSkill(skillSlotSet.Value);
		}


		public void CursorStatusChange(CursorStatus cursorStatus)
		{
			ChangeCursorStatus onChangeCursorStatus = OnChangeCursorStatus;
			if (onChangeCursorStatus == null)
			{
				return;
			}

			onChangeCursorStatus(cursorStatus);
		}


		public void StartPlayerConcentratingBySkill(SkillSlotSet skillSlotSet, float chargingTime, float minRange,
			float maxRange, float minAngle, float maxAngle, bool useProgress, bool showIndicator, Vector3? direction)
		{
			SkillData skillData = GetSkillData(skillSlotSet);
			if (skillData == null)
			{
				return;
			}

			if (string.IsNullOrEmpty(skillData.Guideline))
			{
				return;
			}

			if (CheckUnprocessedAdditionalInputs(skillSlotSet))
			{
				return;
			}

			Splat spellIndicator = null;
			if (showIndicator)
			{
				float range;
				Splat spellIndicator2 = UpdateIndicator(skillData, out range);
				SetSkillIndicator(spellIndicator2, range, skillData);
				SplatManager splatManager = this.splatManager;
				spellIndicator = splatManager != null ? splatManager.GetIndicator(skillData.Guideline) : null;
			}

			myCharacter.StartThrowingCoroutine(
				UpdateConcentrationSkillIndicator(spellIndicator, skillSlotSet, skillData, chargingTime, minRange,
					maxRange, minAngle, maxAngle, useProgress, direction, Time.time),
				delegate(Exception exception)
				{
					Log.E(string.Format("[EXCEPTION][SKILL][INDICATOR][{0}] Message:{1}, StackTrace:{2}",
						skillData.code, exception.Message, exception.StackTrace));
				});
		}


		private IEnumerator UpdateConcentrationSkillIndicator(Splat spellIndicator, SkillSlotSet skillSlotSet,
			SkillData skillData, float chargingTime, float minRange, float maxRange, float minAngle, float maxAngle,
			bool useProgress, Vector3? direction, float startTime)
		{
			currentSkillSlotSet = skillSlotSet;
			isFixedIndicator = true;
			while (myCharacter.IsConcentrating && currentSkillSlotSet != null)
			{
				if (spellIndicator != null)
				{
					float num = Time.time - startTime;
					float num2 = 0f;
					if (Mathf.Abs(chargingTime) > Mathf.Epsilon)
					{
						num2 = num / chargingTime;
						if (1f < num2)
						{
							num2 = 1f;
						}
					}

					float num3 = Mathf.Lerp(minRange, maxRange, num2);
					float angle = Mathf.Lerp(minAngle, maxAngle, num2);
					float length = Mathf.Lerp(minRange, maxRange, num2);
					if (useProgress)
					{
						spellIndicator.Range = maxRange;
						spellIndicator.MaxRange = maxRange;
						spellIndicator.Length = maxRange;
						spellIndicator.Progress = num3 / maxRange;
					}
					else
					{
						spellIndicator.Range = num3;
						spellIndicator.MaxRange = maxRange;
						spellIndicator.Length = length;
						spellIndicator.Progress = 0f;
					}

					spellIndicator.Angle = angle;
					spellIndicator.Direction = direction;
					SetSkillIndicator(spellIndicator, num3, skillData);
				}

				ChangeCursorStatus onChangeCursorStatus = OnChangeCursorStatus;
				if (onChangeCursorStatus != null)
				{
					onChangeCursorStatus(CursorStatus.SkillCanMove);
				}

				yield return null;
			}
		}


		public void OnStartConcentration() { }


		public void OnEndConcentration(SkillData skillData)
		{
			SkillSlotSet skillSlotSet = SkillSlotSet.None;
			MasteryType masteryType = MasteryType.None;
			if (GameDB.skill.FindSkillSlotAndMastery(myCharacter.CharacterCode, skillData.code, myCharacter.ObjectType,
				ref skillSlotSet, ref masteryType))
			{
				SkillSlotSet skillSlotSet2 = skillSlotSet;
				SkillSlotSet? skillSlotSet3 = currentSkillSlotSet;
				ReleaseFixedIndicator((skillSlotSet2 == skillSlotSet3.GetValueOrDefault()) & (skillSlotSet3 != null));
			}

			ChangeCursorStatus onChangeCursorStatus = OnChangeCursorStatus;
			if (onChangeCursorStatus == null)
			{
				return;
			}

			onChangeCursorStatus(CursorStatus.Normal);
		}


		public void OnInputReleaseEvent(GameInputEvent inputEvent, LocalObject hitTarget, Vector3 hitPoint,
			Vector3 releasePoint)
		{
			if (inputEvent == GameInputEvent.Select)
			{
				if (currentSkillSlotSet != null)
				{
					if (!PossibleUseActiveSkill(currentSkillSlotSet.Value, hitTarget, hitPoint))
					{
						return;
					}

					SkillSlotSet value = currentSkillSlotSet.Value;
					SkillData skillData = GetSkillData(value);
					int localSkillSequence = myPlayer.GetLocalSkillSequence(value);
					bool isAdditionalInput = IsAdditionalInput(value, localSkillSequence, skillData);
					if (skillData.CastWaysType == SkillCastWaysType.PickPointThenDirection &&
					    pickPointWhenUseSkillPickPointThenDirection != null)
					{
						ChangeCursorStatus onChangeCursorStatus = OnChangeCursorStatus;
						if (onChangeCursorStatus != null)
						{
							onChangeCursorStatus(CursorStatus.Normal);
						}

						UseSkill(value, skillData, localSkillSequence, hitTarget,
							pickPointWhenUseSkillPickPointThenDirection.Value, releasePoint, isAdditionalInput);
					}
				}

				pickPointWhenUseSkillPickPointThenDirection = null;
				return;
			}

			if (inputEvent - GameInputEvent.Active1 > 4)
			{
				return;
			}

			SkillSlotIndex skillSlotIndex = GetSkillSlotIndex(inputEvent);
			SkillSlotSet? skillSlotSet = myPlayer.CharacterSkill.GetSkillSlotSet(skillSlotIndex);
			if (skillSlotSet == null)
			{
				return;
			}

			if (currentSkillSlotSet != null)
			{
				SkillSlotSet value2 = currentSkillSlotSet.Value;
				SkillSlotSet? skillSlotSet2 = skillSlotSet;
				if (!((value2 == skillSlotSet2.GetValueOrDefault()) & (skillSlotSet2 != null)))
				{
					return;
				}
			}

			if (!Singleton<LocalSetting>.inst.setting.quickCastKeys.ContainsKey(inputEvent))
			{
				return;
			}

			bool flag = Singleton<LocalSetting>.inst.setting.quickCastKeys[inputEvent];
			SkillData skillData2 = GetSkillData(skillSlotSet.Value);
			bool flag2 = false;
			if (skillData2.AdditionalAction.CanInputReleaseKey() &&
			    (flag || skillData2.AdditionalAction == AdditionalAction.ReleaseKey))
			{
				flag2 = true;
			}

			if (!flag2 && skillData2.CastWaysType.CanInputReleaseKey() && flag)
			{
				flag2 = true;
			}

			if (!flag2)
			{
				return;
			}

			if (!myCharacter.LocalSkillPlayer.IsPlaying(skillData2.SkillId) &&
			    skillData2.CastWaysType != SkillCastWaysType.PickPointThenDirection)
			{
				SkillInputsCheckValidate();
				bool flag3 = false;
				foreach (SkillInput skillInput in skillInputs)
				{
					if (skillInput.SkillSlotSet == skillSlotSet.Value)
					{
						flag3 = true;
						break;
					}
				}

				if (!flag3)
				{
					return;
				}
			}

			if (IsLockSkill(skillSlotIndex))
			{
				MonoBehaviourInstance<GameUI>.inst.ToastMessage.ShowMessage(Ln.Get(useSkillErrorMessage[0]));
				return;
			}

			if (myCharacter == null)
			{
				return;
			}

			if (myCharacter.CheckDyingCondition())
			{
				return;
			}

			if (!PossibleUseActiveSkill(skillSlotSet.Value, hitTarget, hitPoint))
			{
				return;
			}

			int localSkillSequence2 = myPlayer.GetLocalSkillSequence(skillSlotSet.Value);
			bool isAdditionalInput2 = IsAdditionalInput(skillSlotSet.Value, localSkillSequence2, skillData2);
			if (ShouldChangeCursorStatusToSkill(skillData2, isAdditionalInput2, flag))
			{
				if (!PossibleShowSkillIndicator(skillSlotSet.Value) ||
				    skillData2.CastWaysType == SkillCastWaysType.PickPointThenDirection)
				{
					return;
				}

				ShowSkillIndicator(skillSlotSet.Value);
				ChangeCursorStatus onChangeCursorStatus2 = OnChangeCursorStatus;
				if (onChangeCursorStatus2 == null)
				{
					return;
				}

				onChangeCursorStatus2(CursorStatus.Skill);
			}
			else
			{
				ChangeCursorStatus onChangeCursorStatus3 = OnChangeCursorStatus;
				if (onChangeCursorStatus3 != null)
				{
					onChangeCursorStatus3(CursorStatus.Normal);
				}

				if (skillData2.CastWaysType == SkillCastWaysType.PickPointThenDirection && flag &&
				    pickPointWhenUseSkillPickPointThenDirection != null)
				{
					UseSkill(skillSlotSet.Value, skillData2, localSkillSequence2, hitTarget,
						pickPointWhenUseSkillPickPointThenDirection.Value, releasePoint, isAdditionalInput2);
					pickPointWhenUseSkillPickPointThenDirection = null;
					return;
				}

				UseSkill(skillSlotSet.Value, skillData2, localSkillSequence2, hitTarget, hitPoint, releasePoint,
					isAdditionalInput2);
			}
		}


		public void OnInputEvent(GameInputEvent inputEvent, bool isPressing, LocalObject hitTarget, Vector3 hitPoint,
			Vector3 releasePoint, bool isHitTargetableLayer)
		{
			if (!myCharacter.IsAlive && (inputEvent != GameInputEvent.Select ||
			                             MonoBehaviourInstance<ClientService>.inst.CurGamePlayMode !=
			                             ClientService.GamePlayMode.ObserveTeam))
			{
				return;
			}

			switch (inputEvent)
			{
				case GameInputEvent.Select:
					if (OnSelectEvent(hitTarget, hitPoint))
					{
						SingletonMonoBehaviour<PlayerController>.inst.ClearNextInteractionObject();
						return;
					}

					SingletonMonoBehaviour<PlayerController>.inst.OnSelectEvent(hitTarget, hitPoint, releasePoint,
						isHitTargetableLayer);
					return;
				case GameInputEvent.Move:
					SingletonMonoBehaviour<PlayerController>.inst.OnMoveEvent(isPressing, hitTarget, hitPoint,
						isHitTargetableLayer);
					return;
				case GameInputEvent.Stop:
				case GameInputEvent.Hold:
					break;
				case GameInputEvent.Attack:
					if (OnInputEventEnableAttack())
					{
						SkillSlotIndex skillSlotIndex = GetSkillSlotIndex(inputEvent);
						if (ShowSeparateSkillIndicator(skillSlotIndex))
						{
							ChangeCursorStatus onChangeCursorStatus = OnChangeCursorStatus;
							if (onChangeCursorStatus == null)
							{
								return;
							}

							onChangeCursorStatus(CursorStatus.Attack);
						}
					}

					break;
				case GameInputEvent.Active1:
				case GameInputEvent.Active2:
				case GameInputEvent.Active3:
				case GameInputEvent.Active4:
				case GameInputEvent.WeaponSkill:
				{
					if (!Singleton<LocalSetting>.inst.setting.quickCastKeys.ContainsKey(inputEvent))
					{
						return;
					}

					bool flag = Singleton<LocalSetting>.inst.setting.quickCastKeys[inputEvent];
					SkillSlotIndex skillSlotIndex2 = GetSkillSlotIndex(inputEvent);
					SkillSlotSet? skillSlotSet = myPlayer.CharacterSkill.GetSkillSlotSet(skillSlotIndex2);
					if (skillSlotSet == null)
					{
						return;
					}

					if (!IsPlaying(skillSlotSet.Value) && !flag && currentSkillSlotSet != null)
					{
						if (skillSlotSet.Value == currentSkillSlotSet.Value)
						{
							return;
						}

						HideIndicator();
					}

					OnUseSkillEvent(skillSlotIndex2, hitTarget, hitPoint, releasePoint, flag);
					return;
				}
				case GameInputEvent.LearnPassive:
				case GameInputEvent.LearnActive1:
				case GameInputEvent.LearnActive2:
				case GameInputEvent.LearnActive3:
				case GameInputEvent.LearnActive4:
				case GameInputEvent.LearnWeaponSkill:
				{
					SkillSlotIndex skillSlotIndex3 = GetSkillSlotIndex(inputEvent);
					if (myPlayer.CharacterSkill.SkillEvolution.AnyPoint())
					{
						if (myPlayer.CharacterSkill.SkillEvolution.IsHavePoint(skillSlotIndex3))
						{
							EvolutionSkill(skillSlotIndex3);
						}
					}
					else
					{
						UpgradeSkill(skillSlotIndex3);
					}

					break;
				}
				default:
					if (inputEvent != GameInputEvent.MoveToAttack)
					{
						return;
					}

					if (OnInputEventEnableAttack())
					{
						SkillSlotIndex skillSlotIndex4 = GetSkillSlotIndex(GameInputEvent.Attack);
						if (ShowSeparateSkillIndicator(skillSlotIndex4))
						{
							ChangeCursorStatus onChangeCursorStatus2 = OnChangeCursorStatus;
							if (onChangeCursorStatus2 != null)
							{
								onChangeCursorStatus2(CursorStatus.Attack);
							}
						}

						if (OnSelectEvent(hitTarget, hitPoint))
						{
							SingletonMonoBehaviour<PlayerController>.inst.ClearNextInteractionObject();
							return;
						}

						SingletonMonoBehaviour<PlayerController>.inst.OnSelectEvent(hitTarget, hitPoint, releasePoint,
							isHitTargetableLayer);
					}

					break;
			}
		}


		private bool OnInputEventEnableAttack()
		{
			if (!myCharacter.IsEquippedWeapon())
			{
				MonoBehaviourInstance<GameUI>.inst.ToastMessage.ShowMessage(Ln.Get(useSkillErrorMessage[6]));
				return false;
			}

			return !myCharacter.IsConcentrating;
		}


		private bool OnSelectEvent(LocalObject hitTarget, Vector3 hitPoint)
		{
			if (currentSkillSlotSet != null)
			{
				if (!PossibleUseActiveSkill(currentSkillSlotSet.Value, hitTarget, hitPoint))
				{
					return false;
				}

				SkillSlotSet value = currentSkillSlotSet.Value;
				SkillData skillData = GetSkillData(value);
				int localSkillSequence = myPlayer.GetLocalSkillSequence(value);
				bool isAdditionalInput = IsAdditionalInput(value, localSkillSequence, skillData);
				SkillSlotIndex value2 = myPlayer.CharacterSkill.GetSkillSlotIndex(value).Value;
				if (ShouldChangeCursorStatusToSkill(skillData, isAdditionalInput))
				{
					if (PossibleShowSkillIndicator(value))
					{
						ShowSkillIndicator(value);
						ChangeCursorStatus onChangeCursorStatus = OnChangeCursorStatus;
						if (onChangeCursorStatus != null)
						{
							onChangeCursorStatus(CursorStatus.Skill);
						}
					}

					return false;
				}

				if (skillData.CastWaysType != SkillCastWaysType.PickPointThenDirection)
				{
					ChangeCursorStatus onChangeCursorStatus2 = OnChangeCursorStatus;
					if (onChangeCursorStatus2 != null)
					{
						onChangeCursorStatus2(CursorStatus.Normal);
					}

					return UseSkill(value, skillData, localSkillSequence, hitTarget, hitPoint, hitPoint,
						isAdditionalInput);
				}

				GameInputEvent gameInputEvent = GetGameInputEvent(value2);
				if (!Singleton<LocalSetting>.inst.setting.quickCastKeys.ContainsKey(gameInputEvent))
				{
					return false;
				}

				if (!Singleton<LocalSetting>.inst.setting.quickCastKeys[gameInputEvent])
				{
					pickPointWhenUseSkillPickPointThenDirection = hitPoint;
					SetPickPointThenDirectionIndicator(skillData, pickPointWhenUseSkillPickPointThenDirection.Value,
						StatePickPointThenDirectionIndicator.PickAfter);
				}

				return false;
			}

			if (hitTarget != null && !hitTarget.IsTypeOf<LocalCharacter>())
			{
				return false;
			}

			if (myCharacter.LocalSkillPlayer.IsPlayingCanAdditionalActionActiveSkill())
			{
				MonoBehaviourInstance<GameClient>.inst.Request(new ReqPlayingSkillOnSelect
				{
					targetId = hitTarget != null ? hitTarget.ObjectId : 0,
					hitPosition = hitPoint,
					releasePosition = hitPoint
				});
				return true;
			}

			return false;
		}


		private void OnUseSkillEvent(SkillSlotIndex skillSlotIndex, LocalObject hitTarget, Vector3 hitPoint,
			Vector3 releasePoint, bool isQuickCast)
		{
			if (IsLockSkill(skillSlotIndex))
			{
				MonoBehaviourInstance<GameUI>.inst.ToastMessage.ShowMessage(Ln.Get(useSkillErrorMessage[0]));
				return;
			}

			if (myCharacter == null)
			{
				return;
			}

			if (myCharacter.CheckDyingCondition())
			{
				return;
			}

			SkillSlotSet? skillSlotSet = myPlayer.CharacterSkill.GetSkillSlotSet(skillSlotIndex);
			if (skillSlotSet == null)
			{
				return;
			}

			if (!PossibleUseActiveSkill(skillSlotSet.Value, hitTarget, hitPoint))
			{
				return;
			}

			SkillData skillData = GetSkillData(skillSlotSet.Value);
			int localSkillSequence = myPlayer.GetLocalSkillSequence(skillSlotSet.Value);
			bool isAdditionalInput = IsAdditionalInput(skillSlotSet.Value, localSkillSequence, skillData);
			if (!ShouldChangeCursorStatusToSkill(skillData, isAdditionalInput, isQuickCast))
			{
				ChangeCursorStatus onChangeCursorStatus = OnChangeCursorStatus;
				if (onChangeCursorStatus != null)
				{
					onChangeCursorStatus(CursorStatus.Normal);
				}

				if (UseSkill(skillSlotSet.Value, skillData, localSkillSequence, hitTarget, hitPoint, releasePoint,
					isAdditionalInput) && !skillData.InstantCast())
				{
					SingletonMonoBehaviour<PlayerController>.inst.ClearNextInteractionObject();
				}

				return;
			}

			if (!PossibleShowSkillIndicator(skillSlotSet.Value))
			{
				return;
			}

			if (skillData.CastWaysType == SkillCastWaysType.PickPointThenDirection && isQuickCast)
			{
				pickPointWhenUseSkillPickPointThenDirection = hitPoint;
				SetPickPointThenDirectionIndicator(skillData, pickPointWhenUseSkillPickPointThenDirection.Value,
					StatePickPointThenDirectionIndicator.PickAfter);
			}

			ShowSkillIndicator(skillSlotSet.Value);
			ChangeCursorStatus onChangeCursorStatus2 = OnChangeCursorStatus;
			if (onChangeCursorStatus2 == null)
			{
				return;
			}

			onChangeCursorStatus2(CursorStatus.Skill);
		}


		public void OnUpdateWeaponEquip()
		{
			UpdateSkillHudSlot(SkillSlotIndex.WeaponSkill);
			UpdateWeaponSkillSlotCooldown();
		}


		public void UpdateAllSkillHud()
		{
			UpdateSkillHudSlots();
			UpdateUpgradeSkillSlots(false);
			UpdateEvolutionSkilllSlots(false);
			UpdateSkillSlotCooldown();
			UpdateRemainSkillPoint();
		}


		public void UpdateSkillHudSlots()
		{
			for (SkillSlotIndex skillSlotIndex = SkillSlotIndex.Passive;
				skillSlotIndex <= SkillSlotIndex.SpecialSkill;
				skillSlotIndex++)
			{
				if (skillSlotIndex != SkillSlotIndex.SpecialSkill)
				{
					UpdateSkillHudSlot(skillSlotIndex);
				}
			}
		}


		private void UpdateSkillHudSlot(SkillSlotIndex index)
		{
			SkillData skillData = GetSkillData(index);
			UpdateSkillHudSlot(index, skillData);
			UpdateSkillHudSlotCanUseSkill(index, skillData);
		}


		public void UpdateSkillHudSlot(SkillSlotIndex skillSlotIndex, SkillData skillData = null)
		{
			if (skillSlotIndex == SkillSlotIndex.SpecialSkill)
			{
				return;
			}

			if (skillData == null)
			{
				skillData = GetSkillData(skillSlotIndex);
			}

			if (skillSlotIndex == SkillSlotIndex.WeaponSkill)
			{
				MasteryType equipWeaponMasteryType = myCharacter.GetEquipWeaponMasteryType();
				MonoBehaviourInstance<GameUI>.inst.SkillHud.UpdateWeaponSkillSlot(skillData,
					myPlayer.CharacterSkill.GetSkillLevel(equipWeaponMasteryType),
					myPlayer.CharacterSkill.GetSkillEvolutionLevel(skillSlotIndex),
					myPlayer.Character.GetSkillMaxEvolutionLevel(skillSlotIndex), UpgradeSkill, EvolutionSkill,
					IsLockSkill(skillSlotIndex));
				return;
			}

			MonoBehaviourInstance<GameUI>.inst.SkillHud.UpdateSkillSlot(skillSlotIndex, skillData,
				myCharacter.GetSkillLevel(skillSlotIndex),
				myPlayer.CharacterSkill.GetSkillEvolutionLevel(skillSlotIndex),
				myPlayer.Character.GetSkillMaxEvolutionLevel(skillSlotIndex), UpgradeSkill, EvolutionSkill,
				IsLockSkill(skillSlotIndex));
		}


		public void UpdateSkillHudSlotsCanUseSkill()
		{
			for (SkillSlotIndex skillSlotIndex = SkillSlotIndex.Passive;
				skillSlotIndex <= SkillSlotIndex.SpecialSkill;
				skillSlotIndex++)
			{
				UpdateSkillHudSlotCanUseSkill(skillSlotIndex);
			}
		}


		private void UpdateSkillHudSlotCanUseSkill(SkillSlotIndex skillSlotIndex, SkillData skillData = null)
		{
			if (skillSlotIndex == SkillSlotIndex.SpecialSkill)
			{
				return;
			}

			if (skillData == null)
			{
				skillData = GetSkillData(skillSlotIndex);
			}

			bool isSkillSlotCanUseInSkillScript =
				myCharacter.IsSkillSlotCanUseInSkillScript(skillData, skillSlotIndex) == UseSkillErrorCode.None;
			if (skillSlotIndex == SkillSlotIndex.WeaponSkill)
			{
				MasteryType equipWeaponMasteryType = myCharacter.GetEquipWeaponMasteryType();
				MonoBehaviourInstance<GameUI>.inst.SkillHud.UpdateWeaponSkillSlotCanUse(
					myPlayer.CharacterSkill.GetSkillLevel(equipWeaponMasteryType), IsLockSkill(skillSlotIndex),
					myCharacter.EnoughSkillResources(skillData), isSkillSlotCanUseInSkillScript,
					myPlayer.CharacterSkill.CheckCooldown(skillSlotIndex,
						MonoBehaviourInstance<ClientService>.inst.CurrentSkillTime));
				return;
			}

			MonoBehaviourInstance<GameUI>.inst.SkillHud.UpdateSkillSlotCanUse(skillSlotIndex,
				myCharacter.GetSkillLevel(skillSlotIndex), IsLockSkill(skillSlotIndex),
				myCharacter.EnoughSkillResources(skillData), isSkillSlotCanUseInSkillScript,
				myPlayer.CharacterSkill.CheckCooldown(skillSlotIndex,
					MonoBehaviourInstance<ClientService>.inst.CurrentSkillTime));
		}


		private void UpdateUpgradeSkillSlots(bool increasedPoint, bool isFromEvolutionDone = false)
		{
			for (SkillSlotIndex skillSlotIndex = SkillSlotIndex.Passive;
				skillSlotIndex <= SkillSlotIndex.WeaponSkill;
				skillSlotIndex++)
			{
				UpdateUpgradeSkillSlot(skillSlotIndex, increasedPoint, isFromEvolutionDone);
			}
		}


		private void UpdateUpgradeSkillSlot(SkillSlotIndex skillSlotIndex, bool increasedPoint,
			bool isFromEvolutionDone)
		{
			bool isSkillLevelUpUiOpen = MonoBehaviourInstance<GameUI>.inst.SkillHud.IsSkillLevelUpUiOpen;
			bool flag = myPlayer.CharacterSkill.SkillEvolution.AnyPoint();
			MonoBehaviourInstance<GameUI>.inst.SkillHud.UpdateSkillUpgradeSlot(skillSlotIndex,
				myPlayer.IsHaveSkillUpgradePoint(), myPlayer.CanSkillUpgrade(skillSlotIndex), flag, increasedPoint);
			if (isSkillLevelUpUiOpen != MonoBehaviourInstance<GameUI>.inst.SkillHud.IsSkillLevelUpUiOpen)
			{
				if (isSkillLevelUpUiOpen)
				{
					if (!flag)
					{
						Singleton<SoundControl>.inst.PlayUISound("SkillLvUpClose",
							ClientService.GamePlayMode.Play | ClientService.GamePlayMode.ObserveTeam);
					}
				}
				else if (!isFromEvolutionDone)
				{
					Singleton<SoundControl>.inst.PlayUISound("SkillLvUpOpen",
						ClientService.GamePlayMode.Play | ClientService.GamePlayMode.ObserveTeam);
				}
			}
		}


		public void UpdateEvolutionSkilllSlots(bool increasedPoint)
		{
			for (SkillSlotIndex skillSlotIndex = SkillSlotIndex.Passive;
				skillSlotIndex <= SkillSlotIndex.WeaponSkill;
				skillSlotIndex++)
			{
				UpdateEvolutionSkilllSlots(skillSlotIndex, increasedPoint);
			}
		}


		private void UpdateEvolutionSkilllSlots(SkillSlotIndex skillSlotIndex, bool increasedPoint)
		{
			bool isSkillEvolutionUiOpen = MonoBehaviourInstance<GameUI>.inst.SkillHud.IsSkillEvolutionUiOpen;
			MonoBehaviourInstance<GameUI>.inst.SkillHud.UpdateSkillEvolutionSlot(skillSlotIndex,
				GetSkillData(skillSlotIndex), myPlayer.IsHaveSkillEvolutionPoint(skillSlotIndex),
				myPlayer.CanSkillEvolution(skillSlotIndex), increasedPoint);
			if (isSkillEvolutionUiOpen != MonoBehaviourInstance<GameUI>.inst.SkillHud.IsSkillEvolutionUiOpen)
			{
				if (isSkillEvolutionUiOpen)
				{
					Singleton<SoundControl>.inst.PlayUISound("SkillEvoUpClose",
						ClientService.GamePlayMode.Play | ClientService.GamePlayMode.ObserveTeam);
					return;
				}

				Singleton<SoundControl>.inst.PlayUISound("SkillEvoUpOpen",
					ClientService.GamePlayMode.Play | ClientService.GamePlayMode.ObserveTeam);
			}
		}


		private void UpdateWeaponSkillSlotCooldown()
		{
			MasteryType equipWeaponMasteryType = myPlayer.Character.GetEquipWeaponMasteryType();
			float cooldown = myPlayer.CharacterSkill.GetCooldown(equipWeaponMasteryType,
				MonoBehaviourInstance<ClientService>.inst.CurrentSkillTime);
			SkillData skillData = GetSkillData(SkillSlotIndex.WeaponSkill);
			if (cooldown > 0f)
			{
				float num = skillData != null ? skillData.cooldown : cooldown;
				if (num < cooldown)
				{
					num = cooldown;
				}

				MonoBehaviourInstance<GameUI>.inst.SkillHud.SetCooldown(SkillSlotIndex.WeaponSkill, cooldown, num,
					1f <= cooldown);
			}
			else
			{
				MonoBehaviourInstance<GameUI>.inst.SkillHud.FinishCooldown(SkillSlotIndex.WeaponSkill);
			}

			if (skillData != null && GameDB.skill.GetSkillGroupData(skillData.group).stackAble)
			{
				float stackUseIntervalTime = myPlayer.CharacterSkill.GetStackUseIntervalTime(equipWeaponMasteryType,
					MonoBehaviourInstance<ClientService>.inst.CurrentSkillTime);
				if (stackUseIntervalTime > 0f)
				{
					float stackIntervalTimeMax =
						skillData != null ? skillData.stackUseIntervalTime : stackUseIntervalTime;
					if (myPlayer.CharacterSkill.GetSkillStack(SkillSlotSet.WeaponSkill, equipWeaponMasteryType) >= 1)
					{
						MonoBehaviourInstance<GameUI>.inst.SkillHud.SetStackIntervalTime(SkillSlotIndex.WeaponSkill,
							stackUseIntervalTime, stackIntervalTimeMax);
						return;
					}

					if (myPlayer.CharacterSkill.GetCooldown(equipWeaponMasteryType,
						MonoBehaviourInstance<ClientService>.inst.CurrentSkillTime) < stackUseIntervalTime)
					{
						MonoBehaviourInstance<GameUI>.inst.SkillHud.SetStackIntervalTime(SkillSlotIndex.WeaponSkill,
							stackUseIntervalTime, stackIntervalTimeMax);
					}
				}
			}
		}


		private void UpdateSkillSlotCooldown()
		{
			for (SkillSlotIndex skillSlotIndex = SkillSlotIndex.Passive;
				skillSlotIndex <= SkillSlotIndex.SpecialSkill;
				skillSlotIndex++)
			{
				SkillSlotSet? skillSlotSet = myPlayer.CharacterSkill.GetSkillSlotSet(skillSlotIndex);
				if (skillSlotSet != null)
				{
					SkillSlotSet? skillSlotSet2 = skillSlotSet;
					SkillSlotSet skillSlotSet3 = SkillSlotSet.WeaponSkill;
					float cooldown;
					if ((skillSlotSet2.GetValueOrDefault() == skillSlotSet3) & (skillSlotSet2 != null))
					{
						cooldown = myPlayer.CharacterSkill.GetCooldown(myPlayer.Character.GetEquipWeaponMasteryType(),
							MonoBehaviourInstance<ClientService>.inst.CurrentSkillTime);
					}
					else
					{
						cooldown = myPlayer.CharacterSkill.GetCooldown(skillSlotSet.Value,
							MonoBehaviourInstance<ClientService>.inst.CurrentSkillTime);
					}

					if (cooldown > 0f)
					{
						SkillData skillData = GetSkillData(skillSlotIndex);
						float maxCooldown = Mathf.Max(skillData != null ? skillData.cooldown : cooldown, cooldown);
						MonoBehaviourInstance<GameUI>.inst.SkillHud.SetCooldown(skillSlotIndex, cooldown, maxCooldown,
							1f <= cooldown);
					}
					else
					{
						MonoBehaviourInstance<GameUI>.inst.SkillHud.FinishCooldown(skillSlotIndex);
					}
				}
			}
		}


		private void SetAddSkillUpOrder(int skillCode)
		{
			Dictionary<int, int> dictionary = skillUpOrderMap;
			int key = skillUpOrder + 1;
			skillUpOrder = key;
			dictionary.Add(key, skillCode);
		}


		private void SetAddSkillEvolutionOrder(int skillCode)
		{
			Dictionary<int, int> dictionary = skillEvolutionOrderMap;
			int key = skillEvolutionOrder + 1;
			skillEvolutionOrder = key;
			dictionary.Add(key, skillCode);
		}


		public Dictionary<string, int> GetLogSkillLevelInfo()
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			foreach (KeyValuePair<int, int> keyValuePair in skillUpOrderMap)
			{
				SkillData skillData = GameDB.skill.GetSkillData(keyValuePair.Value);
				if (dictionary.ContainsKey(skillData.Name))
				{
					Dictionary<string, int> dictionary2 = dictionary;
					string name = skillData.Name;
					dictionary2[name]++;
				}
				else
				{
					dictionary.Add(skillData.Name, 1);
				}
			}

			return dictionary;
		}


		public Dictionary<int, string> GetLogSkillUpOrder()
		{
			Dictionary<int, string> dictionary = new Dictionary<int, string>();
			foreach (KeyValuePair<int, int> keyValuePair in skillUpOrderMap)
			{
				SkillData skillData = GameDB.skill.GetSkillData(keyValuePair.Value);
				dictionary.Add(keyValuePair.Key, skillData.Name);
			}

			return dictionary;
		}


		public Dictionary<string, int> GetLogSkillEvolutionLevelInfo()
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			foreach (KeyValuePair<int, int> keyValuePair in skillEvolutionOrderMap)
			{
				SkillData skillData = GameDB.skill.GetSkillData(keyValuePair.Value);
				if (dictionary.ContainsKey(skillData.Name))
				{
					Dictionary<string, int> dictionary2 = dictionary;
					string name = skillData.Name;
					dictionary2[name]++;
				}
				else
				{
					dictionary.Add(skillData.Name, 1);
				}
			}

			return dictionary;
		}


		public Dictionary<int, string> GetLogSkillEvolutionOrder()
		{
			Dictionary<int, string> dictionary = new Dictionary<int, string>();
			foreach (KeyValuePair<int, int> keyValuePair in skillEvolutionOrderMap)
			{
				SkillData skillData = GameDB.skill.GetSkillData(keyValuePair.Value);
				dictionary.Add(keyValuePair.Key, skillData.Name);
			}

			return dictionary;
		}


		public SkillData GetSkillData(SkillSlotSet skillSlotSet)
		{
			return myCharacter.GetSkillData(skillSlotSet);
		}


		public SkillData GetSkillData(SkillSlotIndex skillSlotIndex)
		{
			return myCharacter.GetSkillData(skillSlotIndex);
		}


		public bool IsPlaying(SkillSlotIndex skillSlotIndex)
		{
			SkillData skillData = GetSkillData(skillSlotIndex);
			return skillData != null && myCharacter.LocalSkillPlayer.IsPlaying(skillData.SkillId);
		}


		private Vector3 GetSkillPoint(Vector3 hitPoint, SkillData skillData)
		{
			Vector3 skillMyCharacterPos = myCharacter.LocalSkillPlayer.GetSkillMyCharacterPos(skillData);
			Vector3 vector = hitPoint;
			switch (skillData.CastWaysType)
			{
				case SkillCastWaysType.Directional:
				{
					plane.distance = -Vector3.Dot(plane.normal, skillMyCharacterPos);
					Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					float distance;
					if (plane.Raycast(ray, out distance))
					{
						vector = ray.GetPoint(distance);
					}

					break;
				}
				case SkillCastWaysType.PickPoint:
				{
					vector = hitPoint;
					Vector3 vector2;
					if (MoveAgent.CanStandToPosition(hitPoint, 2147483640, out vector2))
					{
						vector = vector2;
					}
					else
					{
						plane.distance = -Vector3.Dot(plane.normal, skillMyCharacterPos);
						Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
						float distance2;
						if (plane.Raycast(ray2, out distance2))
						{
							vector = ray2.GetPoint(distance2);
						}
					}

					break;
				}
				case SkillCastWaysType.PickPointInArea:
				{
					plane.distance = -Vector3.Dot(plane.normal, skillMyCharacterPos);
					Ray ray3 = Camera.main.ScreenPointToRay(Input.mousePosition);
					float distance3;
					if (plane.Raycast(ray3, out distance3))
					{
						vector = ray3.GetPoint(distance3);
					}

					float num = Mathf.Max(skillData.range, 0.1f);
					if (num < GameUtil.DistanceOnPlane(skillMyCharacterPos, vector))
					{
						vector = skillMyCharacterPos + GameUtil.DirectionOnPlane(skillMyCharacterPos, vector) * num;
					}

					Vector3 origin = vector;
					origin.y = 100f;
					RaycastHit raycastHit;
					Vector3 vector3;
					if (Physics.Raycast(new Ray(origin, Vector3.down), out raycastHit, 200f,
						    GameConstants.LayerMask.GROUND_LAYER, QueryTriggerInteraction.Collide) &&
					    MoveAgent.CanStandToPosition(raycastHit.point, 2147483640, out vector3))
					{
						vector = vector3;
					}

					break;
				}
			}

			return vector;
		}


		public bool IsPlaying(SkillSlotSet skillSlotSet)
		{
			SkillData skillData = GetSkillData(skillSlotSet);
			return skillData != null && myCharacter.LocalSkillPlayer.IsPlaying(skillData.SkillId);
		}


		public bool SetSkillIndicator(SkillSlotSet skillSlotSet, SkillData skillData, bool isMainGuildLine,
			float? range = null, float? length = null, float? width = null, float? angle = null)
		{
			if (splatManager == null)
			{
				return false;
			}

			if (skillData == null)
			{
				skillData = GetSkillData(skillSlotSet);
			}

			if (skillData == null)
			{
				return false;
			}

			currentIndicatorSkillData = skillData;
			if (CheckUnprocessedAdditionalInputs(skillSlotSet))
			{
				return false;
			}

			Splat indicator = splatManager.GetIndicator(isMainGuildLine ? skillData.Guideline : skillData.SubGuideline);
			if (indicator == null)
			{
				return false;
			}

			indicator.Range = range ?? skillData.range;
			indicator.Length = length ?? skillData.length;
			indicator.Width = width ?? skillData.width;
			indicator.Angle = angle ?? skillData.angle;
			splatManager.SetIndicator(indicator);
			currentSkillSlotSet = skillSlotSet;
			isFixedIndicator = true;
			return true;
		}


		public void SwitchSkillSet(SkillSlotIndex skillSlotIndex, SkillSlotSet pastSkillSlotSet)
		{
			if (currentSkillSlotSet == null)
			{
				return;
			}

			if (currentSkillSlotSet.Value != pastSkillSlotSet)
			{
				return;
			}

			ChangeCursorStatus onChangeCursorStatus = OnChangeCursorStatus;
			if (onChangeCursorStatus != null)
			{
				onChangeCursorStatus(CursorStatus.Normal);
			}

			ReleaseFixedIndicator(true);
		}


		private bool CheckUnprocessedAdditionalInputs(SkillSlotSet? targetIndicatorSkillSlotSet)
		{
			if (targetIndicatorSkillSlotSet == null)
			{
				return false;
			}

			for (int i = unprocessedAdditionalInputs.Count - 1; i >= 0; i--)
			{
				SkillSlotSet skillSlotSet = unprocessedAdditionalInputs[i].SkillSlotSet;
				SkillSlotSet? skillSlotSet2 = targetIndicatorSkillSlotSet;
				if ((skillSlotSet == skillSlotSet2.GetValueOrDefault()) & (skillSlotSet2 != null))
				{
					return true;
				}
			}

			return false;
		}


		public bool SkillPickingOrderChange(LocalObject prevTarget, LocalObject nextTarget, GameInputEvent inputEvent)
		{
			SkillData skillData;
			if (currentSkillSlotSet != null)
			{
				skillData = GetSkillData(currentSkillSlotSet.Value);
			}
			else
			{
				if (!skillInputEventMap.ContainsKey(inputEvent))
				{
					return prevTarget.GetObjectOrder() > nextTarget.GetObjectOrder();
				}

				SkillSlotIndex skillSlotIndex = GetSkillSlotIndex(inputEvent);
				SkillSlotSet? skillSlotSet = myPlayer.CharacterSkill.GetSkillSlotSet(skillSlotIndex);
				if (skillSlotSet == null)
				{
					return prevTarget.GetObjectOrder() > nextTarget.GetObjectOrder();
				}

				skillData = GetSkillData(skillSlotSet.Value);
			}

			if (skillData == null)
			{
				return prevTarget.GetObjectOrder() > nextTarget.GetObjectOrder();
			}

			return myCharacter.LocalSkillPlayer.CompareObjectOrder(prevTarget, nextTarget, skillData.SkillId);
		}


		public void OnPlaySkillFinish(SkillSlotSet skillSet)
		{
			ClearUnprocessedAdditionalInputs(skillSet);
		}


		public void SetForcePickableEmmaSummons(bool forcePickable)
		{
			if (!Singleton<LocalSetting>.inst.setting.quickCastKeys[GameInputEvent.Active4])
			{
				return;
			}

			List<LocalSummonBase> ownSummons = myCharacter.GetOwnSummons(IsEmmaSummon);
			if (ownSummons != null)
			{
				foreach (LocalSummonBase localSummonBase in ownSummons)
				{
					localSummonBase.Pickable.ForcePickableDisable(!forcePickable);
				}
			}
		}


		private static bool IsEmmaSummon(LocalSummonBase summon)
		{
			return summon.SummonData.code == Singleton<EmmaSkillActive1Data>.inst.PigeonSummonCode ||
			       summon.SummonData.code == Singleton<EmmaSkillActive2Data>.inst.FireworkHatSummonCode;
		}


		public struct SkillInput
		{
			public SkillSlotSet SkillSlotSet { get; }


			public int Seq { get; }


			public bool IsAddinalInput { get; }


			public SkillInput(SkillSlotSet skillSlotSet, int seq, bool isAddinalInput)
			{
				SkillSlotSet = skillSlotSet;
				Seq = seq;
				IsAddinalInput = isAddinalInput;
				validTime = Time.time + Mathf.Min(0.15f + Singleton<GameTime>.inst.Rtt * 2f, 0.5f);
			}


			public bool CheckValidate()
			{
				return validTime >= Time.time;
			}


			private readonly float validTime;
		}


		public struct UnprocessedAdditionalInput
		{
			public SkillSlotSet SkillSlotSet { get; }


			public UnprocessedAdditionalInput(SkillSlotSet skillSlotSet)
			{
				SkillSlotSet = skillSlotSet;
			}
		}


		[Flags]
		private enum UsableSkillCheckList
		{
			Resource = 1,

			InScriptCheck = 2,

			InScriptSlotEnableCheck = 4,

			EquipWeapon = 8,

			SkillLevel = 16,

			Cooldown = 32,

			Rest = 32,

			All = -1
		}
	}
}