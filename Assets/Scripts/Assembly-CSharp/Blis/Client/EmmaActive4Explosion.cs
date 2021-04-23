using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.EmmaActive4Explosion)]
	public class EmmaActive4Explosion : LocalSkillScript
	{
		public override void Start() { }


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			if (action == 2 && targetPosition != null)
			{
				Self.PlayLocalEffectWorldPoint(
					Singleton<EmmaSkillActive4Data>.inst.FireworkHatExplosionEffectAndSoundCode, targetPosition.Value);
			}
		}


		public override void Finish(bool cancel) { }
	}
}