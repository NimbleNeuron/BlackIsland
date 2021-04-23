using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.AdrianaNormalAttack)]
	public class AdrianaNormalAttack : LocalSkillScript
	{
		private const string NormalAttackCancelTag = "AdrianaNormalAttackCancel";


		private int playCount;

		public override void Start()
		{
			playCount++;
			if (3f < Time.time - lastNormalAttackTime)
			{
				playCount = 0;
			}

			lastNormalAttackTime = Time.time;
			PlayAnimation(Self, 0 < playCount % 2 ? TriggerAttack01 : TriggerAttack02);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			if (cancel)
			{
				StopEffectByTag(Self, "AdrianaNormalAttackCancel");
				StopSoundByTag(Self, "AdrianaNormalAttackCancel");
			}
		}
	}
}