using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	public class ShieldState : CharacterState, IBlockDamage
	{
		
		
		public int CurrentShieldAmount
		{
			get
			{
				return this.currentShieldAmount;
			}
		}

		
		public ShieldState(int stateCode, WorldCharacter self, WorldCharacter caster) : base(stateCode, self, caster)
		{
		}

		
		public void Init(float coefficient, int shieldAmount)
		{
			this.coefficient = coefficient;
			this.shieldAmount = shieldAmount;
		}

		
		protected override void Execute()
		{
			base.Execute();
			this.currentShieldAmount = Mathf.FloorToInt(((float)this.caster.Stat.AttackPower * this.coefficient + (float)this.shieldAmount) * (1f + this.self.Stat.IncreaseModeShieldRatio));
		}

		
		public override bool IsDone()
		{
			return base.Duration > 0f && (base.RemainTime() < 0f || this.currentShieldAmount == 0 || this.stackCount == 0);
		}

		
		public int BlockDamage(Vector3? damagePoint, DamageSubType damageSubType, int damage)
		{
			if (damage < this.currentShieldAmount)
			{
				this.currentShieldAmount -= damage;
				return 0;
			}
			int num = this.currentShieldAmount;
			this.currentShieldAmount = 0;
			return damage - num;
		}

		
		private float coefficient;

		
		private int shieldAmount;

		
		private int currentShieldAmount;
	}
}
