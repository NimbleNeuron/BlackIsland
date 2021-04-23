using Blis.Common.Utils;

namespace Blis.Server.CharacterAction
{
	
	public class WaitForAttackTarget : ActionBase
	{
		
		private readonly float actionDoneTime;

		
		private new readonly WorldMovableCharacter self;

		
		private readonly bool selfIsMonster;

		
		private readonly WorldCharacter target;

		
		public WaitForAttackTarget(WorldMovableCharacter self, WorldCharacter target, float waitTime) : base(self,
			false)
		{
			this.self = self;
			this.target = target;
			actionDoneTime = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime + waitTime;
			selfIsMonster = self as WorldMonster != null;
		}

		
		public override WorldObject GetTarget()
		{
			return target;
		}

		
		public override void Start()
		{
			if (!CheckTargetAlive())
			{
				self.Controller.Stop();
			}
		}

		
		protected override ActionBase Update()
		{
			if (MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime >= actionDoneTime)
			{
				return new Idle(self, true);
			}

			if (selfIsMonster)
			{
				if (!CheckTargetAlive())
				{
					return new Idle(self, true);
				}

				if (CheckTargetable())
				{
					return new AttackTarget(self, target);
				}
			}

			return null;
		}

		
		private bool CheckTargetAlive()
		{
			return !(target == null) && target.IsAlive;
		}

		
		private bool CheckTargetable()
		{
			return !target.IsUntargetable();
		}
	}
}