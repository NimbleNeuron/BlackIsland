using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.YukiActive1Attack)]
	public class YukiActive1Attack : LocalSkillScript
	{
		private const string Yuki_Skill01_L = "Yuki_Skill01";


		private const string Yuki_Skill01_R = "Yuki_Skill01";


		private const string Yuki_Dual_Skill01_1 = "Yuki_Skill01_1";


		private const string Yuki_Dual_Skill01_2 = "Yuki_Skill01_2";


		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill01);
			StopEffectChildManual(Self, "Yuki_Skill01", true);
			StopEffectChildManual(Self, "Yuki_Skill01", true);
			StopEffectChildManual(Self, "Yuki_Skill01_1", true);
			StopEffectChildManual(Self, "Yuki_Skill01_2", true);
			int random = GetRandom(1, 4);
			LocalPlayerCharacter.CharacterVoiceControl.PlayCharacterVoice(
				string.Format("PlaySkill1011200seq1_{0}", random), 15, Self.GetPosition(), true);
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }
	}
}