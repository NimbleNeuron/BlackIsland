using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	public class AirborneState : CharacterState
	{
		
		
		public float _Duration
		{
			get
			{
				return this._duration;
			}
		}

		
		
		public float? _Power
		{
			get
			{
				return this._power;
			}
		}

		
		
		public bool _Moving
		{
			get
			{
				return this._moving;
			}
		}

		
		public AirborneState(int stateCode, WorldCharacter self, WorldCharacter caster) : base(stateCode, self, caster)
		{
		}

		
		public void Init(float duration, float? power = null, bool moving = true)
		{
			this._duration = duration;
			this._power = power;
			this._moving = moving;
		}

		
		public override bool IsDone()
		{
			return this.stackCount == 0 || base.RemainTime() < 0f;
		}

		
		public override bool CancelMove()
		{
			if (base.CancelMove())
			{
				return true;
			}
			Vector3 destination;
			if (!MoveAgent.SamplePosition(this.self.GetPosition(), 2147483640, out destination) && !MoveAgent.SampleWidePosition(this.self.GetPosition(), 2147483640, out destination))
			{
				return false;
			}
			WorldMovableCharacter worldMovableCharacter;
			if ((worldMovableCharacter = (this.self as WorldMovableCharacter)) != null)
			{
				worldMovableCharacter.ResetDestination(destination);
			}
			return false;
		}

		
		private float _duration;

		
		private float? _power;

		
		private bool _moving = true;
	}
}
