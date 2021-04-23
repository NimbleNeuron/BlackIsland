namespace Blis.Server
{
	
	public class GrabState : CharacterState
	{
		
		
		public WorldCharacter Graber
		{
			get
			{
				return this.graber;
			}
		}

		
		
		public float Speed
		{
			get
			{
				return this.speed;
			}
		}

		
		public GrabState(int stateCode, WorldCharacter self, WorldCharacter caster) : base(stateCode, self, caster)
		{
		}

		
		public void Init(WorldCharacter graber, float speed)
		{
			this.graber = graber;
			this.speed = speed;
		}

		
		public override bool IsDone()
		{
			return this.stackCount == 0;
		}

		
		private WorldCharacter graber;

		
		private float speed;
	}
}
