using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	public class MonsterResetState : CharacterState
	{
		
		public MonsterResetState(int stateCode, WorldCharacter self, WorldCharacter caster) : base(stateCode, self, caster)
		{
			this.startTime = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
			this.lastTick = 1;
			this.healPerTick = (float)self.Stat.MaxHp * 0.2f;
		}

		
		public override bool IsDone()
		{
			return false;
		}

		
		protected override void Update()
		{
			while (MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - this.startTime > (float)this.lastTick)
			{
				this.lastTick++;
				int num = Mathf.CeilToInt(this.healPerTick);
				if (num > 0)
				{
					HealInfo healInfo = HealInfo.Create(num, 0);
					healInfo.SetHealer(this.self);
					this.self.Heal(healInfo);
				}
			}
		}

		
		private float healPerTick;

		
		private float startTime;

		
		private int lastTick;
	}
}
