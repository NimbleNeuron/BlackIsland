using UnityEngine;

namespace UltraReal.MobaMovement
{
	
	public abstract class MobaMoverBase : MonoBehaviour
	{
		
		
		
		public static event MobaMoverBase.MoverHandler OnMoverAdded;

		
		
		
		public static event MobaMoverBase.MoverHandler OnMoverRemoved;

		
		protected virtual void Start()
		{
		}

		
		protected virtual void OnEnable()
		{
			if (MobaMoverBase.OnMoverAdded != null)
			{
				MobaMoverBase.OnMoverAdded(this);
			}
		}

		
		protected virtual void OnDisbled()
		{
			if (MobaMoverBase.OnMoverAdded != null)
			{
				MobaMoverBase.OnMoverRemoved(this);
			}
		}

		
		protected virtual void Update()
		{
		}

		
		public abstract void SetDestination(Vector3 location);

		
		[SerializeField]
		protected float _speed = 10f;

		
		public delegate void MoverHandler(MobaMoverBase mobaMover);
	}
}
