using System;
using Blis.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class MostCharacterSlot : BaseUI
	{
		private Image characterImg;


		private GameObject characterNoData;


		private Text maxKills;


		private Text top3;


		private Text useGames;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			characterImg = GameUtil.Bind<Image>(gameObject, "Bg/Portrait");
			characterNoData = transform.Find("Bg/Blank").gameObject;
			useGames = GameUtil.Bind<Text>(gameObject, "Info/Games/Count");
			maxKills = GameUtil.Bind<Text>(gameObject, "Info/MaxKill/Count");
			top3 = GameUtil.Bind<Text>(gameObject, "Info/Top3/Count");
		}


		public void SetNoData()
		{
			characterNoData.SetActive(true);
			characterImg.gameObject.SetActive(false);
			useGames.text = "-";
			maxKills.text = "-";
			top3.text = "-";
		}


		public void SetCharacter(int code)
		{
			characterNoData.SetActive(false);
			characterImg.gameObject.SetActive(true);
			characterImg.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetCharacterFullSprite(code);
		}


		public void SetUseCount(int useCount)
		{
			useGames.text = useCount.ToString();
		}


		public void SetKillCount(int killCount)
		{
			maxKills.text = killCount.ToString();
		}


		public void SetTopCount(int topCount, float topAvg)
		{
			top3.text = string.Format(string.Format("{0} ({1})%", topCount, topAvg), Array.Empty<object>());
		}
	}
}