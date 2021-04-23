using System;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class RecommendRouteSlot : BaseControl
	{
		private Favorite favorite;


		private GameObject imgBring;


		private GameObject imgMyBG;


		private GameObject imgRollover;


		private GameObject imgSelected;


		private GameObject imgUsing;


		private ScrollRect scrollRect;


		private GameObject state;


		private LnText txtCount;


		private LnText txtDateTime;


		private LnText txtName;


		private LnText txtShareName;

		
		
		public event Action<Favorite> OnClickRouteSlot;


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			imgMyBG = transform.FindRecursively("IMG_MyBG").gameObject;
			state = transform.FindRecursively("State").gameObject;
			imgBring = transform.FindRecursively("IMG_Bring").gameObject;
			imgUsing = transform.FindRecursively("IMG_Using").gameObject;
			txtCount = GameUtil.Bind<LnText>(gameObject, "State/TXT_Count");
			txtName = GameUtil.Bind<LnText>(gameObject, "TXT_Name");
			txtDateTime = GameUtil.Bind<LnText>(gameObject, "TXT_DateTime");
			txtShareName = GameUtil.Bind<LnText>(gameObject, "TXT_ShareName");
			imgSelected = transform.FindRecursively("IMG_Selected").gameObject;
			imgRollover = transform.FindRecursively("IMG_Rollover").gameObject;
			scrollRect = GameUtil.Bind<ScrollRect>(MonoBehaviourInstance<LobbyUI>.inst.gameObject,
				"LobbyHUD/FavoritesTab/FavoritesEditorPage/BringRoute/Routes");
			OnScrollEvent += delegate(BaseControl control, PointerEventData eventData)
			{
				scrollRect.OnScroll(eventData);
			};
		}


		public void InitRecommendUI(int slotId, int order, InitRecommendWeaponRoute initRecommendWeaponRoute)
		{
			SetActiveUI(RecommendRouteSortType.RECENT_DATETIME);
			string text = string.IsNullOrEmpty(initRecommendWeaponRoute.title)
				? Ln.Format("{0} 추천 루트", Ln.Get(string.Format("WeaponType/{0}", initRecommendWeaponRoute.weaponType)))
				: initRecommendWeaponRoute.title;
			txtName.text = text;
			txtDateTime.gameObject.SetActive(false);
			txtShareName.text = "NimbleNeuron";
			List<int> integerListFromString = RouteApi.GetIntegerListFromString(initRecommendWeaponRoute.weaponCodes);
			List<int> integerListFromString2 = RouteApi.GetIntegerListFromString(initRecommendWeaponRoute.paths);
			favorite = new Favorite(-1L, initRecommendWeaponRoute.characterCode, slotId, text,
				initRecommendWeaponRoute.weaponType, integerListFromString, integerListFromString2, slotId,
				"NimbleNeuron", true, true, "", RouteFilterType.ALL, order, -1L);
		}


		public void SetRecommendUI(int slotId, int order, RecommendWeaponRoute recommendWeaponRoute,
			RecommendRouteSortType sortType)
		{
			SetActiveUI(sortType);
			imgMyBG.SetActive(recommendWeaponRoute.userNum == Lobby.inst.User.UserNum);
			txtName.text = recommendWeaponRoute.title;
			string str = Ln.Format("{0}년-{1}월-{2}일", recommendWeaponRoute.updateDtm.Year,
				recommendWeaponRoute.updateDtm.Month, recommendWeaponRoute.updateDtm.Day);
			txtDateTime.text = str + " (" + recommendWeaponRoute.version + ")";
			txtDateTime.gameObject.SetActive(true);
			txtShareName.text = recommendWeaponRoute.userNickname;
			if (sortType != RecommendRouteSortType.RECENT_DATETIME)
			{
				if (sortType - RecommendRouteSortType.GET_COUNT <= 1)
				{
					txtCount.text = StringUtil.NumberMeasure(recommendWeaponRoute.count);
				}
			}
			else
			{
				txtCount.text = "";
			}

			List<int> integerListFromString = RouteApi.GetIntegerListFromString(recommendWeaponRoute.weaponCodes);
			List<int> integerListFromString2 = RouteApi.GetIntegerListFromString(recommendWeaponRoute.paths);
			favorite = new Favorite(recommendWeaponRoute.userNum, recommendWeaponRoute.characterCode, slotId,
				recommendWeaponRoute.title, recommendWeaponRoute.weaponType, integerListFromString,
				integerListFromString2, recommendWeaponRoute.id, recommendWeaponRoute.userNickname, true, false,
				recommendWeaponRoute.version, recommendWeaponRoute.routeFilterType, order, recommendWeaponRoute.id);
		}


		public void SetActiveUI(RecommendRouteSortType sortType)
		{
			switch (sortType)
			{
				case RecommendRouteSortType.RECENT_DATETIME:
					state.SetActive(false);
					return;
				case RecommendRouteSortType.GET_COUNT:
					state.SetActive(true);
					imgBring.SetActive(true);
					imgUsing.SetActive(false);
					txtCount.gameObject.SetActive(true);
					return;
				case RecommendRouteSortType.USE_COUNT:
					state.SetActive(true);
					imgBring.SetActive(false);
					imgUsing.SetActive(true);
					txtCount.gameObject.SetActive(true);
					return;
				default:
					return;
			}
		}


		public void HideSelected()
		{
			imgSelected.gameObject.SetActive(false);
		}


		public void ClickedSlot()
		{
			OnClickRouteSlot(favorite);
			imgSelected.gameObject.SetActive(true);
		}


		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);
			imgRollover.gameObject.SetActive(true);
		}


		public override void OnPointerExit(PointerEventData eventData)
		{
			base.OnPointerExit(eventData);
			imgRollover.gameObject.SetActive(false);
		}
	}
}