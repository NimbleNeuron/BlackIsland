using System.Collections.Generic;
using System.Text;
using Blis.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UIItemDrop : BaseUI
	{
		[SerializeField] private Text title = default;


		private List<TitleDescText> titleDescTexts;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			titleDescTexts = new List<TitleDescText>
			{
				new TitleDescText(GameUtil.Bind<Transform>(gameObject, "0")),
				new TitleDescText(GameUtil.Bind<Transform>(gameObject, "1"))
			};
		}


		public void EmptyUI()
		{
			foreach (TitleDescText titleDescText in titleDescTexts)
			{
				titleDescText.EmptyUI();
			}
		}


		public void UpdateUI(ItemData itemData)
		{
			ItemFindInfo itemFindInfo = GameDB.item.GetItemFindInfo(itemData.code);
			int num = 0;
			titleDescTexts.ForEach(delegate(TitleDescText x) { x.Clear(); });
			if (itemFindInfo != null)
			{
				List<int> dropArea = Singleton<ItemService>.inst.GetDropArea(itemData.code);
				if (dropArea.Count > 0)
				{
					titleDescTexts[num++].SetContent(Ln.Get("발견 장소") + " : ", GetDropAreas(dropArea));
				}

				if (num < titleDescTexts.Count && itemFindInfo.IsNeedHunt())
				{
					titleDescTexts[num++].SetContent(Ln.Get("사냥") + " : ", GetMonsterDropDesc(itemFindInfo));
				}

				if (num < titleDescTexts.Count && itemFindInfo.collectibleCode > 0)
				{
					titleDescTexts[num++]
						.SetContent(Ln.Get("채집") + " : ", GetCollectible(itemFindInfo.collectibleCode));
				}

				if (num < titleDescTexts.Count && itemFindInfo.airSupply)
				{
					string param_ = Ln.Get(LnType.GradeColor, itemData.itemGrade.ToString());
					titleDescTexts[num++].SetContent(Ln.Get("항공 보급") + " : ", Ln.Format("{0} 상자에서 탐색", param_));
				}

				if (title != null)
				{
					title.gameObject.SetActive(num > 0);
				}
			}
		}


		private string GetDropAreas(List<int> dropAreaCodes)
		{
			StringBuilder stringBuilder = GameUtil.StringBuilder;
			stringBuilder.Clear();
			if (dropAreaCodes.Count > 0)
			{
				StringBuilder stringBuilder_ = GameUtil.StringBuilder_2;
				stringBuilder_.Clear();
				foreach (int code in dropAreaCodes)
				{
					stringBuilder_.Append(LnUtil.GetAreaName(code));
					stringBuilder_.Append(", ");
				}

				RemoveLastComma(stringBuilder_);
				stringBuilder.Append(Ln.Format("{0}에서 탐색", stringBuilder_.ToString()));
			}

			return stringBuilder.ToString();
		}


		private string GetMonsterDropDesc(ItemFindInfo findInfo)
		{
			StringBuilder stringBuilder = GameUtil.StringBuilder;
			stringBuilder.Clear();
			AppendMonsterDrop(stringBuilder, "닭", findInfo.huntChicken);
			AppendMonsterDrop(stringBuilder, "박쥐", findInfo.huntBat);
			AppendMonsterDrop(stringBuilder, "멧돼지", findInfo.huntBoar);
			AppendMonsterDrop(stringBuilder, "사냥개", findInfo.huntWildDog);
			AppendMonsterDrop(stringBuilder, "늑대", findInfo.huntWolf);
			AppendMonsterDrop(stringBuilder, "곰", findInfo.huntBear);
			AppendMonsterDrop(stringBuilder, "위클라인", findInfo.huntWickline);
			RemoveLastComma(stringBuilder);
			return Ln.Format("{0}(을)를 처치", stringBuilder.ToString());
		}


		private void AppendMonsterDrop(StringBuilder builder, string monsterName, DropFrequency dropFrequency)
		{
			if (dropFrequency != DropFrequency.Never)
			{
				builder.Append(Ln.Get(monsterName));
				builder.Append("(");
				builder.Append(Ln.Get(LnType.DropFrequency, dropFrequency.ToString()));
				builder.Append("), ");
			}
		}


		private string GetCollectible(int collectibleCode)
		{
			return Ln.Format("{0}에서 채집", Ln.Get(LnType.Collectible, collectibleCode.ToString()));
		}


		private void RemoveLastComma(StringBuilder builder)
		{
			builder.Remove(builder.Length - 2, 2);
		}


		private class TitleDescText
		{
			private readonly Text desc;


			private readonly Transform parent;


			private readonly Text title;

			public TitleDescText(Transform parent)
			{
				this.parent = parent;
				title = GameUtil.Bind<Text>(parent.gameObject, "Title");
				desc = GameUtil.Bind<Text>(parent.gameObject, "Desc");
			}


			public void SetContent(string title, string desc)
			{
				parent.gameObject.SetActive(true);
				this.title.text = title;
				this.desc.text = desc;
			}


			public void Clear()
			{
				parent.gameObject.SetActive(false);
				title.text = null;
				desc.text = null;
			}


			public void EmptyUI()
			{
				title.text = "";
				desc.text = "";
			}
		}
	}
}