using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Server.CharacterAction
{
	
	public class Hold : ActionBase
	{
		
		private const float FindTargetTick = 0.1f;

		
		private float lastFindTargetTick;

		
		private WorldCharacter target;

		
		public Hold(WorldMovableCharacter self, WorldCharacter target) : base(self, false)
		{
			this.target = target;
		}

		
		public void SetTarget(WorldCharacter target)
		{
			this.target = target;
		}

		
		public override WorldObject GetTarget()
		{
			return target;
		}

		
		public override void Start()
		{
			StopMove();
			if (self.IsDyingCondition)
			{
				return;
			}

			if (self.IsInvisible)
			{
				return;
			}

			if (!self.IsCanNormalAttack())
			{
				return;
			}

			if (target != null && !self.IsInAttackableDistance(target))
			{
				target = null;
			}

			if (target == null)
			{
				target = FindTarget();
			}

			if (target == null || !self.IsInAttackableDistance(target))
			{
				return;
			}

			self.NormalAttack(target);
		}

		
		protected override ActionBase Update()
		{
			if (self.IsDyingCondition)
			{
				return null;
			}

			if (self.IsInvisible)
			{
				return null;
			}

			if (!self.IsCanNormalAttack())
			{
				return null;
			}

			if (target != null && !self.IsInAttackableDistance(target))
			{
				target = null;
			}

			if (target == null)
			{
				if (MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - lastFindTargetTick < 0.1f)
				{
					return null;
				}

				target = FindTarget();
				lastFindTargetTick = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
			}

			if (target == null)
			{
				return null;
			}

			if (self.SkillController.IsPlaying(SkillSlotIndex.Attack))
			{
				return null;
			}

			self.NormalAttack(target);
			return null;
		}

		
		private WorldCharacter FindTarget()
		{
			WorldCharacter worldCharacter = target;
			if (!self.IsInAttackableDistance(worldCharacter) || !self.IsAttackable(worldCharacter))
			{
				worldCharacter = null;
			}

			if (worldCharacter == null)
			{
				worldCharacter = self.FindEnemyForAttack();
			}

			return worldCharacter;
		}
	}
}