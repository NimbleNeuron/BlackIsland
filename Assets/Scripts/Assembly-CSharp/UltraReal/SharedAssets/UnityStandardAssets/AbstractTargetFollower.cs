using UnityEngine;

namespace UltraReal.SharedAssets.UnityStandardAssets
{
	
	public abstract class AbstractTargetFollower : MonoBehaviour
	{
		
		protected virtual void Start()
		{
			if (this.m_AutoTargetPlayer)
			{
				this.FindAndTargetPlayer();
			}
			if (this.m_Target == null)
			{
				return;
			}
			this.targetRigidbody = this.m_Target.GetComponent<Rigidbody>();
		}

		
		private void FixedUpdate()
		{
			if (this.m_AutoTargetPlayer && (this.m_Target == null || !this.m_Target.gameObject.activeSelf))
			{
				this.FindAndTargetPlayer();
			}
			if (this.m_UpdateType == AbstractTargetFollower.UpdateType.FixedUpdate)
			{
				this.FollowTarget(Time.deltaTime);
			}
		}

		
		private void LateUpdate()
		{
			if (this.m_AutoTargetPlayer && (this.m_Target == null || !this.m_Target.gameObject.activeSelf))
			{
				this.FindAndTargetPlayer();
			}
			if (this.m_UpdateType == AbstractTargetFollower.UpdateType.LateUpdate)
			{
				this.FollowTarget(Time.deltaTime);
			}
		}

		
		public void ManualUpdate()
		{
			if (this.m_AutoTargetPlayer && (this.m_Target == null || !this.m_Target.gameObject.activeSelf))
			{
				this.FindAndTargetPlayer();
			}
			if (this.m_UpdateType == AbstractTargetFollower.UpdateType.ManualUpdate)
			{
				this.FollowTarget(Time.deltaTime);
			}
		}

		
		protected abstract void FollowTarget(float deltaTime);

		
		public void FindAndTargetPlayer()
		{
			GameObject gameObject = GameObject.FindGameObjectWithTag("Player");
			if (gameObject)
			{
				this.SetTarget(gameObject.transform);
			}
		}

		
		public virtual void SetTarget(Transform newTransform)
		{
			this.m_Target = newTransform;
		}

		
		
		public Transform Target
		{
			get
			{
				return this.m_Target;
			}
		}

		
		[SerializeField]
		protected Transform m_Target;

		
		[SerializeField]
		private bool m_AutoTargetPlayer = true;

		
		[SerializeField]
		private AbstractTargetFollower.UpdateType m_UpdateType = default;

		
		protected Rigidbody targetRigidbody = default;

		
		public enum UpdateType
		{
			
			FixedUpdate,
			
			LateUpdate,
			
			ManualUpdate
		}
	}
}
