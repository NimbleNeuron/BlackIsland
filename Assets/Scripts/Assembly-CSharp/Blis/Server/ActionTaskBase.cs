using Blis.Common.Utils;
using NodeCanvas.Framework;

namespace Blis.Server
{
	
	public class ActionTaskBase : ActionTask<WorldMovableCharacter>
	{
		
		protected override void OnExecute()
		{
			if (!MonoBehaviourInstance<GameService>.inst.IsGameStarted())
			{
				base.EndAction(false);
				return;
			}
			if (MonoBehaviourInstance<GameServer>.inst.Seq <= this.seqNumber)
			{
				base.EndAction(false);
				return;
			}
			this.seqNumber = MonoBehaviourInstance<GameServer>.inst.Seq;
			this.OnExecuteCommandFrame();
		}

		
		protected virtual void OnExecuteCommandFrame()
		{
		}

		
		private readonly WaitForFrameUpdate waitFrame = new WaitForFrameUpdate();

		
		private int seqNumber;
	}
}
