﻿using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.EmmaPassiveShield)]
	public class EmmaPassiveShield : SkillScript
	{
		
		protected override void Start()
		{
			base.Start();
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForFrame();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish();
		}
	}
}