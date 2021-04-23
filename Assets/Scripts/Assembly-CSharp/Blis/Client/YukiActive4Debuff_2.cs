using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.YukiActive4Debuff_2)]
	public class YukiActive4Debuff_2 : LocalSkillScript
	{
		private const string FX_BI_Yuki_Skill04_Debuff = "FX_BI_Yuki_Skill04_Debuff";


		private const string Yuki_Skill04_Debuff = "Yuki_Skill04_Debuff";


		private const string Fx_Top = "Fx_Top";


		private const string Yuki_Skill04_Debuff_Sfx = "Yuki_Skill04_Debuff";


		public override void Start()
		{
			PlayEffectChildManual(Self, "Yuki_Skill04_Debuff", "FX_BI_Yuki_Skill04_Debuff", "Fx_Top");
			PlaySoundPoint(Self, "Yuki_Skill04_Debuff");
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "Yuki_Skill04_Debuff", false);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			return "";
		}
	}
}