using System;
using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UIMap : BaseControl, ILnEventHander
	{
		public enum MapModeFlag
		{
			None,

			Restrict,

			SearchItem
		}


		private const float ZoomInMapScale = 1f;


		private const float ZoomOutMapScale = 0.75f;


		[SerializeField] private Vector2 boundaryLT;


		[SerializeField] private Vector2 boundaryLB;


		[SerializeField] private Vector2 boundaryRT;


		[SerializeField] private Vector2 boundaryRB;


		private readonly Color32 defaultGlowColor =
			new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);


		private readonly Color32 reservedGlowColor = new Color32(byte.MaxValue, 175, 42, 170);


		private readonly Color32 restrictedGlowColor = new Color32(212, 8, 0, 220);


		private readonly Color32 unVisitDefaultColor =
			new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);


		private readonly Color32 unVisitedReservedColor = new Color32(byte.MaxValue, 200, 30, byte.MaxValue);


		private readonly Color32 unVisitedRestrictedColor = new Color32(220, 0, 0, byte.MaxValue);


		private readonly Color32 visitDefaultColor = new Color32(125, 125, 125, byte.MaxValue);


		private readonly Color32 visitReservedColor = new Color32(160, 100, 0, byte.MaxValue);


		private readonly Color32 visitRestrictedColor = new Color32(160, 2, 0, byte.MaxValue);


		private RectTransform airSupplyIcons;


		private RectTransform allyIcons;


		private Dictionary<int, Area> areas;


		private Dictionary<int, AreaRestrictionState> areaStateMap;


		private Image cameraFrame;


		private CanvasGroup canvasGroup;


		private RectTransform canvasRect;


		private Vector2 centerPos;


		private RectTransform collectibleIcons;


		private List<Image> collectibleList;


		private Vector3 currentIconScale = Vector3.one;


		private float currentMapScale = 1f;


		private float currentObjectScale = 1f;


		private RectTransform deadCharacterIcons;


		private bool dragCamera;


		private Dictionary<int, ItemDataSlot> dropItemSlots;


		private RectTransform enemyIcons;


		private bool hasBoundary;


		private MinimapIconPool iconPool;


		private MinimapIndicatorManager indicatorManager;


		private LevelData levelData;


		private Camera mainCamera;


		private Transform mainCameraRig;


		private RectTransform markIcons;


		private UIIcon me;


		private uint modeFlags;


		private RectTransform monsterIcons;


		private RectTransform noiseIcons;


		private Dictionary<int, Image> objectMap;


		private UILineRenderer pathLineRenderer;


		private List<Vector2> pathNavCorners;


		private RectTransform pingIcons;


		private Dictionary<int, UIIcon> playerMap;


		private ItemData searchItem;


		private RectTransform staticObjectIcons;


		private RectTransform summonCamera;


		public ItemData SearchItem => searchItem;


		private void LateUpdate()
		{
			if (mainCameraRig != null)
			{
				Vector2 a = GameUIUtility.WorldPosToMapUVSpace(mainCameraRig.position);
				Rect rect = rectTransform.rect;
				Vector2 a2 = a * rect.size;
				cameraFrame.transform.localPosition = a2 - rect.size * 0.5f;
			}
		}


		protected override void OnEnable()
		{
			base.OnEnable();
			this.StartThrowingCoroutine(LineRendererUpdater(),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][UIMapLineUpdater] Message:" + exception.Message + ", StackTrace:" +
					      exception.StackTrace);
				});
		}


		public void OnLnDataChange()
		{
			UpdateAreaName();
		}


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			if (mainCamera == null)
			{
				mainCamera = Camera.main;
			}

			canvasGroup = GameUtil.Bind<CanvasGroup>(gameObject, "Base");
			areas = new Dictionary<int, Area>();
			dropItemSlots = new Dictionary<int, ItemDataSlot>();
			iconPool = GameUtil.Bind<MinimapIconPool>(gameObject, "IconPool");
			iconPool.InitPool();
			me = GameUtil.Bind<UIIcon>(gameObject, "Me");
			pathLineRenderer = GameUtil.Bind<UILineRenderer>(gameObject, "PathLineRenderer");
			pathNavCorners = new List<Vector2>();
			collectibleIcons = GameUtil.Bind<RectTransform>(gameObject, "Collectible");
			staticObjectIcons = GameUtil.Bind<RectTransform>(gameObject, "StaticObject");
			airSupplyIcons = GameUtil.Bind<RectTransform>(gameObject, "AirSupply");
			summonCamera = GameUtil.Bind<RectTransform>(gameObject, "SummonCamera");
			monsterIcons = GameUtil.Bind<RectTransform>(gameObject, "Monsters");
			noiseIcons = GameUtil.Bind<RectTransform>(gameObject, "Noise");
			allyIcons = GameUtil.Bind<RectTransform>(gameObject, "Allies");
			enemyIcons = GameUtil.Bind<RectTransform>(gameObject, "Enemies");
			deadCharacterIcons = GameUtil.Bind<RectTransform>(gameObject, "DeadCharacters");
			markIcons = GameUtil.Bind<RectTransform>(gameObject, "Marks");
			pingIcons = GameUtil.Bind<RectTransform>(gameObject, "Pings");
			collectibleList = new List<Image>();
			objectMap = new Dictionary<int, Image>();
			playerMap = new Dictionary<int, UIIcon>();
			cameraFrame = GameUtil.Bind<Image>(gameObject, "CameraFrame");
			cameraFrame.enabled = false;
			indicatorManager = GameUtil.Bind<MinimapIndicatorManager>(gameObject, "MinimapIndicator");
			indicatorManager.Init(this);
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
			if (SingletonMonoBehaviour<Bootstrap>.inst.IsLobbyScene)
			{
				LobbyUI inst = MonoBehaviourInstance<LobbyUI>.inst;
				canvasRect = (inst != null ? inst.transform : null) as RectTransform;
				mainCameraRig = null;
				return;
			}

			GameUI inst2 = MonoBehaviourInstance<GameUI>.inst;
			canvasRect = (inst2 != null ? inst2.transform : null) as RectTransform;
			mainCameraRig = MonoBehaviourInstance<MobaCamera>.inst.transform;
		}


		public void Init(LevelData levelData)
		{
			this.levelData = levelData;
			hasBoundary = false;
			if (this.levelData == null)
			{
				return;
			}

			if (areas == null)
			{
				areas = new Dictionary<int, Area>();
			}

			areas.Clear();
			if (dropItemSlots == null)
			{
				dropItemSlots = new Dictionary<int, ItemDataSlot>();
			}

			dropItemSlots.Clear();
			RectTransform rectTransform = GameUtil.Bind<RectTransform>(gameObject, "DropItems");
			for (int i = 1; i < 17; i++)
			{
				Area value = new Area(transform, i);
				areas.Add(i, value);
				dropItemSlots.Add(i, GameUtil.Bind<ItemDataSlot>(rectTransform.gameObject, i.ToString()));
			}

			UpdateAreaName();
			SetMapMode(MapModeFlag.Restrict);
		}


		public void InitBoundary(Vector2 LT, Vector2 RB)
		{
			hasBoundary = true;
			boundaryLT = LT;
			boundaryLB = new Vector2(LT.x, RB.y);
			boundaryRT = new Vector2(RB.x, LT.y);
			boundaryRB = RB;
		}


		public void SetEventHandler(IMapSlicePointerEventHandler handler)
		{
			foreach (Area area in areas.Values)
			{
				area.SetEventHandler(handler);
			}
		}


		public void UpdateCameraFrame(MobaCameraMode mode)
		{
			cameraFrame.enabled = mode != MobaCameraMode.Tracking;
		}


		public void UpdateMinimapCenterPosition(Vector3 position)
		{
			Vector2 a = GameUIUtility.WorldPosToMapUVSpace(position);
			Rect rect = rectTransform.rect;
			centerPos = a * rect.size;
			Vector2 vector = rect.size * currentMapScale;
			Vector2 a2 = a * vector;
			rectTransform.localPosition = (a2 - vector * 0.5f) * -1f;
		}


		public void UpdateMyPosition(Vector3 uvPos, ref Vector2 characterPos)
		{
			characterPos = uvPos * rectTransform.rect.size;
			Vector2 v;
			bool inBoundaryPosition = GetInBoundaryPosition(uvPos, characterPos, out v);
			me.rectTransform.localPosition = v;
			if (inBoundaryPosition)
			{
				me.Out(GameUtil.GetRotation(characterPos, centerPos));
				return;
			}

			me.In();
		}


		private bool GetInBoundaryPosition(Vector2 uvPos, Vector2 characterPos, out Vector2 iconPosition)
		{
			bool flag = false;
			bool flag2;
			bool flag3;
			bool flag4;
			bool flag5;
			if (IsOutBoundary(characterPos, out flag2, out flag3, out flag4, out flag5))
			{
				Vector2 vector = centerPos + boundaryLT * currentObjectScale;
				Vector2 vector2 = centerPos + boundaryLB * currentObjectScale;
				Vector2 vector3 = centerPos + boundaryRT * currentObjectScale;
				Vector2 vector4 = centerPos + boundaryRB * currentObjectScale;
				float num = 33f * currentObjectScale;
				Vector2 a;
				if (!flag && flag2 && GetIntersectPoint(vector, vector2, centerPos, characterPos, out a))
				{
					Vector2 a2;
					if (GetIntersectPoint(vector + new Vector2(num, 0f), vector2 + new Vector2(num, 0f), centerPos,
						characterPos, out a2))
					{
						uvPos = a2 / rectTransform.rect.size;
					}
					else
					{
						uvPos = a / rectTransform.rect.size;
					}

					flag = true;
				}

				Vector2 a3;
				if (!flag && flag3 && GetIntersectPoint(vector3, vector4, centerPos, characterPos, out a3))
				{
					Vector2 a4;
					if (GetIntersectPoint(vector3 + new Vector2(-num, 0f), vector4 + new Vector2(-num, 0f), centerPos,
						characterPos, out a4))
					{
						uvPos = a4 / rectTransform.rect.size;
					}
					else
					{
						uvPos = a3 / rectTransform.rect.size;
					}

					flag = true;
				}

				Vector2 a5;
				if (!flag && flag4 && GetIntersectPoint(vector, vector3, centerPos, characterPos, out a5))
				{
					Vector2 a6;
					if (GetIntersectPoint(vector + new Vector2(0f, -num), vector3 + new Vector2(0f, -num), centerPos,
						characterPos, out a6))
					{
						uvPos = a6 / rectTransform.rect.size;
					}
					else
					{
						uvPos = a5 / rectTransform.rect.size;
					}

					flag = true;
				}

				Vector2 a7;
				if (!flag && flag5 && GetIntersectPoint(vector2, vector4, centerPos, characterPos, out a7))
				{
					Vector2 a8;
					if (GetIntersectPoint(vector2 + new Vector2(0f, num), vector4 + new Vector2(0f, num), centerPos,
						characterPos, out a8))
					{
						uvPos = a8 / rectTransform.rect.size;
					}
					else
					{
						uvPos = a7 / rectTransform.rect.size;
					}

					flag = true;
				}
			}

			iconPosition = GameUIUtility.UvSpaceToLocalPos(uvPos, rectTransform);
			return flag;
		}


		private bool GetIntersectPoint(Vector2 AP1, Vector2 AP2, Vector2 BP1, Vector2 BP2, out Vector2 IP)
		{
			IP = Vector2.zero;
			double num = (BP2.y - BP1.y) * (AP2.x - AP1.x) - (BP2.x - BP1.x) * (AP2.y - AP1.y);
			if (num == 0.0)
			{
				return false;
			}

			double num2 = (BP2.x - BP1.x) * (AP1.y - BP1.y) - (BP2.y - BP1.y) * (AP1.x - BP1.x);
			double num3 = (AP2.x - AP1.x) * (AP1.y - BP1.y) - (AP2.y - AP1.y) * (AP1.x - BP1.x);
			double num4 = num2 / num;
			double num5 = num3 / num;
			if (num4 < 0.0 || num4 > 1.0 || num5 < 0.0 || num5 > 1.0)
			{
				return false;
			}

			if (num2 == 0.0 && num3 == 0.0)
			{
				return false;
			}

			IP.x = (float) (AP1.x + num4 * (AP2.x - AP1.x));
			IP.y = (float) (AP1.y + num4 * (AP2.y - AP1.y));
			return true;
		}


		private bool IsOutBoundary(Vector2 localPosition, out bool leftOut, out bool rightOut, out bool topOut,
			out bool bottomOut)
		{
			leftOut = false;
			rightOut = false;
			topOut = false;
			bottomOut = false;
			if (hasBoundary)
			{
				Vector2 vector = centerPos + boundaryLT * currentObjectScale;
				Vector2 vector2 = centerPos + boundaryRB * currentObjectScale;
				if (vector.y < localPosition.y)
				{
					topOut = true;
				}

				if (localPosition.y < vector2.y)
				{
					bottomOut = true;
				}

				if (localPosition.x < vector.x)
				{
					leftOut = true;
				}

				if (vector2.x < localPosition.x)
				{
					rightOut = true;
				}
			}

			return topOut | bottomOut | leftOut | rightOut;
		}


		public void UpdatePathLiner(Vector2 characterPos)
		{
			if (pathNavCorners.Count > 0)
			{
				pathLineRenderer.transform.localScale = Vector3.one;
				if ((pathNavCorners[0] - characterPos).sqrMagnitude < 2f)
				{
					pathNavCorners.RemoveAt(0);
				}

				Vector2[] array = new Vector2[pathNavCorners.Count + 1];
				array[0] = characterPos;
				for (int i = 0; i < pathNavCorners.Count; i++)
				{
					array[i + 1] = pathNavCorners[i];
				}

				pathLineRenderer.Points = array;
				return;
			}

			pathLineRenderer.transform.localScale = Vector3.zero;
		}


		public void UpdateMinimapIndicator(Vector2 characterPos)
		{
			MinimapIndicator currentIndicator = indicatorManager.GetCurrentIndicator();
			if (currentIndicator == null)
			{
				return;
			}

			if (!currentIndicator.IsActive)
			{
				return;
			}

			currentIndicator.SetPosition(characterPos);
		}


		public void OnUpdateRestrictedArea(Dictionary<int, AreaRestrictionState> areaStateMap)
		{
			this.areaStateMap = areaStateMap;
			UpdateMapUI();
		}


		public void SetMapMode(params MapModeFlag[] flags)
		{
			modeFlags = 0U;
			for (int i = 0; i < flags.Length; i++)
			{
				modeFlags |= (uint) flags[i];
			}

			UpdateMapUI();
		}


		public void SetSearchItem(ItemData itemData)
		{
			if (SingletonMonoBehaviour<Bootstrap>.inst.IsLobbyScene)
			{
				LobbyTab currentTab = MonoBehaviourInstance<LobbyUI>.inst.CurrentTab;
				if (currentTab == LobbyTab.FavoritesTab &&
				    MonoBehaviourInstance<LobbyUI>.inst.GetLobbyTab<LobbyFavoritesTab>(currentTab).FavoriteEditorPage
					    .CurrentFavoritesEditorStep == FavoritesEditorStep.Item)
				{
					return;
				}
			}

			searchItem = itemData;
			UpdateMapUI();
		}


		public void SetBaseAlpha(float alpha)
		{
			canvasGroup.alpha = alpha;
		}


		public void HighlightArea(int areaCode, Color color)
		{
			if (areas.ContainsKey(areaCode))
			{
				areas[areaCode].SetGlow(true, color);
			}
		}


		public bool CheckRestrictedArea(int area)
		{
			foreach (KeyValuePair<int, AreaRestrictionState> keyValuePair in areaStateMap)
			{
				if (keyValuePair.Key == area && keyValuePair.Value == AreaRestrictionState.Restricted)
				{
					return true;
				}
			}

			return false;
		}


		public void SetRollOver(int areaCode, bool enable)
		{
			if (GetLaboratoryAreaCode() == areaCode)
			{
				return;
			}

			areas[areaCode].SetRollOver(enable);
		}


		public int GetLaboratoryAreaCode()
		{
			return levelData.LaboratoryArea.code;
		}


		public void PinAreaAlly(int areaCode, int teamSlot, bool enable)
		{
			if (areas.ContainsKey(areaCode))
			{
				areas[areaCode].SetAllyPin(teamSlot, enable);
			}
		}


		public void PinAreaEnemy(int areaCode, int count)
		{
			if (areas.ContainsKey(areaCode))
			{
				areas[areaCode].SetEnemyPin(count);
			}
		}


		public void SetRouteText(List<int> areaCodes)
		{
			foreach (KeyValuePair<int, Area> keyValuePair in areas)
			{
				int num = areaCodes.IndexOf(keyValuePair.Key);
				if (num >= 0)
				{
					keyValuePair.Value.SetNumber((num + 1).ToString());
				}
				else
				{
					keyValuePair.Value.SetNumber(null);
				}
			}
		}


		public void SetHighlightAreas(List<int> areaCodes, List<int> restrictedAreaCodes)
		{
			int num = areaCodes.Count > 3 ? 3 : areaCodes.Count;
			foreach (KeyValuePair<int, Area> keyValuePair in areas)
			{
				bool flag = false;
				for (int i = 0; i < num; i++)
				{
					if (keyValuePair.Key == areaCodes[i])
					{
						flag = true;
					}
				}

				if (flag)
				{
					keyValuePair.Value.SetGlow(true, new Color32(69, 207, 238, byte.MaxValue));
					keyValuePair.Value.SetColor(Color.white);
				}
				else if (restrictedAreaCodes.Contains(keyValuePair.Key))
				{
					keyValuePair.Value.SetGlow(true, restrictedGlowColor);
					keyValuePair.Value.SetColor(restrictedGlowColor);
				}
				else
				{
					keyValuePair.Value.SetGlow(false, Color.white);
					keyValuePair.Value.SetColor(Color.white);
				}
			}
		}


		public void ClearPin()
		{
			foreach (KeyValuePair<int, Area> keyValuePair in areas)
			{
				keyValuePair.Value.SetAllyPin(1, false);
				keyValuePair.Value.SetAllyPin(2, false);
				keyValuePair.Value.SetAllyPin(3, false);
				keyValuePair.Value.SetEnemyPin(0);
			}
		}


		private void ClearMap()
		{
			foreach (KeyValuePair<int, Area> keyValuePair in areas)
			{
				keyValuePair.Value.Reset();
			}

			foreach (KeyValuePair<int, ItemDataSlot> keyValuePair2 in dropItemSlots)
			{
				keyValuePair2.Value.gameObject.SetActive(false);
			}

			for (int i = 0; i < collectibleList.Count; i++)
			{
				iconPool.Push<Image>(collectibleList[i]);
			}

			collectibleList.Clear();
		}


		public void RefrashAreaText(int areaCode)
		{
			if (areaStateMap == null || !areaStateMap.ContainsKey(areaCode))
			{
				return;
			}

			RefrashAreaText(areaStateMap[areaCode], areaCode);
		}


		private void RefrashAreaText(AreaRestrictionState state, int areaCode)
		{
			if (areas == null || !areas.ContainsKey(areaCode))
			{
				return;
			}

			bool flag = false;
			if (MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				flag = !MonoBehaviourInstance<ClientService>.inst.MyPlayer.unvisitAreaCodeList.Contains(areaCode);
			}

			if (state == AreaRestrictionState.Reserved)
			{
				areas[areaCode].SetGlow(true, reservedGlowColor);
				areas[areaCode].SetColor(flag ? visitReservedColor : unVisitedReservedColor);
				return;
			}

			if (state != AreaRestrictionState.Restricted)
			{
				areas[areaCode].SetGlow(false, defaultGlowColor);
				areas[areaCode].SetColor(flag ? visitDefaultColor : unVisitDefaultColor);
				return;
			}

			areas[areaCode].SetGlow(true, restrictedGlowColor);
			areas[areaCode].SetColor(flag ? visitRestrictedColor : unVisitedRestrictedColor);
		}


		private void UpdateMapUI()
		{
			ClearMap();
			if ((modeFlags & 1U) > 0U && areaStateMap != null)
			{
				foreach (KeyValuePair<int, AreaRestrictionState> keyValuePair in areaStateMap)
				{
					RefrashAreaText(keyValuePair.Value, keyValuePair.Key);
				}
			}

			if ((modeFlags & 2U) > 0U)
			{
				if (searchItem != null)
				{
					foreach (int num in Singleton<ItemService>.inst.GetDropArea(searchItem.code))
					{
						if (dropItemSlots.ContainsKey(num))
						{
							int dropCountInBox = Singleton<ItemService>.inst.GetDropCountInBox(num, searchItem.code);
							if (dropCountInBox > 0)
							{
								dropItemSlots[num].gameObject.SetActive(true);
								dropItemSlots[num].SetItemData(searchItem);
								dropItemSlots[num].SetSlotType(SlotType.None);
								dropItemSlots[num].SetSprite(searchItem.GetSprite());
								dropItemSlots[num].SetBackground(searchItem.GetGradeSprite());
								if (SingletonMonoBehaviour<Bootstrap>.inst.IsLobbyScene)
								{
									dropItemSlots[num].SetStackText(dropCountInBox.ToString());
								}
							}
						}
					}

					int num2 = 0;
					ItemFindInfo itemFindInfo = GameDB.item.GetItemFindInfo(searchItem.code);
					if (itemFindInfo != null)
					{
						num2 = itemFindInfo.collectibleCode;
					}

					if (0 < num2)
					{
						List<Vector3> list = new List<Vector3>();
						if (levelData != null)
						{
							list = levelData.GetResourcePositionList(searchItem);
						}

						for (int i = 0; i < list.Count; i++)
						{
							CreateCollectible(list[i], searchItem.GetMapSprite());
						}
					}

					if (itemFindInfo != null && itemFindInfo.huntChicken != DropFrequency.Never)
					{
						MarkMonster(1);
					}

					if (itemFindInfo != null && itemFindInfo.huntBat != DropFrequency.Never)
					{
						MarkMonster(2);
					}

					if (itemFindInfo != null && itemFindInfo.huntBoar != DropFrequency.Never)
					{
						MarkMonster(3);
					}

					if (itemFindInfo != null && itemFindInfo.huntWildDog != DropFrequency.Never)
					{
						MarkMonster(4);
					}

					if (itemFindInfo != null && itemFindInfo.huntWolf != DropFrequency.Never)
					{
						MarkMonster(5);
					}

					if (itemFindInfo != null && itemFindInfo.huntBear != DropFrequency.Never)
					{
						MarkMonster(6);
					}

					if (itemFindInfo != null && itemFindInfo.huntWickline != DropFrequency.Never)
					{
						MarkMonster(7);
					}

					if (itemFindInfo != null && itemFindInfo.airSupply &&
					    SingletonMonoBehaviour<Bootstrap>.inst.IsGameScene)
					{
						ClientService inst = MonoBehaviourInstance<ClientService>.inst;
						if (inst != null)
						{
							foreach (LocalAirSupplyItemBox localAirSupplyItemBox in inst.World
								.FindAll<LocalAirSupplyItemBox>(x => x.MaxItemGrade == searchItem.itemGrade))
							{
								CreateCollectible(localAirSupplyItemBox.transform.position,
									SingletonMonoBehaviour<ResourceManager>.inst.GetAirSupplySprite(
										searchItem.itemGrade));
							}
						}
					}
				}

				if (SingletonMonoBehaviour<Bootstrap>.inst.IsGameScene && levelData != null)
				{
					for (int j = 0; j < levelData.securityConsoleSpawnPoints.Count; j++)
					{
						CreateCollectible(levelData.securityConsoleSpawnPoints[j].transform.position,
							SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Ico_Map_SecurityConsole"));
					}

					for (int k = 0; k < levelData.hyperloopSpawnPoints.Count; k++)
					{
						CreateCollectible(levelData.hyperloopSpawnPoints[k].transform.position,
							SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Ico_Map_HyperLoop"));
					}
				}
			}
		}


		private void MarkMonster(int monsterCode)
		{
			if (GameDB.monster.GetMonsterData(monsterCode) != null)
			{
				List<Vector3> monsterPositionList = levelData.GetMonsterPositionList(monsterCode);
				for (int i = 0; i < monsterPositionList.Count; i++)
				{
					CreateCollectible(monsterPositionList[i],
						SingletonMonoBehaviour<ResourceManager>.inst.GetMonsterIconSprite(monsterCode));
				}
			}
		}


		private void CreateCollectible(Vector3 pos, Sprite sprite)
		{
			Image item = CreateIconCommon(collectibleIcons, pos, sprite);
			collectibleList.Add(item);
		}


		public void CreateStaticObject(int objectId, Vector3 worldPos, Sprite sprite, MiniMapIconType iconType)
		{
			Image image = FindNonPlayerIcon(objectId);
			if (image == null)
			{
				image = CreateIconCommon(staticObjectIcons, worldPos, sprite);
				objectMap.Add(objectId, image);
				return;
			}

			if (sprite != null)
			{
				image.sprite = sprite;
				image.SetNativeSize();
			}

			UpdatePosition(image.transform, worldPos);
		}


		public void CreateAirSupply(int objectId, Vector3 worldPos, Sprite sprite)
		{
			Image image = FindNonPlayerIcon(objectId);
			if (image == null)
			{
				image = CreateIconCommon(airSupplyIcons, worldPos, sprite);
				objectMap.Add(objectId, image);
				return;
			}

			if (sprite != null)
			{
				image.sprite = sprite;
				image.SetNativeSize();
			}

			UpdatePosition(image.transform, worldPos);
		}


		public void CreateSummonCamera(int objectId, Vector3 worldPos, Sprite sprite, MiniMapIconType iconType)
		{
			Image image = FindNonPlayerIcon(objectId);
			if (image == null)
			{
				image = CreateIconCommon(summonCamera, worldPos, sprite);
				objectMap.Add(objectId, image);
				return;
			}

			if (sprite != null)
			{
				image.sprite = sprite;
				image.SetNativeSize();
			}

			UpdatePosition(image.transform, worldPos);
		}


		public void CreateNonPlayer(int objectId, Vector3 worldPos, Sprite sprite, MiniMapIconType iconType)
		{
			Image image = FindNonPlayerIcon(objectId);
			if (image == null)
			{
				image = CreateIconCommon(monsterIcons, worldPos, sprite);
				objectMap.Add(objectId, image);
				return;
			}

			if (sprite != null)
			{
				image.sprite = sprite;
				image.SetNativeSize();
			}

			UpdatePosition(image.transform, worldPos);
		}


		public void MakeNoise(Vector3 worldPos, int creatorObjectId)
		{
			NoiseEffect noise = CreateNoise(worldPos);
			noise.SetCreatorColor(creatorObjectId);
			noise.SetActiveNoise(true);
			noise.PlayNoise(delegate { iconPool.Push<NoiseEffect>(noise); });
		}


		public void MakeNoise(Vector3 worldPos, Color color)
		{
			NoiseEffect noise = CreateNoise(worldPos);
			noise.SetColor(color, color);
			noise.SetActiveNoise(false);
			noise.PlayNoise(delegate { iconPool.Push<NoiseEffect>(noise); });
		}


		private NoiseEffect CreateNoise(Vector3 worldPos)
		{
			Vector2 a = GameUIUtility.WorldPosToMapUVSpace(worldPos) * rectTransform.rect.size;
			GameObject gameObject = iconPool.Pop<NoiseEffect>();
			gameObject.transform.SetParent(noiseIcons);
			NoiseEffect component = gameObject.GetComponent<NoiseEffect>();
			component.transform.localPosition = a - rectTransform.rect.size * 0.5f;
			return component;
		}


		public void CreatePlayer(int objectId, bool isAlly, Vector3 worldPos, Color color, Sprite background,
			Sprite portrait, string nickname, MiniMapIconType iconType, int teamNumber = 0)
		{
			UIIcon playerIcon = GetPlayerIcon(objectId, isAlly);
			if (playerIcon.GetMiniMapIconType() <= iconType)
			{
				playerIcon.SetMiniMapIconType(iconType);
				playerIcon.SetColor(color);
				playerIcon.SetBackground(background);
				playerIcon.SetPortrait(portrait);
				playerIcon.SetNickname(nickname);
				playerIcon.SetTeam(teamNumber);
				if (!isAlly)
				{
					playerIcon.In();
				}
			}

			UpdatePosition(playerIcon.transform, worldPos);
		}


		public void CreateDeadNonPlayer(int objectId, Vector3 worldPos, Sprite sprite)
		{
			Image image = FindNonPlayerIcon(objectId);
			if (image != null)
			{
				image.transform.SetParent(deadCharacterIcons);
				image.sprite = sprite;
				image.SetNativeSize();
				image.transform.localScale = currentIconScale;
				UpdatePosition(image.transform, worldPos);
				return;
			}

			image = CreateIconCommon(deadCharacterIcons, worldPos, sprite);
			objectMap.Add(objectId, image);
		}


		public void CreateDeadPlayer(int objectId, Vector3 worldPos, Sprite sprite)
		{
			if (MonoBehaviourInstance<ClientService>.inst.MyObjectId == objectId)
			{
				me.Clear();
				me.transform.localScale = Vector3.zero;
			}
			else if (FindPlayerIcon(objectId) != null)
			{
				RemovePlayer(objectId);
			}

			Image image = FindNonPlayerIcon(objectId);
			if (image != null)
			{
				image.transform.SetParent(deadCharacterIcons);
				UpdatePosition(image.transform, worldPos);
				return;
			}

			image = CreateIconCommon(deadCharacterIcons, worldPos, sprite);
			objectMap.Add(objectId, image);
		}


		private UIIcon GetPlayerIcon(int objectId, bool isAlly)
		{
			UIIcon uiicon = FindPlayerIcon(objectId);
			if (uiicon == null)
			{
				GameObject gameObject = iconPool.Pop<UIIcon>();
				uiicon = gameObject.GetComponent<UIIcon>();
				playerMap.Add(objectId, uiicon);
				gameObject.transform.SetParent(isAlly ? allyIcons : enemyIcons);
				gameObject.transform.localScale = currentIconScale;
			}

			return uiicon;
		}


		private Image FindNonPlayerIcon(int objectId)
		{
			if (!objectMap.ContainsKey(objectId))
			{
				return null;
			}

			return objectMap[objectId];
		}


		private UIIcon FindPlayerIcon(int objectId)
		{
			if (MonoBehaviourInstance<ClientService>.inst.MyObjectId == objectId)
			{
				me.transform.localScale = currentIconScale;
				return me;
			}

			if (!playerMap.ContainsKey(objectId))
			{
				return null;
			}

			return playerMap[objectId];
		}


		public GameObject CreateMarkIcon(int teamSlot, Vector3 pos)
		{
			Sprite markSprite = SingletonMonoBehaviour<ResourceManager>.inst.GetMarkSprite(teamSlot);
			return CreateIconCommon(markIcons, pos, markSprite).gameObject;
		}


		public GameObject CreatePingIcon(PingType type, Vector3 pos)
		{
			Sprite pingSprite = SingletonMonoBehaviour<ResourceManager>.inst.GetPingSprite(type);
			return CreateIconCommon(pingIcons, pos, pingSprite).gameObject;
		}


		private Image CreateIconCommon(Transform parent, Vector3 pos, Sprite sprite)
		{
			Vector2 uv = GameUIUtility.WorldPosToMapUVSpace(pos);
			GameObject gameObject = iconPool.Pop<Image>(parent);
			Image component = gameObject.GetComponent<Image>();
			component.sprite = sprite;
			component.SetNativeSize();
			component.transform.localScale = currentIconScale;
			gameObject.transform.localPosition =
				GameUIUtility.MapSapceUVToLocalPos((RectTransform) canvasGroup.transform, uv);
			return component;
		}


		public void UpdateNonPlayerIcon(int objectId, Sprite sprite)
		{
			if (objectMap.ContainsKey(objectId))
			{
				objectMap[objectId].sprite = sprite;
				objectMap[objectId].SetNativeSize();
			}
		}


		public void RemoveNonPlayer(int objectId)
		{
			if (objectMap.ContainsKey(objectId))
			{
				iconPool.Push<Image>(objectMap[objectId]);
				objectMap.Remove(objectId);
			}
		}


		public void UpdateAreaName()
		{
			if (areas != null)
			{
				foreach (int num in areas.Keys)
				{
					areas[num].SetName(Ln.Get(string.Format("Area/Name/{0}", num)));
				}
			}
		}


		private IEnumerator LineRendererUpdater()
		{
			for (;;)
			{
				yield return new WaitForSeconds(1f);
				pathLineRenderer.SetVerticesDirty();
			}
		}


		public void UpdatePlayerIcon(int objectId, Color color, Sprite background, Sprite portrait)
		{
			UIIcon uiicon = FindPlayerIcon(objectId);
			if (uiicon != null)
			{
				uiicon.SetColor(color);
			}

			if (uiicon != null)
			{
				uiicon.SetBackground(background);
			}

			if (uiicon == null)
			{
				return;
			}

			uiicon.SetPortrait(portrait);
		}


		public void Revival(int objectId)
		{
			UIIcon uiicon = FindPlayerIcon(objectId);
			if (uiicon == null)
			{
				return;
			}

			uiicon.Alive();
		}


		public void DyingCondition(int objectId)
		{
			UIIcon uiicon = FindPlayerIcon(objectId);
			if (uiicon == null)
			{
				return;
			}

			uiicon.DyingCondition();
		}


		public void UpdateNonPlayerPosition(int objectId, Vector3 worldPos)
		{
			if (objectMap.ContainsKey(objectId))
			{
				UpdatePosition(objectMap[objectId].transform, worldPos);
			}
		}


		public void UpdatePlayerPosition(int objectId, bool isAlly, Vector3 worldPos)
		{
			UIIcon uiicon = FindPlayerIcon(objectId);
			if (uiicon != null)
			{
				if (isAlly)
				{
					Vector2 vector = GameUIUtility.WorldPosToMapUVSpace(worldPos);
					Vector2 vector2 = vector * rectTransform.rect.size;
					Vector2 v;
					bool inBoundaryPosition = GetInBoundaryPosition(vector, vector2, out v);
					uiicon.transform.localPosition = v;
					if (inBoundaryPosition)
					{
						uiicon.Out(GameUtil.GetRotation(vector2, centerPos));
						return;
					}

					uiicon.In();
				}
				else
				{
					uiicon.transform.localPosition =
						GameUIUtility.UvSpaceToLocalPos(GameUIUtility.WorldPosToMapUVSpace(worldPos), rectTransform);
				}
			}
		}


		private void UpdatePosition(Transform icon, Vector3 worldPos)
		{
			icon.localPosition =
				GameUIUtility.UvSpaceToLocalPos(GameUIUtility.WorldPosToMapUVSpace(worldPos), rectTransform);
		}


		public Vector2 WorldPositionToMiniMapPosition(Vector3 worldPos)
		{
			return GameUIUtility.UvSpaceToLocalPos(GameUIUtility.WorldPosToMapUVSpace(worldPos), rectTransform) * -1f;
		}


		public void RemovePlayer(int objectId)
		{
			if (playerMap.ContainsKey(objectId))
			{
				playerMap[objectId].Clear();
				iconPool.Push<UIIcon>(playerMap[objectId]);
				playerMap.Remove(objectId);
			}
		}


		public void RemovePlayer(int objectId, MiniMapIconType iconType)
		{
			if (playerMap.ContainsKey(objectId) && playerMap[objectId].GetMiniMapIconType() <= iconType)
			{
				playerMap[objectId].Clear();
				iconPool.Push<UIIcon>(playerMap[objectId]);
				playerMap.Remove(objectId);
			}
		}


		private void OnClickDown(PointerEventData eventData)
		{
			try
			{
				if (!SingletonMonoBehaviour<Bootstrap>.inst.IsLobbyScene)
				{
					GameUI inst = MonoBehaviourInstance<GameUI>.inst;
					if (inst == null)
					{
						throw new GameException("GameUI is Null");
					}

					if (eventData.button == PointerEventData.InputButton.Left)
					{
						if (MonoBehaviourInstance<ClientService>.inst.IsPlayer)
						{
							if (Input.GetKey(KeyCode.LeftAlt))
							{
								if (inst.PingHud == null)
								{
									throw new GameException("PingHud is Null");
								}

								if (!inst.PingHud.IsActive())
								{
									Vector3 worldPos = GameUIUtility.MapSpaceUVToWorldPos(
										(GameUIUtility.ScreenToRectPos(mainCamera, canvasRect, rectTransform,
											eventData.position) + rectTransform.sizeDelta * 0.5f) /
										rectTransform.sizeDelta);
									inst.PingHud.ActiveOnUI(worldPos, eventData.position);
								}
							}
							else if (Input.GetKey(KeyCode.LeftControl))
							{
								Vector3 targetPosition = GameUIUtility.MapSpaceUVToWorldPos(
									(GameUIUtility.ScreenToRectPos(mainCamera, canvasRect, rectTransform,
										eventData.position) + rectTransform.sizeDelta * 0.5f) /
									rectTransform.sizeDelta);
								ReqMark packet = new ReqMark
								{
									targetPosition = targetPosition,
									targetObjectId = 0,
									teamSlot = MonoBehaviourInstance<ClientService>.inst.GetTeamSlot(
										MonoBehaviourInstance<ClientService>.inst.MyPlayer.userId)
								};
								GameClient inst2 = MonoBehaviourInstance<GameClient>.inst;
								if (inst2 == null)
								{
									throw new GameException("GameClient is Null");
								}

								inst2.Request(packet);
							}
							else if (SingletonMonoBehaviour<PlayerController>.inst != null &&
							         SingletonMonoBehaviour<PlayerController>.inst.CursorStatus == CursorStatus.Attack)
							{
								AttackMoveCameraPos(eventData.position);
							}
							else
							{
								dragCamera = true;
								this.StartThrowingCoroutine(MoveCamera(),
									delegate(Exception exception)
									{
										Log.E("[EXCEPTION][UIMapMoveCamera] Message:" + exception.Message +
										      ", StackTrace:" + exception.StackTrace);
									});
							}
						}
						else
						{
							dragCamera = true;
							MonoBehaviourInstance<MobaCamera>.inst.SetCameraMode(MobaCameraMode.Traveling);
							this.StartThrowingCoroutine(MoveCamera(),
								delegate(Exception exception)
								{
									Log.E("[EXCEPTION][UIMapMoveCamera] Message:" + exception.Message +
									      ", StackTrace:" + exception.StackTrace);
								});
						}
					}
				}
			}
			catch (Exception ex)
			{
				Log.V("[EXCEPTION] " + ex.Message + ": " + ex.StackTrace);
				throw;
			}
		}


		public void OnClickDownMiniMap(PointerEventData eventData)
		{
			OnClickDown(eventData);
			try
			{
				PointerEventData.InputButton button = eventData.button;
				if (button == PointerEventData.InputButton.Right)
				{
					MoveCameraPos(eventData.position);
				}
			}
			catch (Exception ex)
			{
				Log.V("[EXCEPTION] " + ex.Message + ": " + ex.StackTrace);
				throw;
			}
		}


		public void OnClickDownMapWindow(PointerEventData eventData)
		{
			OnClickDown(eventData);
			try
			{
				PointerEventData.InputButton button = eventData.button;
				if (button == PointerEventData.InputButton.Right && Input.GetKey(KeyCode.LeftControl) &&
				    Singleton<LocalSetting>.inst.setting.moveExpandMap)
				{
					MoveCameraPos(eventData.position);
				}
			}
			catch (Exception ex)
			{
				Log.V("[EXCEPTION] " + ex.Message + ": " + ex.StackTrace);
				throw;
			}
		}


		public void OnClickUp(PointerEventData eventData)
		{
			base.OnPointerUp(eventData);
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				dragCamera = false;
			}
		}


		private IEnumerator MoveCamera()
		{
			if (MonoBehaviourInstance<MobaCamera>.inst.Mode != MobaCameraMode.Manual)
			{
				Vector3 currentScreenPos = new Vector2(-1f, -1f);
				MobaCameraMode preMode = MonoBehaviourInstance<MobaCamera>.inst.Mode;
				MonoBehaviourInstance<MobaCamera>.inst.SetCameraMode(MobaCameraMode.Manual);
				while (dragCamera)
				{
					if (currentScreenPos != Input.mousePosition)
					{
						currentScreenPos = Input.mousePosition;
						FocusMinimapCamera(currentScreenPos);
					}

					yield return null;
				}

				if (preMode == MobaCameraMode.Tracking)
				{
					MonoBehaviourInstance<MobaCamera>.inst.ResetCameraPosition();
				}

				MonoBehaviourInstance<MobaCamera>.inst.SetCameraMode(preMode);
				currentScreenPos = default;
			}
		}


		private void FocusMinimapCamera(Vector2 screenPos)
		{
			Vector3 worldPos = GameUIUtility.MapSpaceUVToWorldPos(
				(GameUIUtility.ScreenToRectPos(mainCamera, canvasRect, rectTransform, screenPos) +
				 rectTransform.sizeDelta * 0.5f) / rectTransform.sizeDelta);
			worldPos.y = 0f;
			MonoBehaviourInstance<MobaCamera>.inst.SetCameraPosition(worldPos, 0f);
		}


		private void MoveCameraPos(Vector2 screenPos)
		{
			Vector3 destination = GameUIUtility.MapSpaceUVToWorldPos(
				(GameUIUtility.ScreenToRectPos(mainCamera, canvasRect, rectTransform, screenPos) +
				 rectTransform.sizeDelta * 0.5f) / rectTransform.sizeDelta);
			SingletonMonoBehaviour<PlayerController>.inst.MoveTo(destination, false);
		}


		private void AttackMoveCameraPos(Vector2 screenPos)
		{
			Vector3 hitPoint = GameUIUtility.MapSpaceUVToWorldPos(
				(GameUIUtility.ScreenToRectPos(mainCamera, canvasRect, rectTransform, screenPos) +
				 rectTransform.sizeDelta * 0.5f) / rectTransform.sizeDelta);
			SingletonMonoBehaviour<PlayerController>.inst.AttackMoveTo(hitPoint);
			SingletonMonoBehaviour<PlayerController>.inst.SetCursorStatus(CursorStatus.Normal);
		}


		public void SetCorners(List<Vector3> corners)
		{
			pathNavCorners.Clear();
			for (int i = 0; i < corners.Count; i++)
			{
				Vector2 vector = GameUIUtility.WorldPosToMapUVSpace(corners[i]);
				vector *= rectTransform.rect.size;
				pathNavCorners.Add(vector);
				pathLineRenderer.SetVerticesDirty();
			}
		}


		public GameObject AddMark(int teamSlot, Vector3 worldPos)
		{
			return CreateMarkIcon(teamSlot, worldPos);
		}


		public GameObject AddPing(PingType type, Vector3 worldPos, int senderObjectId)
		{
			if (!MonoBehaviourInstance<ClientService>.inst.IgnoreTargetService.IsIgnoreUser(IgnoreType.Ping,
				senderObjectId))
			{
				MakeNoise(worldPos, LocalPingUtil.GetPingColor(type));
			}

			return CreatePingIcon(type, worldPos);
		}


		public void CreateIndicators()
		{
			indicatorManager.CreateIndicators();
		}


		public void SetIndicator(float range)
		{
			indicatorManager.SetIndicator(range);
		}


		public void HideIndicator()
		{
			indicatorManager.HideIndicator();
		}


		public void ZoomIn()
		{
			currentMapScale = 1f;
			currentObjectScale = 1f / currentMapScale;
			currentIconScale = new Vector3(currentObjectScale, currentObjectScale, currentObjectScale);
			transform.localScale = Vector3.one * currentMapScale;
			foreach (KeyValuePair<int, Image> keyValuePair in objectMap)
			{
				keyValuePair.Value.transform.localScale = currentIconScale;
			}

			foreach (KeyValuePair<int, UIIcon> keyValuePair2 in playerMap)
			{
				keyValuePair2.Value.transform.localScale = currentIconScale;
			}
		}


		public void ZoomOut()
		{
			currentMapScale = 0.75f;
			currentObjectScale = 1f / currentMapScale;
			currentIconScale = new Vector3(currentObjectScale, currentObjectScale, currentObjectScale);
			transform.localScale = Vector3.one * currentMapScale;
			foreach (KeyValuePair<int, Image> keyValuePair in objectMap)
			{
				keyValuePair.Value.transform.localScale = currentIconScale;
			}

			foreach (KeyValuePair<int, UIIcon> keyValuePair2 in playerMap)
			{
				keyValuePair2.Value.transform.localScale = currentIconScale;
			}
		}


		private class Area
		{
			private const float PADDING = 5f;


			private readonly Image allyPin1;


			private readonly Image allyPin2;


			private readonly Image allyPin3;


			private readonly RectTransform allyPins;


			private readonly RectTransform areaName;


			private readonly Text areaNameText;


			private readonly Text enemyCount;


			private readonly Image enemyPin;


			private readonly UIMapSlice mapSlice;


			private readonly Text number;


			private readonly Image numberPlate;


			public Area(Transform transform, int areaCode)
			{
				areaName = GameUtil.Bind<RectTransform>(transform.gameObject,
					string.Format("AreaNames/{0}/Name/Bg", areaCode));
				areaNameText = GameUtil.Bind<Text>(areaName.gameObject, "Text");
				numberPlate = GameUtil.Bind<Image>(areaName.gameObject, "Number");
				number = GameUtil.Bind<Text>(numberPlate.gameObject, "Text");
				mapSlice = GameUtil.Bind<UIMapSlice>(transform.gameObject, string.Format("Areas/{0}/Glow", areaCode));
				mapSlice.areaCode = areaCode;
				allyPins = GameUtil.Bind<RectTransform>(transform.gameObject,
					string.Format("Areas/{0}/Pins/AllyPin", areaCode));
				allyPin1 = GameUtil.Bind<Image>(allyPins.gameObject, "Pin (1)");
				allyPin2 = GameUtil.Bind<Image>(allyPins.gameObject, "Pin (2)");
				allyPin3 = GameUtil.Bind<Image>(allyPins.gameObject, "Pin (3)");
				enemyPin = GameUtil.Bind<Image>(transform.gameObject,
					string.Format("Areas/{0}/Pins/EnemyPin", areaCode));
				enemyCount = GameUtil.Bind<Text>(enemyPin.gameObject, "Count");
				allyPins.gameObject.SetActive(false);
			}

			
			
			public event Action<int> OnClickEvent = delegate { };


			public void SetName(string name)
			{
				areaNameText.text = name;
			}


			public void SetNumber(string number)
			{
				this.number.text = number;
				numberPlate.gameObject.SetActive(!string.IsNullOrEmpty(number));
			}


			public void SetColor(Color color)
			{
				areaNameText.color = color;
			}


			public void SetGlow(bool enable, Color color)
			{
				mapSlice.SetMapState(enable, color);
			}


			public void SetAllyPin(int teamSlot, bool enable)
			{
				switch (teamSlot)
				{
					case 1:
						allyPin1.gameObject.SetActive(enable);
						break;
					case 2:
						allyPin2.gameObject.SetActive(enable);
						break;
					case 3:
						allyPin3.gameObject.SetActive(enable);
						break;
				}

				if (allyPin1.gameObject.activeSelf || allyPin2.gameObject.activeSelf || allyPin3.gameObject.activeSelf)
				{
					if (!allyPins.gameObject.activeSelf)
					{
						allyPins.gameObject.SetActive(true);
					}
				}
				else if (allyPins.gameObject.activeSelf)
				{
					allyPins.gameObject.SetActive(false);
				}
			}


			public void SetEnemyPin(int count)
			{
				enemyPin.gameObject.SetActive(count > 0);
				enemyCount.text = null;
				if (count > 1)
				{
					enemyCount.text = count.ToString();
				}
			}


			public void SetRollOver(bool enable)
			{
				mapSlice.SetRollOver(enable);
			}


			public void Reset()
			{
				SetGlow(false, Color.white);
				SetColor(Color.white);
				SetAllyPin(1, false);
				SetAllyPin(2, false);
				SetAllyPin(3, false);
				SetEnemyPin(0);
				SetNumber(null);
			}


			public void SetEventHandler(IMapSlicePointerEventHandler eventHandler)
			{
				mapSlice.SetEventHandler(eventHandler);
			}
		}
	}
}