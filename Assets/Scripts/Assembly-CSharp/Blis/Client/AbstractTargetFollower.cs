using UnityEngine;

namespace Blis.Client
{
	public abstract class AbstractTargetFollower : MonoBehaviour
	{
		public enum UpdateType
		{
			FixedUpdate,

			LateUpdate,

			ManualUpdate
		}


		[SerializeField] protected Transform m_Target;


		[SerializeField] private bool m_AutoTargetPlayer = true;


		[SerializeField] private UpdateType m_UpdateType = default;


		protected Rigidbody targetRigidbody;


		public Transform Target => m_Target;

		protected virtual void Start()
		{
			if (m_AutoTargetPlayer)
			{
				FindAndTargetPlayer();
			}

			if (m_Target == null)
			{
				return;
			}

			targetRigidbody = m_Target.GetComponent<Rigidbody>();
		}


		private void FixedUpdate()
		{
			if (m_AutoTargetPlayer && (m_Target == null || !m_Target.gameObject.activeSelf))
			{
				FindAndTargetPlayer();
			}

			if (m_UpdateType == UpdateType.FixedUpdate)
			{
				FollowTarget(Time.deltaTime);
			}
		}


		private void LateUpdate()
		{
			if (m_AutoTargetPlayer && (m_Target == null || !m_Target.gameObject.activeSelf))
			{
				FindAndTargetPlayer();
			}

			if (m_UpdateType == UpdateType.LateUpdate)
			{
				FollowTarget(Time.deltaTime);
			}
		}


		public void ManualUpdate()
		{
			if (m_AutoTargetPlayer && (m_Target == null || !m_Target.gameObject.activeSelf))
			{
				FindAndTargetPlayer();
			}

			if (m_UpdateType == UpdateType.ManualUpdate)
			{
				FollowTarget(Time.deltaTime);
			}
		}


		protected abstract void FollowTarget(float deltaTime);


		public void FindAndTargetPlayer()
		{
			GameObject gameObject = GameObject.FindGameObjectWithTag("Player");
			if (gameObject)
			{
				SetTarget(gameObject.transform);
			}
		}


		public virtual void SetTarget(Transform newTransform)
		{
			m_Target = newTransform;
		}
	}
}