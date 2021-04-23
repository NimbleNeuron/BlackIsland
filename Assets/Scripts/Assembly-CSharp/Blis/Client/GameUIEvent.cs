using System.Collections.Generic;
using Blis.Client.UI;
using Blis.Common;
using Blis.Common.Utils;
using Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class GameUIEvent
	{
		private readonly GameUI gameUI;
		private int currentArea;

		public GameUIEvent(GameUI ui)
		{
			gameUI = ui;
		}


		public void Init()
		{
			MonoBehaviourInstance<GameInput>.inst.OnKeyPressed += OnPressKey;
			MonoBehaviourInstance<GameInput>.inst.OnKeyPressing += OnPressingKey;
			MonoBehaviourInstance<GameInput>.inst.OnKeyRelease += OnKeyRelease;
			SingletonMonoBehaviour<PlayerController>.inst.ItemGuide.OnNavigationRecommendsUpdate +=
				OnRecommendDataUpdate;
			gameUI.NavigationHud.OnRequestTargetItemRemove +=
				SingletonMonoBehaviour<PlayerController>.inst.ItemGuide.RemoveTargetItem;
			gameUI.CombineWindow.OnRequestAddItem += RequestAddItemGuide;
		}


		public void OnGameSceneLoaded()
		{
			LoadingView.inst.UpdateLoading(Ln.Get("다른 플레이어를 기다리는 중"), 1f);
		}


		public void RequestAddItemGuide(ItemData itemData)
		{
			SingletonMonoBehaviour<PlayerController>.inst.ItemGuide.AddTargetItem(itemData);
		}


		public void OnPressKey(GameInputEvent gameInputEvent, Vector3 mousePos)
		{
			gameUI.OnPressKey(gameInputEvent, mousePos);
			gameUI.InventoryHud.OnPressKey(gameInputEvent, mousePos);
		}


		public void OnPressingKey(GameInputEvent gameInputEvent, Vector3 mousePos)
		{
			gameUI.OnPressingKey(gameInputEvent);
		}


		public void OnKeyRelease(GameInputEvent gameInputEvent, Vector3 mousePos)
		{
			gameUI.OnReleaseKey(gameInputEvent, mousePos);
		}


		public void OnNavigationClear(ItemData itemData)
		{
			gameUI.InventoryHud.OnNavigationClear(itemData);
			gameUI.StatusHud.OnNavigationClear(itemData);
		}


		public void OnUpdateRestrictedArea(LevelData levelData, Dictionary<int, AreaRestrictionState> areaStateMap,
			float remainTime, DayNight dayNight, int day)
		{
			gameUI.RestrictedArea.OnUpdateRestrictArea(levelData, areaStateMap, remainTime, dayNight, day);
			gameUI.BattleInfoHud.SetPlayTimeFlag(true);
			gameUI.MapWindow.OnUpdateRestrictedArea(areaStateMap);
			gameUI.CombineWindow.OnUpdateRestrictedArea(areaStateMap);
			gameUI.Minimap.UIMap.OnUpdateRestrictedArea(areaStateMap);
			gameUI.NavigationHud.OnUpdateRestrictedArea(areaStateMap);
			gameUI.HyperloopWindow.OnUpdateRestrictedArea(areaStateMap);
		}


		public void OnUpdateCurrentArea(AreaData areaData, AreaRestrictionState areaState)
		{
			int num = areaData != null ? areaData.code : 0;
			if (currentArea != num)
			{
				currentArea = num;
				if (areaData != null)
				{
					gameUI.RestrictedArea.OnUpdateCurrentArea(areaData.code, areaState);
					MonoBehaviourInstance<GameUI>.inst.NavigationHud.OnUpdateCurrentArea(areaData.code);
					return;
				}

				gameUI.RestrictedArea.OnUpdateCurrentArea(-1, areaState);
				MonoBehaviourInstance<GameUI>.inst.NavigationHud.OnUpdateCurrentArea(0);
			}
		}


		public void OnRecommendDataUpdate(Dictionary<RecommendItemType, List<ItemData>> recommendDatas,
			List<ItemData> preferItems)
		{
			gameUI.CombineWindow.SetRecommendData(recommendDatas, preferItems);
		}


		public void OnUpdateStrategySheets(long userId, int teamNumber, int teamSlot, int startingAreaCode)
		{
			if (MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				gameUI.StartingView.OnUpdateStrategySheets(userId, teamNumber, teamSlot, startingAreaCode);
				return;
			}

			gameUI.ObserverStartingView.OnUpdateStrategySheets(userId, teamNumber, teamSlot, startingAreaCode);
		}


		public void OnAirSupplyAnnounce(int objectId, Vector3 pos, Sprite sprite)
		{
			gameUI.MapWindow.UIMap.CreateAirSupply(objectId, pos, sprite);
			gameUI.Minimap.UIMap.CreateAirSupply(objectId, pos, sprite);
		}


		public void OnSpawnAirSupply(int objectId, Sprite sprite)
		{
			gameUI.MapWindow.UIMap.UpdateNonPlayerIcon(objectId, sprite);
			gameUI.Minimap.UIMap.UpdateNonPlayerIcon(objectId, sprite);
		}


		public void OnRemoveAirSupply(int objectId)
		{
			gameUI.MapWindow.UIMap.RemoveNonPlayer(objectId);
			gameUI.Minimap.UIMap.RemoveNonPlayer(objectId);
		}


		public void OnUpdateCharacterPosition(Vector3 position)
		{
			Vector2 v = GameUIUtility.WorldPosToMapUVSpace(position);
			Vector2 zero = Vector2.zero;
			if (gameUI.MapWindow.IsOpen)
			{
				gameUI.MapWindow.UIMap.UpdateMyPosition(v, ref zero);
				gameUI.MapWindow.UIMap.UpdatePathLiner(zero);
				gameUI.MapWindow.UIMap.UpdateMinimapIndicator(zero);
			}

			if (gameUI.CombineWindow.IsOpen)
			{
				gameUI.CombineWindow.UIMap.UpdateMyPosition(v, ref zero);
			}

			if (gameUI.HyperloopWindow.IsOpen)
			{
				gameUI.HyperloopWindow.UIMap.UpdateMyPosition(v, ref zero);
			}

			gameUI.Minimap.UIMap.UpdateMyPosition(v, ref zero);
			gameUI.Minimap.UIMap.UpdatePathLiner(zero);
			gameUI.Minimap.UIMap.UpdateMinimapIndicator(zero);
		}


		public void OnUpdateMapCenter(Vector3 position)
		{
			gameUI.Minimap.UIMap.UpdateMinimapCenterPosition(position);
		}


		public void OnUpdateInventory(List<Item> items, List<int> itemSlotIndex)
		{
			UISystem.Action(new UpdateInventory
			{
				inventory = items
			});
			gameUI.InventoryHud.OnInventoryUpdate(items, itemSlotIndex);
			gameUI.CombineWindow.OnInventoryUpdate(items);
		}


		public void OnUpdateNeedItem(List<ItemData> needItems)
		{
			gameUI.UITracker.ConveyNeedItems(needItems);
		}


		public void OnUpdateDropItemName(ItemFloatingUI floatingUI)
		{
			gameUI.UITracker.OnUpdateDropItemName(floatingUI);
		}


		public void OnUpdateEquipment(List<Item> items, List<Item> updates)
		{
			UISystem.Action(new UpdateEquipment
			{
				equipment = items
			});
			gameUI.StatusHud.OnUpdateEquips(items, updates);
			gameUI.CombineWindow.OnUpdateEquipment(items);
		}


		public void OnUpdateFavoritesData(Favorite favorite)
		{
			gameUI.MapWindow.SetFavorite(favorite);
			gameUI.HyperloopWindow.SetFavorite(favorite);
			gameUI.NavigationHud.SetFavoriteName(favorite.title);
		}


		public void OnUpdateButtonState()
		{
			gameUI.CombineWindow.OnUpdateButtonState();
		}
	}
}