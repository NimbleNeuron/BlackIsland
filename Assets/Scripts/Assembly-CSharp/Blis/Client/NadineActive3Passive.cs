using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.NadineActive3Passive)]
	public class NadineActive3Passive : LocalSkillScript
	{
		public override void Start() { }


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition)
		{
			if (actionNo == 1)
			{
				AddBulletLine(Self, "NadineActive3", target, Singleton<NadineSkillActive3Data>.inst.LineMaterial,
					Singleton<NadineSkillActive3Data>.inst.LineStartWidth,
					Singleton<NadineSkillActive3Data>.inst.LineEndWidth);
				return;
			}

			if (actionNo == 2)
			{
				RemoveBulletLine(Self, "NadineActive3");
			}
		}


		public override void Finish(bool cancel)
		{
			RemoveBulletLine(Self, "NadineActive3");
		}
	}
}