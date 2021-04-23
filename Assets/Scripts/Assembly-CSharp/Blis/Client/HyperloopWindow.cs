using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Blis.Client
{
	public class HyperloopWindow : BaseWindow, IMapSlicePointerEventHandler
	{
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


		private Favorite favorite;


		private int hyperLoopId;


		private Coroutine pathDrawRoutine;


		private UILineRenderer uiLineRendererFavorite;


		private UIMap uiMap;


		public UIMap UIMap => uiMap;


		public void OnPointerMapEnter(int areaCode)
		{
			if (MonoBehaviourInstance<GameClient>.inst.IsTutorial && uiMap.CheckRestrictedArea(areaCode))
			{
				return;
			}

			uiMap.SetRollOver(areaCode, true);
		}


		public void OnPointerMapExit(int areaCode)
		{
			uiMap.SetRollOver(areaCode, false);
		}


		public void OnPointerMapClick(int areaCode, PointerEventData.InputButton button)
		{
			if (button == PointerEventData.InputButton.Left)
			{
				if (MonoBehaviourInstance<GameClient>.inst.IsTutorial)
				{
					if (uiMap.CheckRestrictedArea(areaCode))
					{
						MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("튜토리얼 중에는 금지구역으로 하이퍼루프를 이용할 수 없습니다."),
							new Popup.Button
							{
								text = Ln.Get("확인")
							});
						return;
					}
				}
				else if (uiMap.GetLaboratoryAreaCode() == areaCode)
				{
					return;
				}

				OnClickMap(areaCode);
			}
		}


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			uiMap = GameUtil.Bind<UIMap>(gameObject, "Map");
			uiLineRendererFavorite = GameUtil.Bind<UILineRenderer>(gameObject, "Map/Areas/LineRendererFavorite");
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
			uiMap.Init(MonoBehaviourInstance<ClientService>.inst.CurrentLevel);
			uiMap.SetEventHandler(this);
			uiMap.SetMapMode(UIMap.MapModeFlag.Restrict);
			uiLineRendererFavorite.color = GameConstants.UIColor.uiLineRendererFavorite;
		}


		protected override void OnClose()
		{
			base.OnClose();
			hyperLoopId = 0;
		}


		private void OnClickMap(int areaCode)
		{
			if (!MonoBehaviourInstance<ClientService>.inst.MyPlayer.Character.CanAnyAction(ActionType.Hyperloop))
			{
				return;
			}

			MonoBehaviourInstance<ClientService>.inst.ActionCasting(
				MonoBehaviourInstance<ClientService>.inst.MyPlayer.Character, CastingActionType.Hyperloop, delegate
				{
					SingletonMonoBehaviour<PlayerController>.inst.HyperLoop(hyperLoopId, areaCode);
					Close();
				}, null);
		}


		public void SetTargetHyperLoopId(int hyperLoopId)
		{
			this.hyperLoopId = hyperLoopId;
		}


		public void OnUpdateRestrictedArea(Dictionary<int, AreaRestrictionState> areaStateMap)
		{
			uiMap.OnUpdateRestrictedArea(areaStateMap);
			UpdateRoutePath();
		}


		public void UpdateMapPlayerPosition(int objectId, bool isAlly, Vector3 worldPos)
		{
			if (!IsOpen)
			{
				return;
			}

			uiMap.UpdatePlayerPosition(objectId, isAlly, worldPos);
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

			uiMap.SetRouteText(favorite.paths);
			uiLineRendererFavorite.Points = (from x in favorite.paths
				select areaPosition[x - 1]).ToArray<Vector2>();
			uiLineRendererFavorite.SetVerticesDirty();
			uiLineRendererFavorite.transform.localScale = 0 < favorite.paths.Count ? Vector3.one : Vector3.zero;
		}
	}
}