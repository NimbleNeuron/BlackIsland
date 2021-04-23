using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.ShoichiPassiveDaggerRemove)]
	public class ShoichiPassiveDaggerRemove : LocalSkillScript
	{
		public override void Start()
		{
			PlaySoundPoint(Self, "Shoichi_Passive_Dager_Get", 15);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }
	}
}