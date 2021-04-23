using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.LiDailinNormalAttack)]
	public class LocalLiDailinNormalAttack : LocalSkillScript
	{
		private const string NormalAttackCancelTag = "LIDailinNormalAttackCancel";


		private const string attackNunchaku_r1 = "attackNunchaku_r1";


		private const string attackNunchaku_r2 = "attackNunchaku_r2";


		private int playCount;

		public override void Start()
		{
			if (3f < Time.time - lastNormalAttackTime)
			{
				playCount = 0;
			}
			else
			{
				playCount++;
			}

			lastNormalAttackTime = Time.time;
			PlayAnimation(Self, playCount % 2 == 0 ? TriggerAttack01 : TriggerAttack02);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			if (cancel)
			{
				StopEffectByTag(Self, "LIDailinNormalAttackCancel");
				StopSoundByTag(Self, "LIDailinNormalAttackCancel");
			}
		}
	}
}