using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.SilviaHumanNormalAttackMultifyState)]
	public class LocalSilviaHumanNormalAttackMultifyState : LocalSkillScript
	{
		public override void Start()
		{
			PlayEffectChildManual(Self, "Silvia_Skill08_Buff", "FX_BI_Silvia_Skill08_Buff", "Bip001 R Hand");
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "Silvia_Skill08_Buff", true);
		}
	}
}