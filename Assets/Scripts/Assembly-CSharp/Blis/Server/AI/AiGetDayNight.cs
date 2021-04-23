using Blis.Common;
using Blis.Common.Utils;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("현재 밤/낮 정보를 가져와서 dayNight에 저장한다.")]
	public class AiGetDayNight : ActionTaskBase
	{
		
		protected override void OnExecuteCommandFrame()
		{
			this.dayNight.value = MonoBehaviourInstance<GameService>.inst.Area.DayNight;
			base.EndAction(true);
		}

		
		public BBParameter<DayNight> dayNight;
	}
}
