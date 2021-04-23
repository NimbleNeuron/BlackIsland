using System.Collections.Generic;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	public class SpRegenState : CharacterState
	{
		
		public SpRegenState(int stateCode, WorldCharacter self, WorldCharacter caster) : base(stateCode, self, caster)
		{
			this.Init();
		}

		
		private void Init()
		{
			this.startTime = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
			this.lastTick = 1;
			this.spHealRemain = 0f;
		}

		
		public override bool IsDone()
		{
			return this.lastTick > 15;
		}

		
		protected override void Update()
		{
			while (MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - this.startTime > (float)this.lastTick)
			{
				if (this.lastTick == 15)
				{
					HealInfo healInfo = HealInfo.Create(0, this.spRecover);
					healInfo.SetHealer(this.self);
					healInfo.SetStateCode(base.StateData.code);
					this.self.Heal(healInfo);
				}
				else
				{
					int num = Mathf.FloorToInt(this.spHealPerTick + this.spHealRemain);
					HealInfo healInfo2 = HealInfo.Create(0, num);
					healInfo2.SetHealer(this.self);
					healInfo2.SetStateCode(base.StateData.code);
					this.self.Heal(healInfo2);
					this.spHealRemain += this.spHealPerTick - (float)num;
					this.spRecover -= num;
				}
				this.lastTick++;
			}
		}

		
		public override void Reserve(CharacterState state)
		{
			base.Reserve(state);
			if (this.reserveSpRegenPerTicks == null)
			{
				this.reserveSpRegenPerTicks = new Queue<int>();
			}
			SpRegenState spRegenState = state as SpRegenState;
			this.reserveSpRegenPerTicks.Enqueue(spRegenState.spRecover);
		}

		
		public override void ExecuteReserve()
		{
			base.ExecuteReserve();
			this.Init();
			this.SetSpRecover(this.reserveSpRegenPerTicks.Dequeue());
		}

		
		public void SetSpRecover(int recoverAmount)
		{
			this.spRecover = recoverAmount;
			this.spHealPerTick = (float)recoverAmount / 15f;
		}

		
		private int spRecover;

		
		private float spHealRemain;

		
		private float spHealPerTick;

		
		private float startTime;

		
		private int lastTick;

		
		private Queue<int> reserveSpRegenPerTicks;
	}
}
