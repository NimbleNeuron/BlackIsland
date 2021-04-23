using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	public class WaitForFrameUpdate : CustomYieldInstruction
	{
		
		
		public override bool keepWaiting
		{
			get
			{
				return MonoBehaviourInstance<GameServer>.inst.Seq <= this.startSeq - 1;
			}
		}

		
		public WaitForFrameUpdate Frame(int seq)
		{
			if (seq < 1)
			{
				seq = 1;
			}
			this.startSeq = MonoBehaviourInstance<GameServer>.inst.Seq + seq;
			return this;
		}

		
		public WaitForFrameUpdate Seconds(float seconds)
		{
			int num = Mathf.CeilToInt(seconds / 0.033333335f);
			if (num < 1)
			{
				num = 1;
			}
			this.startSeq = MonoBehaviourInstance<GameServer>.inst.Seq + num;
			return this;
		}

		
		private int startSeq;
	}
}
