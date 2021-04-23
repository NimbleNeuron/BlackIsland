using System;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class BaseTrackUI : BaseUI
	{
		public Vector3 offset;


		private Camera mainCamera;


		private Transform target;


		private Vector3? targetPosition;


		public Vector3 Offset => offset;


		protected override void OnEnable()
		{
			base.OnEnable();
			if (mainCamera == null)
			{
				mainCamera = Camera.main;
			}

			MonoBehaviourInstance<MainCameraEvent>.inst.OnPreRenderEvent += UpdateUI;
		}


		protected override void OnDisable()
		{
			base.OnDisable();
			if (MonoBehaviourInstance<MainCameraEvent>.inst != null)
			{
				MonoBehaviourInstance<MainCameraEvent>.inst.OnPreRenderEvent -= UpdateUI;
			}
		}


		
		
		public event Action OnUpdateUIFinish;


		public virtual void ResetUI()
		{
			target = null;
			offset = Vector3.zero;
			transform.localScale = Vector3.one;
			OnUpdateUIFinish = null;
		}


		public Transform GetTrackingTarget()
		{
			return target;
		}


		public void SetTrackingTarget(Transform target)
		{
			this.target = target;
			targetPosition = null;
			UpdateUI();
		}


		public void SetTrackingOffset(Vector3 offset)
		{
			this.offset = offset;
			UpdateUI();
		}


		public void SetTrackingPosition(Vector3 worldPos)
		{
			targetPosition = worldPos;
			target = null;
			UpdateUI();
		}


		public void ReleaseTracking()
		{
			target = null;
			targetPosition = null;
		}


		protected virtual void UpdateUI()
		{
			if (mainCamera != null)
			{
				if (target != null)
				{
					transform.position = mainCamera.WorldToScreenPoint(target.position + offset);
				}
				else if (targetPosition != null)
				{
					transform.position = mainCamera.WorldToScreenPoint(targetPosition.Value + offset);
				}

				Action onUpdateUIFinish = OnUpdateUIFinish;
				if (onUpdateUIFinish == null)
				{
					return;
				}

				onUpdateUIFinish();
			}
		}
	}
}