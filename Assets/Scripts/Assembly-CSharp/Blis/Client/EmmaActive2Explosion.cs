using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.EmmaActive2Explosion)]
	public class EmmaActive2Explosion : LocalEmmaSkillScript
	{
		public override void Start() { }


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			if (action == 2 && targetPosition != null)
			{
				Self.PlayLocalEffectWorldPoint(
					Singleton<EmmaSkillActive2Data>.inst.FireworkHatExplosionEffectAndSoundCode, targetPosition.Value);
			}
		}


		public override void Finish(bool cancel) { }
	}
}