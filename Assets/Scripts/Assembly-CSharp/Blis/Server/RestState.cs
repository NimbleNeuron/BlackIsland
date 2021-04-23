using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	public class RestState : CharacterState
	{
		
		public RestState(int stateCode, WorldCharacter self, WorldCharacter caster) : base(stateCode, self, caster)
		{
			this.startTime = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
			this.lastTick = 1;
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
				int num = 0;
				if (this.self.Status.Hp < this.self.Stat.MaxHp)
				{
					num = Mathf.FloorToInt(((float)(20 + this.self.Status.Level) + (float)this.self.Stat.MaxHp * 0.02f) * (1f + this.self.Stat.IncreaseModeHealRatio));
				}
				int num2 = 0;
				if (this.self.Status.Sp < this.self.Stat.MaxSp)
				{
					num2 = Mathf.FloorToInt(15f + (float)this.self.Status.Level * 0.5f + (float)this.self.Stat.MaxSp * 0.02f);
				}
				if (0 < num + num2)
				{
					HealInfo healInfo = HealInfo.Create(num, num2);
					healInfo.SetHealer(this.self);
					healInfo.SetStateCode(base.StateData.code);
					this.self.Heal(healInfo);
				}
			}
		}

		
		private readonly float startTime;

		
		private int lastTick;
	}
}
