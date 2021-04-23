using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("지정한 위치로 이동한다.")]
	public class AiMoveTo : ActionTaskBase
	{
		
		protected override void OnExecuteCommandFrame()
		{
			if (base.agent.StateEffector.CanCharacterControl())
			{
				base.agent.Controller.MoveTo(this.moveTo.value, false);
			}
			base.EndAction(true);
		}

		
		[RequiredField]
		public BBParameter<Vector3> moveTo;
	}
}
