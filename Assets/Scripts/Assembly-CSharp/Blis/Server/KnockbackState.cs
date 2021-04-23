using System;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	public class KnockbackState : CharacterState
	{
		
		
		public Vector3 _Direction
		{
			get
			{
				return this._direction;
			}
		}

		
		
		public float _Distance
		{
			get
			{
				return this._distance;
			}
		}

		
		
		public float _Duration
		{
			get
			{
				return this._duration;
			}
		}

		
		
		public EasingFunction.Ease _Ease
		{
			get
			{
				return this._ease;
			}
		}

		
		
		public bool _IsPassingWall
		{
			get
			{
				return this._isPassingWall;
			}
		}

		
		
		public Action<SkillAgent> _OnCollisionWall
		{
			get
			{
				return this._onCollisionWall;
			}
		}

		
		public KnockbackState(int stateCode, WorldCharacter self, WorldCharacter caster) : base(stateCode, self, caster)
		{
		}

		
		public void Init(Vector3 direction, float distance, float duration, EasingFunction.Ease ease, bool isPassingWall)
		{
			this._direction = direction;
			this._distance = distance;
			this._duration = duration;
			this._ease = ease;
			this._isPassingWall = isPassingWall;
		}

		
		public void SetActionOnCollisionWall(Action<SkillAgent> action)
		{
			this._onCollisionWall = action;
		}

		
		public override bool IsDone()
		{
			return this.stackCount == 0;
		}

		
		public override bool CancelMove()
		{
			if (base.CancelMove())
			{
				return true;
			}
			Vector3 dest;
			if (!MoveAgent.SamplePosition(this.self.GetPosition(), 2147483640, out dest) && !MoveAgent.SampleWidePosition(this.self.GetPosition(), 2147483640, out dest))
			{
				return false;
			}
			if (this._distance <= 0f)
			{
				return false;
			}
			float num = GameUtil.DistanceOnPlane(this.self.GetPosition(), dest);
			float num2 = num / this._distance;
			float duration = this._duration * num2;
			Vector3 direction = GameUtil.DirectionOnPlane(this.self.GetPosition(), dest);
			this.Init(direction, num, duration, this._Ease, this._isPassingWall);
			return true;
		}

		
		private Vector3 _direction;

		
		private float _distance;

		
		private float _duration;

		
		private EasingFunction.Ease _ease;

		
		private bool _isPassingWall;

		
		private Action<SkillAgent> _onCollisionWall;
	}
}
