using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.SisselaPassiveNormalAttackState)]
	public class LocalSisselaPassiveNormalAttackState : LocalSisselaSkill
	{
		public override void Start()
		{
			PlayEffectChildManual(Self, "passivebuff", "FX_BI_Sissela_Passive_Buff1", "Bip001 R Hand");
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			base.Play(action, target, targetPosition);
		}


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "passivebuff", true);
		}
	}
}