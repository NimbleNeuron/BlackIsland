using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("bbMMR(Blackboard MMR 지정)과 입력한 mmrValue와 비교한다.")]
	public class AiCheckBotMMR : ConditionTaskBase
	{
		
		protected override bool OnCheckCommandFrame()
		{
			return OperationTools.Compare(this.bbMMR.value, this.mmrValue, this.checkType);
		}

		
		public BBParameter<int> bbMMR;

		
		public int mmrValue;

		
		public CompareMethod checkType;
	}
}
