namespace Blis.Server
{
	
	public class CommonState : CharacterState
	{
		
		
		public object ExtraData
		{
			get
			{
				return this.extraData;
			}
		}

		
		public CommonState(int stateCode, WorldCharacter self, WorldCharacter caster) : base(stateCode, self, caster)
		{
			this.extraData = null;
		}

		
		public void SetExtraData(object extraData)
		{
			this.extraData = extraData;
		}

		
		public override bool IsDone()
		{
			return (this.stackCount == 0 && base.StateGroupData.defaultStack != 0) || (base.Duration > 0f && base.RemainTime() < 0f);
		}

		
		private object extraData;
	}
}
