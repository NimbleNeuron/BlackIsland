using System.Collections.Generic;
using NodeCanvas.Framework;
using UnityEngine;

namespace Blis.Server
{
	
	public class AiPatrol : ActionTaskBase
	{
		
		protected override string OnInit()
		{
			this.patrolPointList = new List<Vector3>
			{
				this.patrolPoint1.value,
				this.patrolPoint2.value
			};
			return base.OnInit();
		}

		
		protected override void OnExecuteCommandFrame()
		{
			if (this.patrolPointList.Count == 0)
			{
				base.EndAction(false);
				return;
			}
			if (this.patrolPointList.Count == 1)
			{
				this.index = 0;
			}
			else
			{
				this.index = (int)Mathf.Repeat((float)(this.index + 1), (float)this.patrolPointList.Count);
			}
			this.moveTarget.value = this.patrolPointList[this.index];
			base.EndAction(true);
		}

		
		public BBParameter<Vector3> patrolPoint1;

		
		public BBParameter<Vector3> patrolPoint2;

		
		public BBParameter<Vector3> moveTarget;

		
		private List<Vector3> patrolPointList;

		
		private int index = -1;
	}
}
