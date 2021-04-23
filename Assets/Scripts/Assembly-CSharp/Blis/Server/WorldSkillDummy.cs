using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[ObjectAttr(ObjectType.SkillDummy)]
	public class WorldSkillDummy : WorldDummy
	{
		
		protected override ObjectType GetObjectType()
		{
			return ObjectType.SkillDummy;
		}

		
		
		public int DummySkillStateCode
		{
			get
			{
				return this.dummySkillStateCode;
			}
		}

		
		
		public float DummySkillStateDuration
		{
			get
			{
				return this.dummySkillStateDuration;
			}
		}

		
		
		public SkillAgent SkillTarget
		{
			get
			{
				return this.skillTarget;
			}
		}

		
		public override void Init(int prefabNo)
		{
			base.Init(prefabNo);
			base.StartCoroutine(CoroutineUtil.DelayedAction(1f, delegate()
			{
				SkillData skillData = GameDB.skill.GetSkillData(Singleton<DummySkillData>.inst.DummySkillId);
				SkillUseInfo skillUseInfo = SkillUseInfo.Create(this.mySkillAgent, this.mySkillAgent, skillData, SkillSlotSet.Passive_1, MasteryType.None, 1, Vector3.zero, Vector3.zero, null, true);
				base.PlayPassiveSkill(skillUseInfo);
			}));
		}

		
		public void SetSkillState(SkillAgent skillAgent, int stateCode, float duration)
		{
			this.skillTarget = skillAgent;
			this.dummySkillStateCode = stateCode;
			this.dummySkillStateDuration = duration;
		}

		
		public override void DestroySelf()
		{
			this.skillTarget = null;
			base.StartCoroutine(CoroutineUtil.DelayedAction(1, delegate()
			{
				MonoBehaviourInstance<GameService>.inst.Spawn.DestroyWorldObject(this);
			}));
		}

		
		[SerializeField]
		private int dummySkillStateCode;

		
		private float dummySkillStateDuration;

		
		private SkillAgent skillTarget;
	}
}
