using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.ShoichiActive2Debuff)]
	public class ShoichiActive2Debuff : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		private float stateDuration;

		
		protected override void Start()
		{
			base.Start();
			if (stateDuration == 0f)
			{
				CharacterStateData data =
					GameDB.characterState.GetData(Singleton<ShoichiSkillActive2Data>.inst.DebuffState);
				stateDuration = data != null ? data.duration : 0f;
			}
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForSeconds(stateDuration);
			Finish();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			parameterCollection.Clear();
			parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
				Singleton<ShoichiSkillActive2Data>.inst.SkillApCoef);
			parameterCollection.Add(SkillScriptParameterType.Damage,
				Singleton<ShoichiSkillActive2Data>.inst.DamageByLevel[SkillLevel]);
			DamageTo(Target, DamageType.Skill, DamageSubType.Normal, 0, parameterCollection,
				Singleton<ShoichiSkillActive2Data>.inst.EffectAndSoundCode);
		}
	}
}