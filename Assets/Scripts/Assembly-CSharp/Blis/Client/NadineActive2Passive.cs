using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.NadineActive2Passive)]
	public class NadineActive2Passive : LocalSkillScript
	{
		public override void Start() { }


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition)
		{
			if (actionNo == 1)
			{
				if (SingletonMonoBehaviour<PlayerController>.inst.IsMe(Self.ObjectId))
				{
					GameObject resource = Self.LoadEffect("Fx_BI_Nadine_Skill02_Range");
					target.PlayLocalEffectChildManual("Fx_BI_Nadine_Skill02_Range", resource, null);
				}
			}
			else if (actionNo == 2 && SingletonMonoBehaviour<PlayerController>.inst.IsMe(Self.ObjectId))
			{
				target.StopLocalEffectChildManual("Fx_BI_Nadine_Skill02_Range", true);
			}
		}


		public override void Finish(bool cancel) { }
	}
}