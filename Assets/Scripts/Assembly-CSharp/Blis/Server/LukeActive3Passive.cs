using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.LukeActive3Passive)]
	public class LukeActive3Passive : SkillScript
	{
		
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
			CharacterStateData data =
				GameDB.characterState.GetData(Singleton<LukeSkillActive3Data>.inst.SilentVacuumCleanerStateCode);
			if (!Caster.IsHaveStateByGroup(data.group, Caster.ObjectId))
			{
				AddStateLukeActive3NoiseIgnore(Caster);
				AddState(Caster, Singleton<LukeSkillActive3Data>.inst.SilentVacuumCleanerStateCode);
			}
		}

		
		private void AddStateLukeActive3NoiseIgnore(SkillAgent caster)
		{
			AddState(caster, CharacterStateDB.NoiseIgnoreCode[NoiseType.BasicHit]);
			AddState(caster, CharacterStateDB.NoiseIgnoreCode[NoiseType.Gunshot]);
			AddState(caster, CharacterStateDB.NoiseIgnoreCode[NoiseType.MonsterKilled]);
			AddState(caster, CharacterStateDB.NoiseIgnoreCode[NoiseType.PlayerKilled]);
		}
	}
}