using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.LukePassiveAddStack)]
	public class LukePassiveAddStack : LocalSkillScript
	{
		private const string Passive_Buff = "FX_BI_Luke_Passive_Cleaning";


		private const string Passive_key = "Effect_Passive_key";


		public override void Start()
		{
			PlayEffectChildManual(Self, "Effect_Passive_key", "FX_BI_Luke_Passive_Cleaning");
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }
	}
}