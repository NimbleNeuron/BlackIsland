using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.LukeActive2Passive)]
	public class LukeActive2Passive : SkillScript
	{
		
		private int prevSkillSlot2Level;

		
		protected override void Start()
		{
			base.Start();
			OnUpgradePassiveSkill();
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForFrame();
		}

		
		public override void OnUpgradePassiveSkill()
		{
			base.OnUpgradePassiveSkill();
			if (prevSkillSlot2Level != SkillLevel)
			{
				int reduceCCBuffCode = Singleton<LukeSkillActive2Data>.inst.ReduceCCBuffCode;
				if (SkillLevel != 0)
				{
					Caster.RemoveStateByGroup(GameDB.characterState.GetData(reduceCCBuffCode).group, Caster.ObjectId);
				}

				CharacterState characterState = CreateState(Caster, reduceCCBuffCode, 0, 0f);
				characterState.AddExternalStat(StatType.ReduceAirborneDurationRatio,
					Singleton<LukeSkillActive2Data>.inst.ReductionCCBuffCodeByLevel[SkillLevel], StatType.None, 0f);
				characterState.AddExternalStat(StatType.ReduceFetterDurationRatio,
					Singleton<LukeSkillActive2Data>.inst.ReductionCCBuffCodeByLevel[SkillLevel], StatType.None, 0f);
				characterState.AddExternalStat(StatType.ReduceSlowDurationRatio,
					Singleton<LukeSkillActive2Data>.inst.ReductionCCBuffCodeByLevel[SkillLevel], StatType.None, 0f);
				characterState.AddExternalStat(StatType.ReduceStunDurationRatio,
					Singleton<LukeSkillActive2Data>.inst.ReductionCCBuffCodeByLevel[SkillLevel], StatType.None, 0f);
				characterState.AddExternalStat(StatType.ReduceSuppressedDurationRatio,
					Singleton<LukeSkillActive2Data>.inst.ReductionCCBuffCodeByLevel[SkillLevel], StatType.None, 0f);
				AddState(Caster, characterState);
				prevSkillSlot2Level = SkillLevel;
			}
		}
	}
}