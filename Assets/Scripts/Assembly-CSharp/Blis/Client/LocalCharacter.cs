using System;
using System.Collections;
using System.Collections.Generic;
using BIFog;
using Blis.Client.UIModel;
using Blis.Common;
using Blis.Common.Utils;
using UnityChan;
using UnityEngine;

namespace Blis.Client
{
	[RequireComponent(typeof(CharacterFloatingUI))]
	public abstract class LocalCharacter : LocalObject, ISightEventHandler
	{
		private const int SkillDamageUpdateCount = 5;
		private const float MIN_AIRBORNE_HEIGHT = 0.5f;
		private const float MAX_AIRBORNE_HEIGHT = 2.5f;
		protected static readonly int AnimID_Dead = Animator.StringToHash("dead");
		protected static readonly int AnimID_Downed = Animator.StringToHash("bDowned");
		protected static readonly int AnimID_DownedTrigger = Animator.StringToHash("tDowned");
		protected static readonly int AnimID_DownToDead = Animator.StringToHash("bDownToDead");
		protected static readonly int AnimID_IsCombat = Animator.StringToHash("bIsCombat");
		private readonly List<CharacterStateValue> floatingUiStates = new List<CharacterStateValue>();
		private readonly HashSet<StateType> hasStateType =
			new HashSet<StateType>(SingletonComparerEnum<StateTypeComparer, StateType>.Instance);
		private readonly List<LocalProjectile> ownProjectiles = new List<LocalProjectile>();
		private readonly RegenHealInfo regenHealInfo = new RegenHealInfo();
		private readonly List<SkillDamageInfo> skillDamageQueue = new List<SkillDamageInfo>();
		[NonSerialized] public ActiveOnHostileType activeOnHostileType;
		private Coroutine airborneRoutine;
		protected Animator characterAnimator;
		protected GameObject characterObject;
		protected CharacterRenderer characterRenderer;
		private CharacterColliderAgent colliderAgent;
		private CollisionObject3D collisionObject;
		private CharacterStateValue curFloatingUiState;
		protected CharacterFloatingUI floatingUi;
		private FogHiderOnCenter fogHiderOnCenter;
		protected bool isAlive;
		private bool isInCombat;
		protected Pickable pickable;
		protected LocalSightAgent sightAgent;
		private int SkillDamageFrameCount;
		private SpringManager springManager;
		private LocalCharacterStat stat;
		protected TargetInfoHud targetInfoHud;
		public string Nickname => GetNickname();
		public HostileAgent HostileAgent => GetHostileAgent();
		public LocalSightAgent SightAgent => sightAgent;
		public Animator CharacterAnimator => characterAnimator;
		public CharacterFloatingUI FloatingUi => floatingUi;
		public FogHiderOnCenter FogHiderOnCenter => fogHiderOnCenter;
		public bool IsAlive => isAlive;
		public bool IsInvisible => SightAgent != null && SightAgent.IsInvisible;
		public LocalCharacterStat Stat => stat;
		
		public CharacterStatus Status { get; protected set; }
		public List<CharacterStateValue> States { get; } = new List<CharacterStateValue>();
		public int CharacterCode => GetCharacterCode();
		public int SkinIndex => GetSkinIndex();

		protected virtual bool IsInBush => false;

		public Pickable Pickable => pickable;

		public bool IsInCombat => isInCombat;


		private void Update()
		{
			if (MonoBehaviourInstance<ClientService>.inst == null)
			{
				return;
			}

			if (!MonoBehaviourInstance<ClientService>.inst.IsGameStarted ||
			    MonoBehaviourInstance<ClientService>.inst.IsGameEnd)
			{
				return;
			}

			UpdateInternal();
			UpdateSight();
		}


		private void LateUpdate()
		{
			if (MonoBehaviourInstance<ClientService>.inst == null)
			{
				return;
			}

			if (!MonoBehaviourInstance<ClientService>.inst.IsGameStarted ||
			    MonoBehaviourInstance<ClientService>.inst.IsGameEnd)
			{
				return;
			}

			UpdateSkillDamage();
		}


		private void OnDestroy()
		{
			if (floatingUi != null)
			{
				floatingUi.Hide();
			}
		}


		public virtual void OnSight()
		{
			UpdateInvisible(IsInvisible);
			EnableSpringManager(true);
		}


		public virtual void OnHide()
		{
			pickable.EnablePickable(false);
			CharacterRenderer characterRenderer = this.characterRenderer;
			if (characterRenderer != null)
			{
				characterRenderer.EnableRenderer(false);
			}

			HideMiniMapIcon(MiniMapIconType.Sight);
			EnableSpringManager(false);
		}


		Transform ISightEventHandler.transform => base.transform;

		protected abstract int GetCharacterCode();


		protected virtual int GetSkinIndex()
		{
			return 0;
		}


		protected abstract string GetNickname();


		protected abstract HostileAgent GetHostileAgent();


		protected override bool GetIsOutSight()
		{
			return false;
		}


		protected abstract GameObject GetCharacterPrefab();


		protected override ColliderAgent GetColliderAgent()
		{
			return colliderAgent;
		}


		public override string GetLocalizedName(bool includeColor)
		{
			if (!includeColor)
			{
				return LnUtil.GetCharacterName(CharacterCode);
			}

			if (MonoBehaviourInstance<ClientService>.inst.IsAlly(this))
			{
				return StringUtil.GetColorString(StringUtil.ColorStringType.Ally,
					LnUtil.GetCharacterName(CharacterCode));
			}

			return StringUtil.GetColorString(StringUtil.ColorStringType.Enemy, LnUtil.GetCharacterName(CharacterCode));
		}


		public override void Init(byte[] snapshotData)
		{
			CharacterSnapshot characterSnapshot = serializer.Deserialize<CharacterSnapshot>(snapshotData);
			InitMoveAgent(characterSnapshot.moveAgentSnapshot);
			if (characterObject == null)
			{
				InstantiateCharacterObject();
				characterAnimator = characterObject.GetComponent<Animator>();
				if (characterAnimator == null)
				{
					characterAnimator = characterObject.GetComponentInChildren<Animator>();
				}

				GameUtil.BindOrAdd<CharacterFloatingUI>(gameObject, ref floatingUi);
				floatingUi.Init(this);
				GameUtil.BindOrAdd<FogHiderOnCenter>(gameObject, ref fogHiderOnCenter);
				fogHiderOnCenter.Init(ObjectType);
				GameUtil.BindOrAdd<CharacterRenderer>(gameObject, ref characterRenderer);
				characterRenderer.Init(characterObject);
				characterRenderer.SetMaterial(MaterialSwitchType.InBush, IsInBush);
				springManager = characterObject.GetComponent<SpringManager>();
				if (springManager != null)
				{
					springManager.enabled = false;
					springManager.isRendering -= IsRendering;
					springManager.isRendering += IsRendering;
				}
			}

			if (characterSnapshot.moveAgentSnapshot != null)
			{
				if (characterSnapshot.moveAgentSnapshot.isInBush)
				{
					InBush();
				}
				else
				{
					OutBush();
				}
			}

			stat = CreateCharacterStat();
			stat.Update(characterSnapshot.initialStat);
			InitStatus(characterSnapshot.statusSnapshot);
			isAlive = 0 < Status.Hp;
			GameUtil.BindOrAdd<CharacterColliderAgent>(gameObject, ref colliderAgent);
			colliderAgent.Init(stat.Radius);
			collisionObject = new CollisionCircle3D(GetPosition(), stat.Radius);
			GameUtil.BindOrAdd<LocalSightAgent>(gameObject, ref sightAgent);
			sightAgent.InitCharacterSight(this);
			sightAgent.SetDetect(true, false);
			sightAgent.SetIsInvisible(characterSnapshot.isInvisible);
			pickable = AttachPickable(characterObject);
			pickable.Init(this);
			OnUpdateStat();
			if (!isAlive)
			{
				CharacterFloatingUI characterFloatingUI = floatingUi;
				if (characterFloatingUI != null)
				{
					characterFloatingUI.SetDead();
				}
			}

			MonoBehaviourInstance<BushManager>.inst.OnBushEvevnt -= OnBushEvent;
			MonoBehaviourInstance<BushManager>.inst.OnBushEvevnt += OnBushEvent;
			if (characterObject != null)
			{
				activeOnHostileType = characterObject.GetComponent<ActiveOnHostileType>();
			}
		}


		protected abstract LocalCharacterStat CreateCharacterStat();


		protected virtual void InitMoveAgent(MoveAgentSnapshot snapshot) { }


		private bool IsRendering()
		{
			return characterRenderer != null && characterRenderer.IsViewRendering();
		}


		public void InBush()
		{
			characterRenderer.SetMaterial(MaterialSwitchType.InBush, true);
		}


		public void OutBush()
		{
			characterRenderer.SetMaterial(MaterialSwitchType.InBush, false);
		}


		protected virtual void OnBushEvent(bool isInBush, int objectId)
		{
			if (this.objectId == objectId)
			{
				characterRenderer.SetMaterial(MaterialSwitchType.InBush, isInBush);
			}
		}


		public void InitStateEffect(List<CharacterStateValue> initialStateEffect)
		{
			foreach (CharacterStateValue characterStateValue in new List<CharacterStateValue>(States))
			{
				RemoveEffectState(characterStateValue.Group, characterStateValue.CasterId);
			}

			foreach (CharacterStateValue addState in initialStateEffect)
			{
				AddEffectState(addState);
			}
		}


		protected virtual void InitStatus(byte[] snapshot)
		{
			CharacterStatusSnapshot snapshot2 = serializer.Deserialize<CharacterStatusSnapshot>(snapshot);
			Status = new CharacterStatus(snapshot2);
		}


		protected virtual void InstantiateCharacterObject()
		{
			characterObject = Instantiate<GameObject>(GetCharacterPrefab(), base.transform);
			characterObject.transform.localPosition = Vector3.zero;
			characterObject.transform.localRotation = Quaternion.identity;
		}


		public virtual void OnDead(LocalCharacter attacker)
		{
			if (IsInvisible)
			{
				UpdateInvisible(false);
			}

			if (sightAgent != null && sightAgent.GetOwner() != null)
			{
				sightAgent.GetOwner().RemoveAllySight(sightAgent);
			}

			List<CharacterStateValue> list = States;
			if (list != null)
			{
				list.Clear();
			}

			CharacterStatus characterStatus = Status;
			if (characterStatus != null)
			{
				characterStatus.SubHp(stat.MaxHp);
			}

			CharacterStatus characterStatus2 = Status;
			if (characterStatus2 != null)
			{
				characterStatus2.SubSp(stat.MaxSp);
			}

			CharacterStatus characterStatus3 = Status;
			if (characterStatus3 != null)
			{
				characterStatus3.UpdateBullet(0);
			}

			isAlive = false;
			PlayDeadAnimation();
			if (characterRenderer != null && characterRenderer.IsRendering)
			{
				ShowMiniMapIcon(MiniMapIconType.Sight);
			}

			if (floatingUi != null)
			{
				floatingUi.SetDead();
			}

			if (targetInfoHud != null)
			{
				targetInfoHud.SetTargetState(States);
			}
		}


		public void SetCharacterAnimatorTrigger(string triggerName, bool reset = true)
		{
			if (characterAnimator == null)
			{
				return;
			}

			if (reset)
			{
				characterAnimator.ResetTrigger(GameConstants.AnimationKey.ANIMATION_CANCEL_TRIGGER_KEY);
			}

			characterAnimator.SetTrigger(triggerName);
		}


		public void SetCharacterAnimatorResetTrigger(int id)
		{
			if (characterAnimator == null)
			{
				return;
			}

			characterAnimator.ResetTrigger(id);
		}


		public void SetCharacterAnimatorTrigger(int id)
		{
			if (characterAnimator == null)
			{
				return;
			}

			characterAnimator.SetTrigger(id);
		}


		public void SetCharacterAnimatorBool(string boolName, bool isTrue)
		{
			if (characterAnimator == null)
			{
				return;
			}

			characterAnimator.SetBool(boolName, isTrue);
		}


		public void SetCharacterAnimatorBool(int id, bool isTrue)
		{
			if (characterAnimator == null)
			{
				return;
			}

			characterAnimator.SetBool(id, isTrue);
		}


		public bool? GetCharacterAnimatorBool(int id)
		{
			if (characterAnimator == null)
			{
				return null;
			}

			return characterAnimator.GetBool(id);
		}


		public void SetCharacterAnimatorFloat(int id, float value)
		{
			if (characterAnimator == null)
			{
				return;
			}

			characterAnimator.SetFloat(id, value);
		}


		public float? GetCharacterAnimatorFloat(int id)
		{
			if (characterAnimator == null)
			{
				return null;
			}

			return characterAnimator.GetFloat(id);
		}


		public void SetCharacterAnimatorInteger(int id, int value)
		{
			if (characterAnimator == null)
			{
				return;
			}

			characterAnimator.SetInteger(id, value);
		}


		public int? GetCharacterAnimatorInteger(int id)
		{
			if (characterAnimator == null)
			{
				return null;
			}

			return characterAnimator.GetInteger(id);
		}


		public void SetCharacterAnimatorEnable(bool enable)
		{
			if (characterAnimator == null)
			{
				return;
			}

			characterAnimator.enabled = enable;
		}


		public int GetCharacterAnimatorLayerIndex(string layerName)
		{
			if (characterAnimator != null)
			{
				return characterAnimator.GetLayerIndex(layerName);
			}

			Log.E("[EXCEPTION] 'Animator' is null!!");
			return -1;
		}


		public int GetCharacterAnimatorLayerCount()
		{
			if (characterAnimator != null)
			{
				return characterAnimator.layerCount;
			}

			Log.E("[EXCEPTION] 'Animator' is null!!");
			return 0;
		}


		public float GetCharacterAnimatorLayerWeight(int layerIndex)
		{
			if (characterAnimator != null)
			{
				return characterAnimator.GetLayerWeight(layerIndex);
			}

			Log.E("[EXCEPTION] 'Animator' is null!!");
			return -1f;
		}


		public void SetCharacterAnimatorLayerWeight(int layerIndex, float weight)
		{
			if (characterAnimator == null)
			{
				Log.E("[EXCEPTION] 'Animator' is null!!");
				return;
			}

			characterAnimator.SetLayerWeight(layerIndex, weight);
		}


		public void SetCharacterAnimatorLogWarnings(bool value)
		{
			if (characterAnimator == null)
			{
				Log.E("[EXCEPTION] 'Animator' is null!!");
				return;
			}

			characterAnimator.logWarnings = value;
		}


		public virtual void SetAnimatorCullingMode(AnimatorCullingMode cullingMode)
		{
			if (characterAnimator == null)
			{
				Log.E("[EXCEPTION] 'Animator' is null!!");
				return;
			}

			characterAnimator.cullingMode = cullingMode;
		}


		protected virtual void PlayDeadAnimation()
		{
			SetCharacterAnimatorBool(AnimID_Dead, true);
		}


		public void AddDamage(int attackerId, bool isSkillDamage, int damage, bool isCritical, int effectCode)
		{
			if (Status != null)
			{
				Status.SubHp(damage);
				if (floatingUi != null)
				{
					floatingUi.SetHp(Status.Hp, stat.MaxHp);
				}

				if (MonoBehaviourInstance<GameUI>.inst != null)
				{
					MonoBehaviourInstance<GameUI>.inst.HudUpdateHp(ObjectId, Status.Hp, stat.MaxHp);
				}

				if (targetInfoHud != null)
				{
					targetInfoHud.SetTargetStat(new UITargetInfoHudStat(stat, Status));
				}
			}

			SkillDamageInfo skillDamageInfo = skillDamageQueue.Find(info =>
				info.attackerId.Equals(attackerId) && info.isCritical.Equals(isCritical) &&
				info.isSkillDamage.Equals(isSkillDamage));
			if (skillDamageInfo != null)
			{
				skillDamageInfo.damage += damage;
				damage = 0;
			}

			skillDamageInfo = new SkillDamageInfo
			{
				attackerId = attackerId,
				damage = damage,
				effectCode = effectCode,
				isSkillDamage = isSkillDamage,
				isCritical = isCritical
			};
			skillDamageQueue.Add(skillDamageInfo);
		}


		public virtual void OnDamage(int attackerId, bool isSkillDamage, int damage, bool isCritical, int effectCode)
		{
			if (Status != null)
			{
				Status.SubHp(damage);
				if (floatingUi != null)
				{
					floatingUi.SetHp(Status.Hp, stat.MaxHp);
				}

				if (MonoBehaviourInstance<GameUI>.inst != null)
				{
					MonoBehaviourInstance<GameUI>.inst.HudUpdateHp(ObjectId, Status.Hp, stat.MaxHp);
				}

				if (targetInfoHud != null)
				{
					targetInfoHud.SetTargetStat(new UITargetInfoHudStat(stat, Status));
				}
			}

			ShowDamage(attackerId, isSkillDamage, damage, isCritical, effectCode);
		}


		private void ShowDamage(int attackerId, bool isSkillDamage, int damage, bool isCritical, int effectCode,
			float offset = 2f)
		{
			if (characterRenderer == null || !characterRenderer.IsRendering)
			{
				return;
			}

			LocalObject localObject = null;
			Vector3 direction = Vector3.zero;
			ClientService inst = MonoBehaviourInstance<ClientService>.inst;
			if (inst != null && inst.World.TryFind<LocalObject>(attackerId, ref localObject))
			{
				direction = GetPosition() - localObject.GetPosition();
			}

			if (floatingUi != null && damage > 0)
			{
				floatingUi.ShowDamage(isSkillDamage, damage, isCritical, direction, offset);
			}

			PlayLocalDamagedEffect(attackerId == 0 ? this : localObject, effectCode);
		}


		public void OnSpDamage(int attackerId, int damage, int effectCode)
		{
			if (Status != null)
			{
				Status.SubSp(damage);
				if (floatingUi != null)
				{
					floatingUi.SetSp(Status.Sp, stat.MaxSp);
				}

				if (MonoBehaviourInstance<GameUI>.inst != null)
				{
					MonoBehaviourInstance<GameUI>.inst.HudUpdateSp(ObjectId, Status.Sp, stat.MaxSp);
				}

				if (targetInfoHud != null)
				{
					targetInfoHud.SetTargetStat(new UITargetInfoHudStat(stat, Status));
				}
			}

			ShowSpDamage(attackerId, damage, effectCode);
		}


		private void ShowSpDamage(int attackerId, int damage, int effectCode)
		{
			if (characterRenderer != null && characterRenderer.IsRendering)
			{
				LocalObject localObject = null;
				Vector3 direction = Vector3.zero;
				ClientService inst = MonoBehaviourInstance<ClientService>.inst;
				if (inst != null && inst.World.TryFind<LocalObject>(attackerId, ref localObject))
				{
					direction = GetPosition() - localObject.GetPosition();
				}

				if (floatingUi != null)
				{
					floatingUi.ShowSpDamage(damage, direction);
				}

				PlayLocalDamagedEffect(attackerId == 0 ? this : localObject, effectCode);
			}
		}


		public virtual void OnBlock(int blockedDamage)
		{
			if (characterRenderer != null && characterRenderer.IsRendering && floatingUi != null)
			{
				floatingUi.ShowBlock(blockedDamage);
			}
		}


		public void OnRegenHeal(int addHp, int addSp, int stateCode, int effectCode, bool showUI)
		{
			SetStatusByHeal(addHp, addSp);
			if (!showUI || stateCode <= 0)
			{
				OnHealEffectTextRendering(addHp, addSp, effectCode, showUI);
				return;
			}

			if (GetCharacterStateValueByCode(stateCode, objectId) == null)
			{
				OnHealEffectTextRendering(addHp, addSp, effectCode, showUI);
				return;
			}

			if (IsShowRegenHealTextUI(stateCode, addHp, addSp))
			{
				regenHealInfo.Add(addHp, addSp);
				OnHealEffectTextRendering(regenHealInfo.AddHp, regenHealInfo.AddSp, effectCode, showUI);
				regenHealInfo.Clear();
				return;
			}

			regenHealInfo.Add(addHp, addSp);
			OnHealEffectTextRendering(0, 0, effectCode, showUI);
		}


		private bool IsShowRegenHealTextUI(int stateCode, int addHp, int addSp)
		{
			if (stateCode == 10001)
			{
				return true;
			}

			CharacterStateValue characterStateValue = null;
			CharacterStateValue characterStateValue2 = null;
			CharacterStateValue characterStateValue3 = null;
			for (int i = 0; i < States.Count; i++)
			{
				if (States[i].code == 10001)
				{
					characterStateValue = States[i];
				}
				else if (States[i].code == 10002)
				{
					characterStateValue2 = States[i];
				}
				else if (States[i].code == 10003)
				{
					characterStateValue3 = States[i];
				}
			}

			if (characterStateValue != null)
			{
				return characterStateValue.code == stateCode;
			}

			if (characterStateValue2 != null && characterStateValue3 != null)
			{
				float num = characterStateValue2.createdTime + characterStateValue2.Duration;
				float num2 = characterStateValue3.createdTime + characterStateValue3.Duration;
				if (num > num2)
				{
					if (Status.Hp >= stat.MaxHp && stateCode.Equals(characterStateValue3.code))
					{
						return true;
					}

					if (stateCode.Equals(characterStateValue2.code))
					{
						return true;
					}
				}

				if (num <= num2)
				{
					if (Status.Sp >= stat.MaxSp && stateCode.Equals(characterStateValue2.code))
					{
						return true;
					}

					if (stateCode.Equals(characterStateValue3.code))
					{
						return true;
					}
				}

				return false;
			}

			return true;
		}


		private void OnHealEffectTextRendering(int addHp, int addSp, int effectCode, bool showUI)
		{
			if (characterRenderer == null)
			{
				return;
			}

			if (characterRenderer.IsRendering && showUI)
			{
				if (0 < addSp && 0 < addHp)
				{
					floatingUi.ShowSpHeal(addSp, 1.7f);
					floatingUi.ShowHpHeal(addHp);
				}
				else if (0 < addSp)
				{
					floatingUi.ShowSpHeal(addSp);
				}
				else if (0 < addHp)
				{
					floatingUi.ShowHpHeal(addHp);
				}
			}

			PlayLocalEffect(effectCode);
			TargetInfoHud targetInfoHud = this.targetInfoHud;
			if (targetInfoHud == null)
			{
				return;
			}

			targetInfoHud.SetTargetStat(new UITargetInfoHudStat(stat, Status));
		}


		private void SetStatusByHeal(int addHp, int addSp)
		{
			if (Status == null)
			{
				return;
			}

			Status.AddHp(addHp);
			Status.AddSp(addSp);
			if (floatingUi != null)
			{
				floatingUi.SetHp(Status.Hp, stat.MaxHp);
				floatingUi.SetSp(Status.Sp, stat.MaxSp);
			}

			GameUI inst = MonoBehaviourInstance<GameUI>.inst;
			if (inst != null)
			{
				inst.HudUpdateHp(ObjectId, Status.Hp, stat.MaxHp);
				inst.HudUpdateSp(ObjectId, Status.Sp, stat.MaxSp);
			}
		}


		public virtual void OnHeal(int addHp, int addSp, int effectCode, bool showUI)
		{
			SetStatusByHeal(addHp, addSp);
			OnHealEffectTextRendering(addHp, addSp, effectCode, showUI);
		}


		public virtual void OnSetExtraPoint(int setExtraPoint) { }


		public virtual void OnEvasion() { }


		public CollisionObject3D GetCollisionObject()
		{
			CollisionObject3D collisionObject3D = collisionObject;
			if (collisionObject3D != null)
			{
				collisionObject3D.UpdatePosition(GetPosition());
			}

			return collisionObject;
		}


		public virtual void InitStat(List<CharacterStatValue> updates, byte[] statusSnapshot)
		{
			LocalCharacterStat localCharacterStat = stat;
			if (localCharacterStat != null)
			{
				localCharacterStat.Update(updates);
			}

			InitStatus(statusSnapshot);
			OnUpdateStat();
		}


		public virtual void UpdateStat(List<CharacterStatValue> updates)
		{
			LocalCharacterStat localCharacterStat = stat;
			if (localCharacterStat != null)
			{
				localCharacterStat.Update(updates);
			}

			CharacterStatus characterStatus = Status;
			if (characterStatus != null)
			{
				characterStatus.SetHp(Mathf.Min(stat.MaxHp, Status.Hp));
			}

			CharacterStatus characterStatus2 = Status;
			if (characterStatus2 != null)
			{
				characterStatus2.SetSp(Mathf.Min(stat.MaxSp, Status.Sp));
			}

			OnUpdateStat();
		}


		private void OnUpdateStat()
		{
			if (sightAgent != null)
			{
				sightAgent.UpdateSightRange(stat.SightRange);
				sightAgent.UpdateSightAngle(stat.SightAngle);
			}

			if (floatingUi != null)
			{
				floatingUi.SetHp(Status.Hp, stat.MaxHp);
				floatingUi.SetSp(Status.Sp, stat.MaxSp);
				floatingUi.UpdateLevel(Status.Level);
			}

			if (FogHiderOnCenter != null)
			{
				FogHiderOnCenter.SetRadius(stat.Radius);
			}

			GameUI inst = MonoBehaviourInstance<GameUI>.inst;
			if (inst != null)
			{
				inst.HudUpdateLevel(ObjectId, Status.Level);
				inst.HudUpdateHp(ObjectId, Status.Hp, stat.MaxHp);
				inst.HudUpdateSp(ObjectId, Status.Sp, stat.MaxSp);
			}

			if (targetInfoHud != null)
			{
				targetInfoHud.SetTargetStat(new UITargetInfoHudStat(stat, Status));
			}
		}


		public void UpdateSight()
		{
			OnUpdateSight();
		}


		private void OnUpdateSight()
		{
			UpdateMapIconPos();
		}


		protected void EnableSpringManager(bool enable)
		{
			if (springManager != null)
			{
				springManager.enabled = enable;
			}
		}


		protected virtual Sprite GetMapIcon()
		{
			return null;
		}


		public virtual void ShowMiniMapIcon(MiniMapIconType miniMapIconType)
		{
			GameUI inst = MonoBehaviourInstance<GameUI>.inst;
			if (inst == null)
			{
				return;
			}

			inst.Minimap.UIMap.CreateNonPlayer(ObjectId, GetPosition(), GetMapIcon(), miniMapIconType);
		}


		public virtual void HideMiniMapIcon(MiniMapIconType miniMapIconType)
		{
			GameUI inst = MonoBehaviourInstance<GameUI>.inst;
			if (inst == null)
			{
				return;
			}

			inst.Minimap.UIMap.RemoveNonPlayer(ObjectId);
		}


		public virtual void ShowMapIcon(MiniMapIconType miniMapIconType)
		{
			GameUI inst = MonoBehaviourInstance<GameUI>.inst;
			if (inst == null)
			{
				return;
			}

			inst.MapWindow.UIMap.CreateNonPlayer(ObjectId, GetPosition(), GetMapIcon(), miniMapIconType);
		}


		public virtual void HideMapIcon(MiniMapIconType miniMapIconType)
		{
			GameUI inst = MonoBehaviourInstance<GameUI>.inst;
			if (inst == null)
			{
				return;
			}

			inst.MapWindow.UIMap.RemoveNonPlayer(ObjectId);
		}


		protected virtual void UpdateMapIconPos()
		{
			if (characterRenderer != null && characterRenderer.IsRendering)
			{
				GameUI inst = MonoBehaviourInstance<GameUI>.inst;
				if (inst == null)
				{
					return;
				}

				inst.MapWindow.UpdateMapNonPlayerPosition(ObjectId, GetPosition());
				inst.Minimap.UIMap.UpdateNonPlayerPosition(ObjectId, GetPosition());
			}
		}


		public virtual void UpdateMapIcon()
		{
			GameUI inst = MonoBehaviourInstance<GameUI>.inst;
			if (inst == null)
			{
				return;
			}

			inst.MapWindow.UIMap.UpdateNonPlayerIcon(objectId, GetMapIcon());
			inst.Minimap.UIMap.UpdateNonPlayerIcon(objectId, GetMapIcon());
		}


		public void UpdateMapIconDyingCondition()
		{
			ClientService inst = MonoBehaviourInstance<ClientService>.inst;
			if (inst == null)
			{
				return;
			}

			GameUI inst2 = MonoBehaviourInstance<GameUI>.inst;
			if (inst2 != null)
			{
				return;
			}

			if (inst.MyObjectId == objectId || inst.IsAlly(this))
			{
				inst2.MapWindow.UIMap.DyingCondition(ObjectId);
				inst2.Minimap.UIMap.DyingCondition(ObjectId);
			}
		}


		public void UpdateMapIconRevival()
		{
			ClientService inst = MonoBehaviourInstance<ClientService>.inst;
			if (inst == null)
			{
				return;
			}

			GameUI inst2 = MonoBehaviourInstance<GameUI>.inst;
			if (inst2 != null)
			{
				return;
			}

			if (inst.MyObjectId == objectId || inst.IsAlly(this))
			{
				inst2.MapWindow.UIMap.Revival(ObjectId);
				inst2.Minimap.UIMap.Revival(ObjectId);
			}
		}


		public void SwitchMaterialChildManualFromDefault(string childName, int index, string materialName)
		{
			if (IsInvisible)
			{
				return;
			}

			if (string.IsNullOrEmpty(childName) || string.IsNullOrEmpty(materialName))
			{
				return;
			}

			Material defaultMaterial = SingletonMonoBehaviour<ResourceManager>.inst.GetDefaultMaterial(materialName);
			Transform transform = base.transform.FindRecursively(childName);
			if (transform == null)
			{
				return;
			}

			Renderer component = transform.GetComponent<Renderer>();
			Material[] sharedMaterials = component.sharedMaterials;
			sharedMaterials[index] = defaultMaterial;
			component.sharedMaterials = sharedMaterials;
			MaterialSwitcher component2 = transform.GetComponent<MaterialSwitcher>();
			if (component2 != null)
			{
				component2.SetNewOrinalMaterials(index);
			}
		}


		public void SwitchMaterialChildManualFromEffect(string childName, int index, string materialName)
		{
			if (IsInvisible)
			{
				return;
			}

			if (string.IsNullOrEmpty(childName) || string.IsNullOrEmpty(materialName))
			{
				return;
			}

			Material effectMaterial = SingletonMonoBehaviour<ResourceManager>.inst.GetEffectMaterial(materialName);
			Transform transform = base.transform.FindRecursively(childName);
			if (transform == null)
			{
				return;
			}

			Renderer component = transform.GetComponent<Renderer>();
			Material[] sharedMaterials = component.sharedMaterials;
			sharedMaterials[index] = effectMaterial;
			component.sharedMaterials = sharedMaterials;
			MaterialSwitcher component2 = transform.GetComponent<MaterialSwitcher>();
			if (component2 != null)
			{
				component2.SetNewOrinalMaterials(index);
			}
		}


		public void ResetMaterialChildManual(string childName, int index)
		{
			if (string.IsNullOrEmpty(childName))
			{
				return;
			}

			Transform transform = base.transform.FindRecursively(childName);
			if (transform == null)
			{
				return;
			}

			Renderer component = transform.GetComponent<Renderer>();
			Material[] sharedMaterials = component.sharedMaterials;
			sharedMaterials[index] = null;
			component.sharedMaterials = sharedMaterials;
		}


		public void AttachTargetInfoHud(TargetInfoHud targetInfoHud)
		{
			this.targetInfoHud = targetInfoHud;
		}


		public void DetachTargetInfoHud()
		{
			targetInfoHud = null;
		}


		public CharacterStateValue GetCharacterStateValueByGroup(int stateGroup, int casterId)
		{
			if (casterId != 0)
			{
				return States.Find(x => x.Group == stateGroup && x.CasterId == casterId);
			}

			return States.Find(x => x.Group == stateGroup);
		}


		public CharacterStateValue GetCharacterStateValueByCode(int stateCode, int casterId)
		{
			if (casterId != 0)
			{
				return States.Find(x => x.code == stateCode && x.CasterId == casterId);
			}

			return States.Find(x => x.code == stateCode);
		}


		public int GetStateStackByGroup(int stateGroup, int casterId)
		{
			CharacterStateValue characterStateValueByGroup = GetCharacterStateValueByGroup(stateGroup, casterId);
			if (characterStateValueByGroup == null)
			{
				return 0;
			}

			return characterStateValueByGroup.StackCount;
		}


		public int GetMaxStateStackByGroup(int stateGroup)
		{
			int num = 0;
			foreach (CharacterStateValue characterStateValue in States)
			{
				if (characterStateValue.Group == stateGroup && num < characterStateValue.StackCount)
				{
					num = characterStateValue.StackCount;
				}
			}

			return num;
		}


		protected virtual void OnAddEffectState(CharacterStateValue state) { }


		public void AddEffectState(CharacterStateValue addState)
		{
			RemoveEffectState(addState.Group, addState.CasterId);
			if (addState.StateType != StateType.Slow && addState.StateType.IsShowFloatingUi())
			{
				bool flag = true;
				if (curFloatingUiState != null && curFloatingUiState.createdTime == addState.createdTime)
				{
					flag = addState.StateType.IsUpperPriority(curFloatingUiState.StateType);
				}

				if (flag)
				{
					curFloatingUiState = addState;
					if (floatingUi != null)
					{
						floatingUi.SetStateName(Ln.Get(curFloatingUiState.StateType.ToString()));
					}
				}

				floatingUiStates.Add(addState);
			}

			States.Add(addState);
			if (!hasStateType.Contains(addState.StateType))
			{
				hasStateType.Add(addState.StateType);
			}

			if (targetInfoHud != null)
			{
				targetInfoHud.AddTargetState(addState);
			}

			OnAddEffectState(addState);
			SingletonMonoBehaviour<LocalBattleEventCollector>.inst.OnAddEffectState(addState, this);
		}


		protected virtual void OnRemoveEffectState(CharacterStateValue state) { }


		public void RemoveEffectState(int group, int casterId)
		{
			CharacterStateValue characterStateValueByGroup = GetCharacterStateValueByGroup(group, casterId);
			if (characterStateValueByGroup == null)
			{
				return;
			}

			if (targetInfoHud != null)
			{
				targetInfoHud.RemoveTargetState(characterStateValueByGroup);
			}

			States.Remove(characterStateValueByGroup);
			bool flag = false;
			using (List<CharacterStateValue>.Enumerator enumerator = States.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.StateType == characterStateValueByGroup.StateType)
					{
						flag = true;
						break;
					}
				}
			}

			if (!flag)
			{
				hasStateType.Remove(characterStateValueByGroup.StateType);
			}

			OnRemoveEffectState(characterStateValueByGroup);
			SingletonMonoBehaviour<LocalBattleEventCollector>.inst.OnRemoveEffectState(characterStateValueByGroup,
				this);
			if (floatingUiStates.Contains(characterStateValueByGroup))
			{
				floatingUiStates.Remove(characterStateValueByGroup);
				if (curFloatingUiState != null && curFloatingUiState.code == characterStateValueByGroup.code)
				{
					if (floatingUiStates.Count == 0)
					{
						if (floatingUi != null)
						{
							floatingUi.ResetStateName();
						}

						curFloatingUiState = null;
						return;
					}

					curFloatingUiState = floatingUiStates[floatingUiStates.Count - 1];
					int num = floatingUiStates.Count - 2;
					while (num >= 0 && curFloatingUiState.createdTime == floatingUiStates[num].createdTime)
					{
						if (floatingUiStates[num].StateType.IsUpperPriority(curFloatingUiState.StateType))
						{
							curFloatingUiState = floatingUiStates[num];
						}

						num--;
					}

					if (floatingUi != null)
					{
						floatingUi.SetStateName(Ln.Get(curFloatingUiState.StateType.ToString()));
					}
				}
			}
		}


		protected virtual void OnUpdateEffectState(CharacterStateValue state) { }


		public void UpdateEffectState(int group, int casterId, int stack, int reserve, float duration,
			float createdTime)
		{
			CharacterStateValue characterStateValueByGroup = GetCharacterStateValueByGroup(group, casterId);
			if (characterStateValueByGroup == null)
			{
				return;
			}

			characterStateValueByGroup.SetStackCount(stack);
			characterStateValueByGroup.SetReserveCount(reserve);
			characterStateValueByGroup.SetDuration(duration);
			characterStateValueByGroup.SetCreateTime(createdTime);
			if (targetInfoHud != null)
			{
				targetInfoHud.ChangeTargetState(characterStateValueByGroup);
			}

			OnUpdateEffectState(characterStateValueByGroup);
		}


		public void ResetCreateTimeEffectState(int group, int casterId)
		{
			CharacterStateValue characterStateValueByGroup = GetCharacterStateValueByGroup(group, casterId);
			if (characterStateValueByGroup == null)
			{
				return;
			}

			characterStateValueByGroup.SetCreateTime(MonoBehaviourInstance<ClientService>.inst.CurrentServerFrameTime);
			if (targetInfoHud != null)
			{
				targetInfoHud.ChangeTargetState(characterStateValueByGroup);
			}

			OnUpdateEffectState(characterStateValueByGroup);
		}


		public virtual void DurationPauseState(int group, int casterId, float durationPauseEndTime, float duration)
		{
			CharacterStateValue characterStateValueByGroup = GetCharacterStateValueByGroup(group, casterId);
			if (characterStateValueByGroup == null)
			{
				return;
			}

			characterStateValueByGroup.SetDurationPauseEndTime(durationPauseEndTime);
			characterStateValueByGroup.SetDuration(duration);
			if (targetInfoHud != null)
			{
				targetInfoHud.ChangeTargetState(characterStateValueByGroup);
			}

			OnUpdateEffectState(characterStateValueByGroup);
		}


		public virtual void UpdateInvisible(bool isInvisible)
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

			int myObjectId = inst.MyObjectId;
			if (myObjectId == objectId)
			{
				GameUI inst2 = MonoBehaviourInstance<GameUI>.inst;
				if (inst2 != null)
				{
					inst2.StelthOverlayUI.SetActive(IsInvisible);
				}
			}

			if (myObjectId == objectId || inst.IsInAllySight(sightAgent, GetPosition(), stat.Radius,
				sightAgent != null && sightAgent.IsInvisibleCheckWithMemorizer(myObjectId)))
			{
				pickable.EnablePickable(true);
				if (characterRenderer != null)
				{
					characterRenderer.EnableRenderer(true);
				}

				if (floatingUi != null)
				{
					floatingUi.Show();
					floatingUi.SetAutoVisible(true);
				}

				ShowMiniMapIcon(MiniMapIconType.Sight);
			}
			else
			{
				pickable.EnablePickable(false);
				if (characterRenderer != null)
				{
					characterRenderer.EnableRenderer(false);
				}

				if (floatingUi != null)
				{
					floatingUi.Hide();
					floatingUi.SetAutoVisible(false);
				}

				HideMiniMapIcon(MiniMapIconType.Sight);
			}

			if (characterRenderer != null)
			{
				characterRenderer.SetMaterial(MaterialSwitchType.Stealth, IsInvisible);
			}
		}


		public void ExtraPointBarSettingColor(Color? clr)
		{
			floatingUi.SettingForceEpGaugeColor(clr);
			MonoBehaviourInstance<GameUI>.inst.StatusHud.SettingForceEpGaugeColor(clr);
		}


		public override void DestroySelf()
		{
			MonoBehaviourInstance<GameUI>.inst.Minimap.UIMap.RemoveNonPlayer(ObjectId);
			MonoBehaviourInstance<GameUI>.inst.MapWindow.UIMap.RemoveNonPlayer(ObjectId);
		}


		public override GameObject ReleaseChildren()
		{
			base.ReleaseChildren();
			GameObject result = characterObject;
			characterRenderer.Clear();
			characterObject = null;
			characterAnimator = null;
			springManager = null;
			return result;
		}


		public override void OnEffectCreated(GameObject obj, bool addCharacterRenderer)
		{
			base.OnEffectCreated(obj, addCharacterRenderer);
			if (addCharacterRenderer)
			{
				characterRenderer.AddChildEffect(obj);
				if (obj != null)
				{
					FogHiderOnCenter component = obj.GetComponent<FogHiderOnCenter>();
					if (component != null)
					{
						component.SetRadius(Stat.Radius);
						component.SetSightAgent(SightAgent);
					}
				}
			}
		}


		protected virtual void UpdateInternal() { }


		protected void UpdateSkillDamage()
		{
			if (skillDamageQueue.Count <= 0)
			{
				SkillDamageFrameCount = 0;
				return;
			}

			SkillDamageFrameCount++;
			if (SkillDamageFrameCount < 5)
			{
				return;
			}

			SkillDamageFrameCount = 0;
			skillDamageQueue.Sort((info, damageInfo) => info.isSkillDamage.CompareTo(damageInfo.isSkillDamage));
			for (int i = 0; i < skillDamageQueue.Count; i++)
			{
				if (skillDamageQueue.Count < 2)
				{
					ShowDamage(skillDamageQueue[i].attackerId, skillDamageQueue[i].isSkillDamage,
						skillDamageQueue[i].damage, skillDamageQueue[i].isCritical, skillDamageQueue[i].effectCode);
				}
				else
				{
					float offset = skillDamageQueue[i].isSkillDamage ? 2f : 1.7f;
					ShowDamage(skillDamageQueue[i].attackerId, skillDamageQueue[i].isSkillDamage,
						skillDamageQueue[i].damage, skillDamageQueue[i].isCritical, skillDamageQueue[i].effectCode,
						offset);
				}

				SingletonMonoBehaviour<LocalBattleEventCollector>.inst.OnAfterSkillDamage(this, skillDamageQueue[i]);
			}

			skillDamageQueue.Clear();
		}


		public bool IsCurrentAnimatorStateAnyName(int index, params string[] stateNames)
		{
			if (characterAnimator == null)
			{
				return false;
			}

			AnimatorStateInfo currentAnimatorStateInfo = characterAnimator.GetCurrentAnimatorStateInfo(index);
			foreach (string name in stateNames)
			{
				if (currentAnimatorStateInfo.IsName(name))
				{
					return true;
				}
			}

			return false;
		}


		public void RotationLocalObject(Vector3 direction)
		{
			if (characterObject != null)
			{
				characterObject.transform.rotation = GameUtil.LookRotation(direction);
			}
		}


		public virtual void SetIsInCombat(bool isCombat)
		{
			isInCombat = isCombat;
		}


		public void Airborne(float airborneDuration, float airbornePower)
		{
			if (characterObject == null)
			{
				return;
			}

			if (airborneRoutine != null)
			{
				StopCoroutine(airborneRoutine);
			}

			airborneRoutine = this.StartThrowingCoroutine(_Airborne(airborneDuration, airbornePower),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][Airborne] Message:" + exception.Message + ", StackTrace:" +
					      exception.StackTrace);
				});
		}


		private IEnumerator _Airborne(float airborneDuration, float airbornePower)
		{
			Vector3 beginPos = Vector3.zero;
			beginPos.y = characterObject.transform.localPosition.y;
			Vector3 middlePos = Vector3.zero;
			middlePos.y = EasingFunction.GetEasingFunction(EasingFunction.Ease.EaseOutQuad)(0.5f, 2.5f, airbornePower);
			middlePos.y +=
				EasingFunction.GetEasingFunction(EasingFunction.Ease.EaseOutQuad)(0f, 1.25f, beginPos.y / 2.5f);
			float startTime = Time.time;
			while (Time.time - startTime <= airborneDuration)
			{
				float num = (Time.time - startTime) / airborneDuration;
				Vector3 localPosition = beginPos;
				if (num < 0.5f)
				{
					float t = EasingFunction.GetEasingFunction(EasingFunction.Ease.EaseOutQuad)(0f, 1f, num * 2f);
					localPosition.y = GameUtil.CalculateLinearBezierPoint(t, beginPos, middlePos);
				}
				else
				{
					float t2 = EasingFunction.GetEasingFunction(EasingFunction.Ease.EaseInQuad)(0f, 1f,
						(num - 0.5f) * 2f);
					localPosition.y = GameUtil.CalculateLinearBezierPoint(t2, middlePos, Vector3.zero);
				}

				characterObject.transform.localPosition = localPosition;
				yield return null;
			}

			characterObject.transform.localPosition = Vector3.zero;
		}


		public virtual void OnChangeDayNight(DayNight dayNight) { }


		public bool IsUntargetable()
		{
			return hasStateType.Contains(StateType.Untargetable);
		}


		public bool IsPlayingSkill(SkillId skillId)
		{
			return LocalSkillPlayer.IsPlaying(skillId);
		}


		public void LaunchProjectile(LocalProjectile projectile)
		{
			ownProjectiles.Add(projectile);
		}


		public void RemoveOwnProjectile(LocalProjectile removeTarget)
		{
			ownProjectiles.Remove(removeTarget);
		}


		public LocalProjectile GetOwnProjectile(Func<LocalProjectile, bool> condition)
		{
			for (int i = ownProjectiles.Count - 1; i >= 0; i--)
			{
				if (ownProjectiles[i] == null || !ownProjectiles[i].IsAlive)
				{
					ownProjectiles.RemoveAt(i);
				}
				else if (condition(ownProjectiles[i]))
				{
					return ownProjectiles[i];
				}
			}

			return null;
		}


		public List<LocalProjectile> GetOwnProjectiles(Func<LocalProjectile, bool> condition)
		{
			List<LocalProjectile> list = null;
			for (int i = ownProjectiles.Count - 1; i >= 0; i--)
			{
				if (ownProjectiles[i] == null || !ownProjectiles[i].IsAlive)
				{
					ownProjectiles.RemoveAt(i);
				}
				else if (condition(ownProjectiles[i]))
				{
					if (list == null)
					{
						list = new List<LocalProjectile>();
					}

					list.Add(ownProjectiles[i]);
				}
			}

			return list;
		}


		public override bool IsMouseHitPossible(LocalSightAgent targetSightAgent, bool isInvisible)
		{
			return targetSightAgent.IsInAllySight(SightAgent, GetPosition(), Stat.Radius, isInvisible);
		}


		public bool IsInAllySight(LocalSightAgent targetSightAgent)
		{
			return targetSightAgent.IsInAllySight(SightAgent, GetPosition(), Stat.Radius,
				SightAgent.IsInvisibleCheckWithMemorizer(targetSightAgent.SightOwnerId));
		}


		public override bool SetCursor(LocalPlayerCharacter myPlayer)
		{
			if (!IsInAllySight(myPlayer.SightAgent))
			{
				MonoBehaviourInstance<GameInput>.inst.GameCursor.SetCursorTarget(CursorTarget.None);
				return true;
			}

			return false;
		}


		public void InSight(Vector3 position, MoveAgentSnapshot moveAgentSnapshot)
		{
			SetPosition(position);
			ILocalMoveAgentOwner localMoveAgentOwner;
			if ((localMoveAgentOwner = this as ILocalMoveAgentOwner) != null)
			{
				localMoveAgentOwner.MoveAgent.ApplySnapshot(moveAgentSnapshot,
					MonoBehaviourInstance<ClientService>.inst.World);
			}

			InSight();
		}


		public virtual void InSight()
		{
			SightAgent.InSightOnServer();
		}


		public virtual void OutSight()
		{
			SetPosition(GameConstants.CharacterOutSightPosition);
			SightAgent.OutSightOnServer();
		}


		public virtual void OnVisible()
		{
			ClientService inst = MonoBehaviourInstance<ClientService>.inst;
			if (inst == null)
			{
				return;
			}

			FogHiderOnCenter.InSight();
			EnableSpringManager(true);
			pickable.EnablePickable(true);
			if (characterRenderer != null)
			{
				characterRenderer.EnableRenderer(true);
				characterRenderer.SetMaterial(MaterialSwitchType.Stealth, IsInvisible);
			}

			if (floatingUi != null)
			{
				floatingUi.Show();
				floatingUi.SetAutoVisible(true);
			}

			if (inst.MyObjectId != objectId && inst.IsAlly(this))
			{
				ShowMapIcon(MiniMapIconType.Sight);
			}

			ShowMiniMapIcon(MiniMapIconType.Sight);
		}


		public virtual void OnInvisible()
		{
			if (MonoBehaviourInstance<ClientService>.inst == null)
			{
				return;
			}

			FogHiderOnCenter.OutSight();
			EnableSpringManager(false);
			pickable.EnablePickable(false);
			if (characterRenderer != null)
			{
				characterRenderer.EnableRenderer(false);
			}

			if (floatingUi != null)
			{
				floatingUi.Hide();
				floatingUi.SetAutoVisible(false);
			}

			HideMiniMapIcon(MiniMapIconType.Sight);
		}


		public override GameObject LoadProjectile(string projectileName)
		{
			return SingletonMonoBehaviour<ResourceManager>.inst.LoadProjectile(GetCharacterCode(), GetSkinIndex(),
				projectileName);
		}


		public override GameObject LoadObject(string objectName)
		{
			return SingletonMonoBehaviour<ResourceManager>.inst.LoadObject(GetCharacterCode(), GetSkinIndex(),
				objectName);
		}


		public override GameObject LoadEffect(string effectName)
		{
			return SingletonMonoBehaviour<ResourceManager>.inst.LoadEffect(GetCharacterCode(), GetSkinIndex(),
				effectName);
		}


		public override AudioClip LoadFXSound(string soundName)
		{
			return SingletonMonoBehaviour<ResourceManager>.inst.LoadFXSound(GetCharacterCode(), GetSkinIndex(),
				soundName);
		}


		public override AudioClip LoadVoice(string characterResource, string voiceName, int randomCount = 0)
		{
			return SingletonMonoBehaviour<ResourceManager>.inst.LoadVoice(GetCharacterCode(), GetSkinIndex(),
				characterResource, voiceName, randomCount);
		}


		public class SkillDamageInfo
		{
			public int attackerId;


			public int damage;


			public int effectCode;


			public bool isCritical;


			public bool isSkillDamage;
		}


		public class RegenHealInfo
		{
			private int addHp;


			private int addSp;


			public int AddHp => addHp;


			public int AddSp => addSp;


			public void Add(int addHp, int addSp)
			{
				this.addHp += addHp;
				this.addSp += addSp;
			}


			public void Clear()
			{
				addHp = 0;
				addSp = 0;
			}
		}
	}
}