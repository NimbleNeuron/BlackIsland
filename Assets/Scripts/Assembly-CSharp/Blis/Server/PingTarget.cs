using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	public class PingTarget : MonoBehaviour
	{
		
		
		public BlisVector[] MarkInfos
		{
			get
			{
				return this.markInfos;
			}
		}

		
		
		public bool ForbidPing
		{
			get
			{
				return this.forbidPing;
			}
		}

		
		private void Awake()
		{
			this.markInfos = new BlisVector[5];
			this.currentPingNumber = 0;
		}

		
		public void AddMarkTarget(int teamSlot, Vector3 targetPosition)
		{
			this.RemoveMark(teamSlot);
			if (teamSlot < this.markInfos.Length)
			{
				this.markInfos[teamSlot] = new BlisVector(targetPosition);
			}
		}

		
		public bool IsAdjacentMark(int teamSlot, Vector3 targetPosition)
		{
			return teamSlot < this.markInfos.Length && this.markInfos[teamSlot] != null && Vector3.Distance(this.markInfos[teamSlot].ToVector3(), targetPosition) < 0.4f;
		}

		
		public void RemoveMark(int teamSlot)
		{
			if (teamSlot < this.markInfos.Length)
			{
				this.markInfos[teamSlot] = null;
			}
		}

		
		public void SendPingTarget()
		{
			if (this.forbidPing)
			{
				return;
			}
			if (this.currentPingNumber > 10)
			{
				this.forbidPing = true;
				base.StartCoroutine(this.PingOffForbid());
				return;
			}
			this.currentPingNumber++;
			base.StartCoroutine(this.PingOff());
		}

		
		private IEnumerator PingOffForbid()
		{
			yield return this.waitPingForbid;
			this.forbidPing = false;
		}

		
		private IEnumerator PingOff()
		{
			yield return this.waitPingCheck;
			this.currentPingNumber--;
		}

		
		private int currentPingNumber;

		
		private BlisVector[] markInfos;

		
		private bool forbidPing;

		
		private WaitForSeconds waitPingCheck = new WaitForSeconds(3f);

		
		private WaitForSeconds waitPingForbid = new WaitForSeconds(5f);
	}
}
