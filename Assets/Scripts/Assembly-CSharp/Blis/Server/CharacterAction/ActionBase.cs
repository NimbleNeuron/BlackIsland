namespace Blis.Server.CharacterAction
{
	
	public abstract class ActionBase
	{
		
		public readonly WorldMovableCharacter self;

		

		
		protected ActionBase(WorldMovableCharacter self, bool findAttackTarget)
		{
			this.self = self;
			FindAttackTarget = findAttackTarget;
		}

		
		
		public bool FindAttackTarget { get; }

		
		public virtual WorldObject GetTarget()
		{
			return null;
		}

		
		protected abstract ActionBase Update();

		
		public abstract void Start();

		
		public ActionBase FrameUpdate()
		{
			ActionBase actionBase = Update();
			if (actionBase == null && FindAttackTarget)
			{
				if (!self.IsAlive || self.IsDyingCondition)
				{
					return null;
				}

				if (self.IsInvisible && !self.IsAI)
				{
					return null;
				}

				WorldCharacter worldCharacter = null;
				WorldObject target = GetTarget();
				if (target != null && target is WorldCharacter)
				{
					worldCharacter = target as WorldCharacter;
					if (worldCharacter != null && !worldCharacter.IsAlive)
					{
						worldCharacter = null;
					}
				}

				if (worldCharacter == null)
				{
					worldCharacter = self.FindEnemyForAttack();
				}

				if (worldCharacter != null && !worldCharacter.IsUntargetable())
				{
					if (self.IsInBush)
					{
						if (self.CompareBush(worldCharacter) && self.IsCanNormalAttack())
						{
							actionBase = new AttackTarget(self, worldCharacter);
						}
						else
						{
							actionBase = null;
						}
					}
					else if (self.IsCanNormalAttack())
					{
						actionBase = new AttackTarget(self, worldCharacter);
					}
				}
			}

			return actionBase;
		}

		
		protected void StopMove()
		{
			if (self.CanMove())
			{
				self.StopMove();
			}
		}
	}
}