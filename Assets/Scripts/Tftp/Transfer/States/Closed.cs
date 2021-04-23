namespace Tftp.Net.Transfer.States
{
	
	internal class Closed : BaseState
	{
		
		public override void OnStateEnter()
		{
			base.Context.Dispose();
		}
	}
}
