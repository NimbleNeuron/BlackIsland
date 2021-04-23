using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class ProductionGoalWindow : BaseWindow
	{
		[SerializeField] private UIMap uiMap = default;


		[SerializeField] private ItemDataSlotTable itemDataSlotTable = default;


		[SerializeField] private Text countDownText = default;


		[SerializeField] private UILineRenderer uiLineRendererFavorite = default;


		[SerializeField] private LnText guideText = default;


		private readonly List<Vector2> areaPosition = new List<Vector2>
		{
			new Vector2(240f, 16f),
			new Vector2(366f, 296f),
			new Vector2(67f, 179f),
			new Vector2(151f, 90f),
			new Vector2(212f, 473f),
			new Vector2(74f, 280f),
			new Vector2(304f, 375f),
			new Vector2(485f, 255f),
			new Vector2(415f, 391f),
			new Vector2(97f, 402f),
			new Vector2(320f, 191f),
			new Vector2(197f, 174f),
			new Vector2(410f, 65f),
			new Vector2(284f, 105f),
			new Vector2(169f, 344f)
		};


		private Coroutine closeTimeoutRoutine;


		public UIMap UIMap => uiMap;


		public void LoadData(Favorite favorite)
		{
			InitMap(favorite.paths);
			itemDataSlotTable.Clear();
			for (int i = 0; i < favorite.weaponCodes.Count; i++)
			{
				ItemData itemData = GameDB.item.FindItemByCode(favorite.weaponCodes[i]);
				ItemDataSlot itemDataSlot = itemDataSlotTable.CreateSlot(itemData);
				itemDataSlot.SetItemData(itemData);
				itemDataSlot.SetSlotType(SlotType.None);
				itemDataSlot.SetSprite(itemData.GetSprite());
				itemDataSlot.SetBackground(itemData.GetGradeSprite());
			}

			uiLineRendererFavorite.Points = (from x in favorite.paths
				select areaPosition[x - 1]).ToArray<Vector2>();
			uiLineRendererFavorite.SetVerticesDirty();
		}


		private void InitMap(List<int> path)
		{
			uiMap.Init(MonoBehaviourInstance<ClientService>.inst.CurrentLevel);
			uiMap.SetMapMode(UIMap.MapModeFlag.Restrict);
			uiMap.SetRouteText(path);
			uiLineRendererFavorite.color = GameConstants.UIColor.uiLineRendererFavorite;
		}


		private IEnumerator CountDownUI()
		{
			int time = 5;
			Text text = countDownText;
			string key = "{0}초 후에 자동으로 닫힙니다.";
			int num = time;
			time = num - 1;
			text.text = Ln.Format(key, num);
			yield return new WaitForSeconds(1f);
			Text text2 = countDownText;
			string key2 = "{0}초 후에 자동으로 닫힙니다.";
			num = time;
			time = num - 1;
			text2.text = Ln.Format(key2, num);
			yield return new WaitForSeconds(1f);
			Text text3 = countDownText;
			string key3 = "{0}초 후에 자동으로 닫힙니다.";
			num = time;
			time = num - 1;
			text3.text = Ln.Format(key3, num);
			yield return new WaitForSeconds(1f);
			Text text4 = countDownText;
			string key4 = "{0}초 후에 자동으로 닫힙니다.";
			num = time;
			time = num - 1;
			text4.text = Ln.Format(key4, num);
			yield return new WaitForSeconds(1f);
			Text text5 = countDownText;
			string key5 = "{0}초 후에 자동으로 닫힙니다.";
			num = time;
			time = num - 1;
			text5.text = Ln.Format(key5, num);
			yield return new WaitForSeconds(1f);
			if (IsOpen)
			{
				Close();
			}
		}


		protected override void OnClose()
		{
			base.OnClose();
			if (closeTimeoutRoutine != null)
			{
				StopCoroutine(closeTimeoutRoutine);
			}

			countDownText.gameObject.SetActive(false);
		}


		public void FinalSurvivalOpen()
		{
			Open();
			transform.localScale = new Vector3(0.8f, 0.8f, 1f);
			rectTransform.transform.localPosition = new Vector3(-50f, 185f, 0f);
			guideText.text = Ln.Format("{0}초 후에 자동으로 닫힙니다.", 5);
			closeTimeoutRoutine = this.StartThrowingCoroutine(CountDownUI(),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][CountDownUI] Message:" + exception.Message + ", StackTrace:" +
					      exception.StackTrace);
				});
		}


		public void FinalSurvivalClose()
		{
			Close();
			transform.localScale = new Vector3(1f, 1f, 1f);
			rectTransform.transform.localPosition = new Vector3(-50f, 24f, 0f);
		}
	}
}