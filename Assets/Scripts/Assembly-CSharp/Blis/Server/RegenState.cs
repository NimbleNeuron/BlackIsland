using System.Collections.Generic;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	public class RegenState : CharacterState
	{
		
		public RegenState(int stateCode, WorldCharacter self, WorldCharacter caster) : base(stateCode, self, caster)
		{
			this.Init();
		}

		
		private void Init()
		{
			this.startTime = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
			this.lastTick = 1;
			this.hpHealRemain = 0f;
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
					HealInfo healInfo = HealInfo.Create(Mathf.FloorToInt((float)this.hpRecover * (1f + this.self.Stat.IncreaseModeHealRatio)), 0);
					healInfo.SetHealer(this.self);
					healInfo.SetStateCode(base.StateData.code);
					this.self.Heal(healInfo);
				}
				else
				{
					int num = Mathf.FloorToInt((this.hpHealPerTick + this.hpHealRemain) * (1f + this.self.Stat.IncreaseModeHealRatio));
					HealInfo healInfo2 = HealInfo.Create(num, 0);
					healInfo2.SetHealer(this.self);
					healInfo2.SetStateCode(base.StateData.code);
					this.self.Heal(healInfo2);
					this.hpHealRemain += this.hpHealPerTick - (float)num;
					this.hpRecover -= num;
				}
				this.lastTick++;
			}
		}

		
		public override void Reserve(CharacterState state)
		{
			base.Reserve(state);
			RegenState regenState = state as RegenState;
			if (this.reserveHpRecover == null)
			{
				this.reserveHpRecover = new Queue<int>();
			}
			this.reserveHpRecover.Enqueue(regenState.hpRecover);
		}

		
		public override void ExecuteReserve()
		{
			base.ExecuteReserve();
			this.Init();
			this.SetHpRecover(this.reserveHpRecover.Dequeue());
		}

		
		public void SetHpRecover(int recoverAmount)
		{
			this.hpRecover = recoverAmount;
			this.hpHealPerTick = (float)recoverAmount / 15f;
		}

		
		private int hpRecover;

		
		private float hpHealRemain;

		
		private float hpHealPerTick;

		
		private float startTime;

		
		private int lastTick;

		
		private Queue<int> reserveHpRecover;
	}
}
