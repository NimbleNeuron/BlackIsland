using System;
using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using VolumetricFogAndMist;

namespace Blis.Client
{
	public class MobaCamera : MonoBehaviourInstance<MobaCamera>
	{
		private static readonly float sin5 = Mathf.Sin(0.08726646f);
		private static readonly float cos5 = Mathf.Cos(0.08726646f);

		[SerializeField] private float trackingSpeed = 0.05f;
		[SerializeField] private float travelingSpeed = 10f;
		[SerializeField] private float travelingSmoothTime = 0.05f;
		[SerializeField] private float zoomSpeed = 1.5f;
		[SerializeField] private float fvAnimationSpeed = 0.75f;
		[SerializeField] private float minSight = 8f;
		[SerializeField] private float maxSight = 12f;
		[SerializeField] private float minFV = 27f;
		[SerializeField] private float maxFV = 34f;

		private readonly int threshold = 10;
		private Vector3 _horizon;
		private Vector3 _vertical;
		private Collider cameraBoundCollider;
		private Vector3 cameraBoundPos;
		private Vector3 cameraLocalBack;
		private Vector3 cameraOriPos;
		private float cameraTravelingSpeed;
		private IEnumerator coroutineCameraFV;
		private DepthOfField depthOfField;
		private int focusTeamSlot;
		private bool holdCamera;
		private bool isFocusTeam;
		private MobaCameraMode mode;
		private Transform pivot;
		private Coroutine pivotAnimation;
		private PostProcessVolume postProcessVolume;
		private Vector3 preFrameCameraPos = Vector3.zero;
		private Vector3 preFrameReqCameraPos = Vector3.positiveInfinity;
		private float preFrameSmoothTime;
		private MobaCameraMode preMode;
		private Camera targetCamera;
		private Transform targetCameraTransform;
		private LocalObject trackingTarget;
		private LocalMovableCharacter trackingTargetLmc;
		private Transform trackingTargetTransform;
		private float zOffset;
		private Coroutine zOffsetAnimation;
		public MobaCameraMode Mode => mode;
		public MobaCameraMode PreMode => preMode;
		public Transform Pivot => pivot;
		public Vector3 GetTargetCameraPosition {
			get
			{
				if (targetCameraTransform == null)
				{
					return Vector3.zero;
				}

				return targetCameraTransform.position;
			}
		}

		public LocalObject TrackingTarget => trackingTarget;
		public LocalMovableCharacter TrackingTargetLmc => trackingTargetLmc;

		private void LateUpdate()
		{
			if (isFocusTeam)
			{
				MoveTeamCamera();
				return;
			}

			MobaCameraMode mobaCameraMode = mode;
			if (mobaCameraMode != MobaCameraMode.Tracking)
			{
				if (mobaCameraMode == MobaCameraMode.Traveling)
				{
					UpdateTravelingMode();
				}
			}
			else if (IsCameraTraveling())
			{
				SetCameraMode(MobaCameraMode.Traveling);
			}
			else
			{
				UpdateTrackingMode();
			}

			if (mode == MobaCameraMode.Tracking && trackingTarget != null)
			{
				MonoBehaviourInstance<GameUI>.inst.Events.OnUpdateMapCenter(trackingTarget.GetPosition());
				return;
			}

			if (mode == MobaCameraMode.Traveling)
			{
				MonoBehaviourInstance<GameUI>.inst.Events.OnUpdateMapCenter(transform.position);
				return;
			}

			if (mode == MobaCameraMode.Manual && preMode == MobaCameraMode.Tracking && trackingTarget != null)
			{
				MonoBehaviourInstance<GameUI>.inst.Events.OnUpdateMapCenter(trackingTarget.GetPosition());
			}
		}
		
		public event Action<MobaCameraMode> OnCameraModeChange;

		protected override void _Awake()
		{
			targetCamera = GetComponentInChildren<Camera>();
			targetCameraTransform = targetCamera.transform;
			pivot = targetCamera.transform.parent;
			Transform transform = this.transform.parent.Find("CameraBound");
			cameraBoundPos = transform.position;
			cameraBoundCollider = transform.GetComponent<Collider>();
			Init();
		}

		private void Init()
		{
			if (Singleton<LocalSetting>.inst.setting.holdCamera)
			{
				preMode = MobaCameraMode.Tracking;
				mode = MobaCameraMode.Tracking;
			}
			else
			{
				preMode = MobaCameraMode.Traveling;
				mode = MobaCameraMode.Traveling;
			}

			_horizon = Vector3.Cross(targetCamera.transform.forward, Vector3.up);
			_vertical = Vector3.Cross(_horizon, Vector3.up);
			cameraLocalBack = targetCamera.transform.localRotation * Vector3.back;
			cameraOriPos = targetCamera.transform.localPosition;
			SetCameraSpeed(Singleton<LocalSetting>.inst.setting.cameraSpeed);
		}

		public void SetTrackingTarget(LocalObject trackingTarget)
		{
			this.trackingTarget = trackingTarget;
			trackingTargetLmc = trackingTarget as LocalMovableCharacter;
			trackingTargetTransform = trackingTarget.transform;
			VolumetricFog component = targetCamera.GetComponent<VolumetricFog>();
			if (component != null)
			{
				component.character = trackingTarget.gameObject;
			}

			SetCameraPosition(trackingTarget.GetPosition(), 0f);
		}


		public void SetAudioListener(Transform targetTransform)
		{
			Transform transform = MonoBehaviourInstance<GameClient>.inst.GetAudioListener().transform;
			transform.parent = targetTransform;
			transform.localPosition = new Vector3(0f, 6f, 0f);
		}


		public void SetAudioListener()
		{
			SetAudioListener(transform);
		}


		public void SetZOffset(float offset)
		{
			zOffset = offset;
			if (zOffsetAnimation != null)
			{
				StopCoroutine(zOffsetAnimation);
			}

			Vector3 targetPos = cameraLocalBack * zOffset + cameraOriPos;
			zOffsetAnimation = this.StartThrowingCoroutine(CoroutineUtil.MoveTo(targetCamera.transform,
					targetCamera.transform.localPosition, targetPos, zoomSpeed, Space.Self, delegate
					{
						targetCamera.transform.localPosition = targetPos;
						zOffsetAnimation = null;
					}),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][SetZOffset] Message:" + exception.Message + ", StackTrace:" +
					      exception.StackTrace);
				});
		}


		public void SetPivot(Vector3 offset)
		{
			offset.y = 0f;
			if (pivotAnimation != null)
			{
				StopCoroutine(pivotAnimation);
			}

			offset = pivot.transform.InverseTransformVector(offset);
			pivotAnimation = this.StartThrowingCoroutine(CoroutineUtil.MoveTo(pivot, pivot.transform.localPosition,
					offset, zoomSpeed, Space.Self, delegate
					{
						pivot.transform.localPosition = offset;
						pivotAnimation = null;
					}),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][SetPivot] Message:" + exception.Message + ", StackTrace:" +
					      exception.StackTrace);
				});
		}


		public void SetCameraSpeed(float speed)
		{
			travelingSpeed = speed;
			cameraTravelingSpeed = travelingSpeed * 2f;
		}


		public void SetCameraMode(MobaCameraMode pMode)
		{
			if (mode == pMode)
			{
				return;
			}

			preMode = mode;
			mode = pMode;
			if (mode == MobaCameraMode.Manual && pMode == MobaCameraMode.Traveling)
			{
				MonoBehaviourInstance<GameUI>.inst.Events.OnUpdateMapCenter(transform.position);
			}

			Action<MobaCameraMode> onCameraModeChange = OnCameraModeChange;
			if (onCameraModeChange == null)
			{
				return;
			}

			onCameraModeChange(mode);
		}


		public void SetZoomSpeed(float zoomSpeed)
		{
			this.zoomSpeed = zoomSpeed;
		}


		private void UpdateTrackingMode()
		{
			if (trackingTargetTransform != null)
			{
				SetCameraPosition(trackingTargetTransform.position, trackingSpeed);
			}
		}


		private void UpdateTravelingMode()
		{
			if (holdCamera)
			{
				if (trackingTargetTransform != null)
				{
					SetCameraPosition(trackingTargetTransform.position, 0f);
				}

				return;
			}

			Vector3 worldPos = NextPosition();
			SetCameraPosition(worldPos, travelingSmoothTime);
		}


		private bool IsCameraTraveling()
		{
			if (!MonoBehaviourInstance<ClientService>.inst.IsGameStarted)
			{
				return false;
			}

			if (MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				return false;
			}

			float x = Input.mousePosition.x;
			float y = Input.mousePosition.y;
			Vector3 zero = Vector3.zero;
			return x < threshold || x > Screen.width - threshold || y < threshold || y > Screen.height - threshold;
		}


		private Vector3 NextPosition()
		{
			float x = Input.mousePosition.x;
			float y = Input.mousePosition.y;
			Vector3 vector = Vector3.zero;
			if (x < threshold)
			{
				vector += Time.deltaTime * cameraTravelingSpeed * _horizon;
			}
			else if (x > Screen.width - threshold)
			{
				vector -= Time.deltaTime * cameraTravelingSpeed * _horizon;
			}

			if (y < threshold)
			{
				vector += Time.deltaTime * cameraTravelingSpeed * _vertical;
			}
			else if (y > Screen.height - threshold)
			{
				vector -= Time.deltaTime * cameraTravelingSpeed * _vertical;
			}

			return transform.position + vector;
		}


		private void MoveTeamCamera()
		{
			if (!MonoBehaviourInstance<ClientService>.inst.IsTeamMode)
			{
				return;
			}

			if (focusTeamSlot == 1)
			{
				ResetCameraPosition();
				return;
			}

			if (focusTeamSlot == 2)
			{
				TeamMemberSlot teamMemberSlot = MonoBehaviourInstance<GameUI>.inst.TeamHud.TeamMemberSlots[0];
				if (teamMemberSlot.gameObject.activeSelf)
				{
					LocalCharacter localCharacter = null;
					if (MonoBehaviourInstance<ClientService>.inst.World.TryFind<LocalCharacter>(teamMemberSlot.ObjectId,
						ref localCharacter))
					{
						SetCameraPosition(localCharacter.GetPosition(), 0f);
					}
				}
			}
			else if (focusTeamSlot == 3)
			{
				TeamMemberSlot teamMemberSlot2 = MonoBehaviourInstance<GameUI>.inst.TeamHud.TeamMemberSlots[1];
				if (teamMemberSlot2.gameObject.activeSelf)
				{
					LocalCharacter localCharacter2 = null;
					if (MonoBehaviourInstance<ClientService>.inst.World.TryFind<LocalCharacter>(
						teamMemberSlot2.ObjectId, ref localCharacter2))
					{
						SetCameraPosition(localCharacter2.GetPosition(), 0f);
					}
				}
			}
		}


		public void KeyArrowMovePosition(GameInputEvent inputEvent)
		{
			if (holdCamera)
			{
				return;
			}

			Vector3 vector = Vector3.zero;
			if (inputEvent == GameInputEvent.UpArrow)
			{
				vector -= Time.deltaTime * cameraTravelingSpeed * _vertical;
			}
			else if (inputEvent == GameInputEvent.DownArrow)
			{
				vector += Time.deltaTime * cameraTravelingSpeed * _vertical;
			}
			else if (inputEvent == GameInputEvent.RightArrow)
			{
				vector -= Time.deltaTime * cameraTravelingSpeed * _horizon;
			}
			else if (inputEvent == GameInputEvent.LeftArrow)
			{
				vector += Time.deltaTime * cameraTravelingSpeed * _horizon;
			}

			Vector3 worldPos = transform.position + vector;
			SetCameraPosition(worldPos, travelingSmoothTime);
		}


		public void ResetCameraPosition()
		{
			if (trackingTargetTransform != null)
			{
				SetCameraPosition(trackingTargetTransform.position, 0f);
			}
		}


		public void HoldCameraInTarget(bool hold)
		{
			holdCamera = hold;
		}


		public void SetCameraPosition(Vector3 worldPos, float smoothTime)
		{
			if (preFrameReqCameraPos == worldPos && preFrameSmoothTime == smoothTime)
			{
				return;
			}

			if (mode == MobaCameraMode.Traveling && (MonoBehaviourInstance<GameUI>.inst.PingHud.IsActive() ||
			                                         MonoBehaviourInstance<GameUI>.inst.EmotionPlateHud.IsActive()))
			{
				return;
			}

			preFrameReqCameraPos = worldPos;
			preFrameSmoothTime = smoothTime;
			worldPos.y = GetCameraPosY(worldPos, smoothTime);
			Vector3 forward = targetCameraTransform.forward;
			Vector3 b = (cameraBoundPos.y - worldPos.y) / forward.y * forward;
			Vector3 vector = worldPos + b;
			Vector3 direction = vector - cameraBoundPos;
			RaycastHit raycastHit;
			if (Physics.Raycast(cameraBoundPos, direction, out raycastHit, direction.magnitude,
				GameConstants.LayerMask.CAMERA_BOUND_LAYER))
			{
				float d = (cameraBoundPos.y - preFrameCameraPos.y) / forward.y;
				Vector3 b2 = forward * d;
				Vector3 vector2 = preFrameCameraPos + b2;
				Vector3 vector3 = vector - vector2;
				float magnitude = vector3.magnitude;
				bool flag = false;
				RaycastHit raycastHit2;
				if (Physics.Raycast(vector2, vector3, out raycastHit2, magnitude,
					GameConstants.LayerMask.CAMERA_BOUND_LAYER) && preFrameCameraPos != raycastHit2.point)
				{
					worldPos = preFrameCameraPos + raycastHit2.point - vector2;
					worldPos.y = GetCameraPosY(worldPos, smoothTime);
					flag = true;
				}

				if (!flag)
				{
					float num = -vector3.x;
					float num2 = -vector3.z;
					Vector3 vector4 = new Vector3(num * cos5 - num2 * sin5, 0f, num * sin5 + num2 * cos5);
					Vector3 vector5 = new Vector3(num * cos5 + num2 * sin5, 0f, -num * sin5 + num2 * cos5);
					float magnitude2 = vector4.magnitude;
					RaycastHit raycastHit3;
					if (Physics.Raycast(vector + vector4, -vector4, out raycastHit3, magnitude2,
						GameConstants.LayerMask.CAMERA_BOUND_LAYER))
					{
						worldPos = worldPos + raycastHit3.point - vector;
					}
					else if (Physics.Raycast(vector + vector5, -vector5, out raycastHit3, magnitude2,
						GameConstants.LayerMask.CAMERA_BOUND_LAYER))
					{
						worldPos = worldPos + raycastHit3.point - vector;
					}
					else
					{
						worldPos = preFrameCameraPos;
					}

					worldPos.y = GetCameraPosY(worldPos, smoothTime);
				}
			}

			preFrameCameraPos = worldPos;
			transform.position = worldPos;
		}


		private float GetCameraPosY(Vector3 worldPos, float smoothTime)
		{
			Vector3 vector;
			MoveAgent.SamplePosition(worldPos, 2147483640, out vector);
			float num = 0f;
			return Mathf.SmoothDamp(worldPos.y, vector.y, ref num, smoothTime);
		}


		public float SightToFV(float sightRange)
		{
			float num = sightRange;
			if (num <= minSight)
			{
				num = minSight;
			}
			else if (num >= maxSight)
			{
				num = maxSight;
			}

			float num2 = (num - minSight) / (maxSight - minSight);
			return (maxFV - minFV) * num2 + minFV;
		}


		public void SetCameraFV(float destinationFV)
		{
			if (targetCamera.fieldOfView == destinationFV)
			{
				return;
			}

			if (coroutineCameraFV != null)
			{
				StopCoroutine(coroutineCameraFV);
			}

			coroutineCameraFV = CorCameraFVAnimation(destinationFV);
			this.StartThrowingCoroutine(coroutineCameraFV,
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][SetCameraFV] Message:" + exception.Message + ", StackTrace:" +
					      exception.StackTrace);
				});
		}


		private IEnumerator CorCameraFVAnimation(float destinationFV)
		{
			float time = 0f;
			float startValue = targetCamera.fieldOfView;
			float ChangeValue = destinationFV - targetCamera.fieldOfView;
			while (time <= fvAnimationSpeed)
			{
				time += Time.deltaTime;
				targetCamera.fieldOfView = startValue + ChangeValue * time / fvAnimationSpeed;
				yield return null;
			}
		}


		public void TrackingAliveTeamPlayer(bool showNextTarget)
		{
			List<PlayerContext> teamMember =
				MonoBehaviourInstance<ClientService>.inst.GetTeamMember(MonoBehaviourInstance<ClientService>.inst
					.MyTeamNumber);
			List<LocalPlayerCharacter> list = new List<LocalPlayerCharacter>(teamMember.Count);
			for (int i = 0; i < teamMember.Count; i++)
			{
				if (teamMember[i].Character.IsAlive)
				{
					list.Add(teamMember[i].Character);
				}
			}

			int count = list.Count;
			if (count == 0)
			{
				return;
			}

			if (count == 1)
			{
				SetTrackingTarget(list[0]);
				SetAudioListener(list[0].transform);
				return;
			}

			if (trackingTarget == null)
			{
				SetTrackingTarget(list[0]);
				SetAudioListener(list[0].transform);
				return;
			}

			int num = -1;
			for (int j = 0; j < count; j++)
			{
				if (trackingTarget.ObjectId == list[j].ObjectId)
				{
					num = j;
					break;
				}
			}

			if (showNextTarget)
			{
				num++;
				if (num >= count)
				{
					num = 0;
				}
			}
			else
			{
				num--;
				if (num < 0)
				{
					num = count - 1;
				}
			}

			SetTrackingTarget(list[num]);
			SetAudioListener(list[num].transform);
		}


		public void StartTeamMoveCamera(int teamSlot)
		{
			if (isFocusTeam)
			{
				return;
			}

			isFocusTeam = true;
			focusTeamSlot = teamSlot;
		}


		public void StopTeamMoveCamera(int teamSlot)
		{
			if (!isFocusTeam)
			{
				return;
			}

			if (focusTeamSlot != teamSlot)
			{
				return;
			}

			isFocusTeam = false;
			focusTeamSlot = 0;
		}
	}
}