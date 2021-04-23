using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class MapWindow : BaseWindow, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
	{
		private const float MAP_ALPHA = 0.9f;


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


		private readonly Vector2[] emptyPath = new Vector2[0];


		private Image bg;


		private Favorite favorite;


		private UILineRenderer uiLineRendererFavorite;


		private UIMap uiMap;


		private Coroutine updateCorutine;


		private WicklineAppearRemainTime wicklineAppearRemainTime;


		private int wicklineResponRemainTime;


		public UIMap UIMap => uiMap;


		private void Update()
		{
			if (MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.LeftControl))
				{
					SetEnableInput(true);
					return;
				}

				SetEnableInput(false);
			}
		}


		public void OnPointerDown(PointerEventData eventData)
		{
			uiMap.OnClickDownMapWindow(eventData);
		}


		public void OnPointerUp(PointerEventData eventData)
		{
			uiMap.OnClickUp(eventData);
		}


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			uiMap = GameUtil.Bind<UIMap>(gameObject, "Map");
			wicklineAppearRemainTime = GameUtil.Bind<WicklineAppearRemainTime>(gameObject, "WicklineInfo/RemainTime");
			uiLineRendererFavorite = GameUtil.Bind<UILineRenderer>(gameObject, "Map/Areas/LineRendererFavorite");
			bg = GameUtil.Bind<Image>(gameObject, "Bg");
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
			uiMap.Init(MonoBehaviourInstance<ClientService>.inst.CurrentLevel);
			uiMap.SetBaseAlpha(0.9f);
			uiLineRendererFavorite.color = GameConstants.UIColor.uiLineRendererFavorite;
			if (!MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				MonoBehaviourInstance<MobaCamera>.inst.OnCameraModeChange += uiMap.UpdateCameraFrame;
			}
		}


		protected override void OnOpen()
		{
			base.OnOpen();
			uiMap.SetMapMode(UIMap.MapModeFlag.Restrict, UIMap.MapModeFlag.SearchItem);
			UpdateRoutePath();
			if (MonoBehaviourInstance<GameClient>.inst.IsTutorial)
			{
				MonoBehaviourInstance<TutorialController>.inst.SuccessOpenMapWindowTutorial();
			}

			SetEnableInput(!MonoBehaviourInstance<ClientService>.inst.IsPlayer);
			updateCorutine = this.StartThrowingCoroutine(CorUpdate(),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][CorUpdate] Message:" + exception.Message + ", StackTrace:" +
					      exception.StackTrace);
				});
		}


		protected override void OnClose()
		{
			base.OnClose();
			uiMap.SetSearchItem(null);
			MonoBehaviourInstance<Tooltip>.inst.Hide(this);
			if (updateCorutine != null)
			{
				StopCoroutine(updateCorutine);
			}
		}


		private IEnumerator CorUpdate()
		{
			bool remainTimeActive = false;
			while (IsActive())
			{
				if (MonoBehaviourInstance<ClientService>.inst.IsWicklineDead)
				{
					wicklineAppearRemainTime.SetActive(false);
					yield break;
				}

				if (!remainTimeActive)
				{
					wicklineAppearRemainTime.SetActive(true);
					remainTimeActive = true;
				}

				if (MonoBehaviourInstance<ClientService>.inst.WicklineResponRemainTime <= 0f)
				{
					wicklineAppearRemainTime.SettingShowMode(WicklineAppearRemainTime.ShowMode.Activate);
				}
				else
				{
					wicklineAppearRemainTime.SettingShowMode(WicklineAppearRemainTime.ShowMode.RemainTime);
					wicklineAppearRemainTime.ShowReaminTime();
				}

				yield return null;
			}
		}


		public void SetFavorite(Favorite favorite)
		{
			this.favorite = favorite;
			UpdateRoutePath();
		}


		private void UpdateRoutePath()
		{
			if (favorite == null)
			{
				favorite = new Favorite();
			}

			bool flag = 0 < favorite.paths.Count;
			uiMap.SetRouteText(favorite.paths);
			uiLineRendererFavorite.Points = flag
				? (from x in favorite.paths
					select areaPosition[x - 1]).ToArray<Vector2>()
				: emptyPath;
			uiLineRendererFavorite.SetVerticesDirty();
			uiLineRendererFavorite.transform.localScale = flag ? Vector3.one : Vector3.zero;
		}


		public void SetItemMark(ItemData itemData)
		{
			uiMap.SetSearchItem(itemData);
			uiMap.SetRouteText(favorite.paths);
			uiLineRendererFavorite.Points = (from x in favorite.paths
				select areaPosition[x - 1]).ToArray<Vector2>();
			uiLineRendererFavorite.SetVerticesDirty();
		}


		public bool ContainItemMark(ItemData itemData)
		{
			return uiMap.SearchItem != null && uiMap.SearchItem.code == itemData.code;
		}


		public void OnUpdateRestrictedArea(Dictionary<int, AreaRestrictionState> areaStateMap)
		{
			uiMap.OnUpdateRestrictedArea(areaStateMap);
			if (favorite != null && IsOpen)
			{
				uiMap.SetRouteText(favorite.paths);
				uiLineRendererFavorite.Points = (from x in favorite.paths
					select areaPosition[x - 1]).ToArray<Vector2>();
				uiLineRendererFavorite.SetVerticesDirty();
			}
		}


		public void UpdateMapPlayerPosition(int objectId, bool isAlly, Vector3 worldPos)
		{
			if (!IsOpen)
			{
				return;
			}

			uiMap.UpdatePlayerPosition(objectId, isAlly, worldPos);
		}


		public void UpdateMapNonPlayerPosition(int objectId, Vector3 worldPos)
		{
			if (!IsOpen)
			{
				return;
			}

			uiMap.UpdateNonPlayerPosition(objectId, worldPos);
		}


		private void SetEnableInput(bool enable)
		{
			bg.raycastTarget = enable;
		}
	}
}