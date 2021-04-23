using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.DummyDance)]
	public class DummyDance : LocalSkillScript
	{
		private const string Hart_Skill04 = "Hart_Skill04";


		private const string FX_BI_Hart_Skill04_Range_3 = "FX_BI_Hart_Skill04_Range_3";

		public override void Start()
		{
			PlayEffectChildManual(Self, "Hart_Skill04", "FX_BI_Hart_Skill04_Range_3");
			List<ParticleSystem> list = new List<ParticleSystem>();
			Self.GetComponentsInChildren<ParticleSystem>(list);
			foreach (ParticleSystem particleSystem in list)
			{
				// co: value type
				ParticleSystem.MainModule main = particleSystem.main;
				main.loop = true;
			}
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "Hart_Skill04", true);
		}
	}
}