using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;
using Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	public class AreaService : ServiceBase
	{
		
		
		public WorldSecurityConsole LastSafeConsole
		{
			get
			{
				return this.lastSafeConsole;
			}
		}

		
		
		public DayNight DayNight
		{
			get
			{
				return this.dayNight;
			}
		}

		
		
		public int Day
		{
			get
			{
				return this.day;
			}
		}

		
		
		public bool IsStopRestrictedArea
		{
			get
			{
				return this.isStopAreaRestriction;
			}
		}

		
		
		public float AreaRestrictionRemainTime
		{
			get
			{
				return (float)Mathf.FloorToInt(0.033333335f * (float)Mathf.Max(this.nextUpdateRestrictedAreaSeq - MonoBehaviourInstance<GameServer>.inst.Seq, 0));
			}
		}

		
		
		public Dictionary<int, AreaRestrictionState> AreaStateMap
		{
			get
			{
				return this.areaStateList.ToDictionary((Area x) => x.AreaCode, (Area x) => x.AreaRestrictionState);
			}
		}

		
		
		public int AccelerationCount
		{
			get
			{
				return this.accelerationCount;
			}
		}

		
		public void LoadLevel(LevelData levelData, bool onAcceleration)
		{
			this.areaStateList.Clear();
			for (int i = 1; i < 17; i++)
			{
				this.areaStateList.Add(new Area(i, levelData.nearByAreaMap[i]));
				this.areaDataList.Add(GameDB.level.GetAreaData(i));
			}
			this.GetAreaByCode(levelData.LaboratoryArea.code).UpdateAreaState(AreaRestrictionState.Restricted);
			this.areaTraveler = new GraphTraveler<int>(delegate(int current)
			{
				List<int> nearByAreaCodes = this.GetAreaByCode(current).nearByAreaCodes;
				List<int> list = new List<int>();
				foreach (int num in nearByAreaCodes)
				{
					if (this.GetAreaByCode(num).AreaRestrictionState == AreaRestrictionState.Normal)
					{
						list.Add(num);
					}
				}
				return list;
			});
			this.game.AirSupply.Init();
			this.isOnAcceleration = onAcceleration;
		}

		
		public void LoadLevel(LevelData levelData)
		{
			this.LoadLevel(levelData, true);
		}

		
		public Area GetAreaByCode(int areaCode)
		{
			return this.areaStateList[areaCode - 1];
		}

		
		public bool IsInFinalSafeZone(Vector3 position)
		{
			return this.lastSafeConsole != null && this.lastSafeConsole.IsNear(position);
		}

		
		private void RestrictedAreaDOT(WorldPlayerCharacter character, float interval)
		{
			if (!character.IsAlive)
			{
				return;
			}
			if ((character.GetCurrentAreaMask() & this.restrictedAreaMask) > 0 && !this.IsInFinalSafeZone(character.GetPosition()))
			{
				character.UpdateSurvivableTime(character.SurvivableTime - interval);
				if (character.SurvivableTime <= 0f)
				{
					DirectDamageCalculator damageCalculator = new DirectDamageCalculator(WeaponType.None, DamageType.RedZone, DamageSubType.Normal, 0, 0, character.Stat.MaxHp, 0, 1f);
					Singleton<DamageService>.inst.EnvironmentDamageTo(character, damageCalculator, null, 2000001);
				}
			}
		}

		
		public UpdateMasteryInfo CheckFirstVisitArea(WorldPlayerCharacter character)
		{
			if (character.CheckFirstVisit(character.GetCurrentAreaMask()))
			{
				return new UpdateMasteryInfo
				{
					conditionType = MasteryConditionType.MoveNewArea,
					takeMasteryValue = 1
				};
			}
			return null;
		}

		
		private IEnumerator CoRestrictedAreaDOT()
		{
			float interval = 1f;
			float timeStack = 0f;
			for (;;)
			{
				yield return this.waitForFrameUpdate_1.Frame(1);
				timeStack += MonoBehaviourInstance<GameService>.inst.ServerFrameDeltaTime;
				if (timeStack >= interval)
				{
					timeStack -= interval;
					try
					{
						this.game.World.FindAllDoAction<WorldPlayerCharacter>(delegate(WorldPlayerCharacter playerCharacter)
						{
							this.RestrictedAreaDOT(playerCharacter, interval);
						});
						continue;
					}
					catch (Exception ex)
					{
						Log.W(ex.ToString());
						continue;
					}
				}
			}
		}

		
		private IEnumerator CoCheckFirstVisitArea()
		{
			float interval = 1f;
			float timeStack = 0f;
			for (;;)
			{
				yield return this.waitForFrameUpdate_2.Frame(1);
				timeStack += MonoBehaviourInstance<GameService>.inst.ServerFrameDeltaTime;
				if (timeStack >= interval)
				{
					timeStack -= interval;
					try
					{
						this.game.World.FindAllDoAction<WorldPlayerCharacter>(delegate(WorldPlayerCharacter playerCharacter)
						{
							if (!playerCharacter.IsAI)
							{
								UpdateMasteryInfo updateMasteryInfo = this.CheckFirstVisitArea(playerCharacter);
								playerCharacter.AddMasteryConditionExp(updateMasteryInfo);
							}
						});
						continue;
					}
					catch (Exception ex)
					{
						Log.W(ex.ToString());
						continue;
					}
				}
			}
		}

		
		private void RestrictEvent(int currentStep, bool isAcceleration, int stepDuration)
		{
			int num = GameDB.level.GetRestrictedAreaCount() - currentStep;
			switch (num)
			{
			case 0:
				this.game.Announce.LastSafeConsoleAnnounce(LastSafeConsoleAnnounceType.Deactive);
				if (this.lastSafeConsole != null)
				{
					this.lastSafeConsole.ActiveSafeArea(false);
					this.lastSafeConsole = null;
				}
				break;
			case 1:
				this.game.Announce.LastSafeConsoleAnnounce(LastSafeConsoleAnnounceType.Active);
				base.StartCoroutine(CoroutineUtil.DelayedAction(Mathf.Max(stepDuration - 60, 0), delegate()
				{
					this.game.Announce.LastSafeConsoleAnnounce(LastSafeConsoleAnnounceType.PreDeactive);
				}));
				MonoBehaviourInstance<GameService>.inst.OnFinalSafeZoneActive();
				break;
			case 2:
			{
				this.game.Announce.RestrictionAreaAnnounce(AreaRestrictionAnnounceType.Last);
				Area lastSafeArea = this.areaStateList.FirstOrDefault((Area x) => x.AreaRestrictionState == AreaRestrictionState.Normal);
				this.lastSafeConsole = this.world.FindAll<WorldSecurityConsole>().FirstOrDefault((WorldSecurityConsole x) => x.AreaCode == lastSafeArea.AreaCode);
				if (this.lastSafeConsole != null)
				{
					this.lastSafeConsole.ActiveSafeArea(true);
				}
				break;
			}
			default:
				if (currentStep != 0)
				{
					if (isAcceleration)
					{
						this.game.Announce.RestrictionAreaAnnounce(AreaRestrictionAnnounceType.Accelerater);
					}
					else
					{
						this.game.Announce.RestrictionAreaAnnounce(AreaRestrictionAnnounceType.Normal);
					}
				}
				break;
			}
			if (this.game.MatchingMode.IsTutorialMode() && !GameDB.tutorial.GetTutorialSettingData(this.game.MatchingMode.ConvertToTutorialType()).enableAirSupply)
			{
				return;
			}
			this.game.AirSupply.AirSupply();
			this.game.AirSupply.AirSupplyAnnounce((float)stepDuration);
		}

		
		private IEnumerator AreaRestriction(DayNight firstDayNight)
		{
			this.day = 1;
			this.dayNight = firstDayNight;
			while (!GameDB.level.IsFinalAreaRestriction(this.currentRestrictionCount))
			{
				RestrictedAreaInfo restrictedAreaData = GameDB.level.GetRestrictedAreaData(this.currentRestrictionCount);
				bool flag = this.isOnAcceleration && this.game.Player.AlivePlayerCharacterCount() + MonoBehaviourInstance<GameService>.inst.Bot.GetAliveBotCharacterCount() <= restrictedAreaData.minimumSurvivors;
				this.RestrictEvent(this.currentRestrictionCount, flag, restrictedAreaData.durationSeconds);
				this.ApplyAreaRestriction(flag);
				this.nextUpdateRestrictedAreaSeq = MonoBehaviourInstance<GameServer>.inst.Seq + Mathf.CeilToInt((float)restrictedAreaData.durationSeconds / 0.033333335f);
				if (flag)
				{
					this.accelerationCount++;
				}
				GameServer server = this.server;
				RpcUpdateRestrictedArea rpcUpdateRestrictedArea = new RpcUpdateRestrictedArea();
				rpcUpdateRestrictedArea.remainTime = restrictedAreaData.durationSeconds;
				rpcUpdateRestrictedArea.areaStateMap = this.areaStateList.ToDictionary((Area x) => x.AreaCode, (Area x) => x.AreaRestrictionState);
				rpcUpdateRestrictedArea.dayNight = this.dayNight;
				server.Broadcast(rpcUpdateRestrictedArea, NetChannel.ReliableOrdered);
				foreach (WorldPlayerCharacter worldPlayerCharacter in this.world.FindAll<WorldPlayerCharacter>())
				{
					if (this.dayNight == DayNight.Night)
					{
						worldPlayerCharacter.AddState(new CommonState(10004, worldPlayerCharacter, worldPlayerCharacter), 0);
					}
					else
					{
						worldPlayerCharacter.RemoveStateByGroup(10004);
					}
				}
				foreach (WorldMonster worldMonster in this.world.FindAll<WorldMonster>())
				{
					worldMonster.SetAggressive(this.dayNight);
				}
				yield return new WaitForFrameUpdate().Seconds((float)restrictedAreaData.durationSeconds);
				this.dayNight = this.dayNight.Next();
				if (this.dayNight == DayNight.Day)
				{
					this.day++;
				}
			}
			this.RestrictEvent(this.currentRestrictionCount, false, 0);
		}

		
		private void ChangeAreaStateInRandom(List<Area> list, int count, AreaRestrictionState state)
		{
			if (list.Count <= 0 || count <= 0)
			{
				return;
			}
			List<Area> list2 = ListUtil.Random<Area>(list);
			for (int i = 0; i < count; i++)
			{
				for (int j = 0; j < list2.Count; j++)
				{
					if (this.TryUpdateAreaState(list2[j], state))
					{
						list2.RemoveAt(j);
						break;
					}
				}
			}
		}

		
		private bool TryUpdateAreaState(Area area, AreaRestrictionState state)
		{
			AreaRestrictionState areaRestrictionState = area.AreaRestrictionState;
			area.UpdateAreaState(state);
			if (this.CheckAreaIsolation())
			{
				area.UpdateAreaState(areaRestrictionState);
				return false;
			}
			return true;
		}

		
		private bool CheckAreaIsolation()
		{
			this.cacheNormalArea.Clear();
			foreach (Area areaState in this.areaStateList)
			{
				if (areaState.AreaRestrictionState == AreaRestrictionState.Normal)
					this.cacheNormalArea.Add(areaState);
			}
			if (this.cacheNormalArea.Count <= 0)
				return false;
			int visitCount = 0;
			this.areaTraveler.TravelBFS(this.cacheNormalArea[0].AreaCode, (GraphTraveler<int>.VisitEvent) (node => ++visitCount));
			return this.cacheNormalArea.Count != visitCount;
			
			// co: dotPeek
			// this.cacheNormalArea.Clear();
			// foreach (Area area in this.areaStateList)
			// {
			// 	if (area.AreaRestrictionState == AreaRestrictionState.Normal)
			// 	{
			// 		this.cacheNormalArea.Add(area);
			// 	}
			// }
			// if (this.cacheNormalArea.Count <= 0)
			// {
			// 	return false;
			// }
			// int visitCount = 0;
			// this.areaTraveler.TravelBFS(this.cacheNormalArea[0].AreaCode, delegate(int node)
			// {
			// 	int visitCount = visitCount;
			// 	visitCount++;
			// });
			// return this.cacheNormalArea.Count != visitCount;
		}

		
		private void ChangeAreaState(List<Area> list, AreaRestrictionState state)
		{
			for (int i = list.Count - 1; i >= 0; i--)
			{
				this.GetAreaByCode(list[i].AreaCode).UpdateAreaState(state);
			}
		}

		
		private void MakeNextAreaRestriction(bool isAcceleration)
		{
			if (GameDB.level.IsFinalAreaRestriction(this.currentRestrictionCount))
			{
				return;
			}
			RestrictedAreaInfo restrictedAreaInfo = GameDB.level.GetRestrictedAreaData(this.currentRestrictionCount);
			int count = this.currentRestrictionCount + 1;
			if (!GameDB.level.IsFinalAreaRestriction(count))
			{
				RestrictedAreaInfo restrictedAreaData = GameDB.level.GetRestrictedAreaData(count);
				if (isAcceleration)
				{
					restrictedAreaInfo = new RestrictedAreaInfo
					{
						durationSeconds = restrictedAreaData.durationSeconds,
						reservedCount = restrictedAreaInfo.reservedCount + restrictedAreaData.reservedCount,
						clearCount = restrictedAreaInfo.clearCount + restrictedAreaData.clearCount,
						damageOnTime = restrictedAreaData.damageOnTime,
						minimumSurvivors = restrictedAreaData.minimumSurvivors
					};
				}
			}
			int reservedCount = restrictedAreaInfo.reservedCount;
			int clearCount = restrictedAreaInfo.clearCount;
			this.cacheRestrictedAreas.Clear();
			this.cacheReservedAreas.Clear();
			this.cacheNormalAreas.Clear();
			foreach (Area area in this.areaStateList)
			{
				if (area.AreaCode != this.game.CurrentLevel.LaboratoryArea.code)
				{
					if (area.AreaRestrictionState == AreaRestrictionState.Normal)
					{
						this.cacheNormalAreas.Add(area);
					}
					else if (area.AreaRestrictionState == AreaRestrictionState.Reserved)
					{
						this.cacheReservedAreas.Add(area);
					}
					else if (area.AreaRestrictionState == AreaRestrictionState.Restricted)
					{
						this.cacheRestrictedAreas.Add(area);
					}
				}
			}
			if (this.cacheRestrictedAreas.Count < clearCount)
			{
				throw new Exception();
			}
			this.ChangeAreaState(this.cacheReservedAreas, AreaRestrictionState.Restricted);
			this.ChangeAreaStateInRandom(this.cacheRestrictedAreas, clearCount, AreaRestrictionState.Normal);
			if (this.cacheNormalAreas.Count < reservedCount)
			{
				throw new Exception();
			}
			this.ChangeAreaStateInRandom(this.cacheNormalAreas, reservedCount, AreaRestrictionState.Reserved);
			this.currentRestrictionCount = (isAcceleration ? (this.currentRestrictionCount + 2) : (this.currentRestrictionCount + 1));
		}

		
		public void ApplyAreaRestriction(bool isAcceleration)
		{
			this.MakeNextAreaRestriction(isAcceleration);
			this.ApplyAreaRestriction__();
		}

		
		public int GetSafeStateAreaCount()
		{
			int num = 0;
			foreach (Area area in this.areaStateList)
			{
				if (area.AreaRestrictionState == AreaRestrictionState.Normal || area.AreaRestrictionState == AreaRestrictionState.Reserved)
				{
					num++;
				}
			}
			return num;
		}

		
		private void ApplyAreaRestriction__()
		{
			this.restrictedAreaMask = this.game.CurrentLevel.LaboratoryArea.maskCode;
			foreach (Area area in this.areaStateList)
			{
				if (area.AreaRestrictionState == AreaRestrictionState.Restricted && area.AreaCode != this.game.CurrentLevel.LaboratoryArea.code)
				{
					this.restrictedAreaMask |= this.game.CurrentLevel.areaDataMap[area.AreaCode].maskCode;
				}
			}
		}

		
		public List<Area> GetAreasByState(AreaRestrictionState state)
		{
			this.cacheGetAreasByStateList.Clear();
			foreach (Area area in this.areaStateList)
			{
				if (area.AreaRestrictionState == state)
				{
					this.cacheGetAreasByStateList.Add(area);
				}
			}
			return this.cacheGetAreasByStateList;
		}

		
		public IEnumerable<AreaData> getAreaDataListByState(AreaRestrictionState areaRestrictionState)
		{
			return this.getAreaDataList((Area area) => area.AreaRestrictionState == areaRestrictionState);
		}

		
		public IEnumerable<AreaData> getAreaDataList(Func<Area, bool> predicate)
		{
			this.cacheGetAreaList.Clear();
			foreach (Area area2 in this.areaStateList)
			{
				if (predicate(area2))
				{
					this.cacheGetAreaList.Add(area2);
				}
			}
			this.cacheGetAreaDataList.Clear();
			using (List<AreaData>.Enumerator enumerator2 = this.areaDataList.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					AreaData areaData = enumerator2.Current;
					if (this.cacheGetAreaList.Any((Area area) => area.AreaCode == areaData.code))
					{
						this.cacheGetAreaDataList.Add(areaData);
					}
				}
			}
			return this.cacheGetAreaDataList;
		}

		
		public void ToggleAreaRestriction()
		{
			if (this.coAreaRestriction == null)
			{
				this.StartRestriction(this.dayNight.Next());
				return;
			}
			this.StopRestriction();
		}

		
		public void StartRestriction(DayNight beginDayNight)
		{
			this.coAreaRestriction = base.StartCoroutine(this.AreaRestriction(beginDayNight));
			this.coRestrictedAreaDOT = base.StartCoroutine(this.CoRestrictedAreaDOT());
		}

		
		public void StopRestriction()
		{
			if (this.coAreaRestriction != null)
			{
				base.StopCoroutine(this.coAreaRestriction);
				this.coAreaRestriction = null;
			}
			if (this.coRestrictedAreaDOT != null)
			{
				base.StopCoroutine(this.coRestrictedAreaDOT);
				this.coRestrictedAreaDOT = null;
			}
			this.isStopAreaRestriction = true;
			this.server.Broadcast(new RpcStopRestrictedArea(), NetChannel.ReliableOrdered);
		}

		
		public void StartCheckFirstVisitArea()
		{
			if (this.coCheckFirstVisitArea != null)
			{
				base.StopCoroutine(this.coCheckFirstVisitArea);
			}
			this.coCheckFirstVisitArea = base.StartCoroutine(this.CoCheckFirstVisitArea());
		}

		
		public void ApplyTutorialRestrictArea(TutorialSettingData tutorialSettingData)
		{
			foreach (Area area in this.areaStateList)
			{
				if (tutorialSettingData.restrictedAreas.Contains(area.AreaCode))
				{
					area.UpdateAreaState(AreaRestrictionState.Restricted);
				}
				else
				{
					area.UpdateAreaState(AreaRestrictionState.Normal);
				}
			}
			this.ApplyAreaRestriction__();
			int remainTime = 0;
			if (tutorialSettingData.enableBattleTimer)
			{
				remainTime = GameDB.level.GetRestrictedAreaData(this.currentRestrictionCount).durationSeconds;
			}
			GameServer server = this.server;
			RpcUpdateRestrictedArea rpcUpdateRestrictedArea = new RpcUpdateRestrictedArea();
			rpcUpdateRestrictedArea.remainTime = remainTime;
			rpcUpdateRestrictedArea.areaStateMap = this.areaStateList.ToDictionary((Area x) => x.AreaCode, (Area x) => x.AreaRestrictionState);
			rpcUpdateRestrictedArea.dayNight = this.dayNight;
			server.Broadcast(rpcUpdateRestrictedArea, NetChannel.ReliableOrdered);
		}

		
		public IEnumerable<WorldSecurityCamera> GetSecurityCameras(int areaCode)
		{
			return this.world.FindAll<WorldSecurityCamera>((WorldSecurityCamera x) => x.AreaCode == areaCode);
		}

		
		public bool IsLastSafeZoneActive()
		{
			return this.areaStateList.Count == this.areaStateList.Count((Area x) => x.AreaRestrictionState == AreaRestrictionState.Restricted);
		}

		
		private GraphTraveler<int> areaTraveler;

		
		private readonly List<Area> areaStateList = new List<Area>();

		
		private readonly List<AreaData> areaDataList = new List<AreaData>();

		
		private int restrictedAreaMask;

		
		private int currentRestrictionCount;

		
		private Coroutine coAreaRestriction;

		
		private Coroutine coRestrictedAreaDOT;

		
		private Coroutine coCheckFirstVisitArea;

		
		private WorldSecurityConsole lastSafeConsole;

		
		private DayNight dayNight;

		
		private bool isOnAcceleration;

		
		private int day;

		
		private bool isStopAreaRestriction;

		
		private int nextUpdateRestrictedAreaSeq;

		
		private int accelerationCount;

		
		private readonly WaitForFrameUpdate waitForFrameUpdate_1 = new WaitForFrameUpdate();

		
		private readonly WaitForFrameUpdate waitForFrameUpdate_2 = new WaitForFrameUpdate();

		
		private readonly List<Area> cacheNormalArea = new List<Area>();

		
		private readonly List<Area> cacheRestrictedAreas = new List<Area>();

		
		private readonly List<Area> cacheReservedAreas = new List<Area>();

		
		private readonly List<Area> cacheNormalAreas = new List<Area>();

		
		private readonly List<Area> cacheGetAreasByStateList = new List<Area>();

		
		private readonly List<Area> cacheGetAreaList = new List<Area>();

		
		private readonly List<AreaData> cacheGetAreaDataList = new List<AreaData>();
	}
}
