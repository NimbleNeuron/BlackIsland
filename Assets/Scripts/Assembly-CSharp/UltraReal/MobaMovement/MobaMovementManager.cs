using UnityEngine;

namespace UltraReal.MobaMovement
{
	
	public class MobaMovementManager : MonoBehaviour
	{
		
		
		
		public static LayerMask ClickLayerMask
		{
			get
			{
				return MobaMovementManager._instance._clickLayerMask;
			}
			set
			{
				MobaMovementManager._instance._clickLayerMask = value;
			}
		}

		
		
		
		public MobaMoverBase TargetMobaMover
		{
			get
			{
				return this._targetMobaMover;
			}
			set
			{
				this._targetMobaMover = value;
			}
		}

		
		private void Awake()
		{
			MobaMovementManager._instance = this;
			if (!this._pickCamera)
			{
				this._pickCamera = Camera.main;
			}
			if (!this._pickCamera)
			{
				Debug.LogError("MobaMovementManager: Requires a Camera assignment, or a camera tagged as MainCamera");
			}
			if (!this._mobaCameraTrack)
			{
				this._mobaCameraTrack = UnityEngine.Object.FindObjectOfType<MobaCameraTrack>();
			}
			if (!this._mobaInput)
			{
				this._mobaInput = UnityEngine.Object.FindObjectOfType<MobaInput>();
			}
			if (!this._mobaInput)
			{
				Debug.Log("MobaMovementManager: There needs to be at least one MobaInput based script in the scene.");
			}
			MobaMover[] array = UnityEngine.Object.FindObjectsOfType<MobaMover>();
			if (!this._init)
			{
				foreach (MobaMover mobaMover in array)
				{
					this.OnMoverAdded(mobaMover);
				}
			}
		}

		
		private void OnEnable()
		{
			MobaMoverBase.OnMoverAdded += this.OnMoverAdded;
		}

		
		private void OnDisable()
		{
			MobaMoverBase.OnMoverRemoved += this.OnMoverRemoved;
		}

		
		private void Update()
		{
			RaycastHit hit;
			if (this._mobaInput != null && this._mobaInput.GetMobaMoveButton() && Physics.Raycast(this._pickCamera.ScreenPointToRay(this._mobaInput.GetMousePosition()), out hit, 10000f, MobaMovementManager.ClickLayerMask, QueryTriggerInteraction.Collide))
			{
				if (MobaMovementManager.OnCameraPick != null)
				{
					MobaMovementManager.OnCameraPick(this._pickCamera, hit);
				}
				if (this._mobaInput.GetMobaMoveButton() && this._targetMobaMover != null && this._mouseClickType == MobaMovementManager.MouseClickType.ClickOrHoldToMove)
				{
					this._targetMobaMover.SetDestination(hit.point);
				}
				if (this._mobaInput.GetMobaMoveButtonDown())
				{
					if (MobaMovementManager.OnCameraPickDown != null)
					{
						MobaMovementManager.OnCameraPickDown(this._pickCamera, hit);
					}
					if (this._moveToEffectPrefab != null && this._mobaInput.GetMobaMoveButton())
					{
						UnityEngine.Object.Instantiate<GameObject>(this._moveToEffectPrefab, hit.point, Quaternion.identity);
					}
					if (this._moveToEffectPrefab != null && this._mobaInput.GetMobaMoveButton() && this._mouseClickType == MobaMovementManager.MouseClickType.ClickToMove)
					{
						UnityEngine.Object.Instantiate<GameObject>(this._moveToEffectPrefab, hit.point, Quaternion.identity);
						this._targetMobaMover.SetDestination(hit.point);
					}
				}
			}
		}

		
		private void OnMoverAdded(MobaMoverBase mobaMover)
		{
			this._init = true;
			if (mobaMover.gameObject.tag == "Player")
			{
				this._targetMobaMover = mobaMover;
				if (this._mobaCameraTrack)
				{
					this._mobaCameraTrack.SetTarget(this._targetMobaMover.transform);
				}
			}
		}

		
		private void OnMoverRemoved(MobaMoverBase mobaMover)
		{
		}

		
		private static MobaMovementManager _instance;

		
		private MobaCameraTrack _mobaCameraTrack;

		
		private bool _init;

		
		public static MobaMovementManager.CameraPickEvent OnCameraPick;

		
		public static MobaMovementManager.CameraPickEvent OnCameraPickDown;

		
		public MobaInput _mobaInput;

		
		[SerializeField]
		private Camera _pickCamera = default;

		
		[SerializeField]
		private MobaMovementManager.MouseClickType _mouseClickType = MobaMovementManager.MouseClickType.ClickOrHoldToMove;

		
		[SerializeField]
		private MobaMoverBase _targetMobaMover = default;

		
		[SerializeField]
		private GameObject _moveToEffectPrefab = default;

		
		[SerializeField]
		private LayerMask _clickLayerMask = default;

		
		public delegate void CameraPickEvent(Camera fromCamera, RaycastHit hit);

		
		public enum MouseClickType
		{
			
			ClickToMove,
			
			ClickOrHoldToMove
		}
	}
}
