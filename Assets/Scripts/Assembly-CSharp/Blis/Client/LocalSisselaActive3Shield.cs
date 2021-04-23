using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.SisselaActive3Shield)]
	public class LocalSisselaActive3Shield : LocalShieldStateSkill
	{
		protected override string effectKey => "Sissela_SkillE";


		protected override string effectName => "FX_BI_Sissela_Skill03_Shield_Loop";


		protected override string soundName => "Sissela_Skill03_Shield";


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }
	}
}