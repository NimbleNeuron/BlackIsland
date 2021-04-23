using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.ChiaraPassiveStigmaBuff)]
	public class LocalChiaraPassiveStigmaBuff : LocalSkillScript
	{
		private const string Passivebuff = "FX_BI_Chiara_Passive_Buff";


		private const string Passivebuff_key = "FX_BI_Chiara_Passive_Buff_key";


		public override void Start()
		{
			PlayEffectChildManual(Self, "FX_BI_Chiara_Passive_Buff_key", "FX_BI_Chiara_Passive_Buff");
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "FX_BI_Chiara_Passive_Buff_key", true);
		}
	}
}