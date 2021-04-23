namespace Blis.Server.CharacterAction
{
	
	public class Idle : ActionBase
	{
		
		public Idle(WorldMovableCharacter self, bool findAttackTarget) : base(self, findAttackTarget) { }

		
		public override void Start() { }

		
		protected override ActionBase Update()
		{
			return null;
		}
	}
}