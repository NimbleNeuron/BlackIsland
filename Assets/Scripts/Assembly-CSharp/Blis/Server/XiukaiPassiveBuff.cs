using System.Collections;
using System.Collections.Generic;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.XiukaiPassiveBuff)]
	public class XiukaiPassiveBuff : SkillScript
	{
		
		protected override void Start()
		{
			base.Start();
			Dictionary<StatType, float> dictionary = new Dictionary<StatType, float>();
			dictionary.Add(StatType.MaxHp, Singleton<XiukaiSkillPassiveData>.inst.AddMaxHp);
			Caster.Stat.UpdateStateStat(dictionary);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return null;
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
		}
	}
}