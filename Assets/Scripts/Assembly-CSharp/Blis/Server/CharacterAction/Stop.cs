namespace Blis.Server.CharacterAction
{
	
	public class Stop : ActionBase
	{
		
		public Stop(WorldMovableCharacter self) : base(self, false) { }

		
		public override void Start()
		{
			StopMove();
		}

		
		protected override ActionBase Update()
		{
			return null;
		}
	}
}