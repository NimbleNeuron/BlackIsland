using System;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	[ObjectAttr(ObjectType.Monster)]
	public class LocalMonster : LocalMovableCharacter
	{
		private const float BewareOutRangeBuffer = 2f;
		private readonly int BBeware = Animator.StringToHash("bBeware");
		private readonly int BInCombat = Animator.StringToHash("bInCombat");
		private readonly Collider[] colliders = new Collider[10];
		private readonly int TAppear = Animator.StringToHash("tAppear");
		private readonly int TBeware = Animator.StringToHash("tBeware");
		private readonly int TEndBattle = Animator.StringToHash("tEndBattle");
		private readonly int TEndBeware = Animator.StringToHash("tEndBeware");
		private int activeRadius;
		private bool appear;
		private ClientCharacter clientCharacter;
		private HeadLookController headLookController;
		private Vector3 homeLocation;
		private MonsterHostileAgent hostileAgent;
		private bool isEndBattle;
		private bool isInSight;
		private bool isMoving;
		private int lastInitObjectId;
		private LocalCharacter lookTarget;
		private MonsterData monsterData;
		public MonsterType MonsterType => monsterData.monster;
		public ClientCharacter ClientCharacter => clientCharacter;
		public bool Appear => appear;
		protected override ObjectType GetObjectType()
		{
			return ObjectType.Monster;
		}
		protected override HostileAgent GetHostileAgent()
		{
			return hostileAgent;
		}
		protected override int GetCharacterCode()
		{
			return monsterData.code;
		}
		protected override GameObject GetCharacterPrefab()
		{
			return SingletonMonoBehaviour<ResourceManager>.inst.LoadMonster(monsterData.resource);
		}
		protected override string GetNickname()
		{
			return Ln.Get(LnType.Monster_Name, monsterData.code.ToString());
		}
		public override string GetLocalizedName(bool includeColor)
		{
			if (includeColor)
			{
				return StringUtil.GetColorString(StringUtil.ColorStringType.Enemy,
					LnUtil.GetMonsterName(monsterData.code));
			}

			return LnUtil.GetMonsterName(monsterData.code);
		}
		public override void Init(byte[] snapshotData)
		{
			MonsterSnapshot monsterSnapshot = serializer.Deserialize<MonsterSnapshot>(snapshotData);
			monsterData = GameDB.monster.GetMonsterData(monsterSnapshot.monsterCode);
			hostileAgent = new MonsterHostileAgent(this);
			base.Init(snapshotData);
			homeLocation = GetPosition();
			floatingUi.UpdateLevel(Status.Level);
			activeRadius = Mathf.FloorToInt(Stat.SightRange * 2f);
			if (lastInitObjectId == objectId)
			{
				OnChangeDayNight(MonoBehaviourInstance<ClientService>.inst.DayNight);
				if (!isAlive)
				{
					PlayDeadAnimation();
				}

				return;
			}

			lastInitObjectId = objectId;
			headLookController = GetComponentInChildren<HeadLookController>();
			if (headLookController != null)
			{
				headLookController.IsLobbyCharacter = false;
				headLookController.enabled = false;
			}

			gameObject.GetComponent<CapsuleCollider>().enabled = true;
			clientCharacter = characterObject.GetComponentInChildren<ClientCharacter>();
			if (clientCharacter != null)
			{
				clientCharacter.Init(this, objectId);
			}

			appear = true;
			isMoving = false;
			isEndBattle = false;
			SetCharacterAnimatorLogWarnings(false);
			if (isAlive)
			{
				SetCharacterAnimatorTrigger(TAppear);
			}
			else
			{
				appear = false;
				PlayDeadAnimation();
			}

			int characterAnimatorLayerIndex = GetCharacterAnimatorLayerIndex(DayNight.Night.ToString());
			if (0 <= characterAnimatorLayerIndex)
			{
				SetCharacterAnimatorLayerWeight(characterAnimatorLayerIndex,
					MonoBehaviourInstance<ClientService>.inst.DayNight == DayNight.Night ? 1f : 0f);
			}

			if (monsterData.monster == MonsterType.Wickline)
			{
				string text = Ln.Get("위클라인등장");
				MonoBehaviourInstance<GameUI>.inst.ChattingUi.AddSystemChatting(text, true);
				MonoBehaviourInstance<GameUI>.inst.SpecialAnnounceUI.ShowMessage(AnnounceType.Wickeline_Appear, text);
				MonoBehaviourInstance<ClientService>.inst.SpawnWickline(this);
				if (!MonoBehaviourInstance<ClientService>.inst.IsPlayer)
				{
					LocalSightAgent localSightAgent = gameObject.AddComponent<LocalSightAgent>();
					localSightAgent.InitAttachSight(this, -1);
					localSightAgent.SetOwner(MonoBehaviourInstance<ClientService>.inst.MyObserver.Observer.SightAgent);
					localSightAgent.UpdateSightRange(Stat.SightRange);
					localSightAgent.UpdateSightAngle(360);
				}
			}

			SetIsInCombat(monsterSnapshot.isInCombat);
			this.StartThrowingCoroutine(CoroutineUtil.DelayedAction(1.5f, delegate { appear = false; }),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][MonsterAppear] Message:" + exception.Message + ", StackTrace:" +
					      exception.StackTrace);
				});
		}


		protected override LocalCharacterStat CreateCharacterStat()
		{
			return new LocalMonsterStat();
		}


		protected override void InitStatus(byte[] snapshot)
		{
			MonsterStatusSnapshot snapshot2 = serializer.Deserialize<MonsterStatusSnapshot>(snapshot);
			Status = new CharacterStatus(snapshot2);
		}


		protected override void InstantiateCharacterObject()
		{
			characterObject =
				MonoBehaviourInstance<ClientService>.inst.World.GetMonsterCharacterObject(monsterData.monster,
					transform);
			characterObject.transform.localPosition = Vector3.zero;
			characterObject.transform.localRotation = Quaternion.identity;
		}


		protected override Sprite GetMapIcon()
		{
			return SingletonMonoBehaviour<ResourceManager>.inst.GetMapIconSprite(isAlive);
		}


		protected override void UpdateInternal()
		{
			base.UpdateInternal();
			if (!isAlive || !isInSight)
			{
				return;
			}

			UpdateUI();
		}


		private void UpdateUI()
		{
			int sp = Mathf.CeilToInt(Mathf.Ceil(activeRadius - Vector3.Distance(GetPosition(), homeLocation)) /
				activeRadius * 100f);
			CharacterFloatingUI floatingUi = this.floatingUi;
			if (floatingUi != null)
			{
				floatingUi.SetSp(sp, 100);
			}

			TargetInfoHud targetInfoHud = this.targetInfoHud;
			if (targetInfoHud == null)
			{
				return;
			}

			targetInfoHud.SetTargetStatSpBar(sp, 100);
		}


		protected override void UpdateCharacterAnimator()
		{
			base.UpdateCharacterAnimator();
			if (!isAlive || !isInSight)
			{
				return;
			}

			UpdateLookTarget();
			UpdateHomecomming();
		}


		private void UpdateLookTarget()
		{
			if (Appear)
			{
				return;
			}

			if (IsInCombat)
			{
				return;
			}

			if (isEndBattle)
			{
				return;
			}

			if (moveAgent.IsMoving)
			{
				return;
			}

			if (lookTarget == null)
			{
				int num = Physics.OverlapSphereNonAlloc(GetPosition(), Stat.SightRange, colliders,
					GameConstants.LayerMask.WORLD_OBJECT_LAYER);
				for (int i = 0; i < num; i++)
				{
					LocalPlayerCharacter component = colliders[i].GetComponent<LocalPlayerCharacter>();
					if (!(component == null) && component.IsAlive)
					{
						EnableHeadLook();
						SetCharacterAnimatorTrigger(TBeware);
						SetCharacterAnimatorBool(BBeware, true);
						lookTarget = component;
						return;
					}
				}

				return;
			}

			if (headLookController != null)
			{
				headLookController.target = lookTarget.GetPosition();
			}

			if (Vector3.Distance(lookTarget.GetPosition(), homeLocation) >= Stat.SightRange + 2f)
			{
				DisableHeadLook();
				SetCharacterAnimatorTrigger(TEndBeware);
				SetCharacterAnimatorBool(BBeware, false);
				lookTarget = null;
			}
		}


		private void UpdateHomecomming()
		{
			if (Appear)
			{
				return;
			}

			if (IsInCombat)
			{
				return;
			}

			if (!isMoving)
			{
				return;
			}

			if (!moveAgent.IsMoving && IsHome())
			{
				SetCharacterAnimatorTrigger(TEndBeware);
				SetCharacterAnimatorBool(BBeware, false);
				isMoving = false;
			}
		}


		private bool IsHome()
		{
			return Vector3.Distance(GetPosition(), homeLocation) <= 0.5f;
		}


		public override void OnChangeDayNight(DayNight dayNight)
		{
			SetCharacterAnimatorLayerWeight(GetCharacterAnimatorLayerIndex(DayNight.Night.ToString()),
				dayNight == DayNight.Night ? 1f : 0f);
		}


		protected override void OnAddEffectState(CharacterStateValue state)
		{
			base.OnAddEffectState(state);
			if (GameDB.characterState.GetGroupData(state.Group).stateType == StateType.MonsterReset)
			{
				SetCharacterAnimatorBool(BInCombat, false);
				SetCharacterAnimatorTrigger(TEndBattle);
				isEndBattle = true;
			}
		}


		protected override void OnStartMove()
		{
			base.OnStartMove();
			isMoving = true;
			ClientService inst = MonoBehaviourInstance<ClientService>.inst;
			if (inst == null)
			{
				return;
			}

			if (monsterData.monster == MonsterType.Wickline && !IsInCombat &&
			    inst.WicklineVoiceControl.ActionType != WicklineActionType.StartMove)
			{
				inst.WicklineVoiceControl.PlayLoopWicklineVoice(WicklineActionType.StartMove);
			}
		}


		public override void SetIsInCombat(bool isCombat)
		{
			base.SetIsInCombat(isCombat);
			if (IsInCombat)
			{
				isEndBattle = false;
				SetCharacterAnimatorBool(BInCombat, true);
				DisableHeadLook();
			}

			if (monsterData.monster == MonsterType.Wickline)
			{
				WicklineActionType actionType =
					IsInCombat ? WicklineActionType.CombatStart : WicklineActionType.CombatEnd;
				WicklineVoiceType voiceType = IsInCombat ? WicklineVoiceType.CombatStart : WicklineVoiceType.CombatEnd;
				ClientService inst = MonoBehaviourInstance<ClientService>.inst;
				if (inst != null)
				{
					WicklineVoiceControl wicklineVoiceControl = inst.WicklineVoiceControl;
					if (wicklineVoiceControl != null)
					{
						wicklineVoiceControl.StopWicklineVoice();
					}

					WicklineVoiceControl wicklineVoiceControl2 = inst.WicklineVoiceControl;
					if (wicklineVoiceControl2 == null)
					{
						return;
					}

					wicklineVoiceControl2.PlayWicklineVoice(actionType, voiceType);
				}
			}
		}


		public override void OnSight()
		{
			base.OnSight();
			isInSight = true;
		}


		public override void OnHide()
		{
			base.OnHide();
			isInSight = false;
		}


		public override void OnDead(LocalCharacter attacker)
		{
			base.OnDead(attacker);
			DisableHeadLook();
		}


		public override void ShowMiniMapIcon(MiniMapIconType miniMapIconType)
		{
			GameUI inst = MonoBehaviourInstance<GameUI>.inst;
			if (inst == null)
			{
				return;
			}

			if (isAlive)
			{
				inst.Minimap.UIMap.CreateNonPlayer(ObjectId, GetPosition(), GetMapIcon(), miniMapIconType);
				return;
			}

			inst.Minimap.UIMap.CreateDeadNonPlayer(ObjectId, GetPosition(), GetMapIcon());
		}


		public override void ShowMapIcon(MiniMapIconType miniMapIconType) { }


		public override void HideMapIcon(MiniMapIconType miniMapIconType) { }


		protected override void OnBushEvent(bool isInBush, int objectId) { }


		private void EnableHeadLook()
		{
			if (headLookController != null)
			{
				headLookController.enabled = true;
			}
		}


		private void DisableHeadLook()
		{
			if (headLookController != null)
			{
				headLookController.enabled = false;
			}
		}


		public override bool IsMouseHitPossible(LocalSightAgent targetSightAgent, bool isInvisible)
		{
			return !Appear && base.IsMouseHitPossible(targetSightAgent, isInvisible);
		}


		public override ObjectOrder GetObjectOrder()
		{
			if (IsAlive)
			{
				if (!MonoBehaviourInstance<ClientService>.inst.IsAlly(this))
				{
					return ObjectOrder.AliveEnemy;
				}

				return ObjectOrder.AliveAlly;
			}

			if (!MonoBehaviourInstance<ClientService>.inst.IsAlly(this))
			{
				return ObjectOrder.DeadEnemy;
			}

			return ObjectOrder.DeadAlly;
		}


		public override void InSight()
		{
			base.InSight();
			if (clientCharacter != null)
			{
				clientCharacter.SetInSight(true);
			}
		}


		public override void OutSight()
		{
			base.OutSight();
			if (clientCharacter != null)
			{
				clientCharacter.SetInSight(false);
			}
		}


		public override void OnVisible()
		{
			base.OnVisible();
			if (clientCharacter != null)
			{
				clientCharacter.SetVisible(true);
			}
		}


		public override void OnInvisible()
		{
			base.OnInvisible();
			if (clientCharacter != null)
			{
				clientCharacter.SetVisible(false);
			}
		}


		public override GameObject LoadProjectile(string projectileName)
		{
			return LoadCommonProjectile(projectileName);
		}


		public override GameObject LoadObject(string objectName)
		{
			return LoadCommonObject(objectName);
		}


		public override GameObject LoadEffect(string effectName)
		{
			return LoadCommonEffect(effectName);
		}


		public override AudioClip LoadFXSound(string soundName)
		{
			return LoadCommonFXSound(soundName);
		}


		public override AudioClip LoadVoice(string characterResource, string voiceName, int randomCount = 0)
		{
			return LoadCommonVoice(voiceName);
		}
	}
}