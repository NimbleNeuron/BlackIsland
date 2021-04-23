using System;
using System.Collections.Generic;
using BIFog;
using BIOutline;
using Blis.Client.UI;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace Blis.Client
{
	public class PlayerController : SingletonMonoBehaviour<PlayerController>
	{
		private const float MouseMoveClickDelay = 0.1f;
		private readonly SkillSlotIndex[] skillSlotIndexes = (SkillSlotIndex[]) Enum.GetValues(typeof(SkillSlotIndex));
		private ClientService clientService;
		private LocalCharacter currentTarget;
		private CursorStatus cursorStatus;
		private GameClient gameClient;
		private RaycastHit[] getMouseHits = new RaycastHit[50];
		private MouseHit hit;
		private GameInput input;
		private InstallSummonContext installSummonContext;
		private int interactionBoxId;
		private bool isActive;
		private bool isLockInput;
		private LocalSummonBase lastIndicatorRange;
		private Camera mainCamera;
		private float mouseMoveClickTime;
		private GameObject moveAttackCursor;
		private GameObject moveCursor;
		private LocalPlayerCharacter myCharacter;
		private MyPlayerContext myPlayer;
		private LocalObject nextInteractionObject;
		private LocalPingTarget pingTarget;
		private PlayerSkill playerSkill;
		private SplatManager splatManager;

		public MouseOverManager MouseOverManager { get; } = new MouseOverManager();
		public CursorStatus CursorStatus => cursorStatus;
		public PlayerSkill PlayerSkill => playerSkill;
		public ItemGuide ItemGuide { get; } = new ItemGuide();
		public bool IsLockInput => isLockInput;
		public LocalPingTarget PingTarget => pingTarget;
		public int CharcterCode => myCharacter.CharacterCode;

		public int myObjectId {
			get
			{
				if (!(myCharacter != null))
				{
					return 0;
				}

				return myCharacter.ObjectId;
			}
		}

		public List<int> MakeSourceItems { get; } = new List<int>();
		public LocalObject mouseHitObject {
			get
			{
				if (hit != null)
				{
					return hit.target;
				}

				return null;
			}
		}

		private void Update()
		{
			if (IsInvalid())
			{
				return;
			}

			UpdateTarget();
			UpdateMouseOver();
		}

		private void LateUpdate()
		{
			if (IsInvalid())
			{
				return;
			}

			if (myCharacter == null || !myCharacter.IsAlive)
			{
				return;
			}

			MonoBehaviourInstance<GameUI>.inst.Events.OnUpdateCharacterPosition(myCharacter.GetPosition());
		}

		protected override void OnAwakeSingleton()
		{
			isActive = true;
			mainCamera = Camera.main;
			input = gameObject.AddComponent<GameInput>();
			input.OnMousePressed += OnMousePressed;
			input.OnMousePressing += OnMousePressing;
			input.OnMouseReleased += OnMouseReleased;
			input.OnKeyPressed += OnKeyPressed;
			input.OnKeyPressing += OnKeyPressing;
			input.OnKeyRelease += OnKeyReleased;
		}


		public void Init(GameClient gameClient, ClientService clientService)
		{
			this.gameClient = gameClient;
			this.clientService = clientService;
			moveCursor = null;
			moveAttackCursor = null;
			myPlayer = clientService.MyPlayer;
			myCharacter = clientService.MyPlayer.Character;
			GameUtil.BindOrAdd<SplatManager>(myCharacter.gameObject, ref splatManager);
			splatManager.InitIndicators(myCharacter.CharacterCode);
			playerSkill = new PlayerSkill(myPlayer, splatManager);
			playerSkill.OnChangeCursorStatus = OnChangeCursorStatus;
			MonoBehaviourInstance<FogManager>.inst.SetMyFogSight(myCharacter.SightAgent.FogSight);
			pingTarget = gameObject.AddComponent<LocalPingTarget>();
		}


		protected override void OnDestroySingleton()
		{
			input.OnMousePressed -= OnMousePressed;
			input.OnMousePressing -= OnMousePressing;
			input.OnMouseReleased -= OnMouseReleased;
			input.OnKeyPressed -= OnKeyPressed;
			input.OnKeyRelease -= OnKeyReleased;
		}


		private bool IsInvalid()
		{
			return clientService == null || !clientService.IsGameStarted || !isActive || clientService.MyPlayer == null;
		}


		private void UpdateTarget()
		{
			if (currentTarget == null)
			{
				return;
			}

			if (!currentTarget.IsAlive || !currentTarget.IsInAllySight(myCharacter.SightAgent))
			{
				ClearTarget();
			}
		}


		private void ClearTarget()
		{
			if (currentTarget != null)
			{
				SelectionRenderer componentInChildren = currentTarget.GetComponentInChildren<SelectionRenderer>();
				if (componentInChildren != null)
				{
					componentInChildren.SetSelection(false);
				}

				currentTarget.DetachTargetInfoHud();
				MonoBehaviourInstance<GameUI>.inst.TargetInfoHud.HideTargetHud();
				currentTarget = null;
			}
		}


		public Vector3 GetMyCharacterPos()
		{
			return myCharacter.GetPosition();
		}


		private void GetMouseGroundHit(Ray ray, out Vector3 mouseGroundHit)
		{
			RaycastHit raycastHit;
			mouseGroundHit =
				Physics.Raycast(ray, out raycastHit, 100f, GameConstants.LayerMask.GROUND_LAYER,
					QueryTriggerInteraction.Collide)
					? raycastHit.point
					: Vector3.zero;
		}


		private MouseHit GetMouseHit(Vector3 mousePosition, GameInputEvent inputEvent, out Vector3 mouseGroundHit)
		{
			Ray ray = mainCamera.ScreenPointToRay(mousePosition);
			GetMouseGroundHit(ray, out mouseGroundHit);
			if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
			{
				return new MouseHit(MouseHit.HitType.UI, Vector3.zero, null);
			}

			int num = Physics.RaycastNonAlloc(ray, getMouseHits, 100f, GameConstants.LayerMask.PICKABLE_OBJECT_LAYER,
				QueryTriggerInteraction.Collide);
			while (getMouseHits.Length <= num)
			{
				getMouseHits = new RaycastHit[getMouseHits.Length * 2];
				num = Physics.RaycastNonAlloc(ray, getMouseHits, 100f, GameConstants.LayerMask.PICKABLE_OBJECT_LAYER,
					QueryTriggerInteraction.Collide);
			}

			try
			{
				LocalObject localObject = null;
				for (int i = 0; i < num; i++)
				{
					Pickable component = getMouseHits[i].transform.GetComponent<Pickable>();
					if (!(component == null))
					{
						LocalObject @object = component.GetObject();
						if (!(@object == null) && @object.ObjectId != myCharacter.ObjectId &&
						    @object.IsMouseHitPossible(myCharacter.SightAgent, false))
						{
							if (!(localObject == null))
							{
								PlayerSkill playerSkill = this.playerSkill;
								if (playerSkill == null ||
								    !playerSkill.SkillPickingOrderChange(localObject, @object, inputEvent))
								{
									goto IL_11B;
								}
							}

							localObject = @object;
						}
					}

					IL_11B: ;
				}

				if (localObject != null)
				{
					return new MouseHit(MouseHit.HitType.OBJECT, localObject.GetPosition(), localObject);
				}
			}
			catch (Exception) { }

			if (!mouseGroundHit.Equals(Vector3.zero))
			{
				return new MouseHit(MouseHit.HitType.GROUND, mouseGroundHit, null);
			}

			RaycastHit raycastHit;
			if (Physics.Raycast(ray, out raycastHit, 100f, GameConstants.LayerMask.FOG_LAYER,
				QueryTriggerInteraction.Collide))
			{
				return new MouseHit(MouseHit.HitType.FOG, raycastHit.point, null);
			}

			return new MouseHit(MouseHit.HitType.INVALID, Vector3.zero, null);
		}


		private void UpdateMouseOver()
		{
			Vector3 externalMousePosition;
			hit = GetMouseHit(input.GetMousePosition(), GameInputEvent.None, out externalMousePosition);
			splatManager.SetExternalMousePosition(externalMousePosition);
			HostileType? hostileType = null;
			LocalCharacter target;
			if ((target = hit.target as LocalCharacter) != null)
			{
				hostileType = myPlayer.GetHostileType(target);
			}

			MouseOverManager.Update(hit.target, hostileType);
			if (cursorStatus == CursorStatus.Summon)
			{
				SetCursorInstallSummon(hit);
				return;
			}

			SetCursor(hit);
		}


		public bool GetMouseGroundHit(out Vector3 mouseGroundHit)
		{
			Ray ray = mainCamera.ScreenPointToRay(input.GetMousePosition());
			GetMouseGroundHit(ray, out mouseGroundHit);
			return !mouseGroundHit.Equals(Vector3.zero);
		}


		private bool IsSummonInstallablePosition(Vector3 position, out Vector3 alignedPosition, bool isIndicatorCheck)
		{
			HideSummonRangeIndicator();
			alignedPosition = position;
			NavMeshHit hit;
			if (!NavMesh.SamplePosition(alignedPosition, out hit, 2f, 2147483640) ||
			    installSummonContext == null || installSummonContext.summonData == null)
			{
				return false;
			}

			alignedPosition = hit.position;
			List<PlayerContext> teamMembers =
				MonoBehaviourInstance<ClientService>.inst.GetTeamMember(myCharacter.TeamNumber);
			for (int i = 0; i < teamMembers.Count; ++i)
			{
				foreach (LocalSummonBase localSummonBase in clientService.World.FindAll<LocalSummonBase>(
					summon => summon.IsOwner(teamMembers[i].Character.ObjectId)))
				{
					LocalSummonBase summon = localSummonBase;
					if (summon.SummonData.pileRange > 0.0)
					{
						float magnitude = Vector3
							.Scale(alignedPosition - summon.GetPosition(), new Vector3(1f, 0.0f, 1f)).magnitude;
						float distance = 0.0f;
						Action action = () =>
						{
							if (!isIndicatorCheck)
							{
								return;
							}

							(lastIndicatorRange = summon).SetSummonRangeIndicator(true, distance);
						};
						distance = summon.SummonData.pileRange;
						if (installSummonContext.summonData.objectType == summon.SummonData.objectType &&
						    magnitude <= (double) distance)
						{
							action();
							return false;
						}

						distance = installSummonContext.summonData.radius + summon.SummonData.radius;
						if (magnitude <= (double) distance)
						{
							action();
							return false;
						}
					}
				}
			}

			return true;

			// co: dotPeek
			// PlayerController.<>c__DisplayClass53_0 CS$<>8__locals1 = new PlayerController.<>c__DisplayClass53_0();
			// CS$<>8__locals1.isIndicatorCheck = isIndicatorCheck;
			// CS$<>8__locals1.<>4__this = this;
			// this.HideSummonRangeIndicator();
			// alignedPosition = position;
			// NavMeshHit navMeshHit;
			// if (!NavMesh.SamplePosition(alignedPosition, out navMeshHit, 2f, 2147483640))
			// {
			// 	return false;
			// }
			// if (this.installSummonContext == null || this.installSummonContext.summonData == null)
			// {
			// 	return false;
			// }
			// alignedPosition = navMeshHit.position;
			// CS$<>8__locals1.teamMembers = MonoBehaviourInstance<ClientService>.inst.GetTeamMember(this.myCharacter.TeamNumber);
			// PlayerController.<>c__DisplayClass53_1 CS$<>8__locals2 = new PlayerController.<>c__DisplayClass53_1();
			// CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
			// CS$<>8__locals2.i = 0;
			// while (CS$<>8__locals2.i < CS$<>8__locals2.CS$<>8__locals1.teamMembers.Count)
			// {
			// 	WorldBase<LocalObject> world = this.clientService.World;
			// 	Func<LocalSummonBase, bool> checkCondition;
			// 	if ((checkCondition = CS$<>8__locals2.<>9__0) == null)
			// 	{
			// 		checkCondition = (CS$<>8__locals2.<>9__0 = ((LocalSummonBase summon) => summon.IsOwner(CS$<>8__locals2.CS$<>8__locals1.teamMembers[CS$<>8__locals2.i].Character.ObjectId)));
			// 	}
			// 	using (List<LocalSummonBase>.Enumerator enumerator = world.FindAll<LocalSummonBase>(checkCondition).GetEnumerator())
			// 	{
			// 		while (enumerator.MoveNext())
			// 		{
			// 			PlayerController.<>c__DisplayClass53_2 CS$<>8__locals3 = new PlayerController.<>c__DisplayClass53_2();
			// 			CS$<>8__locals3.CS$<>8__locals2 = CS$<>8__locals2;
			// 			CS$<>8__locals3.summon = enumerator.Current;
			// 			if (CS$<>8__locals3.summon.SummonData.pileRange > 0f)
			// 			{
			// 				float magnitude = Vector3.Scale(alignedPosition - CS$<>8__locals3.summon.GetPosition(), new Vector3(1f, 0f, 1f)).magnitude;
			// 				float distance = 0f;
			// 				Action action = delegate()
			// 				{
			// 					if (CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.isIndicatorCheck)
			// 					{
			// 						(CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.lastIndicatorRange = CS$<>8__locals3.summon).SetSummonRangeIndicator(true, distance);
			// 					}
			// 				};
			// 				distance = CS$<>8__locals3.summon.SummonData.pileRange;
			// 				if (this.installSummonContext.summonData.objectType == CS$<>8__locals3.summon.SummonData.objectType && magnitude <= distance)
			// 				{
			// 					action();
			// 					return false;
			// 				}
			// 				distance = this.installSummonContext.summonData.radius + CS$<>8__locals3.summon.SummonData.radius;
			// 				if (magnitude <= distance)
			// 				{
			// 					action();
			// 					return false;
			// 				}
			// 			}
			// 		}
			// 	}
			// 	int i = CS$<>8__locals2.i + 1;
			// 	CS$<>8__locals2.i = i;
			// }
			// return true;
		}


		private bool IsPickingTarget(LocalObject hitTarget)
		{
			if (hitTarget != null && hitTarget is LocalSummonBase)
			{
				LocalSummonBase localSummonBase = hitTarget as LocalSummonBase;
				if (localSummonBase != null && localSummonBase.SummonData != null)
				{
					return localSummonBase.SummonData.isPickingTarget;
				}
			}

			return true;
		}


		private void SetCursorInstallSummon(MouseHit hit)
		{
			if (hit.hitType == MouseHit.HitType.INVALID)
			{
				MonoBehaviourInstance<GameInput>.inst.GameCursor.SetCursorTarget(CursorTarget.BlockSummon);
				return;
			}

			Vector3 vector;
			if (!IsSummonInstallablePosition(hit.point, out vector, true))
			{
				MonoBehaviourInstance<GameInput>.inst.GameCursor.SetCursorTarget(CursorTarget.BlockSummon);
				return;
			}

			if (!IsPickingTarget(hit.target))
			{
				MonoBehaviourInstance<GameInput>.inst.GameCursor.SetCursorTarget(CursorTarget.None);
				return;
			}

			if (hit.hitType == MouseHit.HitType.OBJECT)
			{
				MonoBehaviourInstance<GameInput>.inst.GameCursor.SetCursorTarget(CursorTarget.BlockSummon);
				return;
			}

			MonoBehaviourInstance<GameInput>.inst.GameCursor.SetCursorTarget(CursorTarget.None);
		}


		private void SetCursor(MouseHit hit)
		{
			if (hit.hitType == MouseHit.HitType.INVALID)
			{
				MonoBehaviourInstance<GameInput>.inst.GameCursor.SetCursorTarget(CursorTarget.Block);
				return;
			}

			if (hit.hitType != MouseHit.HitType.OBJECT)
			{
				MonoBehaviourInstance<GameInput>.inst.GameCursor.SetCursorTarget(CursorTarget.None);
				return;
			}

			hit.target.SetCursor(myCharacter);
		}


		public void OnMousePressed(GameInputEvent inputEvent, Vector3 mousePosition)
		{
			if (IsInvalid())
			{
				return;
			}

			mouseMoveClickTime = -0.1f;
			if (inputEvent != GameInputEvent.None)
			{
				ProcessInputRay(inputEvent, false, mousePosition);
			}
		}


		public void OnMousePressing(GameInputEvent inputEvent, Vector3 mousePosition)
		{
			if (IsInvalid())
			{
				return;
			}

			if (inputEvent == GameInputEvent.Move || inputEvent == GameInputEvent.Warp)
			{
				mouseMoveClickTime += Time.deltaTime;
				if (mouseMoveClickTime >= 0.1f)
				{
					ProcessInputPressingRay(inputEvent, true, mousePosition);
					mouseMoveClickTime -= 0.1f;
				}
			}
		}


		public void OnMouseReleased(GameInputEvent inputEvent, Vector3 mousePressedPosition, Vector3 mousePosition)
		{
			if (IsInvalid())
			{
				return;
			}

			if (inputEvent == GameInputEvent.Move || inputEvent == GameInputEvent.Warp)
			{
				mouseMoveClickTime = 0f;
			}

			ProcessInputMouseReleaseRay(inputEvent, false, mousePressedPosition, mousePosition);
		}


		public void OnKeyPressed(GameInputEvent inputEvent, Vector3 mousePosition)
		{
			if (IsInvalid())
			{
				return;
			}

			if (ChangeKeyPopup.changekeyPopupOpen)
			{
				return;
			}

			if (myCharacter != null && myCharacter.IsEmma())
			{
				PlayerSkill playerSkill = this.playerSkill;
				if (playerSkill != null)
				{
					playerSkill.SetForcePickableEmmaSummons(true);
				}
			}

			ProcessInputRay(inputEvent, false, mousePosition);
		}


		public void OnKeyPressing(GameInputEvent inputEvent, Vector3 mousePosition)
		{
			if (IsInvalid())
			{
				return;
			}

			if (ChangeKeyPopup.changekeyPopupOpen)
			{
				return;
			}

			ProcessInputPressingRay(inputEvent, false, mousePosition);
		}


		public void OnKeyReleased(GameInputEvent inputEvent, Vector3 mousePosition)
		{
			if (IsInvalid())
			{
				return;
			}

			if (myCharacter != null && myCharacter.IsEmma())
			{
				PlayerSkill playerSkill = this.playerSkill;
				if (playerSkill != null)
				{
					playerSkill.SetForcePickableEmmaSummons(false);
				}
			}

			ProcessInputKeyReleaseRay(inputEvent, false, mousePosition);
		}


		private void ShowMoveCursor(Vector3 point)
		{
			if (moveCursor == null)
			{
				moveCursor = SingletonMonoBehaviour<ResourceManager>.inst.LoadPrefab("MoveArrow");
			}

			Instantiate<GameObject>(moveCursor, point, Quaternion.identity);
		}


		private void ShowAttackMoveCursor(Vector3 point)
		{
			if (moveAttackCursor == null)
			{
				moveAttackCursor = SingletonMonoBehaviour<ResourceManager>.inst.LoadPrefab("MoveArrow_Attack");
			}

			Instantiate<GameObject>(moveAttackCursor, point, Quaternion.identity);
		}


		public void LockInput(bool isLock)
		{
			isLockInput = isLock;
		}


		public void ProcessInputRay(GameInputEvent eventType, bool isPressing, Vector3 mousePosition)
		{
			if (IsInvalid())
			{
				return;
			}

			Vector3 hitGroundPoint;
			MouseHit mouseHit = GetMouseHit(mousePosition, eventType, out hitGroundPoint);
			bool isHitTargetableLayer = mouseHit.hitType == MouseHit.HitType.GROUND ||
			                            mouseHit.hitType == MouseHit.HitType.OBJECT;
			
			OnInputEvent(eventType, isPressing, mouseHit.target, mouseHit.point, hitGroundPoint, mouseHit.point,
				isHitTargetableLayer);
		}


		public void ProcessInputPressingRay(GameInputEvent eventType, bool isPressing, Vector3 mousePosition)
		{
			if (IsInvalid())
			{
				return;
			}

			Vector3 hitGroundPoint;
			MouseHit mouseHit = GetMouseHit(mousePosition, eventType, out hitGroundPoint);
			bool isHitTargetableLayer = mouseHit.hitType == MouseHit.HitType.GROUND ||
			                            mouseHit.hitType == MouseHit.HitType.OBJECT;
			OnInputPressingEvent(eventType, isPressing, mouseHit.target, mouseHit.point, hitGroundPoint,
				isHitTargetableLayer);
		}


		public void ProcessInputKeyReleaseRay(GameInputEvent eventType, bool isPressing, Vector3 mousePosition)
		{
			if (IsInvalid())
			{
				return;
			}

			Vector3 vector;
			MouseHit mouseHit = GetMouseHit(mousePosition, eventType, out vector);
			bool isHitTargetableLayer = mouseHit.hitType == MouseHit.HitType.GROUND ||
			                            mouseHit.hitType == MouseHit.HitType.OBJECT;
			OnInputReleaseEvent(eventType, isPressing, mouseHit.target, mouseHit.point, vector, vector,
				isHitTargetableLayer);
		}


		public void ProcessInputMouseReleaseRay(GameInputEvent eventType, bool isPressing, Vector3 mousePressedPosition,
			Vector3 mousePosition)
		{
			if (clientService == null || !clientService.IsGameStarted)
			{
				return;
			}

			Vector3 hitGroundPoint;
			MouseHit mouseHit = GetMouseHit(mousePressedPosition, eventType, out hitGroundPoint);
			bool isHitTargetableLayer = mouseHit.hitType == MouseHit.HitType.GROUND ||
			                            mouseHit.hitType == MouseHit.HitType.OBJECT;
			Vector3 releasePoint;
			GetMouseGroundHit(mainCamera.ScreenPointToRay(mousePosition), out releasePoint);
			OnInputReleaseEvent(eventType, isPressing, mouseHit.target, mouseHit.point, hitGroundPoint, releasePoint,
				isHitTargetableLayer);
		}


		private bool IsTargetable(LocalObject localObject)
		{
			if (localObject == null)
			{
				return false;
			}

			if (localObject.IsTypeOf<LocalSummonBase>())
			{
				LocalSummonBase localSummonBase = (LocalSummonBase) localObject;
				if (!localSummonBase.IsAlive)
				{
					return false;
				}

				if (myCharacter.ObjectId.Equals(localSummonBase.OwnerId) || clientService.IsAlly(localSummonBase))
				{
					return true;
				}

				if (!localSummonBase.IsInAllySight(myCharacter.SightAgent))
				{
					return false;
				}
			}

			return true;
		}


		private void OnInputPressingEvent(GameInputEvent inputEvent, bool isPressing, LocalObject hitTarget,
			Vector3 hitPoint, Vector3 hitGroundPoint, bool isHitTargetableLayer)
		{
			if (IsInvalid())
			{
				return;
			}

			if (IsLockInput)
			{
				return;
			}

			if (inputEvent != GameInputEvent.Move)
			{
				if (inputEvent - GameInputEvent.UpArrow > 3)
				{
					return;
				}

				MonoBehaviourInstance<MobaCamera>.inst.SetCameraMode(MobaCameraMode.Traveling);
				MonoBehaviourInstance<MobaCamera>.inst.KeyArrowMovePosition(inputEvent);
			}
			else
			{
				PlayerSkill playerSkill = this.playerSkill;
				if (playerSkill == null)
				{
					return;
				}

				playerSkill.OnInputEvent(inputEvent, isPressing, hitTarget, hitGroundPoint, hitPoint,
					isHitTargetableLayer);
			}
		}


		private void OnInputEvent(GameInputEvent inputEvent, bool isPressing, LocalObject hitTarget, Vector3 hitPoint,
			Vector3 hitGroundPoint, Vector3 releasePoint, bool isHitTargetableLayer)
		{
			if (IsInvalid())
			{
				return;
			}

			if (inputEvent == GameInputEvent.Escape)
			{
				if (myCharacter.IsConcentrating)
				{
					MonoBehaviourInstance<GameUI>.inst.Escape();
					return;
				}

				if (MonoBehaviourInstance<GameUI>.inst.ChattingUi.IsFocus)
				{
					MonoBehaviourInstance<GameUI>.inst.ChattingUi.DeactivateInput();
					return;
				}

				if (BaseWindow.FocusedWindow != null)
				{
					MonoBehaviourInstance<GameUI>.inst.Escape();
					ClearNextInteractionObject();
					return;
				}

				if (splatManager != null && splatManager.CurrentIndicator != null)
				{
					OnChangeCursorStatus(CursorStatus.Normal);
					return;
				}

				MonoBehaviourInstance<GameUI>.inst.Escape();
			}
			else
			{
				if (IsLockInput)
				{
					return;
				}

				switch (inputEvent)
				{
					case GameInputEvent.Select:
					case GameInputEvent.Move:
					case GameInputEvent.Attack:
					case GameInputEvent.Active1:
					case GameInputEvent.Active2:
					case GameInputEvent.Active3:
					case GameInputEvent.Active4:
					case GameInputEvent.WeaponSkill:
					case GameInputEvent.LearnPassive:
					case GameInputEvent.LearnActive1:
					case GameInputEvent.LearnActive2:
					case GameInputEvent.LearnActive3:
					case GameInputEvent.LearnActive4:
					case GameInputEvent.LearnWeaponSkill:
					case GameInputEvent.TeamRevival:
					case GameInputEvent.MoveToAttack:
					{
						// PlayerSkill playerSkill = this.playerSkill;
						if (playerSkill == null)
						{
							return;
						}

						playerSkill.OnInputEvent(inputEvent, isPressing, hitTarget, hitGroundPoint, releasePoint,
							isHitTargetableLayer);
						return;
					}
					case GameInputEvent.Stop:
					{
						ClearNextInteractionObject();
						GameClient gameClient = this.gameClient;
						if (gameClient == null)
						{
							return;
						}

						gameClient.Request<ReqStop, ResStop>(new ReqStop(), delegate(ResStop res)
						{
							if (res.cancelNormalAttack)
							{
								myCharacter.OnStopNormalAttack();
							}
						});
						return;
					}
					case GameInputEvent.Hold:
					{
						ClearNextInteractionObject();
						GameClient gameClient2 = gameClient;
						if (gameClient2 == null)
						{
							return;
						}

						gameClient2.Request(new ReqHold());
						return;
					}
					case GameInputEvent.OpenMap:
					case GameInputEvent.OpenScoreboard:
					case GameInputEvent.OpenCharacterMastery:
					case GameInputEvent.OpenCharacterStat:
					case GameInputEvent.OpenCombineWindow:
					case GameInputEvent.ChangeCameraMode:
					case GameInputEvent.ResetCamera:
					case GameInputEvent.ShowObjectText:
					case GameInputEvent.Escape:
					case GameInputEvent.ShowRouteList:
					case GameInputEvent.ObserveNextPlayer:
					case GameInputEvent.ObservePreviousPlayer:
					case GameInputEvent.Alpha1:
					case GameInputEvent.Alpha2:
					case GameInputEvent.Alpha3:
					case GameInputEvent.Alpha4:
					case GameInputEvent.Alpha5:
					case GameInputEvent.Alpha6:
					case GameInputEvent.Alpha7:
					case GameInputEvent.Alpha8:
					case GameInputEvent.Alpha9:
					case GameInputEvent.Alpha0:
					case GameInputEvent.Stance:
					case GameInputEvent.ChangeEnableTrackerName:
					case GameInputEvent.QuickCombine:
					case GameInputEvent.AddGuide:
					case GameInputEvent.ChatItem:
					case GameInputEvent.ThrowItem:
					case GameInputEvent.CameraTeam1:
					case GameInputEvent.CameraTeam2:
					case GameInputEvent.CameraTeam3:
						break;
					case GameInputEvent.Rest:
						ClearNextInteractionObject();
						if (!MonoBehaviourInstance<GameUI>.inst.Caster.IsCasting())
						{
							SwitchRest(!myCharacter.IsRest);
						}

						break;
					case GameInputEvent.Reload:
					{
						Item weapon = myCharacter.GetWeapon();
						if (weapon != null && weapon.ItemData.IsGunType())
						{
							if (myCharacter.IsFullBullet() || myCharacter.IsGunReloading)
							{
								return;
							}

							if (myCharacter.CheckDyingCondition())
							{
								return;
							}

							if (gameClient == null)
							{
								return;
							}

							if (gameClient.IsTutorial)
							{
								MonoBehaviourInstance<TutorialController>.inst.fromInputReload = true;
							}

							gameClient.Request(new ReqGunReload());
						}

						break;
					}
					case GameInputEvent.ChangeEnableGameUI:
						MonoBehaviourInstance<GameUI>.inst.ChangeEnableHud();
						return;
					case GameInputEvent.ChangeEnableTrackerStatusBar:
						MonoBehaviourInstance<GameUI>.inst.UITracker.ChangeEnableTrackerUIStatusBar();
						return;
					case GameInputEvent.Emotion1:
					case GameInputEvent.Emotion2:
					case GameInputEvent.Emotion3:
					case GameInputEvent.Emotion4:
					case GameInputEvent.Emotion5:
					case GameInputEvent.Emotion6:
					{
						int emotionCharVoiceType =
							(int) CharacterVoiceUtil.EmotionGameInputTypeConvertToCharacterVoiceType(inputEvent);
						GameClient gameClient3 = gameClient;
						if (gameClient3 == null)
						{
							return;
						}

						gameClient3.Request(new ReqEmotionCharacterVoice
						{
							characterObjectId = clientService.MyObjectId,
							emotionCharVoiceType = emotionCharVoiceType
						});
						return;
					}
					case GameInputEvent.ChatActive:
					case GameInputEvent.ChatActive2:
						MonoBehaviourInstance<GameUI>.inst.ChattingUi.EnterChat(false);
						return;
					case GameInputEvent.AllChatActive:
						MonoBehaviourInstance<GameUI>.inst.ChattingUi.EnterChat(false);
						return;
					case GameInputEvent.MaxChatWindow:
						MonoBehaviourInstance<GameUI>.inst.ChattingUi.SetMaxChatSize();
						if (!MonoBehaviourInstance<GameUI>.inst.ChattingUi.IsActive())
						{
							MonoBehaviourInstance<GameUI>.inst.ChattingUi.Active(false);
						}

						break;
					case GameInputEvent.PingTarget:
						if (hit.hitType == MouseHit.HitType.INVALID)
						{
							return;
						}

						if (hitTarget != null && hitTarget is LocalMovableCharacter)
						{
							PingType pingType = PingType.Select;
							LocalPlayerCharacter target;
							if ((target = hitTarget as LocalPlayerCharacter) != null)
							{
								if (!clientService.IsAlly(target))
								{
									pingType = PingType.Target;
								}
							}
							else if (hitTarget is LocalMonster)
							{
								pingType = PingType.Target;
							}

							ReqPingTarget packet = new ReqPingTarget
							{
								pingType = pingType,
								targetObjectId = hitTarget.ObjectId,
								targetPosition = hitPoint
							};
							GameClient gameClient4 = gameClient;
							if (gameClient4 == null)
							{
								return;
							}

							gameClient4.Request(packet);
						}
						else if (!MonoBehaviourInstance<GameUI>.inst.PingHud.IsActive())
						{
							MonoBehaviourInstance<GameUI>.inst.PingHud.Active(hitPoint, hitGroundPoint, hitTarget);
						}

						break;
					case GameInputEvent.MarkTarget:
					{
						if (hit.hitType == MouseHit.HitType.INVALID)
						{
							return;
						}

						ReqMark packet2 = new ReqMark
						{
							targetPosition = hitPoint,
							targetObjectId = hitTarget ? hitTarget.ObjectId : 0,
							teamSlot = clientService.GetTeamSlot(MonoBehaviourInstance<ClientService>.inst.MyPlayer
								.userId)
						};
						GameClient gameClient5 = gameClient;
						if (gameClient5 == null)
						{
							return;
						}

						gameClient5.Request(packet2);
						return;
					}
					case GameInputEvent.DeleteMarkTarget:
					{
						ReqRemoveMark packet3 = new ReqRemoveMark
						{
							teamSlot = clientService.GetTeamSlot(MonoBehaviourInstance<ClientService>.inst.MyPlayer
								.userId)
						};
						GameClient gameClient6 = gameClient;
						if (gameClient6 == null)
						{
							return;
						}

						gameClient6.Request(packet3);
						return;
					}
					case GameInputEvent.MinimapZoomIn:
						MonoBehaviourInstance<GameUI>.inst.HudButton.MapZoomIn();
						return;
					case GameInputEvent.MinimapZoomOut:
						MonoBehaviourInstance<GameUI>.inst.HudButton.MapZoomOut();
						return;
					case GameInputEvent.EmotionPlate:
						if (myCharacter.IsAlive && !MonoBehaviourInstance<GameUI>.inst.EmotionPlateHud.IsActive())
						{
							MonoBehaviourInstance<GameUI>.inst.EmotionPlateHud.Active(hitGroundPoint);
						}

						break;
					default:
						return;
				}
			}
		}


		public void OnSelectEvent(LocalObject hitTarget, Vector3 hitPoint, Vector3 releasePoint,
			bool isHitTargetableLayer)
		{
			CursorStatus cursorStatus = this.cursorStatus;
			if (cursorStatus == CursorStatus.Attack)
			{
				if (isHitTargetableLayer && IsTargetable(hitTarget) && hitTarget.IsTypeOf<LocalCharacter>())
				{
					LocalCharacter localCharacter = (LocalCharacter) hitTarget;
					if (localCharacter.IsAlive)
					{
						ShowTargetInfo(hitTarget);
						if (!clientService.IsAlly(localCharacter))
						{
							Target(localCharacter);
							SetCursorStatus(CursorStatus.Normal);
							return;
						}
					}
				}

				AttackMoveTo(hitPoint);
				SetCursorStatus(CursorStatus.Normal);
				return;
			}

			if (cursorStatus == CursorStatus.Summon)
			{
				ClearNextInteractionObject();
				InstallSummon(hitPoint, isHitTargetableLayer);
				SetCursorStatus(CursorStatus.Normal);
				return;
			}

			if (isHitTargetableLayer && IsTargetable(hitTarget) && hitTarget.IsTypeOf<LocalCharacter>() &&
			    ((LocalCharacter) hitTarget).IsAlive)
			{
				ShowTargetInfo(hitTarget);
				return;
			}

			ClearTarget();
		}


		public void AttackMoveTo(Vector3 hitPoint)
		{
			ShowAttackMoveCursor(hitPoint);
			MoveTo(
				GameUtil.DistanceOnPlane(myCharacter.GetPosition(), hitPoint) <= 0.5f
					? myCharacter.GetPosition()
					: hitPoint, cursorStatus == CursorStatus.Attack);
		}


		public void OnMoveEvent(bool isPressing, LocalObject hitTarget, Vector3 hitPoint, bool isHitTargetableLayer)
		{
			if (isHitTargetableLayer)
			{
				if (myCharacter.IsRest)
				{
					SwitchRest(false);
					return;
				}

				if (IsTargetable(hitTarget))
				{
					if (hitTarget.IsTypeOf<LocalCharacter>())
					{
						LocalPlayerCharacter localPlayerCharacter = hitTarget as LocalPlayerCharacter;
						if ((localPlayerCharacter == null || !localPlayerCharacter.IsDyingCondition) &&
						    clientService.IsAlly(localPlayerCharacter))
						{
							MoveToPoint(isPressing, hitPoint);
							SetCursorStatus(CursorStatus.Normal);
							return;
						}
					}

					Target(hitTarget);
				}
				else
				{
					MoveToPoint(isPressing, hitPoint);
				}
			}

			if (cursorStatus != CursorStatus.SkillCanMove)
			{
				SetCursorStatus(CursorStatus.Normal);
			}
		}


		private void MoveToPoint(bool isPressing, Vector3 hitPoint)
		{
			if (GameUtil.DistanceOnPlane(myCharacter.GetPosition(), hitPoint) <= 0.5f)
			{
				ClearNextInteractionObject();
				return;
			}

			if (!isPressing)
			{
				ShowMoveCursor(hitPoint);
			}

			MoveTo(hitPoint, false);
		}


		private void OnInputReleaseEvent(GameInputEvent inputEvent, bool isPressing, LocalObject hitTarget,
			Vector3 hitPoint, Vector3 hitGroundPoint, Vector3 releasePoint, bool isHitTargetableLayer)
		{
			if (!myCharacter.IsAlive)
			{
				return;
			}

			if (inputEvent != GameInputEvent.Select && inputEvent - GameInputEvent.Active1 > 4)
			{
				if (inputEvent != GameInputEvent.MaxChatWindow)
				{
					return;
				}

				MonoBehaviourInstance<GameUI>.inst.ChattingUi.SetNormalChatSize();
			}
			else
			{
				PlayerSkill playerSkill = this.playerSkill;
				if (playerSkill == null)
				{
					return;
				}

				playerSkill.OnInputReleaseEvent(inputEvent, hitTarget, hitGroundPoint, releasePoint);
			}
		}


		private void OnChangeCursorStatus(CursorStatus cursorStatus)
		{
			SetCursorStatus(cursorStatus);
		}


		public void SetCursorStatus(CursorStatus nextCursorStatus)
		{
			if (cursorStatus == nextCursorStatus)
			{
				return;
			}

			cursorStatus = nextCursorStatus;
			if (nextCursorStatus != CursorStatus.Normal)
			{
				if (nextCursorStatus - CursorStatus.Attack <= 3)
				{
					MonoBehaviourInstance<GameInput>.inst.GameCursor.SetCursorMode(CursorMode.Target);
				}
			}
			else
			{
				MonoBehaviourInstance<GameInput>.inst.GameCursor.SetCursorMode(CursorMode.Common);
			}

			HideSkillIndicator();
			HideSummonRangeIndicator();
		}


		public void ShowSkillIndicator(SkillSlotIndex skillSlotIndex)
		{
			if (cursorStatus == CursorStatus.Normal)
			{
				PlayerSkill playerSkill = this.playerSkill;
				if (playerSkill == null)
				{
					return;
				}

				playerSkill.ShowSeparateSkillIndicator(skillSlotIndex);
			}
		}


		public void HideSkillIndicator()
		{
			if (cursorStatus == CursorStatus.Normal)
			{
				PlayerSkill playerSkill = this.playerSkill;
				if (playerSkill == null)
				{
					return;
				}

				playerSkill.HideIndicator();
			}
		}


		private void HideSummonRangeIndicator()
		{
			if (lastIndicatorRange != null)
			{
				lastIndicatorRange.SetSummonRangeIndicator(false, 0f);
				lastIndicatorRange = null;
			}
		}


		private void InstallSummon(Vector3 hitPoint, bool isHitTargetableLayer)
		{
			if (isHitTargetableLayer)
			{
				Vector3 hitPoint2;
				if (IsSummonInstallablePosition(hitPoint, out hitPoint2, false))
				{
					InstallSummonRequest(hitPoint2);
					return;
				}

				MonoBehaviourInstance<GameUI>.inst.ToastMessage.ShowMessage(Ln.Get("설치할 수 없는 위치입니다."));
			}
		}


		private void InstallSummonRequest(Vector3 hitPoint)
		{
			if (myPlayer.Character.IsRest)
			{
				MonoBehaviourInstance<GameUI>.inst.ToastMessage.ShowMessage(Ln.Get("휴식 중에는 할 수 없습니다."));
				return;
			}

			if (installSummonContext == null)
			{
				return;
			}

			ActionType actionType = installSummonContext.summonData.castingActionType == CastingActionType.None
				? ActionType.NoCastInstallSummon
				: ActionType.CastInstallSummon;
			if (!MonoBehaviourInstance<ClientService>.inst.MyPlayer.Character.CanAnyAction(actionType))
			{
				return;
			}

			Vector3 position = hitPoint;
			NavMeshHit navMeshHit;
			if (NavMesh.SamplePosition(hitPoint, out navMeshHit, 5f, 2147483640))
			{
				position = navMeshHit.position;
			}

			GameClient gameClient = this.gameClient;
			if (gameClient != null)
			{
				gameClient.Request(new ReqInstallSummon
				{
					itemId = installSummonContext.itemId,
					position = position,
					madeType = ItemMadeType.None
				});
			}

			installSummonContext = null;
		}


		public void MoveTo(Vector3 destination, bool findAttackTarget)
		{
			if (IsLockInput)
			{
				return;
			}

			ClearNextInteractionObject();
			GameClient gameClient = this.gameClient;
			if (gameClient == null)
			{
				return;
			}

			gameClient.Request(new ReqMoveTo
			{
				destination = destination,
				findAttackTarget = findAttackTarget
			});
		}


		private void SetNextInteractionObject(LocalObject target)
		{
			ClearNextInteractionObject();
			nextInteractionObject = target;
			SelectionRenderer componentInChildren = nextInteractionObject.GetComponentInChildren<SelectionRenderer>();
			if (componentInChildren != null)
			{
				componentInChildren.SetSelection(true);
			}
		}


		public void ClearNextInteractionObject(ActionCategoryType canNotActionCategory = ActionCategoryType.None)
		{
			if (nextInteractionObject == null)
			{
				return;
			}

			if (canNotActionCategory.IsCanNotAction(nextInteractionObject))
			{
				return;
			}

			SelectionRenderer componentInChildren = nextInteractionObject.GetComponentInChildren<SelectionRenderer>();
			if (componentInChildren != null)
			{
				componentInChildren.SetSelection(false);
				HostileType? hostileType = null;
				LocalCharacter target;
				if ((target = nextInteractionObject as LocalCharacter) != null)
				{
					hostileType = myPlayer.GetHostileType(target);
				}

				MouseOverManager.SetOutlineColor(nextInteractionObject, hostileType);
			}

			nextInteractionObject = null;
		}


		private void Target(LocalObject target)
		{
			if (target.ObjectId == myCharacter.ObjectId)
			{
				return;
			}

			if (myCharacter.IsDyingCondition)
			{
				ShowTargetInfo(target);
				MoveTo(target.GetPosition(), false);
				return;
			}

			if (target.ObjectId == interactionBoxId)
			{
				return;
			}

			if (nextInteractionObject != null && target.ObjectId == nextInteractionObject.ObjectId)
			{
				return;
			}

			ShowTargetInfo(target);
			GameClient gameClient = this.gameClient;
			if (gameClient == null)
			{
				return;
			}

			gameClient.Request(new ReqTarget
			{
				targetId = target.ObjectId
			});
		}


		public void ShowTargetInfo(LocalObject target)
		{
			if (currentTarget != target)
			{
				ClearTarget();
			}

			LocalCharacter localCharacter = target as LocalCharacter;
			if (localCharacter == null)
			{
				SetNextInteractionObject(target);
				return;
			}

			if (!localCharacter.IsAlive)
			{
				SetNextInteractionObject(target);
				return;
			}

			myCharacter.CharacterVoiceControl.PlayCharacterVoice(CharacterVoiceType.TargetAttack, 15,
				myCharacter.GetPosition());
			if (!localCharacter.IsInAllySight(myCharacter.SightAgent))
			{
				return;
			}

			LocalSummonBase localSummonBase;
			if ((localSummonBase = target as LocalSummonBase) != null && localSummonBase.SummonData.isInvincibility)
			{
				return;
			}

			MonoBehaviourInstance<GameUI>.inst.TargetInfoHud.ShowTargetHud(target as LocalCharacter);
			currentTarget = target as LocalCharacter;
			SelectionRenderer componentInChildren = currentTarget.GetComponentInChildren<SelectionRenderer>();
			if (componentInChildren != null)
			{
				componentInChildren.SetSelection(true);
			}

			currentTarget.AttachTargetInfoHud(MonoBehaviourInstance<GameUI>.inst.TargetInfoHud);
		}


		public void HyperLoop(int hyperLoopId, int areaCode)
		{
			GameClient gameClient = this.gameClient;
			if (gameClient == null)
			{
				return;
			}

			gameClient.Request(new ReqHyperloop
			{
				areaCode = areaCode,
				objectId = hyperLoopId
			});
		}


		public void SwitchRest(bool rest)
		{
			if (rest && myCharacter.IsInCombat)
			{
				MonoBehaviourInstance<GameUI>.inst.ToastMessage.ShowMessage(Ln.Get("전투중휴식불가"));
				return;
			}

			foreach (SkillSlotIndex skillSlotIndex in skillSlotIndexes)
			{
				if (playerSkill.IsPlaying(skillSlotIndex))
				{
					MonoBehaviourInstance<GameUI>.inst.ToastMessage.ShowMessage(Ln.Get("전투중휴식불가"));
					return;
				}
			}

			if (rest && MonoBehaviourInstance<ClientService>.inst.IsFinalSafeZone() &&
			    MonoBehaviourInstance<ClientService>.inst.IsInFinalSafeZone(myCharacter, null))
			{
				MonoBehaviourInstance<GameUI>.inst.ToastMessage.ShowMessage(Ln.Get("최종안전지대휴식불가"));
				return;
			}

			if (!myCharacter.CanAnyAction(ActionType.ToRest))
			{
				return;
			}

			if (rest && myCharacter.CheckDyingCondition())
			{
				return;
			}

			CastingActionType actionCostType = ActionCostData.GetActionCostType(rest);
			MonoBehaviourInstance<ClientService>.inst.ActionCasting(myCharacter, actionCostType, delegate
			{
				if (!MonoBehaviourInstance<GameUI>.inst.Caster.IsCasting())
				{
					GameClient gameClient = this.gameClient;
					if (gameClient == null)
					{
						return;
					}

					gameClient.Request(new ReqRest
					{
						rest = rest
					});
				}
			}, null);
		}


		public void MakeItem(ItemData resultItem)
		{
			GameClient gameClient = this.gameClient;
			if (gameClient != null && gameClient.IsTutorial)
			{
				if (IsLockInput)
				{
					return;
				}

				ItemDataSlot itemDataSlot =
					MonoBehaviourInstance<GameUI>.inst.ShortCutCraftHud.FindDataSlot(resultItem);
				if (itemDataSlot == null)
				{
					return;
				}

				if (MonoBehaviourInstance<TutorialController>.inst.TutorialType == TutorialType.BasicGuide)
				{
					if (!itemDataSlot.GetNeedMarkEnable())
					{
						MonoBehaviourInstance<TutorialController>.inst.ClickCombineableNoNeed();
						return;
					}

					MonoBehaviourInstance<TutorialController>.inst.FinishTutorialBoxCombine(resultItem.code);
				}
				else if (MonoBehaviourInstance<TutorialController>.inst.TutorialType == TutorialType.PowerUp)
				{
					if (!itemDataSlot.GetNeedMarkEnable())
					{
						MonoBehaviourInstance<TutorialController>.inst.ClickCombineableNoNeed();
						return;
					}
				}
				else if (MonoBehaviourInstance<TutorialController>.inst.TutorialType == TutorialType.FinalSurvival &&
				         !itemDataSlot.GetNeedMarkEnable() &&
				         !MonoBehaviourInstance<TutorialController>.inst.MakedGatlingGun)
				{
					MonoBehaviourInstance<TutorialController>.inst.ClickCombineableNoNeed();
					return;
				}
			}

			if (!myPlayer.Character.CanAnyAction(ActionType.Craft))
			{
				return;
			}

			if (resultItem.IsLeafNodeItem())
			{
				return;
			}

			ItemType itemType = resultItem.itemType;
			Item sourceA = GameDB.item.GetItemFromCharacter(resultItem.makeMaterial1, itemType, myPlayer.Inventory,
				myPlayer.Character.Equipment);
			Item sourceB = GameDB.item.GetItemFromCharacter(resultItem.makeMaterial2, itemType, myPlayer.Inventory,
				myPlayer.Character.Equipment);
			if (sourceA == null || sourceB == null)
			{
				MonoBehaviourInstance<GameUI>.inst.ToastMessage.ShowMessage(Ln.Get("NotEnoughItem"));
				return;
			}

			Item item = new Item(0, resultItem.code, resultItem.initialCount, 0, resultItem);
			Item item2 =
				SingletonMonoBehaviour<LocalBattleEventCollector>.inst
					.OnBeforeMakeItemProcess(myPlayer.Character, item);
			item = item2 != null ? item2 : item;
			if (GameDB.item.IsCombinable(item, myPlayer.Inventory, myPlayer.Character.Equipment, ref sourceA,
				ref sourceB))
			{
				ClearNextInteractionObject();
				CastingActionType actionCostType = ActionCostData.GetActionCostType(resultItem);
				MonoBehaviourInstance<ClientService>.inst.ActionCasting(myCharacter, actionCostType, delegate
				{
					if (!MonoBehaviourInstance<GameUI>.inst.Caster.IsCasting())
					{
						MakeSourceItems.Clear();
						MakeSourceItems.Add(sourceA.id);
						MakeSourceItems.Add(sourceB.id);
						GameClient gameClient2 = this.gameClient;
						if (gameClient2 == null)
						{
							return;
						}

						gameClient2.Request(new ReqMakeItem
						{
							resultItem = resultItem.code
						});
					}
				}, null);
				return;
			}

			MonoBehaviourInstance<GameUI>.inst.ToastMessage.ShowMessage(Ln.Get("NotEnoughInventory"));
		}


		public bool IsRest()
		{
			return myCharacter != null && myCharacter.IsRest;
		}


		public bool IsMe(int objectId)
		{
			LocalPlayerCharacter localPlayerCharacter = myCharacter;
			return localPlayerCharacter != null && localPlayerCharacter.ObjectId == objectId;
		}


		public void EquipItem(Item item)
		{
			if (myCharacter.CheckDyingCondition())
			{
				return;
			}

			if (!myCharacter.CanAnyAction(ActionType.ItemEquipOrUnequip))
			{
				return;
			}

			if (item.ItemData.itemType != ItemType.Weapon && item.ItemData.itemType != ItemType.Armor)
			{
				return;
			}

			if (item.ItemData.itemType == ItemType.Weapon && !myCharacter.IsEquipableWeapon(item.ItemData))
			{
				return;
			}

			if (!myCharacter.EquipableItem(item))
			{
				MonoBehaviourInstance<GameUI>.inst.ToastMessage.ShowMessage(Ln.Get("지금은 할 수 없습니다."));
				return;
			}

			if (item.ItemData.IsThrowType())
			{
				if (myPlayer.ListCooldowns.Exists(x => x.WeaponItem.id == item.id))
				{
					myPlayer.StopBulletCoolDownIdList.Add(item.id);
				}

				Item weapon = myPlayer.Character.GetWeapon();
				if (weapon != null && weapon.ItemData.IsThrowType() && !weapon.IsFullBullet())
				{
					myPlayer.StopBulletCoolDownIdList.Add(weapon.id);
				}
			}

			GameClient gameClient = this.gameClient;
			if (gameClient == null)
			{
				return;
			}

			gameClient.Request<ReqEquipItem, ResEquipItem>(new ReqEquipItem
			{
				itemId = item.id,
				madeType = item.madeType
			}, delegate(ResEquipItem res)
			{
				if (res.errorCode != 0)
				{
					myPlayer.StopBulletCoolDownIdList.Clear();
				}
			});
		}


		public void UnequipItem(Item item)
		{
			if (myCharacter.CheckDyingCondition())
			{
				return;
			}

			if (!myCharacter.CanAnyAction(ActionType.ItemEquipOrUnequip))
			{
				return;
			}

			if (!myCharacter.UnEquipableItem(item))
			{
				MonoBehaviourInstance<GameUI>.inst.ToastMessage.ShowMessage(Ln.Get("지금은 할 수 없습니다."));
				return;
			}

			if (item.ItemData.IsThrowType() && !item.IsFullBullet())
			{
				MonoBehaviourInstance<GameUI>.inst.StatusHud.StopBulletCooldown(item.id);
			}

			GameClient gameClient = this.gameClient;
			if (gameClient == null)
			{
				return;
			}

			gameClient.Request(new ReqUnequipItem
			{
				itemId = item.id
			});
		}


		public void DropItem(SlotType slotType, Item item)
		{
			if (!myCharacter.CanAnyAction(ActionType.ItemDrop))
			{
				return;
			}

			if (!myCharacter.IsAlive)
			{
				return;
			}

			if (myCharacter.CheckDyingCondition())
			{
				return;
			}

			if (!myCharacter.UnEquipableItem(item))
			{
				MonoBehaviourInstance<GameUI>.inst.ToastMessage.ShowMessage(Ln.Get("지금은 할 수 없습니다."));
				return;
			}

			if (slotType != SlotType.Equipment)
			{
				if (slotType != SlotType.Inventory)
				{
					return;
				}

				if (item.ItemData.IsThrowType())
				{
					MonoBehaviourInstance<GameUI>.inst.InventoryHud.StopBulletCooldown(item.id);
					myPlayer.FinishBulletCooldown(item.id);
				}

				GameClient gameClient = this.gameClient;
				if (gameClient == null)
				{
					return;
				}

				gameClient.Request(new ReqDropItemFromInventory
				{
					itemId = item.id,
					madeType = item.madeType
				});
			}
			else
			{
				if (item.ItemData.IsThrowType())
				{
					MonoBehaviourInstance<GameUI>.inst.StatusHud.StopBulletCooldown(item.id);
					myPlayer.FinishBulletCooldown(item.id);
				}

				GameClient gameClient2 = gameClient;
				if (gameClient2 == null)
				{
					return;
				}

				gameClient2.Request(new ReqDropItemFromEquipment
				{
					itemId = item.id
				});
			}
		}


		public void DropItemPiece(SlotType slotType, Item item)
		{
			if (!myCharacter.CanAnyAction(ActionType.ItemDrop))
			{
				return;
			}

			if (!myCharacter.IsAlive)
			{
				return;
			}

			if (myCharacter.CheckDyingCondition())
			{
				return;
			}

			if (slotType == SlotType.Inventory)
			{
				GameClient gameClient = this.gameClient;
				if (gameClient == null)
				{
					return;
				}

				gameClient.Request(new ReqDropItemPieceFromInventory
				{
					itemId = item.id,
					madeType = item.madeType
				});
			}
		}


		public void TakeItem(Item item)
		{
			if (myCharacter.CheckDyingCondition())
			{
				return;
			}

			if (!myCharacter.CanAnyAction(ActionType.ItemPickup))
			{
				return;
			}

			if (interactionBoxId != 0)
			{
				GameClient gameClient = this.gameClient;
				if (gameClient == null)
				{
					return;
				}

				gameClient.Request(new ReqTakeItem
				{
					targetId = interactionBoxId,
					itemId = item.id
				});
			}
		}

		// 박스 열기
		public void OpenBox(BoxWindowType boxWindowType, LocalObject targetBox, List<Item> items)
		{
			if (targetBox == null)
			{
				return;
			}

			if (interactionBoxId == targetBox.ObjectId)
			{
				return;
			}

			if (myCharacter.CheckDyingCondition())
			{
				return;
			}

			if (!myCharacter.CanAnyAction(targetBox.ObjectType == ObjectType.AirSupplyItemBox
				? ActionType.AirSupplyOpen
				: ActionType.OpenNoCastBox))
			{
				return;
			}

			interactionBoxId = targetBox.ObjectId;
			targetBox.IfTypeOf<LocalItemBox>(delegate(LocalItemBox box) { box.OpenBox(); });
			UISystem.Action(new OpenBox
			{
				boxWindowType = boxWindowType,
				boxId = targetBox.ObjectId,
				boxItems = items
			});
		}


		public void CloseBox()
		{
			if (interactionBoxId != 0)
			{
				GameClient gameClient = this.gameClient;
				if (gameClient != null)
				{
					gameClient.Request<ReqCloseBox, ResSuccess>(new ReqCloseBox(),
						delegate { UISystem.Action(new CloseBox()); });
				}

				interactionBoxId = 0;
			}
		}


		public void UseItem(Item item, bool isQuickCast, Vector3 mousePos)
		{
			if (!myCharacter.IsAlive)
			{
				return;
			}

			if (myCharacter.CheckDyingCondition())
			{
				return;
			}

			if (!myCharacter.CanAnyAction(ActionType.ItemUse) && !item.IsRecoveryItem())
			{
				return;
			}

			ItemData itemData = item.ItemData;
			if (itemData.itemType == ItemType.Special)
			{
				ItemSpecialData itemData3 = item.GetItemData<ItemSpecialData>();
				if (itemData3.specialItemType == SpecialItemType.Summon)
				{
					if (myCharacter.IsConcentrating)
					{
						return;
					}

					SummonData summonData = GameDB.character.GetSummonData(itemData3.summonCode);
					if (summonData != null)
					{
						installSummonContext = new InstallSummonContext(summonData, item.id);
						if (isQuickCast)
						{
							Vector3 vector;
							MouseHit mouseHit = GetMouseHit(mousePos, GameInputEvent.None, out vector);
							bool isHitTargetableLayer = mouseHit.hitType == MouseHit.HitType.GROUND ||
							                            mouseHit.hitType == MouseHit.HitType.OBJECT;
							InstallSummon(mouseHit.point, isHitTargetableLayer);
							return;
						}

						SetCursorStatus(CursorStatus.Summon);
						SplatManager splatManager = this.splatManager;
						RangeIndicator rangeIndicator =
							(RangeIndicator) (splatManager != null
								? splatManager.GetIndicator("RangeIndicator")
								: null);
						rangeIndicator.Range = summonData.createRange;
						SplatManager splatManager2 = this.splatManager;
						if (splatManager2 != null)
						{
							splatManager2.SetIndicator(rangeIndicator);
						}

						MonoBehaviourInstance<GameUI>.inst.Minimap.UIMap.SetIndicator(rangeIndicator.Range);
					}
				}

				return;
			}

			if (itemData.itemType == ItemType.Consume)
			{
				ItemConsumableData itemData2 = item.GetItemData<ItemConsumableData>();
				if (itemData2.hpRecover > 0 || itemData2.heal > 0)
				{
					int num = 0;
					foreach (CharacterStateValue characterStateValue in MonoBehaviourInstance<ClientService>.inst
						.MyPlayer.Character.States)
					{
						if (characterStateValue.code == 10002)
						{
							num = characterStateValue.ReserveCount + 1;
							break;
						}
					}

					if (num >= 3)
					{
						MonoBehaviourInstance<GameUI>.inst.ToastMessage.ShowMessage(Ln.Get("CantConsumItem"));
						return;
					}
				}
				else if (itemData2.spRecover > 0)
				{
					int num2 = 0;
					foreach (CharacterStateValue characterStateValue2 in MonoBehaviourInstance<ClientService>.inst
						.MyPlayer.Character.States)
					{
						if (characterStateValue2.code == 10003)
						{
							num2 = characterStateValue2.ReserveCount + 1;
							break;
						}
					}

					if (num2 >= 3)
					{
						MonoBehaviourInstance<GameUI>.inst.ToastMessage.ShowMessage(Ln.Get("CantConsumItem"));
						return;
					}
				}
			}

			GameClient gameClient = this.gameClient;
			if (gameClient == null)
			{
				return;
			}

			gameClient.Request<ReqUseItem, ResUseItem>(new ReqUseItem
			{
				itemId = item.id,
				madeType = item.madeType
			}, delegate(ResUseItem res)
			{
				if (res.success && itemData.itemType == ItemType.Consume)
				{
					ItemConsumableData itemConsumableData = (ItemConsumableData) itemData;
					if (0 < itemConsumableData.hpRecover || 0 < itemConsumableData.heal)
					{
						Singleton<SoundControl>.inst.PlayUISound(string.Format("Consume_Eat_{0}", Random.Range(1, 4)));
						return;
					}

					if (0 < itemConsumableData.spRecover)
					{
						Singleton<SoundControl>.inst.PlayUISound(string.Format("Consume_Drink_{0}",
							Random.Range(1, 4)));
					}
				}
			});
		}


		public void SwapInvenItemSlot(int indexA, int indexB)
		{
			GameClient gameClient = this.gameClient;
			if (gameClient == null)
			{
				return;
			}

			gameClient.Request(new ReqSwapInvenItemSlot
			{
				indexA = indexA,
				indexB = indexB
			});
		}


		public void InsertItem(Item item)
		{
			if (myCharacter.CheckDyingCondition())
			{
				return;
			}

			if (!MonoBehaviourInstance<ClientService>.inst.MyPlayer.Character.CanAnyAction(ActionType.ItemDrop))
			{
				return;
			}

			if (interactionBoxId != 0)
			{
				if (item.ItemData.IsThrowType())
				{
					MonoBehaviourInstance<GameUI>.inst.InventoryHud.StopBulletCooldown(item.id);
					myPlayer.FinishBulletCooldown(item.id);
				}

				GameClient gameClient = this.gameClient;
				if (gameClient == null)
				{
					return;
				}

				gameClient.Request(new ReqInsertItem
				{
					targetId = interactionBoxId,
					itemId = item.id,
					madeType = item.madeType
				});
			}
		}


		public void OnInteract(LocalObject target)
		{
			if (myCharacter.CheckDyingCondition())
			{
				ClearNextInteractionObject();
				return;
			}

			if (!myCharacter.CanAnyAction(target))
			{
				MonoBehaviourInstance<GameUI>.inst.ToastMessage.ShowMessage(Ln.Get("지금은 할 수 없습니다."));
				ClearNextInteractionObject();
				return;
			}

			switch (target.ObjectType)
			{
				case ObjectType.PlayerCharacter:
				case ObjectType.Monster:
				case ObjectType.BotPlayerCharacter:
				{
					myPlayer.AddInteractObject(target);
					GameClient gameClient = this.gameClient;
					if (gameClient == null)
					{
						return;
					}

					gameClient.Request<ReqOpenCorpse, ResOpenCorpse>(new ReqOpenCorpse
					{
						targetId = target.ObjectId
					}, delegate(ResOpenCorpse res)
					{
						if (res.success)
						{
							Singleton<SoundControl>.inst.PlayFXSound("DeadOpen", "OpenBox", 15,
								target.transform.position, false);
							OpenBox(BoxWindowType.Corpse, target, res.items);
							return;
						}

						ClearNextInteractionObject();
					});
					return;
				}
				case ObjectType.Item:
				{
					myPlayer.AddInteractObject(target);
					GameClient gameClient2 = gameClient;
					if (gameClient2 == null)
					{
						return;
					}

					gameClient2.Request<ReqPickUpItem, ResPickUpItem>(new ReqPickUpItem
					{
						targetId = target.ObjectId
					}, delegate(ResPickUpItem res)
					{
						if (res.errorCode != 0)
						{
							ClearNextInteractionObject();
						}

						if (res.errorCode == 1)
						{
							MonoBehaviourInstance<GameUI>.inst.ToastMessage.ShowMessage(Ln.Get("NotEnoughInventory"));
						}
					});
					return;
				}
				case ObjectType.ItemBox:
				case ObjectType.StaticItemBox:
				case ObjectType.ResourceItemBox:
				case ObjectType.AirSupplyItemBox:
				{
					LocalItemBox itemBox = (LocalItemBox) target;
					if (!clientService.CheckCollectibleBox(myCharacter, itemBox))
					{
						ClearNextInteractionObject();
						return;
					}

					if (!clientService.CanOpenItemBox(itemBox))
					{
						ClearNextInteractionObject();
						return;
					}

					CastingActionType type = CastingActionType.None;
					switch (target.ObjectType)
					{
						case ObjectType.ItemBox:
						case ObjectType.StaticItemBox:
							type = CastingActionType.BoxOpen;
							break;
						case ObjectType.ResourceItemBox:
						{
							LocalResourceItemBox localResourceItemBox = (LocalResourceItemBox) target;
							CollectibleData collectibleData = localResourceItemBox != null
								? localResourceItemBox.CollectibleData
								: null;
							if (collectibleData != null)
							{
								type = collectibleData.castingActionType;
							}

							break;
						}
						case ObjectType.AirSupplyItemBox:
							type = CastingActionType.AirSupplyOpen;
							break;
					}

					if (nextInteractionObject != null && nextInteractionObject.ObjectId != target.ObjectId)
					{
						return;
					}

					MonoBehaviourInstance<ClientService>.inst.ActionCasting(myCharacter, type, delegate
					{
						myPlayer.AddInteractObject(target);
						SelectionRenderer component = target.GetComponent<SelectionRenderer>();
						if (component != null)
						{
							component.SetUntouched(false);
						}

						GameClient gameClient3 = gameClient;
						if (gameClient3 == null)
						{
							return;
						}

						gameClient3.Request(new ReqOpenItemBox
						{
							targetId = target.ObjectId
						});
					}, delegate { ClearNextInteractionObject(); });
					return;
				}
				case ObjectType.SummonCamera:
				case ObjectType.SummonTrap:
				case ObjectType.SummonServant:
					myPlayer.AddInteractObject(target);
					break;
				case ObjectType.Dummy:
				case ObjectType.SkillDummy:
				case ObjectType.Projectile:
				case ObjectType.HookLineProjectile:
				case ObjectType.SecurityCamera:
					break;
				case ObjectType.SecurityConsole:
					MonoBehaviourInstance<ClientService>.inst.ActionCasting(myCharacter,
						CastingActionType.AreaSecurityCameraSight, delegate
						{
							myPlayer.AddInteractObject(target);
							GameClient gameClient3 = gameClient;
							if (gameClient3 == null)
							{
								return;
							}

							gameClient3.Request(new ReqSecurityConsoleAction
							{
								targetId = target.ObjectId,
								eventType = SecurityConsoleEvent.AreaSecurityCameraSight
							});
						}, delegate { ClearNextInteractionObject(); });
					return;
				case ObjectType.Hyperloop:
					if (this.gameClient == null)
					{
						return;
					}

					if (this.gameClient.IsTutorial && !MonoBehaviourInstance<TutorialController>.inst.EnableHyperloop())
					{
						ClearNextInteractionObject();
						return;
					}

					MonoBehaviourInstance<GameUI>.inst.HyperloopWindow.SetTargetHyperLoopId(target.ObjectId);
					MonoBehaviourInstance<GameUI>.inst.OpenWindow(MonoBehaviourInstance<GameUI>.inst.HyperloopWindow);
					if (this.gameClient.IsTutorial)
					{
						MonoBehaviourInstance<TutorialController>.inst.SuccessHyperloopTutorial();
					}

					break;
				default:
					return;
			}
		}


		public void UpdateStrategySheet(int startingArea, bool updateUI = true)
		{
			if (startingArea < 0)
			{
				return;
			}

			GameClient gameClient = this.gameClient;
			if (gameClient == null)
			{
				return;
			}

			gameClient.Request(new ReqUpdateStrategySheet
			{
				areaCode = startingArea,
				updateUI = updateUI
			});
		}


		public void ForceStartGame()
		{
			GameClient gameClient = this.gameClient;
			if (gameClient == null)
			{
				return;
			}

			gameClient.Request(new ReqForceStartGame());
		}


		public void AdminCreateItem(ItemData itemData) { }


		public void SetActive(bool isActive)
		{
			this.isActive = isActive;
			GameInput gameInput = input;
			if (gameInput == null)
			{
				return;
			}

			gameInput.SetActive(isActive);
		}


		public void OnUpdateWeaponEquip(List<Item> equips)
		{
			PlayerSkill playerSkill = this.playerSkill;
			if (playerSkill != null)
			{
				playerSkill.OnUpdateWeaponEquip();
			}

			if (myCharacter != null && myCharacter.Equipment != null && myCharacter.IsEquippedWeapon())
			{
				SplatManager splatManager = this.splatManager;
				if (splatManager == null)
				{
					return;
				}

				splatManager.CreateWeaponSkillIndicators(myCharacter.GetWeapon().ItemData.GetMasteryType());
			}
		}


		public void CancelIndicator()
		{
			splatManager.CancelIndicator();
			MonoBehaviourInstance<GameUI>.inst.Minimap.UIMap.HideIndicator();
		}


		public void StartSkillIndicatorHide(string indicatorName)
		{
			if (string.IsNullOrEmpty(indicatorName))
			{
				return;
			}

			if (!splatManager.IsCurrentIndicatorCheck(indicatorName))
			{
				return;
			}

			CancelIndicator();
		}


		public void AddWalkableAreas(params int[] areaCodes)
		{
			GameClient gameClient = this.gameClient;
			if (gameClient == null)
			{
				return;
			}

			gameClient.Request<ReqWalkableArea, ResWalkableArea>(new ReqWalkableArea
			{
				areaCodes = areaCodes
			}, delegate(ResWalkableArea res)
			{
				if (myCharacter != null)
				{
					myCharacter.MoveAgent.SetWalkableNavMask(res.walkableNavMask);
				}
			});
		}


		public void NextTutorialSequence()
		{
			GameClient gameClient = this.gameClient;
			if (gameClient == null)
			{
				return;
			}

			gameClient.Request(new ReqNextTutorialSequence());
		}
	}
}