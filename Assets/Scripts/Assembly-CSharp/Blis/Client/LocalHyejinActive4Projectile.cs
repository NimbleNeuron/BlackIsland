using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.HyejinActive4Projectile)]
	public class LocalHyejinActive4Projectile : LocalSkillScript
	{
		public override void Start()
		{
			PlaySoundChildManual(Self, "Hyejin_Skill04_Active", 15);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			StopSoundChildManual(Self, "Hyejin_Skill04_Active");
			PlaySoundPoint(Self, "Hyejin_Skill04_End", 15);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			return "";
		}
	}
}