using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	public class BlockingState : CharacterState, IBlockDamage
	{
		
		
		public int RemainBlockCount
		{
			get
			{
				return this.remainBlockCount;
			}
		}

		
		public BlockingState(int stateCode, WorldCharacter self, WorldCharacter caster) : base(stateCode, self, caster)
		{
		}

		
		public void Init(float angle, int blockCount, DamageSubType[] damageSubTypeFilters)
		{
			this.angle = angle;
			this.blockCount = blockCount;
			this.damageSubTypeFilters = damageSubTypeFilters.ToList<DamageSubType>();
		}

		
		protected override void Execute()
		{
			base.Execute();
			this.remainBlockCount = this.blockCount;
		}

		
		public override bool IsDone()
		{
			return base.Duration > 0f && (base.RemainTime() < 0f || this.remainBlockCount == 0 || this.stackCount == 0);
		}

		
		public int BlockDamage(Vector3? damagePoint, DamageSubType damageSubType, int damage)
		{
			if (this.angle <= 0f || 360f <= this.angle)
			{
				this.remainBlockCount--;
				return 0;
			}
			if (damagePoint != null && this.DamageSubTypeFilter(damageSubType))
			{
				Vector3 forward = this.self.transform.forward;
				forward.y = 0f;
				Vector3 normalized = (damagePoint.Value - base.Self.GetPosition()).normalized;
				if (Vector3.Dot(forward, normalized) <= Mathf.Cos(this.angle * 0.5f * 0.017453292f))
				{
					return damage;
				}
			}
			this.remainBlockCount--;
			return 0;
		}

		
		private bool DamageSubTypeFilter(DamageSubType damageSubType)
		{
			return this.damageSubTypeFilters.Count == 0 || this.damageSubTypeFilters.Exists((DamageSubType ds) => ds == damageSubType);
		}

		
		private float angle;

		
		private int blockCount;

		
		private int remainBlockCount;

		
		public List<DamageSubType> damageSubTypeFilters = new List<DamageSubType>();
	}
}
