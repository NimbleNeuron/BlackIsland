using Blis.Client.UIModel;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class StatExtension : BaseUI
	{
		[SerializeField] private Text range = default;


		[SerializeField] private Text blood = default;


		[SerializeField] private Text critical = default;


		[SerializeField] private Text hpGen = default;


		[SerializeField] private Text spGen = default;


		[SerializeField] private Text sight = default;


		[SerializeField] private Text cooldown = default;

		public void SetStat(UICharacterStat stat)
		{
			critical.text = string.Format("{0:0.##}%", stat.criticalStrikeChance);
			range.text = string.Format("{0:0.###}", stat.attackRange);
			blood.text = string.Format("{0:0}%", stat.blood * 100f);
			sight.text = string.Format("{0:0.#}", stat.sightRange);
			hpGen.text = string.Format("{0:0.#}", stat.hpGen);
			spGen.text = string.Format("{0:0.#}", stat.spGen);
			cooldown.text = string.Format("{0:0}%", stat.cooldown * 100f);
		}
	}
}