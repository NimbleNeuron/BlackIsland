using System;
using Blis.Common;

namespace Blis.Server
{
	
	public abstract class NormalAttackScript : SkillScript
	{
		
		protected override void Start()
		{
			base.Start();
			this.isFinishNormalAttack = false;
		}

		
		protected void FinishNormalAttack()
		{
			if (base.SkillSlotSet.SlotSet2Index() != SkillSlotIndex.Attack)
			{
				return;
			}
			if (this.isFinishNormalAttack)
			{
				return;
			}
			this.isFinishNormalAttack = true;
			Action<SkillScript> onFinishNormalAttack = this.OnFinishNormalAttack;
			if (onFinishNormalAttack != null)
			{
				onFinishNormalAttack(this);
			}
			SingletonMonoBehaviour<BattleEventCollector>.inst.FinishNormalAttack(base.Target.Character, base.Caster.Character);
		}

		
		public bool IsFinishNormalAttack()
		{
			return this.isFinishNormalAttack;
		}

		
		public void FinishNormalAttackAction(Action<SkillScript> action)
		{
			this.OnFinishNormalAttack = action;
		}

		
		public const float noramlAttackCompleteDelay = 0.13f;

		
		protected bool isFinishNormalAttack;

		
		public Action<SkillScript> OnFinishNormalAttack;
	}
}
