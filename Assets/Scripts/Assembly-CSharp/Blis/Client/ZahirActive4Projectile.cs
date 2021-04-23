using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.ZahirActive4Projectile)]
	public class ZahirActive4Projectile : LocalSkillScript
	{
		public override void Start()
		{
			PlayEffectPoint(Self, "FX_BI_Zahir_Skill04_Attack");
			StartCoroutine(PlaySound());
		}


		private IEnumerator PlaySound()
		{
			PlaySoundPoint(Self, "zahir_Skill04_FIre01");
			yield return new WaitForSeconds(1f);
			PlaySoundPoint(Self, "zahir_Skill04_FIre02");
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			return "";
		}
	}
}