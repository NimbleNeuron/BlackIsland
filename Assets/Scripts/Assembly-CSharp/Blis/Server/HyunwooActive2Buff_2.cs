﻿using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.HyunwooActive2Buff_2)]
	public class HyunwooActive2Buff_2 : SkillScript
	{
		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForSeconds(StateDuration);
			Finish();
		}
	}
}