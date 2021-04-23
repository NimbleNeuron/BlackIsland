using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("ML Agents가 스킬을 사용한다.")]
	public class AiUseSkillByDecision : ActionTaskBase
	{
		
		protected override void OnExecuteCommandFrame()
		{
			base.EndAction(true);
		}

		
		[RequiredField]
		public BBParameter<Vector3> moveTo;
	}
}
