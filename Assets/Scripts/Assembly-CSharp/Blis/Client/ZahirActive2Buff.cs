using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.ZahirActive2Buff)]
	public class ZahirActive2Buff : LocalSkillScript
	{
		public override void Start()
		{
			PlaySoundPoint(Self, "zahir_Skill02_Get");
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			PlaySoundPoint(Self, "zahir_Skill02_Destroy");
		}
	}
}