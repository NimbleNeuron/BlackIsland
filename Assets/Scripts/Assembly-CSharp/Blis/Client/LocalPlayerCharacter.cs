using System;
using System.Collections;
using System.Collections.Generic;
using BIFog;
using Blis.Client.UI;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	[ObjectAttr(ObjectType.PlayerCharacter)]
	public class LocalPlayerCharacter : LocalMovableCharacter
	{
		private readonly List<LocalSummonBase> ownSummons = new List<LocalSummonBase>();
		private int characterCode;

		private CharacterVoiceControl characterVoiceControl;
		private ClientCharacter clientCharacter;
		private float combatUntil;
		private Equipment equipment;
		private Coroutine finishCasting;
		private PlayerHostileAgent hostileAgent;
		private bool isDyingCondition;
		private bool isFirstMove;
		private bool isGunReloading;
		private bool isRest;
		private Dictionary<MasteryType, int> masterysLevel =
			new Dictionary<MasteryType, int>(SingletonComparerEnum<MasteryTypeComparer, MasteryType>.Instance);
		private MyPlayerContext myPlayer;
		private PlayerContext playerContext;
		private int rank = -1;
		private int reloadItemId;
		private Coroutine runningReload;
		private int skinIndex;
		private float survivalTime;
		private int teamNumber;
		private int teamSlot;
		private HashSet<int> visibleSummonIds = new HashSet<int>();
		private WeaponMountController weaponMountController;
		private int whoKilledMe;
		public bool IsFirstMove => isFirstMove;
		public int Rank => rank;
		public ClientCharacter ClientCharacter => clientCharacter;


		public PlayerContext PlayerContext => playerContext;


		public WeaponMountController WeaponMount => weaponMountController;


		public CharacterVoiceControl CharacterVoiceControl => characterVoiceControl;


		public Dictionary<MasteryType, int> MasterysLevel => masterysLevel;


		public int TeamSlot => teamSlot;


		public int WhoKilledMe => whoKilledMe;


		public Equipment Equipment => equipment;


		public bool IsRest => isRest;


		public float SurvivableTime => survivalTime;


		public bool IsGunReloading => isGunReloading;


		public bool IsDyingCondition => isDyingCondition;


		public bool IsObserving {
			get
			{
				MyPlayerContext myPlayerContext = myPlayer;
				if (myPlayerContext == null)
				{
					PlayerContext playerContext = this.playerContext;
					return playerContext != null && playerContext.IsObserving;
				}

				return myPlayerContext.IsObserving;
			}
		}


		public bool IsDisconnected {
			get
			{
				MyPlayerContext myPlayerContext = myPlayer;
				if (myPlayerContext == null)
				{
					PlayerContext playerContext = this.playerContext;
					return playerContext != null && playerContext.IsDisconnected;
				}

				return myPlayerContext.IsDisconnected;
			}
		}

		protected override ObjectType GetObjectType()
		{
			return ObjectType.PlayerCharacter;
		}


		protected override int GetTeamNumber()
		{
			return teamNumber;
		}


		protected override int GetCharacterCode()
		{
			return characterCode;
		}


		protected override int GetSkinIndex()
		{
			return skinIndex;
		}


		protected override string GetNickname()
		{
			PlayerContext playerContext = this.playerContext;
			if (playerContext == null)
			{
				return null;
			}

			return playerContext.nickname;
		}


		protected override HostileAgent GetHostileAgent()
		{
			return hostileAgent;
		}


		protected override bool GetIsOutSight()
		{
			return !(SightAgent != null) || SightAgent.IsOutSight;
		}


		protected override GameObject GetCharacterPrefab()
		{
			return SingletonMonoBehaviour<ResourceManager>.inst.LoadPlayerCharacter(characterCode, skinIndex);
		}


		public override void Init(byte[] snapshotData)
		{
			PlayerCharacterSnapshot playerCharacterSnapshot =
				serializer.Deserialize<PlayerCharacterSnapshot>(snapshotData);
			equipment = new Equipment();
			teamNumber = playerCharacterSnapshot.teamNumber;
			whoKilledMe = playerCharacterSnapshot.whoKilledMe;
			characterCode = playerCharacterSnapshot.characterCode;
			CharacterSkinData skinData = GameDB.character.GetSkinData(playerCharacterSnapshot.skinCode);
			skinIndex = skinData != null ? skinData.index : 0;
			masterysLevel = playerCharacterSnapshot.masteryLevels;
			isDyingCondition = playerCharacterSnapshot.isDyingCondition;
			isRest = playerCharacterSnapshot.isRest;
			survivalTime = 30f;
			base.Init(snapshotData);
			if (isDyingCondition)
			{
				isAlive = true;
			}

			GameUtil.BindOrAdd<WeaponMountController>(gameObject, ref weaponMountController);
			weaponMountController.Init(characterCode, skinIndex, characterAnimator);
			bool? characterAnimatorBool = GetCharacterAnimatorBool(AnimID_Downed);
			bool flag = false;
			bool flag2 = (characterAnimatorBool.GetValueOrDefault() == flag) & (characterAnimatorBool != null) &&
			             isDyingCondition;
			CharacterFloatingUI floatingUi = this.floatingUi;
			if (floatingUi != null)
			{
				floatingUi.SetDyingCondition(isDyingCondition);
			}

			SetAnimation(AnimID_Downed, isDyingCondition);
			if (flag2)
			{
				SetCharacterAnimatorTrigger(AnimID_DownedTrigger);
			}

			MonoBehaviourInstance<GameUI>.inst.HudUpdateDyingCondition(ObjectId, isDyingCondition);
			if (isDyingCondition)
			{
				UpdateMapIconDyingCondition();
			}
			else
			{
				UpdateMapIconRevival();
			}

			if (!isAlive)
			{
				SetAnimation(AnimID_DownToDead, false);
				PlayDeadAnimation();
			}

			if (isRest)
			{
				ActionCostData actionCost = GameDB.character.GetActionCost(CastingActionType.ToRest);
				SetCharacterAnimatorTrigger(actionCost.castingAnimTrigger);
			}

			characterVoiceControl = new CharacterVoiceControl(this);
			clientCharacter = characterObject.GetComponentInChildren<ClientCharacter>();
			clientCharacter.Init(this, objectId);
			clientCharacter.AddOnEffectCreatedEvent(OnEffectCreated);
			clientCharacter.SetStealth(IsInvisible);
			clientCharacter.SetInSight(!IsInvisible);
			hostileAgent = new PlayerHostileAgent(this);
			SetIsInCombat(playerCharacterSnapshot.isInCombat);
		}


		protected override void OnBeforeStartSkill(SkillData skillData)
		{
			base.OnBeforeStartSkill(skillData);
			if (skillData != null && !string.IsNullOrEmpty(skillData.Guideline))
			{
				MyPlayerContext myPlayerContext = myPlayer;
				if (myPlayerContext == null)
				{
					return;
				}

				myPlayerContext.StartSkillIndicatorHide(skillData.Guideline);
			}
		}


		public override void DestroySelf() { }


		protected override void InitStatus(byte[] snapshot)
		{
			PlayerStatusSnapshot snapshot2 = serializer.Deserialize<PlayerStatusSnapshot>(snapshot);
			Status = new CharacterStatus(snapshot2);
		}


		protected override void UpdateCharacterAnimator()
		{
			base.UpdateCharacterAnimator();
			if (weaponMountController != null)
			{
				weaponMountController.UpdateAnimator();
			}
		}


		public void SetMyPlayerContext(MyPlayerContext myPlayerContext)
		{
			myPlayer = myPlayerContext;
			playerContext = myPlayer;
			sightAgent.SetSightQuality(SightQuality.High);
			sightAgent.EnableFogSight();
		}


		public void SetRank(int rank)
		{
			this.rank = rank;
		}


		public void SetTeamSlot(int teamSlot)
		{
			this.teamSlot = teamSlot;
		}


		public void FirstMove()
		{
			isFirstMove = true;
		}


		public void SetIsRest(bool rest)
		{
			isRest = rest;
			if (myPlayer != null && MonoBehaviourInstance<GameClient>.inst.IsTutorial)
			{
				MonoBehaviourInstance<TutorialController>.inst.SuccessRestingTutorial();
			}
		}


		public void SetWhoKilledMe(int whoKilledMe)
		{
			this.whoKilledMe = whoKilledMe;
		}


		protected override void OnStartMove()
		{
			MyPlayerContext myPlayerContext = myPlayer;
			if (myPlayerContext == null)
			{
				return;
			}

			myPlayerContext.OnStartMove();
		}


		/// <summary>
		/// 실제 서버로부터 받아와서 움직임 명령을 실행하는 곳
		/// </summary>
		/// <param name="startPosition"></param>
		/// <param name="destination"></param>
		/// <param name="corners"></param>
		public override void MoveToDestination(BlisVector startPosition, BlisVector destination, BlisVector[] corners)
		{
			base.MoveToDestination(startPosition, destination, corners);
			MyPlayerContext myPlayerContext = myPlayer;
			if (myPlayerContext == null)
			{
				return;
			}

			myPlayerContext.OnUpdatePath((moveAgent.CurrentMoveStrategy as MoveToDestination).GetCorners());
		}


		public override void StopMove()
		{
			base.StopMove();
			MyPlayerContext myPlayerContext = myPlayer;
			if (myPlayerContext != null)
			{
				myPlayerContext.OnUpdatePath(new List<Vector3>());
			}

			MyPlayerContext myPlayerContext2 = myPlayer;
			if (myPlayerContext2 == null)
			{
				return;
			}

			myPlayerContext2.OnStop();
		}


		public bool IsEquippedWeapon()
		{
			return equipment.GetWeapon() != null;
		}


		public bool IsEquipableWeapon(ItemData itemData)
		{
			return myPlayer != null && myPlayer.IsEquipableWeapon(itemData);
		}


		public override void OnDead(LocalCharacter attacker)
		{
			SetAnimation(AnimID_Downed, false);
			base.OnDead(attacker);
			ClientService inst = MonoBehaviourInstance<ClientService>.inst;
			if (inst == null)
			{
				return;
			}

			GameUI inst2 = MonoBehaviourInstance<GameUI>.inst;
			if (inst2 == null)
			{
				return;
			}

			inst2.BattleInfoHud.SetAliveCount(MonoBehaviourInstance<ClientService>.inst.GetAliveCount());
			UISystem.Action(new UpdatePlayerInfo(this));
			CharacterFloatingUI floatingUi = this.floatingUi;
			if (floatingUi != null)
			{
				floatingUi.HideSurvivableTimer();
			}

			inst2.HudUpdateDead(ObjectId);
			if (characterRenderer != null && characterRenderer.IsRendering)
			{
				if (inst.IsAlly(this))
				{
					ShowMapIcon(MiniMapIconType.Sight);
				}

				ShowMiniMapIcon(MiniMapIconType.Sight);
			}

			if (myPlayer != null)
			{
				myPlayer.OnDead();
				myPlayer.OnRemoveStateOnDead();
				EndPlayingSequenceSkills();
			}

			if (!inst.IsPlayer)
			{
				inst.MyObserver.OnDead();
			}
		}


		public override void SetAnimatorCullingMode(AnimatorCullingMode cullingMode)
		{
			base.SetAnimatorCullingMode(cullingMode);
			weaponMountController.SetAnimatorCullingMode(cullingMode);
		}


		protected override void PlayDeadAnimation()
		{
			SetAnimation(isDyingCondition ? AnimID_DownToDead : AnimID_Dead, true);
		}


		public override void ShowMiniMapIcon(MiniMapIconType miniMapIconType)
		{
			ClientService inst = MonoBehaviourInstance<ClientService>.inst;
			if (inst == null)
			{
				return;
			}

			GameUI inst2 = MonoBehaviourInstance<GameUI>.inst;
			if (inst2 == null)
			{
				return;
			}

			if (isAlive)
			{
				bool isAlly = inst.IsAlly(this);
				inst2.Minimap.UIMap.CreatePlayer(ObjectId, isAlly, GetPosition(), GetTeamColor(), GetMapIconBg(),
					GetMapIcon(), null, miniMapIconType, TeamNumber);
				return;
			}

			inst2.Minimap.UIMap.CreateDeadPlayer(ObjectId, GetPosition(), GetMapIcon());
		}


		public override void HideMiniMapIcon(MiniMapIconType miniMapIconType)
		{
			GameUI inst = MonoBehaviourInstance<GameUI>.inst;
			if (inst == null)
			{
				return;
			}

			inst.Minimap.UIMap.RemovePlayer(ObjectId, miniMapIconType);
			inst.Minimap.UIMap.RemoveNonPlayer(ObjectId);
		}


		public override void ShowMapIcon(MiniMapIconType miniMapIconType)
		{
			ClientService inst = MonoBehaviourInstance<ClientService>.inst;
			if (inst == null)
			{
				return;
			}

			GameUI inst2 = MonoBehaviourInstance<GameUI>.inst;
			if (inst2 == null)
			{
				return;
			}

			if (isAlive)
			{
				bool isAlly = inst.IsAlly(this);
				inst2.CombineWindow.UIMap.CreatePlayer(ObjectId, isAlly, GetPosition(), GetTeamColor(), GetMapIconBg(),
					GetMapIcon(), Nickname, miniMapIconType);
				inst2.HyperloopWindow.UIMap.CreatePlayer(ObjectId, isAlly, GetPosition(), GetTeamColor(),
					GetMapIconBg(), GetMapIcon(), Nickname, miniMapIconType);
				inst2.MapWindow.UIMap.CreatePlayer(ObjectId, isAlly, GetPosition(), GetTeamColor(), GetMapIconBg(),
					GetMapIcon(), Nickname, miniMapIconType, TeamNumber);
				return;
			}

			inst2.CombineWindow.UIMap.CreateDeadPlayer(ObjectId, GetPosition(), GetMapIcon());
			inst2.HyperloopWindow.UIMap.CreateDeadPlayer(ObjectId, GetPosition(), GetMapIcon());
			inst2.MapWindow.UIMap.CreateDeadPlayer(ObjectId, GetPosition(), GetMapIcon());
		}


		public override void HideMapIcon(MiniMapIconType miniMapIconType)
		{
			GameUI inst = MonoBehaviourInstance<GameUI>.inst;
			if (inst == null)
			{
				return;
			}

			inst.CombineWindow.UIMap.RemovePlayer(ObjectId, miniMapIconType);
			inst.CombineWindow.UIMap.RemoveNonPlayer(ObjectId);
			inst.HyperloopWindow.UIMap.RemovePlayer(ObjectId, miniMapIconType);
			inst.HyperloopWindow.UIMap.RemoveNonPlayer(ObjectId);
			inst.MapWindow.UIMap.RemovePlayer(ObjectId, miniMapIconType);
			inst.MapWindow.UIMap.RemoveNonPlayer(ObjectId);
		}


		public override void UpdateMapIcon()
		{
			GameUI inst = MonoBehaviourInstance<GameUI>.inst;
			if (inst == null)
			{
				return;
			}

			inst.MapWindow.UIMap.UpdatePlayerIcon(ObjectId, GetTeamColor(), GetMapIconBg(), GetMapIcon());
			inst.Minimap.UIMap.UpdatePlayerIcon(ObjectId, GetTeamColor(), GetMapIconBg(), GetMapIcon());
		}


		protected override void UpdateMapIconPos()
		{
			ClientService inst = MonoBehaviourInstance<ClientService>.inst;
			if (inst == null)
			{
				return;
			}

			GameUI inst2 = MonoBehaviourInstance<GameUI>.inst;
			if (inst2 == null)
			{
				return;
			}

			bool flag = !inst.IsPlayer;
			if (!flag)
			{
				flag = characterRenderer != null && characterRenderer.IsRendering;
			}

			if (!flag)
			{
				return;
			}

			bool isAlly = inst.IsAlly(this);
			inst2.CombineWindow.UpdateMapPlayerPosition(ObjectId, isAlly, GetPosition());
			inst2.MapWindow.UpdateMapPlayerPosition(ObjectId, isAlly, GetPosition());
			inst2.Minimap.UIMap.UpdatePlayerPosition(ObjectId, isAlly, GetPosition());
			inst2.HyperloopWindow.UpdateMapPlayerPosition(ObjectId, isAlly, GetPosition());
		}


		private bool IsMatched(WeaponType weaponType, string layerName)
		{
			return weaponType != WeaponType.None && weaponType.ToString().Equals(layerName);
		}


		public void SetCharacterAnimatorLayerByName(string layerName, float targetWeight, float deltaTime)
		{
			int characterAnimatorLayerIndex = GetCharacterAnimatorLayerIndex(layerName);
			if (0 <= characterAnimatorLayerIndex && characterAnimatorLayerIndex < GetCharacterAnimatorLayerCount())
			{
				float num = GetCharacterAnimatorLayerWeight(characterAnimatorLayerIndex);
				if (num < targetWeight)
				{
					num += deltaTime;
					if (targetWeight < num)
					{
						num = targetWeight;
					}
				}
				else
				{
					if (targetWeight >= num)
					{
						return;
					}

					num -= deltaTime;
					if (num < targetWeight)
					{
						num = targetWeight;
					}
				}

				SetCharacterAnimatorLayerWeight(characterAnimatorLayerIndex, num);
			}
		}


		private bool StartAcionCastingPlaySoundType(CastingActionType type)
		{
			return type == CastingActionType.CollectibleOpenWater || type == CastingActionType.CollectibleOpenWood ||
			       type == CastingActionType.CollectibleOpenStone || type == CastingActionType.CollectibleOpenSeaFish ||
			       type == CastingActionType.CollectibleOpenFreshWaterFish ||
			       type == CastingActionType.CollectibleOpenPotato || type == CastingActionType.Hyperloop;
		}


		public void OnStartActionCasting(CastingActionType type, float castTime, int extraParam)
		{
			ActionCostData actionCost = GameDB.character.GetActionCost(type);
			if (StartAcionCastingPlaySoundType(type))
			{
				CharacterVoiceType charVoiceType = CharacterVoiceUtil.CastingActionTypeConvertToCharVoiceType(type);
				CharacterVoiceControl.PlayCharacterVoice(charVoiceType, 15, GetPosition());
				if (actionCost.time <= 0f)
				{
					return;
				}
			}

			string text;
			if (type.IsCraftAction())
			{
				ItemData itemData = GameDB.item.FindItemByCode(extraParam);
				text = itemData.craftAnimTrigger;
				if (myPlayer != null)
				{
					MonoBehaviourInstance<GameUI>.inst.ShortCutCraftHud.PlayCraftItemWithSourceItemFrame(itemData);
				}
			}
			else
			{
				text = actionCost.castingAnimTrigger;
			}

			if (!string.IsNullOrEmpty(text))
			{
				SetCharacterAnimatorBool("bCastingAction", true);
				SetCharacterAnimatorTrigger(text);
				if (weaponMountController)
				{
					weaponMountController.SetWeaponAnimatorTrigger(text);
				}
			}

			finishCasting = this.StartThrowingCoroutine(OnFinishCasting(castTime, type),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][FinishCasting] Message:" + exception.Message + ", StackTrace:" +
					      exception.StackTrace);
				});
			if (myPlayer != null)
			{
				myPlayer.OnStartActionCasting(type, castTime, extraParam);
				MonoBehaviourInstance<ClientService>.inst.OpenAirSupplyItemBox(extraParam);
			}
		}


		private IEnumerator OnFinishCasting(float time, CastingActionType type)
		{
			yield return new WaitForSeconds(time);
			SetCharacterAnimatorTrigger(GameConstants.AnimationKey.ANIMATION_CANCEL_TRIGGER, false);
			SetCharacterAnimatorBool("bCastingAction", false);
			if (weaponMountController)
			{
				weaponMountController.SetWeaponAnimatorTrigger(GameConstants.AnimationKey.ANIMATION_CANCEL_TRIGGER,
					false);
			}

			if (!StartAcionCastingPlaySoundType(type))
			{
				CharacterVoiceType charVoiceType = CharacterVoiceUtil.CastingActionTypeConvertToCharVoiceType(type);
				CharacterVoiceControl.PlayCharacterVoice(charVoiceType, 15, GetPosition());
			}

			if (myPlayer != null && type >= CastingActionType.CraftCommon && type <= CastingActionType.CraftLegend &&
			    MonoBehaviourInstance<ClientService>.inst.isCrafting)
			{
				foreach (EquipItemSlot equipItemSlot in MonoBehaviourInstance<GameUI>.inst.StatusHud.EquipSlots)
				{
					equipItemSlot.StopFocusFrame();
					equipItemSlot.StopSourceItemFrame();
				}

				foreach (InvenItemSlot invenItemSlot in MonoBehaviourInstance<GameUI>.inst.InventoryHud.InvenSlots)
				{
					invenItemSlot.StopFocusFrame();
					invenItemSlot.StopSourceItemFrame();
				}

				MonoBehaviourInstance<ClientService>.inst.isCrafting = false;
			}
		}


		public void OnCancelActionCasting(int extraParam)
		{
			SetCharacterAnimatorTrigger(GameConstants.AnimationKey.ANIMATION_CANCEL_TRIGGER);
			SetCharacterAnimatorBool("bCastingAction", false);
			if (weaponMountController)
			{
				weaponMountController.SetWeaponAnimatorTrigger(GameConstants.AnimationKey.ANIMATION_CANCEL_TRIGGER);
			}

			if (finishCasting != null)
			{
				StopCoroutine(finishCasting);
				finishCasting = null;
			}

			if (myPlayer != null)
			{
				myPlayer.OnCancelActionCasting();
				MonoBehaviourInstance<ClientService>.inst.CancelOpenAirSupplyItemBox(extraParam);
			}
		}


		public override void OnStopNormalAttack()
		{
			base.OnStopNormalAttack();
			WeaponMount.StopAnimation();
		}


		public override void OnCrowdControl(StateType stateType)
		{
			base.OnCrowdControl(stateType);
			if (stateType.CanControl())
			{
				return;
			}

			WeaponMount.StopAnimation();
		}


		public override void SetAnimation(int id)
		{
			base.SetAnimation(id);
			WeaponMount.SetAnimation(id);
		}


		public override void SetAnimation(int id, float value)
		{
			base.SetAnimation(id, value);
			WeaponMount.SetAnimation(id, value);
		}


		public override void SetAnimation(int id, bool value)
		{
			base.SetAnimation(id, value);
			WeaponMount.SetAnimation(id, value);
		}


		public override void SetAnimation(int id, int value)
		{
			base.SetAnimation(id, value);
			WeaponMount.SetAnimation(id, value);
		}


		public void SetWeaponMountAnimation(WeaponMountType type, int id)
		{
			WeaponMount.SetAnimation(type, id);
		}


		public void SetWeaponMountAnimation(WeaponMountType type, int id, float value)
		{
			WeaponMount.SetAnimation(type, id, value);
		}


		public void SetWeaponMountAnimation(WeaponMountType type, int id, bool value)
		{
			WeaponMount.SetAnimation(type, id, value);
		}


		public void SetWeaponMountAnimation(WeaponMountType type, int id, int value)
		{
			WeaponMount.SetAnimation(type, id, value);
		}


		public void ActiveWeaponObject(WeaponMountType mountType, bool isActive)
		{
			WeaponMount.ActiveWeaponObject(mountType, isActive);
		}


		private void OnLevelUp()
		{
			PlayLocalEffectChild("FX_BI_Character_LevelUp", null);
			PlayLocalSoundPoint("effect_levelup", "EffectSound", 20, transform.position);
		}


		public void OnUpdateSurvivableTime(float survivalTime)
		{
			this.survivalTime = survivalTime;
			floatingUi.UpdateSurvivalTimer(survivalTime);
			MyPlayerContext myPlayerContext = myPlayer;
			if (myPlayerContext == null)
			{
				return;
			}

			myPlayerContext.OnUpdateSurvivableTime(survivalTime);
		}


		protected override void UpdateInternal()
		{
			base.UpdateInternal();
			ClientService inst = MonoBehaviourInstance<ClientService>.inst;
			if (inst == null)
			{
				return;
			}

			if (IsAlive)
			{
				if ((GetCurrentAreaMask() & inst.RestrictedAreaMask) > 0)
				{
					if (MonoBehaviourInstance<ClientService>.inst.IsInFinalSafeZone(this, null))
					{
						CharacterFloatingUI floatingUi = this.floatingUi;
						if (floatingUi == null)
						{
							return;
						}

						floatingUi.HideSurvivableTimer();
					}
					else
					{
						CharacterFloatingUI floatingUi2 = floatingUi;
						if (floatingUi2 == null)
						{
							return;
						}

						floatingUi2.ShowSurvivableTimer();
					}
				}
				else
				{
					CharacterFloatingUI floatingUi3 = floatingUi;
					if (floatingUi3 == null)
					{
						return;
					}

					floatingUi3.HideSurvivableTimer();
				}
			}
		}


		public void OnStartGunReload(bool playReloadAnimation)
		{
			isGunReloading = true;
			if (!playReloadAnimation)
			{
				return;
			}

			bool? characterAnimatorBool = GetCharacterAnimatorBool(BoolCastingAction);
			bool flag = true;
			if ((characterAnimatorBool.GetValueOrDefault() == flag) & (characterAnimatorBool != null))
			{
				return;
			}

			SetAnimation(TriggerReload);
		}


		public void OnFinishGunReload(bool cancelReload)
		{
			isGunReloading = false;
			int num = 0;
			if (!cancelReload)
			{
				num = GameDB.item.GetBulletCapacity(GetWeapon().itemCode);
			}

			Status.UpdateBullet(num);
			floatingUi.UpdateBullet(num);
			if (myPlayer != null && MonoBehaviourInstance<GameClient>.inst.IsTutorial)
			{
				MonoBehaviourInstance<TutorialController>.inst.SuccessReloadTutorial();
			}
		}


		public void PlayStartSkillVoice(int skillCode, SkillId skillId)
		{
			SkillSlotSet? skillSlotSet = GameDB.skill.FindSkillSlotSet(CharacterCode, ObjectType, skillCode);
			if (skillSlotSet == null)
			{
				return;
			}

			SkillSlotIndex skillSlotIndex = skillSlotSet.Value.SlotSet2Index();
			SkillData skillData = GameDB.skill.GetSkillData(skillCode);
			bool flag = skillData.SkillId == skillId;
			CharacterVoiceType charVoiceType;
			string customSoundName;
			if (skillSlotIndex >= SkillSlotIndex.Active1 && skillSlotIndex <= SkillSlotIndex.Active4)
			{
				if (!flag)
				{
					return;
				}

				int skillSequence = GameDB.skill.GetSkillSequence(CharacterCode, ObjectType, skillCode);
				if (skillSequence < 0)
				{
					return;
				}

				charVoiceType = CharacterVoiceType.PlaySkillActive;
				customSoundName = string.Format("PlaySkill{0}seq{1}", skillData.group, skillSequence);
			}
			else
			{
				if (skillSlotIndex != SkillSlotIndex.Passive)
				{
					return;
				}

				charVoiceType = CharacterVoiceType.PlaySkillPassive;
				customSoundName = string.Format("PlaySkill{0}", skillData.group);
			}

			CharacterVoiceControl.PlayCharacterVoice(charVoiceType, 15, GetPosition(), customSoundName,
				skillSlotSet.Value);
		}


		public Item GetWeapon()
		{
			return equipment.GetWeapon();
		}


		public List<Item> GetEquipments()
		{
			return equipment.GetEquips();
		}


		public int GetMasteryCategoryLevel(MasteryCategory masteryCategory)
		{
			int num = 0;
			foreach (KeyValuePair<MasteryType, int> keyValuePair in masterysLevel)
			{
				if (keyValuePair.Key.GetCategory() == masteryCategory)
				{
					num += keyValuePair.Value;
				}
			}

			return num;
		}


		public int GetHighestMasteryCategoryLevel(MasteryCategory masteryCategory)
		{
			int num = 0;
			foreach (KeyValuePair<MasteryType, int> keyValuePair in masterysLevel)
			{
				if (keyValuePair.Key.GetCategory() == masteryCategory && num < keyValuePair.Value)
				{
					num = keyValuePair.Value;
				}
			}

			return num;
		}


		public int GetSkillLevel(SkillSlotIndex skillSlotIndex)
		{
			if (playerContext == null)
			{
				return 0;
			}

			if (skillSlotIndex != SkillSlotIndex.WeaponSkill)
			{
				return playerContext.CharacterSkill.GetSkillLevel(skillSlotIndex);
			}

			return playerContext.CharacterSkill.GetSkillLevel(playerContext.Character.GetEquipWeaponMasteryType());
		}


		public int GetSkillEvolutionLevel(SkillSlotIndex skillSlotIndex)
		{
			if (playerContext != null)
			{
				return playerContext.CharacterSkill.GetSkillEvolutionLevel(skillSlotIndex);
			}

			return 0;
		}


		public int GetSkillMaxEvolutionLevel(SkillSlotIndex skillSlotIndex)
		{
			SkillData skillData = null;
			if (skillSlotIndex == SkillSlotIndex.WeaponSkill)
			{
				Item weapon = GetWeapon();
				if (weapon != null)
				{
					skillData = GameDB.skill.GetSkillData(weapon.ItemData.GetMasteryType(),
						GetSkillLevel(skillSlotIndex), 0);
				}
			}
			else
			{
				if (skillSlotIndex == SkillSlotIndex.SpecialSkill)
				{
					return 0;
				}

				SkillSlotSet? skillSlotSet = playerContext.CharacterSkill.GetSkillSlotSet(skillSlotIndex);
				if (skillSlotSet != null)
				{
					skillData = GameDB.skill.GetSkillData(GetCharacterCode(), ObjectType, skillSlotSet.Value,
						GetSkillLevel(skillSlotIndex), 0);
				}
			}

			if (skillData != null)
			{
				return GameDB.skill.GetSkillMaxEvolutionLevel(skillData.group);
			}

			return 0;
		}


		public bool? CheckCooldown(SkillSlotIndex skillSlotIndex)
		{
			SkillSlotSet? skillSlotSet = playerContext.CharacterSkill.GetSkillSlotSet(skillSlotIndex);
			if (skillSlotSet == null)
			{
				return null;
			}

			return CheckCooldown(skillSlotSet.Value);
		}


		public bool CheckCooldown(SkillSlotSet skillSlotSet)
		{
			if (playerContext == null)
			{
				return false;
			}

			if (skillSlotSet != SkillSlotSet.WeaponSkill)
			{
				return playerContext.CharacterSkill.CheckCooldown(skillSlotSet,
					MonoBehaviourInstance<ClientService>.inst.CurrentSkillTime);
			}

			return playerContext.CharacterSkill.CheckCooldown(GetEquipWeaponMasteryType(),
				MonoBehaviourInstance<ClientService>.inst.CurrentSkillTime);
		}


		public float? GetSkillCooldown(SkillSlotIndex skillSlotIndex)
		{
			SkillSlotSet? skillSlotSet = playerContext.CharacterSkill.GetSkillSlotSet(skillSlotIndex);
			if (skillSlotSet == null)
			{
				return null;
			}

			return GetSkillCooldown(skillSlotSet.Value);
		}


		public float GetSkillCooldown(SkillSlotSet skillSlotSet)
		{
			if (playerContext == null)
			{
				return 0f;
			}

			if (skillSlotSet != SkillSlotSet.WeaponSkill)
			{
				return playerContext.CharacterSkill.GetCooldown(skillSlotSet, Time.time);
			}

			return playerContext.CharacterSkill.GetCooldown(GetEquipWeaponMasteryType(), Time.time);
		}


		public float GetSkillMaxCooldown(SkillSlotSet skillSlotSet)
		{
			return GetSkillData(skillSlotSet).cooldown;
		}


		public bool IsLastSequence(SkillSlotSet skillSlotSet)
		{
			return playerContext.CharacterSkill.IsLastSequence(skillSlotSet);
		}


		public int? GetMaxSequence(SkillSlotIndex skillSlotIndex)
		{
			SkillSlotSet? skillSlotSet = playerContext.CharacterSkill.GetSkillSlotSet(skillSlotIndex);
			if (skillSlotSet == null)
			{
				return null;
			}

			return playerContext.CharacterSkill.GetMaxSequence(skillSlotSet.Value);
		}


		private void EndPlayingSequenceSkills()
		{
			List<SkillSlotSet> playingSequenceSkillSlotSets = myPlayer.CharacterSkill.GetPlayingSequenceSkillSlotSets();
			SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.UpdateSkillHudSlots();
			for (int i = 0; i < playingSequenceSkillSlotSets.Count; i++)
			{
				if (playingSequenceSkillSlotSets[i] < SkillSlotSet.Active4_1 ||
				    playingSequenceSkillSlotSets[i] > SkillSlotSet.Active4_5)
				{
					myPlayer.ResetSkillSequence(playingSequenceSkillSlotSets[i]);
					SkillData skillData = GetSkillData(playingSequenceSkillSlotSets[i]);
					StartSkillCooldown(playingSequenceSkillSlotSets[i], GetEquipWeaponMasteryType(),
						skillData.cooldown);
				}
			}
		}


		public bool? IsHoldCooldown(SkillSlotIndex skillSlotIndex)
		{
			SkillSlotSet? skillSlotSet = playerContext.CharacterSkill.GetSkillSlotSet(skillSlotIndex);
			if (skillSlotSet == null)
			{
				return null;
			}

			return IsHoldCooldown(skillSlotSet.Value);
		}


		public bool IsHoldCooldown(SkillSlotSet skillSlotSet)
		{
			if (playerContext == null)
			{
				return false;
			}

			if (skillSlotSet != SkillSlotSet.WeaponSkill)
			{
				return playerContext.CharacterSkill.IsHoldCooldown(skillSlotSet);
			}

			return playerContext.CharacterSkill.IsHoldCooldown(GetEquipWeaponMasteryType());
		}


		private Sprite GetMapIconBg()
		{
			if (MonoBehaviourInstance<ClientService>.inst.MyObjectId == objectId)
			{
				return SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite(1 <= teamSlot
					? string.Format("Ico_Map_PointPin_{0:D2}", teamSlot)
					: "Ico_Map_PointPin_01");
			}

			if (MonoBehaviourInstance<ClientService>.inst.IsAlly(this))
			{
				return SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite(
					string.Format("Ico_Map_PointPin_{0:D2}", teamSlot));
			}

			if (isAlive)
			{
				return SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Ico_Map_PointPin_04");
			}

			return null;
		}


		protected override Sprite GetMapIcon()
		{
			if (!MonoBehaviourInstance<ClientService>.inst.IsAlly(this))
			{
				if (isAlive)
				{
					return SingletonMonoBehaviour<ResourceManager>.inst.GetCharacterMapSprite(characterCode);
				}

				return SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Ico_Map_DeathPin_04");
			}

			if (isAlive)
			{
				return SingletonMonoBehaviour<ResourceManager>.inst.GetCharacterMapSprite(characterCode);
			}

			if (MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				return SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite(
					string.Format("Ico_Map_DeathPin_{0:D2}", TeamSlot));
			}

			return SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Ico_Map_DeathPin_04");
		}


		private Color GetTeamColor()
		{
			return GameConstants.TeamMode.GetTeamColor(TeamSlot);
		}


		public void SetPlayerContext(PlayerContext playerContext)
		{
			this.playerContext = playerContext;
		}


		public override void SetIsInCombat(bool isCombat)
		{
			base.SetIsInCombat(isCombat);
			SetCharacterAnimatorBool(AnimID_IsCombat, isCombat);
			if (MonoBehaviourInstance<ClientService>.inst.MyObjectId == objectId)
			{
				if (isCombat)
				{
					MonoBehaviourInstance<GameUI>.inst.StatusHud.ShowInBattleUI();
					return;
				}

				MonoBehaviourInstance<GameUI>.inst.StatusHud.HideInBattleUI();
			}
		}


		public void SetInitialEquipment(List<Item> equipItems)
		{
			equipment.UpdateEquipment(equipItems);
		}


		public void MergeBulletItems(List<BulletItem> bulletItems, List<EquipItem> equipItems,
			List<InvenItem> invenItems)
		{
			using (List<BulletItem>.Enumerator enumerator = bulletItems.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					BulletItem bulletItem = enumerator.Current;
					EquipItem equipItem = equipItems.Find(x => x.item.id == bulletItem.item.id);
					if (equipItem != null)
					{
						equipItem.item.RemainCoolTime = bulletItem.remainCooldown;
					}
					else
					{
						InvenItem invenItem = invenItems.Find(x => x.item.id == bulletItem.item.id);
						if (invenItem != null)
						{
							invenItem.item.RemainCoolTime = bulletItem.remainCooldown;
						}
					}
				}
			}
		}


		public void OnUpdateEquipment(List<EquipItem> updates, Item oldWeapon = null)
		{
			if (oldWeapon == null)
			{
				oldWeapon = equipment.GetWeapon();
			}

			bool flag = false;
			int ammo = 0;
			bool flag2 = false;
			foreach (EquipItem equipItem in updates)
			{
				equipment.UpdateItem(equipItem);
				if (equipItem.item != null)
				{
					if (equipItem.item.ItemData.IsGunType())
					{
						flag = true;
					}

					if (equipItem.item.ItemData.IsThrowType())
					{
						ammo = equipItem.item.Bullet;
						flag2 = true;
					}

					equipItem.item.ItemData.GetMasteryType().IsWeaponMastery();
				}
			}

			UISystem.Action(new UpdatePlayerInfo(ObjectId, TeamNumber, TeamSlot, Rank, playerContext.userId)
			{
				equipment = equipment.GetEquips()
			});
			Item weapon = equipment.GetWeapon();
			ItemWeaponData itemWeaponData = weapon != null ? weapon.GetItemData<ItemWeaponData>() : null;
			if (itemWeaponData != null)
			{
				if (itemWeaponData.IsGunType() || itemWeaponData.IsThrowType())
				{
					if (flag)
					{
						int bulletCapacity = GameDB.item.GetBulletCapacity(itemWeaponData.code);
						floatingUi.SetupBullet(0, bulletCapacity);
						Status.UpdateBullet(0);
					}

					if (flag2)
					{
						floatingUi.UpdateThrowAmmo(itemWeaponData.weaponType, ammo);
					}
				}
				else
				{
					floatingUi.HideThrowAmmo();
					floatingUi.HideBullet();
				}
			}
			else
			{
				floatingUi.HideThrowAmmo();
				floatingUi.HideBullet();
			}

			bool flag3 = false;
			Item weapon2 = equipment.GetWeapon();
			if (oldWeapon == null && weapon2 == null)
			{
				flag3 = false;
			}
			else if (oldWeapon == null)
			{
				flag3 = true;
			}
			else if (weapon2 == null)
			{
				flag3 = true;
			}
			else if (oldWeapon.ItemData.GetMasteryType() != weapon2.ItemData.GetMasteryType())
			{
				flag3 = true;
			}

			if (flag3)
			{
				if (itemWeaponData != null)
				{
					MyPlayerContext myPlayerContext = myPlayer;
					if (myPlayerContext != null)
					{
						myPlayerContext.CharacterSkill.InitWeaponSkill(itemWeaponData.GetMasteryType());
					}
				}

				if (weapon2 != null)
				{
					weaponMountController.UpdateWeaponAnimation(itemWeaponData.weaponType);
				}
				else
				{
					weaponMountController.UpdateWeaponAnimation(WeaponType.None);
					floatingUi.SetupBullet(0, 0);
				}

				characterRenderer.RebuildAndRedraw();
				LocalSkillPlayer localSkillPlayer = LocalSkillPlayer;
				if (localSkillPlayer != null)
				{
					localSkillPlayer.OnUpdateWeapon();
				}
			}

			List<Item> list = new List<Item>();
			foreach (EquipItem equipItem2 in updates)
			{
				if (equipItem2.item != null)
				{
					list.Add(equipItem2.item);
				}
			}

			if (myPlayer != null)
			{
				myPlayer.OnUpdateEquipment(list, flag3);
				if (weapon != null)
				{
					SingletonMonoBehaviour<GameAnalytics>.inst.SetSelectWeapon(weapon.itemCode);
					if (weapon.ItemData.IsThrowType() && !weapon.IsFullBullet())
					{
						MonoBehaviourInstance<GameUI>.inst.StatusHud.StopBulletCooldown(weapon.id);
						MyPlayerContext myPlayerContext2 = myPlayer;
						if (myPlayerContext2 != null)
						{
							myPlayerContext2.StartBulletCooldown(weapon, false);
						}
					}
				}
				else
				{
					SingletonMonoBehaviour<GameAnalytics>.inst.SetSelectWeapon(0);
				}
			}

			if (MonoBehaviourInstance<GameClient>.inst.IsTutorial)
			{
				MonoBehaviourInstance<TutorialController>.inst.UpdateEquipTutorial(updates);
			}

			TargetInfoHud targetInfoHud = this.targetInfoHud;
			if (targetInfoHud == null)
			{
				return;
			}

			targetInfoHud.SetTargetItems(equipment.GetEquips());
		}


		public void OnUpdateInventory(List<InvenItem> updates)
		{
			MyPlayerContext myPlayerContext = myPlayer;
			if (myPlayerContext == null)
			{
				return;
			}

			myPlayerContext.OnUpdateInventory(updates, UpdateInventoryType.Invalid);
		}


		public new void InitStat(List<CharacterStatValue> updates, byte[] statusSnapshot)
		{
			base.InitStat(updates, statusSnapshot);
			OnUpdateStat(updates);
		}


		public override void UpdateStat(List<CharacterStatValue> updates)
		{
			base.UpdateStat(updates);
			OnUpdateStat(updates);
		}


		private void OnUpdateStat(List<CharacterStatValue> updates)
		{
			MyPlayerContext myPlayerContext = myPlayer;
			if (myPlayerContext != null)
			{
				myPlayerContext.OnUpdateStat();
			}

			if (myPlayer != null)
			{
				bool flag = false;
				bool flag2 = false;
				foreach (CharacterStatValue characterStatValue in updates)
				{
					if (characterStatValue.statType.IsSightRange())
					{
						flag = true;
					}

					if (characterStatValue.statType.IsAttackRangeRange())
					{
						flag2 = true;
					}
				}

				if (flag)
				{
					myPlayer.OnUpdateCameraFV();
				}

				if (flag2)
				{
					SkillSlotSet? skillSlotSet = myPlayer.CharacterSkill.GetSkillSlotSet(SkillSlotIndex.Attack);
					if (skillSlotSet != null)
					{
						float num;
						SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.UpdateIndicator(
							GetSkillData(skillSlotSet.Value), out num);
					}
				}
			}
		}


		public void MasteryLevelUp(MasteryType masteryType, int level)
		{
			masterysLevel[masteryType] = level;
			MasteryCategoryLevelUp(masteryType);
		}


		public void MasteryCategoryLevelUp(MasteryType masteryType)
		{
			if (playerContext == null)
			{
				return;
			}

			switch (masteryType.GetCategory())
			{
				case MasteryCategory.Combat:
					UISystem.Action(new UpdatePlayerInfo(ObjectId, TeamNumber, TeamSlot, Rank, playerContext.userId)
					{
						combatLevel = GetHighestMasteryCategoryLevel(MasteryCategory.Combat)
					});
					return;
				case MasteryCategory.Search:
					UISystem.Action(new UpdatePlayerInfo(ObjectId, TeamNumber, TeamSlot, Rank, playerContext.userId)
					{
						searchLevel = GetMasteryCategoryLevel(MasteryCategory.Search)
					});
					return;
				case MasteryCategory.Growth:
					UISystem.Action(new UpdatePlayerInfo(ObjectId, TeamNumber, TeamSlot, Rank, playerContext.userId)
					{
						growthLevel = GetMasteryCategoryLevel(MasteryCategory.Growth)
					});
					return;
				default:
					return;
			}
		}


		public void OnMasteryLevelUp(MasteryType masteryType)
		{
			if (characterRenderer.IsRendering)
			{
				floatingUi.OnMasteryLevelUp(masteryType);
			}
		}


		public SkillData GetSkillData(SkillSlotSet skillSlotSet)
		{
			int sequence = 0;
			if (this.playerContext != null)
			{
				sequence = this.playerContext.GetLocalSkillSequence(skillSlotSet);
			}

			SkillSlotIndex skillSlotIndex = skillSlotSet.SlotSet2Index();
			if (skillSlotIndex == SkillSlotIndex.WeaponSkill)
			{
				Item weapon = GetWeapon();
				if (weapon == null)
				{
					return null;
				}

				return GameDB.skill.GetSkillData(weapon.ItemData.GetMasteryType(), GetSkillLevel(skillSlotIndex),
					sequence);
			}

			if (skillSlotIndex != SkillSlotIndex.SpecialSkill)
			{
				return GameDB.skill.GetSkillData(GetCharacterCode(), ObjectType, skillSlotSet,
					GetSkillLevel(skillSlotIndex), sequence);
			}

			PlayerContext playerContext = this.playerContext;
			SpecialSkillId? specialSkillId = playerContext != null
				? new SpecialSkillId?(playerContext.CharacterSkill.SpecialSkillId)
				: null;
			if (specialSkillId == null)
			{
				return null;
			}

			return GameDB.skill.GetSkillData(specialSkillId.Value, sequence);
		}


		public SkillData GetSkillData(SkillSlotIndex skillSlotIndex)
		{
			SkillSlotSet? skillSlotSet = playerContext.CharacterSkill.GetSkillSlotSet(skillSlotIndex);
			if (skillSlotSet == null)
			{
				return null;
			}

			return GetSkillData(skillSlotSet.Value);
		}


		public UseSkillErrorCode EnoughMainCost(SkillData data)
		{
			if (data == null)
			{
				return UseSkillErrorCode.NotEnoughStamina;
			}

			return CheckSkillResource(data.CostType, data.CostKey, data.cost);
		}


		public UseSkillErrorCode EnoughExCost(SkillData data)
		{
			if (data == null)
			{
				return UseSkillErrorCode.NotEnoughStamina;
			}

			return CheckSkillResource(data.ExCostType, data.ExCostKey, data.exCost);
		}


		public bool EnoughSkillResources(SkillData data)
		{
			return EnoughMainCost(data) == UseSkillErrorCode.None && EnoughExCost(data) == UseSkillErrorCode.None;
		}


		protected UseSkillErrorCode CheckSkillResource(SkillCostType costType, int costKey, int cost)
		{
			switch (costType)
			{
				case SkillCostType.NoCost:
					return UseSkillErrorCode.None;
				case SkillCostType.Sp:
					if (cost > Status.Sp)
					{
						return UseSkillErrorCode.NotEnoughStamina;
					}

					return UseSkillErrorCode.None;
				case SkillCostType.StateStack:
					if (cost > GetStateStackByGroup(costKey, objectId))
					{
						return UseSkillErrorCode.NotEnoughExCost;
					}

					return UseSkillErrorCode.None;
				case SkillCostType.EquipWeaponStack:
					if (!IsEquippedWeapon())
					{
						return UseSkillErrorCode.NotEnoughExCost;
					}

					if (!GetWeapon().WeaponConsumable)
					{
						return UseSkillErrorCode.None;
					}

					if (cost > GetWeapon().Amount)
					{
						return UseSkillErrorCode.NotEnoughExCost;
					}

					return UseSkillErrorCode.None;
				case SkillCostType.Hp:
					return UseSkillErrorCode.None;
				case SkillCostType.Ep:
					if (cost > Status.ExtraPoint)
					{
						return UseSkillErrorCode.NotEnoughExCost;
					}

					return UseSkillErrorCode.None;
				default:
					return UseSkillErrorCode.NotEnoughStamina;
			}
		}


		public MasteryType GetEquipWeaponMasteryType()
		{
			Item weapon = GetWeapon();
			if (weapon == null)
			{
				return MasteryType.None;
			}

			return weapon.WeaponTypeInfoData.type.GetWeaponMasteryType();
		}


		public bool IsFullBullet()
		{
			Item weapon = GetWeapon();
			return weapon == null || !weapon.ItemData.IsGunType() ||
			       GameDB.item.GetBulletCapacity(GetWeapon().ItemData.code) == Status.Bullet;
		}


		public void OnStartSkill(SkillData skillData)
		{
			MyPlayerContext myPlayerContext = myPlayer;
			if (myPlayerContext == null)
			{
				return;
			}

			myPlayerContext.OnStartSkillCasting(skillData.group, skillData.CastingTime1, skillData.CastingBarType1,
				null);
		}


		protected override void OnAddEffectState(CharacterStateValue state)
		{
			base.OnAddEffectState(state);
			MyPlayerContext myPlayerContext = myPlayer;
			if (myPlayerContext == null)
			{
				return;
			}

			myPlayerContext.OnAddEffectState(state);
		}


		protected override void OnRemoveEffectState(CharacterStateValue state)
		{
			MyPlayerContext myPlayerContext = myPlayer;
			if (myPlayerContext == null)
			{
				return;
			}

			myPlayerContext.OnRemoveEffectState(state);
		}


		protected override void OnUpdateEffectState(CharacterStateValue state)
		{
			MyPlayerContext myPlayerContext = myPlayer;
			if (myPlayerContext == null)
			{
				return;
			}

			myPlayerContext.OnUpdateEffectState(state);
		}


		public override void UpdateInvisible(bool isInvisible)
		{
			ClientService inst = MonoBehaviourInstance<ClientService>.inst;
			if (inst == null)
			{
				return;
			}

			if (SightAgent != null)
			{
				SightAgent.SetIsInvisible(isInvisible);
			}

			if (characterRenderer != null)
			{
				characterRenderer.SetMaterial(MaterialSwitchType.Stealth, IsInvisible);
			}

			if (clientCharacter != null)
			{
				clientCharacter.SetStealth(isInvisible);
			}

			if (inst.MyObjectId == objectId)
			{
				GameUI inst2 = MonoBehaviourInstance<GameUI>.inst;
				if (inst2 != null)
				{
					inst2.StelthOverlayUI.SetActive(IsInvisible);
				}
			}
		}


		protected override void OnBushEvent(bool isInBush, int objectId) { }


		public void OnKill(LocalCharacter deadCharacter)
		{
			if (!deadCharacter.IsTypeOf<LocalMovableCharacter>())
			{
				return;
			}

			if (deadCharacter.IsTypeOf<LocalPlayerCharacter>())
			{
				Status.AddPlayerKillCount();
			}
			else
			{
				Status.AddMonsterKillCount();
			}

			UISystem.Action(new UpdatePlayerInfo(ObjectId, TeamNumber, TeamSlot, Rank, playerContext.userId)
			{
				monsterKill = Status.MonsterKill,
				playerKill = Status.PlayerKill
			});
			MyPlayerContext myPlayerContext = myPlayer;
			if (myPlayerContext != null)
			{
				myPlayerContext.OnKill(deadCharacter);
			}

			if (targetInfoHud != null)
			{
				targetInfoHud.SetTargetKillScore(Status.PlayerKill, Status.PlayerKillAssist, Status.MonsterKill);
			}

			MonoBehaviourInstance<GameUI>.inst.HudUpdateKillCount(ObjectId, Status.PlayerKill);
		}


		public void OnKillAssist(LocalCharacter deadCharacter)
		{
			if (!deadCharacter.IsTypeOf<LocalMovableCharacter>())
			{
				return;
			}

			if (deadCharacter.IsTypeOf<LocalPlayerCharacter>())
			{
				Status.AddPlayerKillAssistCount();
			}

			UISystem.Action(new UpdatePlayerInfo(ObjectId, TeamNumber, TeamSlot, Rank, playerContext.userId)
			{
				playerKillAssist = Status.PlayerKillAssist
			});
			MyPlayerContext myPlayerContext = myPlayer;
			if (myPlayerContext != null)
			{
				myPlayerContext.OnKillAssist(deadCharacter);
			}

			if (targetInfoHud != null)
			{
				targetInfoHud.SetTargetKillScore(Status.PlayerKill, Status.PlayerKillAssist, Status.MonsterKill);
			}

			MonoBehaviourInstance<GameUI>.inst.HudUpdateKillCount(ObjectId, Status.PlayerKill);
		}


		public void OnStartConcentration(SkillData skillData)
		{
			MyPlayerContext myPlayerContext = myPlayer;
			if (myPlayerContext == null)
			{
				return;
			}

			myPlayerContext.OnStartConcentration(skillData);
		}


		public void OnEndConcentration(SkillData skillData, bool cancel)
		{
			MyPlayerContext myPlayerContext = myPlayer;
			if (myPlayerContext == null)
			{
				return;
			}

			myPlayerContext.OnEndConcentration(skillData, cancel);
		}


		public void ConsumeEp(int costEp)
		{
			Status.SubEp(costEp);
			floatingUi.UpdateExtraPoint(Status.ExtraPoint);
			MonoBehaviourInstance<GameUI>.inst.HudUpdateEp(ObjectId, Status.ExtraPoint);
			MyPlayerContext myPlayerContext = myPlayer;
			if (myPlayerContext == null)
			{
				return;
			}

			myPlayerContext.OnUpdateEp();
		}


		public void ConsumeSp(int costSp)
		{
			Status.SubSp(costSp);
			floatingUi.SetSp(Status.Sp, Stat.MaxSp);
			MonoBehaviourInstance<GameUI>.inst.HudUpdateSp(ObjectId, Status.Sp, Stat.MaxSp);
			MyPlayerContext myPlayerContext = myPlayer;
			if (myPlayerContext != null)
			{
				myPlayerContext.OnUpdateStat();
			}

			TargetInfoHud targetInfoHud = this.targetInfoHud;
			if (targetInfoHud == null)
			{
				return;
			}

			targetInfoHud.SetTargetStatSpBar(Status.Sp, Stat.MaxSp);
		}


		public void ConsumeHp(int costhp)
		{
			Status.SubHp(costhp);
			floatingUi.SetHp(Status.Hp, Stat.MaxHp);
			MonoBehaviourInstance<GameUI>.inst.HudUpdateHp(ObjectId, Status.Hp, Stat.MaxHp);
			MyPlayerContext myPlayerContext = myPlayer;
			if (myPlayerContext != null)
			{
				myPlayerContext.OnUpdateStat();
			}

			TargetInfoHud targetInfoHud = this.targetInfoHud;
			if (targetInfoHud == null)
			{
				return;
			}

			targetInfoHud.SetTargetStatHpBar(Status.Hp, Stat.MaxHp);
		}


		public void OnUpdateLevel(int level)
		{
			Status.SetLevel(level);
			floatingUi.UpdateLevel(Status.Level);
			MonoBehaviourInstance<GameUI>.inst.HudUpdateLevel(ObjectId, Status.Level);
			UISystem.Action(new UpdatePlayerInfo(ObjectId, TeamNumber, TeamSlot, Rank, playerContext.userId)
			{
				level = level
			});
			if (level > 1)
			{
				OnLevelUp();
			}

			TargetInfoHud targetInfoHud = this.targetInfoHud;
			if (targetInfoHud == null)
			{
				return;
			}

			targetInfoHud.SetPlayerLevel(Status.Level, Status.Exp);
		}


		public void OnDyingCondition(int setHp, int setSp)
		{
			isDyingCondition = true;
			floatingUi.SetDyingCondition(true);
			Status.SetHp(setHp);
			Status.SetSp(setSp);
			SetAnimation(AnimID_Downed, true);
			SetAnimation(AnimID_DownedTrigger);
			UpdateMapIconDyingCondition();
			UISystem.Action(new UpdatePlayerInfo(this));
			MonoBehaviourInstance<GameUI>.inst.HudUpdateDyingCondition(ObjectId, true);
			MonoBehaviourInstance<GameUI>.inst.HudUpdateHp(ObjectId, setHp, 500);
			MonoBehaviourInstance<GameUI>.inst.HudUpdateSp(ObjectId, setSp, Stat.MaxSp);
			if (myPlayer != null)
			{
				if (MonoBehaviourInstance<GameUI>.inst.HyperloopWindow.IsOpen)
				{
					MonoBehaviourInstance<GameUI>.inst.HyperloopWindow.Close();
				}

				EndPlayingSequenceSkills();
			}

			if (MonoBehaviourInstance<ClientService>.inst.IsAlly(this))
			{
				Singleton<SoundControl>.inst.PlayUISound("TeamDown");
			}

			SingletonMonoBehaviour<LocalBattleEventCollector>.inst.OnDyingCondition(this);
		}


		public void OnTeamRevival(int setHp, int setSp)
		{
			isDyingCondition = false;
			floatingUi.SetDyingCondition(false);
			Status.SetHp(setHp);
			Status.SetSp(setSp);
			PlayLocalEffectChild("FX_BI_Revival", null);
			SetAnimation(AnimID_Downed, false);
			UpdateMapIconRevival();
			UISystem.Action(new UpdatePlayerInfo(this));
			MonoBehaviourInstance<GameUI>.inst.HudUpdateDyingCondition(ObjectId, false);
			MonoBehaviourInstance<GameUI>.inst.HudUpdateHp(ObjectId, setHp, Stat.MaxHp);
			MonoBehaviourInstance<GameUI>.inst.HudUpdateSp(ObjectId, setSp, Stat.MaxSp);
			if (MonoBehaviourInstance<ClientService>.inst.IsAlly(this))
			{
				Singleton<SoundControl>.inst.PlayUISound("TeamRevival");
			}

			Singleton<SoundControl>.inst.PlayFXSound("Revival", "TeamRevival", 8, GetPosition(), false);
			SingletonMonoBehaviour<LocalBattleEventCollector>.inst.OnRevival(this);
		}


		public void ConsumeBullet()
		{
			Status.ConsumeBullet();
			floatingUi.UpdateBullet(Status.Bullet);
			if (myPlayer != null && MonoBehaviourInstance<GameClient>.inst.IsTutorial)
			{
				MonoBehaviourInstance<TutorialController>.inst.CreateReloadTutorial();
			}
		}


		public void UpdateShield(int shield)
		{
			Status.SetShield(shield);
			floatingUi.SetShield(shield);
		}


		public override void OnHeal(int addHp, int addSp, int effectCode, bool show)
		{
			base.OnHeal(addHp, addSp, effectCode, show);
			MyPlayerContext myPlayerContext = myPlayer;
			if (myPlayerContext == null)
			{
				return;
			}

			myPlayerContext.OnUpdateHpSp();
		}


		public override void OnSetExtraPoint(int setExtraPoint)
		{
			base.OnSetExtraPoint(setExtraPoint);
			if (!isAlive)
			{
				return;
			}

			Status.SetExtraPoint(setExtraPoint);
			floatingUi.UpdateExtraPoint(setExtraPoint);
			MyPlayerContext myPlayerContext = myPlayer;
			if (myPlayerContext == null)
			{
				return;
			}

			myPlayerContext.OnUpdateEp();
		}


		public override void OnEvasion()
		{
			base.OnEvasion();
			if (!isAlive)
			{
				return;
			}

			if (characterRenderer != null && characterRenderer.IsRendering)
			{
				floatingUi.ShowEvasion();
			}
		}


		public override void SetNoParentEffectByTag(string tag)
		{
			base.SetNoParentEffectByTag(tag);
			clientCharacter.SetNoParentEffectByTag(tag);
		}


		public override void StopLocalEffectByTag(string tag)
		{
			base.StopLocalEffectByTag(tag);
			clientCharacter.StopEffectByTag(tag);
		}


		public override void StopLocalSoundByTag(string tag)
		{
			base.StopLocalSoundByTag(tag);
			clientCharacter.StopSoundByTag(tag);
		}


		public override void StartSkill(SkillId skillId, SkillData skillData, int evolutionLevel, int targetObjectId)
		{
			base.StartSkill(skillId, skillData, evolutionLevel, targetObjectId);
			if (objectId == MonoBehaviourInstance<ClientService>.inst.MyObjectId)
			{
				SkillSlotSet skillSlotSet = SkillSlotSet.None;
				MasteryType weaponType = MasteryType.None;
				if (GameDB.skill.FindSkillSlotAndMastery(
					MonoBehaviourInstance<ClientService>.inst.MyPlayer.Character.CharacterCode, skillData.code,
					ObjectType, ref skillSlotSet, ref weaponType))
				{
					SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.OnSkillStart(skillId, skillSlotSet,
						skillData);
					if (MonoBehaviourInstance<ClientService>.inst.MyPlayer.CharacterSkill.UseSkillStack(skillSlotSet,
						    weaponType, skillData, MonoBehaviourInstance<ClientService>.inst.CurrentServerFrameTime) &&
					    MonoBehaviourInstance<ClientService>.inst.MyPlayer.CharacterSkill.GetSkillStack(skillSlotSet,
						    weaponType) == 0)
					{
						SkillSlotIndex? skillSlotIndex =
							MonoBehaviourInstance<ClientService>.inst.MyPlayer.CharacterSkill.GetSkillSlotIndex(
								skillSlotSet);
						if (skillSlotIndex != null)
						{
							MonoBehaviourInstance<GameUI>.inst.SkillHud.SetSlotFillAmountTypeChange(
								skillSlotIndex.Value, UICooldown.FillAmountType.FORWARD);
						}
					}
				}
			}

			if (skillData == null)
			{
				return;
			}

			if (skillData.SkillId != skillId)
			{
				return;
			}

			OnStartSkill(skillData);
		}


		public override void FinishSkill(SkillId skillId, bool cancel, SkillSlotSet skillSlotSet)
		{
			base.FinishSkill(skillId, cancel, skillSlotSet);
			if (skillSlotSet != SkillSlotSet.None && MonoBehaviourInstance<ClientService>.inst.IsPlayer &&
			    objectId == MonoBehaviourInstance<ClientService>.inst.MyObjectId)
			{
				SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.OnPlaySkillFinish(skillSlotSet);
			}
		}


		public override void OnDamage(int attackerId, bool isSkillDamage, int damage, bool isCritical, int effectCode)
		{
			base.OnDamage(attackerId, isSkillDamage, damage, isCritical, effectCode);
			if (myPlayer != null)
			{
				myPlayer.OnDamaged(damage);
				myPlayer.OnUpdateStat();
			}
		}


		public void OnPlayPassiveSkill()
		{
			CharacterVoiceControl.PlayCharacterVoice(CharacterVoiceType.PlaySkillPassive, 15, GetPosition());
		}


		public bool CheckDyingCondition()
		{
			if (!IsDyingCondition)
			{
				return false;
			}

			MonoBehaviourInstance<GameUI>.inst.ToastMessage.ShowMessage(Ln.Get("DyingCondition"));
			return true;
		}


		public bool EquipableItem(Item item)
		{
			if (item.ItemData.IsThrowType())
			{
				return true;
			}

			if (item.ItemData.itemType == ItemType.Weapon)
			{
				if (GetWeapon() == null)
				{
					return true;
				}
			}
			else if (item.ItemData.itemType == ItemType.Armor &&
			         MonoBehaviourInstance<GameUI>.inst.StatusHud.GetStatusItemSlot(item).GetItem() == null)
			{
				return true;
			}

			return !IsInCombat;
		}


		public bool UnEquipableItem(Item item)
		{
			return item.ItemData.IsThrowType() || !equipment.GetEquips().Contains(item) || !IsInCombat;
		}


		public void UpdateSkillPoint(int skillPoint)
		{
			MyPlayerContext myPlayerContext = myPlayer;
			if (myPlayerContext == null)
			{
				return;
			}

			myPlayerContext.UpdateSkillPoint(skillPoint);
		}


		public void UpdateSkillPoint(MasteryType masteryType, int skillPoint)
		{
			MyPlayerContext myPlayerContext = myPlayer;
			if (myPlayerContext == null)
			{
				return;
			}

			myPlayerContext.UpdateSkillPoint(masteryType, skillPoint);
		}


		public void UpgradeSkill(SkillSlotIndex skillSlotIndex)
		{
			PlayerContext playerContext = this.playerContext;
			if (playerContext != null)
			{
				playerContext.UpgradeSkill(skillSlotIndex);
			}

			if (skillSlotIndex == SkillSlotIndex.Active4 && GetSkillLevel(skillSlotIndex) == 1)
			{
				MonoBehaviourInstance<GameUI>.inst.HudUpdateStartUltimateSkillCooldown(ObjectId, 0f, 0f);
			}
		}


		public void StartSkillCooldown(SkillSlotSet skillSlotSet, MasteryType masteryType, float cooldownValue)
		{
			PlayerContext playerContext = this.playerContext;
			if (playerContext == null)
			{
				return;
			}

			playerContext.StartSkillCooldown(skillSlotSet, masteryType, cooldownValue);
		}


		public void ModifySkillCooldown(SkillSlotSet skillSlotSet, float timeValue)
		{
			PlayerContext playerContext = this.playerContext;
			if (playerContext == null)
			{
				return;
			}

			playerContext.ModifySkillCooldown(skillSlotSet, timeValue);
		}


		public void HoldSkillCooldown(SkillSlotSet skillSlotSet, MasteryType masteryType, bool isHold)
		{
			PlayerContext playerContext = this.playerContext;
			if (playerContext == null)
			{
				return;
			}

			playerContext.HoldSkillCooldown(skillSlotSet, masteryType, isHold);
		}


		public void Connected()
		{
			PlayerContext playerContext = this.playerContext;
			if (playerContext != null)
			{
				playerContext.Connected();
			}

			UISystem.Action(new UpdatePlayerInfo(ObjectId, TeamNumber, TeamSlot, Rank, this.playerContext.userId)
			{
				isDisconnected = IsDisconnected
			});
		}


		public void Disconnected()
		{
			PlayerContext playerContext = this.playerContext;
			if (playerContext != null)
			{
				playerContext.Disconnected();
			}

			UISystem.Action(new UpdatePlayerInfo(ObjectId, TeamNumber, TeamSlot, Rank, this.playerContext.userId)
			{
				isDisconnected = IsDisconnected
			});
		}


		public void Observing()
		{
			PlayerContext playerContext = this.playerContext;
			if (playerContext != null)
			{
				playerContext.Observing();
			}

			UISystem.Action(new UpdatePlayerInfo(ObjectId, TeamNumber, TeamSlot, Rank, this.playerContext.userId)
			{
				isObserving = IsObserving
			});
		}


		public void ResetSkillCooldown()
		{
			PlayerContext playerContext = this.playerContext;
			if (playerContext == null)
			{
				return;
			}

			playerContext.ResetSkillCooldown();
		}


		protected override void PlayLocalSoundPoint(string soundName, string tag, int maxDistance, Vector3 pos)
		{
			if (!IsVisibleTo(MonoBehaviourInstance<ClientService>.inst.MyObjectId))
			{
				return;
			}

			base.PlayLocalSoundPoint(soundName, tag, maxDistance, pos);
		}


		private bool IsVisibleTo(int objectId)
		{
			return this.objectId == objectId || MonoBehaviourInstance<ClientService>.inst.IsInAllySight(SightAgent,
				GetPosition(), Stat.Radius, SightAgent.IsInvisibleCheckWithMemorizer(objectId));
		}


		public void AddOwnSummon(LocalSummonBase addTarget)
		{
			ownSummons.Add(addTarget);
		}


		public void RemoveOwnSummon(LocalSummonBase removeTarget)
		{
			ownSummons.Remove(removeTarget);
		}


		public LocalSummonBase GetOwnSummon(Func<LocalSummonBase, bool> condition)
		{
			for (int i = ownSummons.Count - 1; i >= 0; i--)
			{
				if (ownSummons[i] == null || !ownSummons[i].IsAlive)
				{
					ownSummons.RemoveAt(i);
				}
				else if (condition(ownSummons[i]))
				{
					return ownSummons[i];
				}
			}

			return null;
		}


		public List<LocalSummonBase> GetOwnSummons(Func<LocalSummonBase, bool> condition)
		{
			List<LocalSummonBase> list = null;
			for (int i = ownSummons.Count - 1; i >= 0; i--)
			{
				if (ownSummons[i] == null || !ownSummons[i].IsAlive)
				{
					ownSummons.RemoveAt(i);
				}
				else if (condition(ownSummons[i]))
				{
					if (list == null)
					{
						list = new List<LocalSummonBase>();
					}

					list.Add(ownSummons[i]);
				}
			}

			return list;
		}


		private bool CanAnyActionSkillCheck()
		{
			foreach (LocalSkillScript localSkillScript in localSkillPlayer.playingScripts.Values)
			{
				if (localSkillScript.CasterId == 0 && localSkillScript.SkillData.SkillId == localSkillScript.SkillId &&
				    localSkillScript.SkillData.PlayType == SkillPlayType.Alone &&
				    localSkillScript.SkillData.InterruptHandlingType == SkillInterruptHandlingType.Ignore)
				{
					return false;
				}
			}

			return !IsDyingCondition;
		}


		public bool CanAnyAction(LocalObject localObject)
		{
			using (List<CharacterStateValue>.Enumerator enumerator = States.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.StateType.CanNotAction().IsCanNotAction(localObject))
					{
						return false;
					}
				}
			}

			return CanAnyActionSkillCheck();
		}


		public bool CanAnyAction(ActionType actionType)
		{
			using (List<CharacterStateValue>.Enumerator enumerator = States.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.StateType.CanNotAction().IsCanNotAction(actionType))
					{
						return false;
					}
				}
			}

			return CanAnyActionSkillCheck();
		}


		public void ShowEmotionIcon(int emotionIconCode)
		{
			EmotionIconData emotionIconData = GameDB.emotionIcon.GetEmotionIconData(emotionIconCode);
			if (emotionIconData == null)
			{
				return;
			}

			floatingUi.ShowEmotionIcon(emotionIconData);
			if (myPlayer != null)
			{
				MonoBehaviourInstance<GameUI>.inst.EmotionPlateHud.OnShowEmotionIcon(emotionIconData.inputDelayTime);
			}

			if (MonoBehaviourInstance<ClientService>.inst.MyTeamNumber == playerContext.Character.teamNumber)
			{
				MonoBehaviourInstance<GameUI>.inst.TeamHud.OnShowEmotionIcon(objectId, emotionIconData);
			}
		}


		public UseSkillErrorCode IsSkillSlotCanUseInSkillScript(SkillData data, SkillSlotIndex skillSlotIndex)
		{
			if (skillSlotIndex == SkillSlotIndex.Passive)
			{
				return UseSkillErrorCode.None;
			}

			if (data == null)
			{
				return UseSkillErrorCode.InvalidAction;
			}

			return LocalSkillPlayer.IsSkillSlotCanUseInSkillScript(data);
		}


		public UseSkillErrorCode IsSkillSlotCanUseInSkillScript(SkillSlotSet skillSlotSet)
		{
			return IsSkillSlotCanUseInSkillScript(GetSkillData(skillSlotSet), skillSlotSet.SlotSet2Index());
		}


		public UseSkillErrorCode IsCanUseSkill(SkillSlotSet skillSlotSet, LocalObject hitTarget,
			Vector3? cursorPosition)
		{
			SkillData skillData = GetSkillData(skillSlotSet);
			if (skillData == null)
			{
				return UseSkillErrorCode.InvalidAction;
			}

			return LocalSkillPlayer.IsCanUseSkill(skillData, hitTarget, cursorPosition);
		}


		public SkillSlotSet? GetSkillSlotSet(SkillSlotIndex skillSlotIndex)
		{
			return playerContext.CharacterSkill.GetSkillSlotSet(skillSlotIndex);
		}


		public void OnUpdateExp(int exp)
		{
			Status.SetExp(exp);
			MyPlayerContext myPlayerContext = myPlayer;
			if (myPlayerContext != null)
			{
				myPlayerContext.OnUpdateExp(Status.Exp);
			}

			TargetInfoHud targetInfoHud = this.targetInfoHud;
			if (targetInfoHud == null)
			{
				return;
			}

			targetInfoHud.SetPlayerLevel(Status.Level, Status.Exp);
		}


		public override ObjectOrder GetObjectOrder()
		{
			if (IsAlive)
			{
				if (!MonoBehaviourInstance<ClientService>.inst.IsAlly(this))
				{
					return ObjectOrder.AlivePlayerEnemy;
				}

				return ObjectOrder.AlivePlayerAlly;
			}

			if (!MonoBehaviourInstance<ClientService>.inst.IsAlly(this))
			{
				return ObjectOrder.DeadPlayerEnemy;
			}

			return ObjectOrder.DeadPlayerAlly;
		}


		public override bool SetCursor(LocalPlayerCharacter myPlayer)
		{
			if (base.SetCursor(myPlayer))
			{
				return true;
			}

			if (!isAlive)
			{
				MonoBehaviourInstance<GameInput>.inst.GameCursor.SetCursorTarget(CursorTarget.Item);
				return true;
			}

			if (MonoBehaviourInstance<ClientService>.inst.IsAlly(this))
			{
				if (IsDyingCondition)
				{
					MonoBehaviourInstance<GameInput>.inst.GameCursor.SetCursorTarget(CursorTarget.AllyDyingCondition);
				}
				else
				{
					MonoBehaviourInstance<GameInput>.inst.GameCursor.SetCursorTarget(CursorTarget.Ally);
				}

				return true;
			}

			if (!myPlayer.IsEquippedWeapon())
			{
				MonoBehaviourInstance<GameInput>.inst.GameCursor.SetCursorTarget(CursorTarget.Disarmed);
				return true;
			}

			MonoBehaviourInstance<GameInput>.inst.GameCursor.SetCursorTarget(CursorTarget.EnemyNotSummon);
			return true;
		}


		public override void OnSight() { }


		public override void OnHide() { }


		public override void InSight()
		{
			base.InSight();
			if (ClientCharacter != null)
			{
				ClientCharacter.SetInSight(true);
			}
		}


		public override void OutSight()
		{
			ClientService inst = MonoBehaviourInstance<ClientService>.inst;
			if (inst != null && inst.MyObjectId != 0 && inst.MyObjectId == ObjectId)
			{
				Debug.LogException(new Exception(string.Format("[OutSight] OutSight Call!! {0}", inst.MyObjectId)));
				return;
			}

			base.OutSight();
			if (ClientCharacter != null)
			{
				ClientCharacter.SetInSight(false);
			}
		}


		public override void OnVisible()
		{
			base.OnVisible();
			ClientService inst = MonoBehaviourInstance<ClientService>.inst;
			if (inst == null)
			{
				return;
			}

			if (clientCharacter != null)
			{
				clientCharacter.SetVisible(true);
			}

			if (!isAlive && inst.IsPlayer)
			{
				LocalPlayerCharacter character = inst.MyPlayer.Character;
				if (character != null && character.characterVoiceControl != null)
				{
					character.characterVoiceControl.PlayCharacterVoice(CharacterVoiceType.DiscoveryDeadEnemyPlayer, 15,
						character.GetPosition());
				}
			}

			if (playerContext != null)
			{
				UISystem.Action(new UpdatePlayerInfo(ObjectId, TeamNumber, TeamSlot, Rank, playerContext.userId)
				{
					isInSight = true
				});
			}
		}


		public override void OnInvisible()
		{
			base.OnInvisible();
			if (clientCharacter != null)
			{
				clientCharacter.SetVisible(false);
			}

			if (playerContext != null)
			{
				UISystem.Action(new UpdatePlayerInfo(ObjectId, TeamNumber, TeamSlot, Rank, playerContext.userId)
				{
					isInSight = false
				});
			}
		}


		public bool IsEmma()
		{
			return characterCode == 19;
		}
	}
}