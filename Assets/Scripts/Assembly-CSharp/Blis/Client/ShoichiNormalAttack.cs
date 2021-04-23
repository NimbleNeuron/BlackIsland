using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.ShoichiNormalAttack)]
	public class ShoichiNormalAttack : LocalSkillScript
	{
		private int playCount;


		public override void Start()
		{
			playCount++;
			if (Singleton<ShoichiSkillNormalAttackData>.inst.NormalAttackNextTime < Time.time - lastNormalAttackTime)
			{
				playCount = 0;
			}

			lastNormalAttackTime = Time.time;
			PlayAnimation(Self, playCount % 2 == 0 ? TriggerAttack01 : TriggerAttack02);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }
	}
}