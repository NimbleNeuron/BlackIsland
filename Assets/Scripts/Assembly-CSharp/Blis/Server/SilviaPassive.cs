using System;
using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.SilviaPassive)]
	public class SilviaPassive : SkillScript
	{
		
		private readonly WaitForFrameUpdate epWaitFrame = new WaitForFrameUpdate();

		
		private readonly HashSet<int> visitAreaCodeList = new HashSet<int>();

		
		private int bikeSpeedFrameStack;

		
		private int currentAreaMask;

		
		private float epTimeStack;

		
		private float forwardAngle;

		
		private bool isHumanMode;

		
		private float pastCalculateSpeed;

		
		private WorldMovableCharacter wmcCaster;

		
		protected override void Start()
		{
			base.Start();
			EpStart();
			Caster.RemoveStateByGroup(Singleton<SilviaSkillCommonData>.inst.bikeStateGroup, Caster.ObjectId);
			Caster.RemoveStateByGroup(Singleton<SilviaSkillCommonData>.inst.humanSkillStateGroup, Caster.ObjectId);
			Caster.RemoveStateByGroup(Singleton<SilviaSkillCommonData>.inst.humanNonSkillStateGroup, Caster.ObjectId);
			if (!Caster.IsHaveStateByGroup(Singleton<SilviaSkillCommonData>.inst.humanInitStateGroup, Caster.ObjectId))
			{
				AddState(Caster, Singleton<SilviaSkillCommonData>.inst.humanInitStateCode);
			}

			isHumanMode = true;
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnCompleteAddStateEvent = (Action<WorldCharacter, CharacterState>) Delegate.Combine(
				inst.OnCompleteAddStateEvent, new Action<WorldCharacter, CharacterState>(OnCompleteAddState));
			BattleEventCollector inst2 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst2.OnBeforePickupItemEvent = (Action<WorldPlayerCharacter, Item>) Delegate.Combine(
				inst2.OnBeforePickupItemEvent, new Action<WorldPlayerCharacter, Item>(OnBeforePickupItemEvent));
			BattleEventCollector inst3 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst3.OnBeforeActionCastingEvent = (Action<WorldCharacter, ActionCostData>) Delegate.Combine(
				inst3.OnBeforeActionCastingEvent,
				new Action<WorldCharacter, ActionCostData>(OnBeforeActionCastingEvent));
			BattleEventCollector inst4 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst4.OnBeforeInstallSummonEvent = (Action<WorldPlayerCharacter, int>) Delegate.Combine(
				inst4.OnBeforeInstallSummonEvent, new Action<WorldPlayerCharacter, int>(OnBeforeInstallSummonEvent));
			BattleEventCollector inst5 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst5.OnBeforeOpenCorpseEvent = (Action<WorldPlayerCharacter, WorldCharacter>) Delegate.Combine(
				inst5.OnBeforeOpenCorpseEvent, new Action<WorldPlayerCharacter, WorldCharacter>(OnBeforeOpenCorpse));
			BattleEventCollector inst6 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst6.OnCurrentAreaCheckEvent = (Action<WorldPlayerCharacter, int>) Delegate.Combine(
				inst6.OnCurrentAreaCheckEvent, new Action<WorldPlayerCharacter, int>(OnCurrentAreaCheckEvent));
			if (visitAreaCodeList.Count > 0)
			{
				RefrashPassiveState();
			}
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			EpFinish();
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnCompleteAddStateEvent = (Action<WorldCharacter, CharacterState>) Delegate.Remove(
				inst.OnCompleteAddStateEvent, new Action<WorldCharacter, CharacterState>(OnCompleteAddState));
			BattleEventCollector inst2 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst2.OnBeforePickupItemEvent = (Action<WorldPlayerCharacter, Item>) Delegate.Remove(
				inst2.OnBeforePickupItemEvent, new Action<WorldPlayerCharacter, Item>(OnBeforePickupItemEvent));
			BattleEventCollector inst3 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst3.OnBeforeActionCastingEvent = (Action<WorldCharacter, ActionCostData>) Delegate.Remove(
				inst3.OnBeforeActionCastingEvent,
				new Action<WorldCharacter, ActionCostData>(OnBeforeActionCastingEvent));
			BattleEventCollector inst4 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst4.OnBeforeInstallSummonEvent = (Action<WorldPlayerCharacter, int>) Delegate.Remove(
				inst4.OnBeforeInstallSummonEvent, new Action<WorldPlayerCharacter, int>(OnBeforeInstallSummonEvent));
			BattleEventCollector inst5 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst5.OnBeforeOpenCorpseEvent = (Action<WorldPlayerCharacter, WorldCharacter>) Delegate.Remove(
				inst5.OnBeforeOpenCorpseEvent, new Action<WorldPlayerCharacter, WorldCharacter>(OnBeforeOpenCorpse));
			BattleEventCollector inst6 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst6.OnCurrentAreaCheckEvent = (Action<WorldPlayerCharacter, int>) Delegate.Remove(
				inst6.OnCurrentAreaCheckEvent, new Action<WorldPlayerCharacter, int>(OnCurrentAreaCheckEvent));
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			StartCoroutine(EpPlay());
			int frameCount = 0;
			for (;;)
			{
				yield return WaitForFrame();
				if (!isHumanMode)
				{
					float num;
					if (Caster.IsStopped() || !Caster.Character.StateEffector.CanMove())
					{
						GetDiffForwardAngle();
						if (pastCalculateSpeed >= 0f)
						{
							num = 0f;
							bikeSpeedFrameStack = 0;
						}
						else
						{
							bikeSpeedFrameStack++;
							if (bikeSpeedFrameStack >=
							    Singleton<SilviaSkillBikeData>.inst.BikeSpeedCalculateStopCCPeriod)
							{
								bikeSpeedFrameStack = 0;
								num = Mathf.Min(0f,
									pastCalculateSpeed + Singleton<SilviaSkillBikeData>.inst
										.BikeSpeedCalculateStopCCRestoreAmount);
							}
							else
							{
								num = pastCalculateSpeed;
							}
						}
					}
					else
					{
						float num2 = GetDiffForwardAngle();
						num2 -= Singleton<SilviaSkillBikeData>.inst.BikeSpeedCalculateIgnoreAngle;
						if (num2 > 0f)
						{
							bikeSpeedFrameStack = 0;
							num = Mathf.Max(
								pastCalculateSpeed -
								Singleton<SilviaSkillBikeData>.inst.BikeSpeedCalculateAnglePerDecrease * num2,
								Singleton<SilviaSkillBikeData>.inst.BikeSpeedCalculateMin);
						}
						else
						{
							bikeSpeedFrameStack++;
							if (bikeSpeedFrameStack >=
							    Singleton<SilviaSkillBikeData>.inst.BikeSpeedCalculateIncreasePriod)
							{
								bikeSpeedFrameStack = 0;
								if (pastCalculateSpeed >= 0f)
								{
									num = Mathf.Min(
										pastCalculateSpeed + Singleton<SilviaSkillBikeData>.inst
											.BikeSpeedCalculateIncreaseAmount,
										Singleton<SilviaSkillBikeData>.inst.BikeSpeedCalculateMax);
								}
								else
								{
									num = Mathf.Min(
										pastCalculateSpeed + Singleton<SilviaSkillBikeData>.inst
											.BikeSpeedCalculateRestoreAmount,
										Singleton<SilviaSkillBikeData>.inst.BikeSpeedCalculateMax);
								}
							}
							else
							{
								num = pastCalculateSpeed;
							}
						}
					}

					if (num != pastCalculateSpeed)
					{
						int num3 = frameCount;
						frameCount = num3 + 1;
						if (frameCount >= 3 || Mathf.Abs(num - pastCalculateSpeed) > 0.4f)
						{
							Caster.SetExternalNonCalculateStatValue(
								Singleton<SilviaSkillBikeData>.inst.BikeSpeedCalculateStateGroup, Caster.ObjectId, num);
							frameCount = 0;
						}

						pastCalculateSpeed = num;
					}
				}
			}
		}

		
		private void OnCompleteAddState(WorldCharacter target, CharacterState characterState)
		{
			if (!Caster.IsAlive)
			{
				return;
			}

			if (Caster.ObjectId != target.ObjectId)
			{
				return;
			}

			if (characterState == null || characterState.Caster == null)
			{
				return;
			}

			if (Caster.ObjectId != characterState.Caster.ObjectId)
			{
				return;
			}

			if (characterState.Code == Singleton<SilviaSkillCommonData>.inst.humanInitStateCode ||
			    characterState.Code == Singleton<SilviaSkillCommonData>.inst.humanSkillStateCode ||
			    characterState.Code == Singleton<SilviaSkillCommonData>.inst.humanNonSkillStateCode)
			{
				isHumanMode = true;
				epTimeStack = 0f;
				return;
			}

			if (characterState.Code == Singleton<SilviaSkillCommonData>.inst.bikeStateCode)
			{
				isHumanMode = false;
				epTimeStack = 0f;
				bikeSpeedFrameStack = 0;
				pastCalculateSpeed = 0f;
				GetDiffForwardAngle();
			}
		}

		
		private void OnBeforePickupItemEvent(WorldPlayerCharacter playerCharacter, Item item)
		{
			if (isHumanMode)
			{
				return;
			}

			if (playerCharacter == null)
			{
				return;
			}

			if (playerCharacter.ObjectId != Caster.ObjectId)
			{
				return;
			}

			if (!Caster.IsAlive)
			{
				return;
			}

			ChangeToHuman();
		}

		
		private void OnBeforeActionCastingEvent(WorldCharacter actionCaster, ActionCostData actionCostData)
		{
			if (isHumanMode)
			{
				return;
			}

			if (actionCaster == null)
			{
				return;
			}

			if (actionCaster.ObjectId != Caster.ObjectId)
			{
				return;
			}

			if (!Caster.IsAlive)
			{
				return;
			}

			ChangeToHuman();
		}

		
		private void OnBeforeInstallSummonEvent(WorldCharacter owner, int summonCode)
		{
			if (isHumanMode)
			{
				return;
			}

			if (owner == null)
			{
				return;
			}

			if (Caster.ObjectId != owner.ObjectId)
			{
				return;
			}

			if (!Caster.IsAlive)
			{
				return;
			}

			ChangeToHuman();
		}

		
		private void OnBeforeOpenCorpse(WorldPlayerCharacter opener, WorldCharacter corpse)
		{
			if (isHumanMode)
			{
				return;
			}

			if (opener == null)
			{
				return;
			}

			if (opener.ObjectId != Caster.ObjectId)
			{
				return;
			}

			if (!Caster.IsAlive)
			{
				return;
			}

			ChangeToHuman();
		}

		
		private void ChangeToHuman()
		{
			float cooldown = GameDB.skill.GetSkillData(Caster.Character.CharacterCode, Caster.ObjectType,
				SkillSlotSet.Active4_1, Caster.GetSkillLevel(SkillSlotIndex.Active4), 0).cooldown;
			wmcCaster.StartSkillCooldown(SkillSlotSet.Active4_1, MasteryType.None, cooldown);
			wmcCaster.StartSkillCooldown(SkillSlotSet.Active4_2, MasteryType.None, cooldown);
			Caster.RemoveStateByGroup(Singleton<SilviaSkillCommonData>.inst.humanInitStateGroup, Caster.ObjectId);
			Caster.RemoveStateByGroup(Singleton<SilviaSkillCommonData>.inst.humanSkillStateGroup, Caster.ObjectId);
			Caster.RemoveStateByGroup(Singleton<SilviaSkillCommonData>.inst.bikeStateGroup, Caster.ObjectId);
			if (!Caster.IsHaveStateByGroup(Singleton<SilviaSkillCommonData>.inst.humanNonSkillStateGroup,
				Caster.ObjectId))
			{
				AddState(Caster, Singleton<SilviaSkillCommonData>.inst.humanNonSkillStateCode);
			}
		}

		
		private void OnCurrentAreaCheckEvent(WorldPlayerCharacter character, int areaMask)
		{
			if (!Caster.IsAlive)
			{
				return;
			}

			if (Caster.ObjectId != character.ObjectId)
			{
				return;
			}

			if (currentAreaMask == 0)
			{
				currentAreaMask = areaMask;
				if (visitAreaCodeList.Count <= 0)
				{
					FirstVisitedAreaEvent(character, areaMask);
				}

				return;
			}

			if (currentAreaMask == areaMask)
			{
				return;
			}

			currentAreaMask = areaMask;
			if (IsReadySkill(Caster, SkillSlotSet.Passive_1) &&
			    Caster.Owner.Status.ExtraPoint < Caster.Owner.Stat.MaxExtraPoint)
			{
				ExtraPointModifyTo(Caster, Singleton<SilviaSkillCommonData>.inst.PassiveEpAmount[SkillLevel]);
				PlayPassiveSkill(info);
				PlaySkillAction(Caster, SkillId.SilviaPassive, 2);
			}

			if (!visitAreaCodeList.Contains(areaMask))
			{
				FirstVisitedAreaEvent(character, areaMask);
			}
		}

		
		private void FirstVisitedAreaEvent(WorldPlayerCharacter character, int areaMask)
		{
			if (areaMask == Singleton<SilviaSkillCommonData>.inst.PassiveSkillNotApplyArea)
			{
				return;
			}

			visitAreaCodeList.Add(areaMask);
			CharacterStateData data =
				GameDB.characterState.GetData(
					Singleton<SilviaSkillCommonData>.inst.PassiveAttackSpeedBuffStateCode[SkillLevel]);
			CharacterState characterState = Caster.FindStateByGroup(data.group, Caster.ObjectId);
			if (characterState == null || characterState.StackCount < data.maxStack)
			{
				AddState(Caster, data.code, 1);
			}

			if (characterState != null && characterState.StackCount >= data.maxStack)
			{
				AddState(Caster, Singleton<SilviaSkillCommonData>.inst.PassiveSkillDamageRatioStateCode);
				PlaySkillAction(Caster, SkillId.SilviaPassive, 3);
				return;
			}

			PlaySkillAction(Caster, SkillId.SilviaPassive, 1);
		}

		
		public override void OnUpgradePassiveSkill()
		{
			base.OnUpgradePassiveSkill();
			RefrashPassiveState();
		}

		
		private void RefrashPassiveState()
		{
			CharacterStateData data =
				GameDB.characterState.GetData(
					Singleton<SilviaSkillCommonData>.inst.PassiveAttackSpeedBuffStateCode[SkillLevel]);
			Caster.RemoveStateByGroup(data.group, Caster.ObjectId);
			AddState(Caster, data.code, visitAreaCodeList.Count);
		}

		
		private float GetDiffForwardAngle()
		{
			float directionAngle = GameUtil.GetDirectionAngle(Caster.Forward, Vector3.forward);
			float num = Mathf.Abs(forwardAngle - directionAngle);
			forwardAngle = directionAngle;
			if (num > 180f)
			{
				num = 360f - num;
			}

			return num;
		}

		
		private void EpStart()
		{
			epTimeStack = 0f;
			if (wmcCaster == null)
			{
				wmcCaster = Caster.Character as WorldMovableCharacter;
			}
		}

		
		private void EpFinish() { }

		
		private IEnumerator EpPlay()
		{
			for (;;)
			{
				epTimeStack += MonoBehaviourInstance<GameService>.inst.ServerFrameDeltaTime;
				if (isHumanMode)
				{
					if (epTimeStack >= Singleton<SilviaSkillHumanData>.inst.FillEpPeriod)
					{
						epTimeStack -= Singleton<SilviaSkillHumanData>.inst.FillEpPeriod;
						ExtraPointModifyTo(Caster, Singleton<SilviaSkillHumanData>.inst.FillEpAmount);
					}
				}
				else
				{
					if (epTimeStack >= Singleton<SilviaSkillBikeData>.inst.ConsumeEpPeriod)
					{
						epTimeStack -= Singleton<SilviaSkillBikeData>.inst.ConsumeEpPeriod;
						ExtraPointModifyTo(Caster, -Singleton<SilviaSkillBikeData>.inst.ConsumeEpAmount);
					}

					if (Caster.Status.ExtraPoint == 0)
					{
						while (Caster.IsPlayingScript(SkillId.SilviaBikeActive2) ||
						       Caster.IsPlayingScript(SkillId.SilviaBikeActive3_1))
						{
							yield return epWaitFrame.Frame(1);
						}

						float cooldown = GameDB.skill.GetSkillData(Caster.Character.CharacterCode, Caster.ObjectType,
							SkillSlotSet.Active4_1, Caster.GetSkillLevel(SkillSlotIndex.Active4), 0).cooldown;
						wmcCaster.StartSkillCooldown(SkillSlotSet.Active4_1, MasteryType.None, cooldown);
						wmcCaster.StartSkillCooldown(SkillSlotSet.Active4_2, MasteryType.None, cooldown);
						Caster.RemoveStateByGroup(Singleton<SilviaSkillCommonData>.inst.humanInitStateGroup,
							Caster.ObjectId);
						Caster.RemoveStateByGroup(Singleton<SilviaSkillCommonData>.inst.humanSkillStateGroup,
							Caster.ObjectId);
						Caster.RemoveStateByGroup(Singleton<SilviaSkillCommonData>.inst.bikeStateGroup,
							Caster.ObjectId);
						if (!Caster.IsHaveStateByGroup(Singleton<SilviaSkillCommonData>.inst.humanNonSkillStateGroup,
							Caster.ObjectId))
						{
							AddState(Caster, Singleton<SilviaSkillCommonData>.inst.humanNonSkillStateCode);
						}
					}
				}

				yield return epWaitFrame.Frame(1);
			}
		}
	}
}