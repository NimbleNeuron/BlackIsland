using NodeCanvas.Framework;

namespace Blis.Server
{
	
	public class ConditionTaskBase : ConditionTask<WorldMovableCharacter>
	{
		
		protected override bool OnCheck()
		{
			return this.OnCheckCommandFrame();
		}

		
		protected virtual bool OnCheckCommandFrame()
		{
			return true;
		}

		
		private int seqNumber;
	}
}
