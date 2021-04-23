using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	internal class FrameCount
	{
		
		public void Update()
		{
			this.frame++;
			if (Time.time - this.frameCheckTime >= 1f)
			{
				if (!Debug.isDebugBuild && Application.targetFrameRate - this.frame >= 20)
				{
					Log.W("fps{0} is lower than targetFrame {1}hz", this.frame, Application.targetFrameRate);
				}
				this.frame = 0;
				this.frameUpdate = 0;
				this.frameCheckTime = Time.time;
			}
		}

		
		public void FrameUpdate()
		{
			this.frameUpdate++;
		}

		
		private int frame;

		
		private int frameUpdate;

		
		private float frameCheckTime;
	}
}
