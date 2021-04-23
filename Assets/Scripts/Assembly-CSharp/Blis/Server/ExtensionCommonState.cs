namespace Blis.Server
{
	
	public class ExtensionCommonState : CommonState
	{
		
		
		public float ExtraValue
		{
			get
			{
				return this.extraValue;
			}
		}

		
		public ExtensionCommonState(int stateCode, WorldCharacter self, WorldCharacter caster) : base(stateCode, self, caster)
		{
		}

		
		public void SetExtraValue(float extraValue)
		{
			this.extraValue = extraValue;
		}

		
		private float extraValue;
	}
}
