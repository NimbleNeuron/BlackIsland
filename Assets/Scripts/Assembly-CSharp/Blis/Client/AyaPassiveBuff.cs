using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.AyaPassiveBuff)]
	public class AyaPassiveBuff : LocalShieldStateSkill
	{
		protected override string effectKey => "Aya_Passive";


		protected override string effectName => "FX_BI_Aya_Passive_01";


		protected override string soundName => "aya_Passive_Activation_v1";


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }
	}
}