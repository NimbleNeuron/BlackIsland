using System;
using BIFog;
using BIOutline;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Blis.Client
{
	public class ObserverController : SingletonMonoBehaviour<ObserverController>
	{
		private const float MouseMoveClickDelay = 0.1f;


		private readonly MouseOverManager mouseOverManager = new MouseOverManager();


		private ClientService clientService;


		private LocalCharacter currentTarget;


		private GameClient gameClient;


		private MouseHit hit;


		private GameInput input;


		private bool isActive;


		private float lastTargetSelectTime;


		private Camera mainCamera;


		private float mouseMoveClickTime;


		private MyObserverContext myContext;


		private LocalObserver myObserver;


		public CursorStatus CursorStatus { get; } = default;


		private void Awake()
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


		private void Update()
		{
			if (clientService == null || !clientService.IsGameStarted || !isActive)
			{
				return;
			}

			UpdateMouseOver();
		}


		private void OnDestroy()
		{
			input.OnMousePressed -= OnMousePressed;
			input.OnMousePressing -= OnMousePressing;
			input.OnMouseReleased -= OnMouseReleased;
			input.OnKeyPressed -= OnKeyPressed;
			input.OnKeyRelease -= OnKeyReleased;
		}


		public void Init(GameClient gameClient, ClientService clientService)
		{
			this.gameClient = gameClient;
			this.clientService = clientService;
			myContext = clientService.MyObserver;
			myObserver = clientService.MyObserver.Observer;
			MonoBehaviourInstance<FogManager>.inst.SetMyFogSight(myObserver.SightAgent.FogSight);
		}


		public void ClearTarget()
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


		private void UpdateMouseOver()
		{
			Vector3 vector;
			hit = GetMouseHit(input.GetMousePosition(), out vector);
			HostileType? hostileType = null;
			LocalCharacter target;
			if ((target = hit.target as LocalCharacter) != null)
			{
				hostileType = myContext.GetHostileType(target);
			}

			mouseOverManager.Update(hit.target, hostileType);
		}


		private MouseHit GetMouseHit(Vector3 mousePosition, out Vector3 mouseGroundHit)
		{
			Ray ray = mainCamera.ScreenPointToRay(mousePosition);
			RaycastHit raycastHit;
			mouseGroundHit =
				Physics.Raycast(ray, out raycastHit, 100f, GameConstants.LayerMask.GROUND_LAYER,
					QueryTriggerInteraction.Collide)
					? raycastHit.point
					: Vector3.zero;
			if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
			{
				return new MouseHit(MouseHit.HitType.UI, Vector3.zero, null);
			}

			RaycastHit[] array = Physics.RaycastAll(ray, 100f, GameConstants.LayerMask.PICKABLE_OBJECT_LAYER,
				QueryTriggerInteraction.Collide);
			try
			{
				LocalObject localObject = null;
				ObjectOrder objectOrder = ObjectOrder.ItemBox;
				for (int i = 0; i < array.Length; i++)
				{
					Pickable component = array[i].transform.GetComponent<Pickable>();
					if (!(component == null))
					{
						LocalObject @object = component.GetObject();
						if (!(@object == null) && @object.ObjectId != myObserver.ObjectId &&
						    @object.IsMouseHitPossible(myObserver.SightAgent, false))
						{
							ObjectOrder objectOrder2 = @object.GetObjectOrder();
							if (localObject == null || objectOrder2 < objectOrder)
							{
								objectOrder = objectOrder2;
								localObject = @object;
							}
						}
					}
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

			if (Physics.Raycast(ray, out raycastHit, 100f, GameConstants.LayerMask.FOG_LAYER,
				QueryTriggerInteraction.Collide))
			{
				return new MouseHit(MouseHit.HitType.FOG, raycastHit.point, null);
			}

			return new MouseHit(MouseHit.HitType.INVALID, Vector3.zero, null);
		}


		public void OnMousePressed(GameInputEvent inputEvent, Vector3 mousePosition)
		{
			if (clientService == null || !clientService.IsGameStarted)
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
			if (clientService == null || !clientService.IsGameStarted)
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
			if (clientService == null || !clientService.IsGameStarted)
			{
				return;
			}

			if (inputEvent == GameInputEvent.Move || inputEvent == GameInputEvent.Warp)
			{
				mouseMoveClickTime = 0f;
			}

			ProcessInputReleaseRay(inputEvent, false, mousePosition);
		}


		public void OnKeyPressed(GameInputEvent inputEvent, Vector3 mousePosition)
		{
			if (clientService == null || !clientService.IsGameStarted)
			{
				return;
			}

			if (ChangeKeyPopup.changekeyPopupOpen)
			{
				return;
			}

			ProcessInputRay(inputEvent, false, mousePosition);
		}


		public void OnKeyPressing(GameInputEvent inputEvent, Vector3 mousePosition)
		{
			if (clientService == null || !clientService.IsGameStarted)
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
			if (clientService == null || !clientService.IsGameStarted)
			{
				return;
			}

			ProcessInputReleaseRay(inputEvent, false, mousePosition);
		}


		public void ProcessInputRay(GameInputEvent eventType, bool isPressing, Vector3 mousePosition)
		{
			if (clientService == null || !clientService.IsGameStarted)
			{
				return;
			}

			Vector3 mouseGroundHit;
			MouseHit mouseHit = GetMouseHit(mousePosition, out mouseGroundHit);
			bool isHitTargetableLayer = mouseHit.hitType == MouseHit.HitType.OBJECT;
			OnInputEvent(eventType, isPressing, mouseHit.target, mouseHit.point, mouseGroundHit, isHitTargetableLayer);
		}


		public void ProcessInputPressingRay(GameInputEvent eventType, bool isPressing, Vector3 mousePosition)
		{
			if (clientService == null || !clientService.IsGameStarted)
			{
				return;
			}

			Vector3 mouseGroundHit;
			MouseHit mouseHit = GetMouseHit(mousePosition, out mouseGroundHit);
			bool isHitTargetableLayer = mouseHit.hitType == MouseHit.HitType.OBJECT;
			OnInputPressingEvent(eventType, isPressing, mouseHit.target, mouseHit.point, mouseGroundHit,
				isHitTargetableLayer);
		}


		public void ProcessInputReleaseRay(GameInputEvent eventType, bool isPressing, Vector3 mousePosition)
		{
			if (clientService == null || !clientService.IsGameStarted)
			{
				return;
			}

			Vector3 mouseGroundHit;
			MouseHit mouseHit = GetMouseHit(mousePosition, out mouseGroundHit);
			bool isHitTargetableLayer = mouseHit.hitType == MouseHit.HitType.OBJECT;
			OnInputReleaseEvent(eventType, isPressing, mouseHit.target, mouseHit.point, mouseGroundHit,
				isHitTargetableLayer);
		}


		private void OnInputEvent(GameInputEvent inputEvent, bool isPressing, LocalObject hitTarget, Vector3 hitPoint,
			Vector3 mouseGroundHit, bool isHitTargetableLayer)
		{
			if (!MonoBehaviourInstance<ClientService>.inst.IsGameStarted)
			{
				return;
			}

			if (inputEvent == GameInputEvent.Escape)
			{
				if (MonoBehaviourInstance<GameUI>.inst.ChattingUi.IsFocus)
				{
					MonoBehaviourInstance<GameUI>.inst.ChattingUi.DeactivateInput();
					return;
				}

				if (currentTarget != null)
				{
					ClearTarget();
					return;
				}

				MonoBehaviourInstance<GameUI>.inst.Escape();
			}
			else
			{
				if (inputEvent <= GameInputEvent.ChangeEnableTrackerStatusBar)
				{
					switch (inputEvent)
					{
						case GameInputEvent.Select:
							if (isHitTargetableLayer)
							{
								LocalPlayerCharacter localPlayerCharacter;
								if ((localPlayerCharacter = hitTarget as LocalPlayerCharacter) != null)
								{
									MonoBehaviourInstance<GameUI>.inst.ObserverHud.SelectTarget(
										localPlayerCharacter.TeamNumber, localPlayerCharacter.TeamSlot);
									return;
								}

								if (SetTarget(hitTarget))
								{
									return;
								}
							}

							ClearTarget();
							return;
						case GameInputEvent.Move:
							ClearTarget();
							return;
						case GameInputEvent.Stop:
						case GameInputEvent.Hold:
							break;
						default:
							switch (inputEvent)
							{
								case GameInputEvent.ChangeEnableGameUI:
									MonoBehaviourInstance<GameUI>.inst.ChangeEnableHud();
									return;
								case GameInputEvent.ChangeEnableTrackerName:
									break;
								case GameInputEvent.ChangeEnableTrackerStatusBar:
									MonoBehaviourInstance<GameUI>.inst.UITracker.ChangeEnableTrackerUIStatusBar();
									break;
								default:
									return;
							}

							break;
					}

					return;
				}

				if (inputEvent == GameInputEvent.MinimapZoomIn)
				{
					MonoBehaviourInstance<GameUI>.inst.HudButton.MapZoomIn();
					return;
				}

				if (inputEvent != GameInputEvent.MinimapZoomOut)
				{
					return;
				}

				MonoBehaviourInstance<GameUI>.inst.HudButton.MapZoomOut();
			}
		}


		private void OnInputPressingEvent(GameInputEvent inputEvent, bool isPressing, LocalObject hitTarget,
			Vector3 hitPoint, Vector3 mouseGroundHit, bool isHitTargetableLayer)
		{
			if (!MonoBehaviourInstance<ClientService>.inst.IsGameStarted)
			{
				return;
			}

			if (inputEvent != GameInputEvent.Move)
			{
				if (inputEvent - GameInputEvent.UpArrow <= 3)
				{
					MonoBehaviourInstance<MobaCamera>.inst.SetCameraMode(MobaCameraMode.Traveling);
					MonoBehaviourInstance<MobaCamera>.inst.KeyArrowMovePosition(inputEvent);
				}
			}
			else
			{
				ClearTarget();
			}
		}


		private void OnInputReleaseEvent(GameInputEvent inputEvent, bool isPressing, LocalObject hitTarget,
			Vector3 hitPoint, Vector3 mouseGroundHit, bool isHitTargetableLayer) { }


		private bool IsTargetable(LocalObject localObject)
		{
			return !(localObject == null);
		}


		public void SetTarget(int objectId)
		{
			LocalObject target = null;
			clientService.World.TryFind<LocalObject>(objectId, ref target);
			SetTarget(target);
		}


		public bool SetTarget(LocalObject target)
		{
			if (!IsTargetable(target))
			{
				return false;
			}

			int teamNumber = myObserver.TeamNumber;
			if (target.ObjectType == ObjectType.PlayerCharacter && myObserver.TeamNumber != target.TeamNumber)
			{
				myObserver.HostileAgent.SelectTeamNumber(target.TeamNumber);
			}

			SetCameraTarget(target);
			if (!(target is LocalCharacter))
			{
				return false;
			}

			LocalCharacter localCharacter = (LocalCharacter) target;
			if (localCharacter == null)
			{
				return false;
			}

			LocalSummonBase localSummonBase;
			if ((localSummonBase = target as LocalSummonBase) != null && localSummonBase.SummonData.isInvincibility)
			{
				return false;
			}

			ShowTargetInfo(localCharacter);
			currentTarget = localCharacter;
			SelectionRenderer componentInChildren = currentTarget.GetComponentInChildren<SelectionRenderer>();
			if (componentInChildren != null)
			{
				componentInChildren.SetSelection(true);
			}

			currentTarget.AttachTargetInfoHud(MonoBehaviourInstance<GameUI>.inst.TargetInfoHud);
			mouseOverManager.Update(localCharacter,
				myObserver.HostileAgent.GetHostileType(localCharacter.HostileAgent));
			UpdateUI(teamNumber, myObserver.TeamNumber);
			lastTargetSelectTime = Time.time;
			return true;
		}


		private void SetCameraTarget(LocalObject target)
		{
			if (target != null)
			{
				if (0f < lastTargetSelectTime && (currentTarget != target || 0.5f < Time.time - lastTargetSelectTime))
				{
					return;
				}

				CameraTrackingTarget(target);
				clientService.UpdateAreaChange(GetCurrentAreaData(target, clientService.CurrentLevel));
			}
		}


		private void ShowTargetInfo(LocalCharacter target)
		{
			if (currentTarget != target)
			{
				ClearTarget();
			}

			if (target == null)
			{
				return;
			}

			MonoBehaviourInstance<GameUI>.inst.TargetInfoHud.ShowTargetHud(target);
		}


		private void CameraTrackingTarget(LocalObject target)
		{
			if (target == null)
			{
				return;
			}

			MonoBehaviourInstance<MobaCamera>.inst.SetCameraMode(MobaCameraMode.Tracking);
			MonoBehaviourInstance<MobaCamera>.inst.SetTrackingTarget(target);
		}


		private void UpdateUI(int beforeTeam, int currentTeam)
		{
			foreach (LocalObject localObject in clientService.World.FindAll<LocalObject>())
			{
				LocalCharacter localCharacter = localObject as LocalCharacter;
				if (localCharacter != null)
				{
					int teamNumber = localCharacter.TeamNumber;
					if ((teamNumber == beforeTeam || teamNumber == currentTeam) && localCharacter.IsAlive &&
					    localCharacter.FloatingUi != null)
					{
						LocalSummonBase localSummonBase = localCharacter as LocalSummonBase;
						if (localSummonBase != null)
						{
							if (!localSummonBase.SummonData.isInvincibility)
							{
								localCharacter.FogHiderOnCenter.InSight();
								localCharacter.FloatingUi.Show();
							}
						}
						else
						{
							localCharacter.FogHiderOnCenter.InSight();
							localCharacter.FloatingUi.Show();
						}
					}

					if (0 < teamNumber)
					{
						localCharacter.ShowMapIcon(MiniMapIconType.Sight);
						localCharacter.ShowMiniMapIcon(MiniMapIconType.Sight);
					}

					if (localCharacter.activeOnHostileType != null)
					{
						localCharacter.activeOnHostileType.Refresh();
					}
				}
				else
				{
					LocalProjectile localProjectile = localObject as LocalProjectile;
					if (localProjectile != null && localProjectile.activeOnHostileType != null)
					{
						localProjectile.activeOnHostileType.Refresh();
					}
				}
			}

			if (MonoBehaviourInstance<GameUI>.inst.ScoreboardWindow.IsOpen)
			{
				MonoBehaviourInstance<GameUI>.inst.ScoreboardWindow.ForceRefresh();
			}
		}


		public AreaData GetCurrentTargetAreaData(LevelData currentLevel)
		{
			if (currentTarget == null)
			{
				return null;
			}

			int maskCode = 2147483640;
			ILocalMoveAgentOwner localMoveAgentOwner;
			if ((localMoveAgentOwner = currentTarget as ILocalMoveAgentOwner) != null)
			{
				maskCode = localMoveAgentOwner.MoveAgent.WalkableNavMask;
			}

			return AreaUtil.GetCurrentAreaDataByMask(currentLevel, maskCode, currentTarget.GetCurrentAreaMask());
		}


		public AreaData GetCurrentAreaData(LevelData currentLevel)
		{
			return GetCurrentAreaData(currentTarget, currentLevel);
		}


		private AreaData GetCurrentAreaData(LocalObject target, LevelData currentLevel)
		{
			AreaData areaData = null;
			if (MonoBehaviourInstance<MobaCamera>.inst.TrackingTargetLmc != null)
			{
				areaData = MonoBehaviourInstance<MobaCamera>.inst.TrackingTargetLmc.GetCurrentAreaData(currentLevel);
			}

			if (areaData == null && target != null)
			{
				areaData = target.GetCurrentAreaData(currentLevel);
			}

			return areaData;
		}
	}
}