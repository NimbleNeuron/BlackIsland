using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.HighAngleFireActiveProjectileDebuff)]
	public class HighAngleFireActiveProjectileDebuff : LocalSkillScript
	{
		public override void Start()
		{
			BlockAllySight(Self, BlockedSightType.HighAngleFireActiveProjectileDebuff, true);
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			BlockAllySight(Self, BlockedSightType.HighAngleFireActiveProjectileDebuff, false);
		}
	}
}