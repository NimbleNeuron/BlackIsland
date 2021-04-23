using UnityEngine;
using UnityEngine.AI;

namespace UltraReal.MobaMovement
{
	
	[RequireComponent(typeof(NavMeshAgent))]
	public class MobaMover : MobaMoverBase
	{
		
		
		
		public float Speed
		{
			get
			{
				return this._speed;
			}
			set
			{
				if (!this.agent)
				{
					this.agent = base.GetComponent<NavMeshAgent>();
				}
				this.agent.speed = value;
				this._speed = value;
			}
		}

		
		protected override void Start()
		{
			base.Start();
			this.agent = base.GetComponent<NavMeshAgent>();
			this.Speed = this._speed;
		}

		
		protected override void Update()
		{
			base.Update();
		}

		
		public override void SetDestination(Vector3 location)
		{
			if (this.agent != null && base.gameObject.activeSelf)
			{
				this.agent.SetDestination(location);
			}
		}

		
		private NavMeshAgent agent;
	}
}
