using System;
using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class LocalPingTarget : MonoBehaviour
	{
		private readonly WaitForSeconds waitPingOff = new WaitForSeconds(6f);
		private List<LocalMarkInfo> markInfos;


		private List<LocalPingInfo> pingInfos;

		private void Awake()
		{
			markInfos = new List<LocalMarkInfo>
			{
				null,
				null,
				null,
				null,
				null
			};
			pingInfos = new List<LocalPingInfo>();
		}


		private GameObject GetPingObject(PingType type)
		{
			switch (type)
			{
				case PingType.Run:
					return SingletonMonoBehaviour<ResourceManager>.inst.LoadEffect("Fx_BI_PingRun_01");
				case PingType.Warning:
					return SingletonMonoBehaviour<ResourceManager>.inst.LoadEffect("Fx_BI_PingWarning_01");
				case PingType.Escape:
					return SingletonMonoBehaviour<ResourceManager>.inst.LoadEffect("Fx_BI_PingEscape_01");
				case PingType.Help:
					return SingletonMonoBehaviour<ResourceManager>.inst.LoadEffect("Fx_BI_PingHelp_01");
				case PingType.Target:
				{
					GameObject gameObject = SingletonMonoBehaviour<ResourceManager>.inst.LoadPointPinPrefab();
					gameObject.GetComponent<Image>().sprite =
						SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Ico_Ping_Target");
					return gameObject;
				}
				case PingType.Select:
					return SingletonMonoBehaviour<ResourceManager>.inst.LoadEffect("Fx_BI_PingSelect_01");
				default:
					return SingletonMonoBehaviour<ResourceManager>.inst.LoadEffect("Fx_BI_PingSelect_01");
			}
		}


		private GameObject GetMarkObject(int teamSlot)
		{
			if (teamSlot - 1 <= 2)
			{
				return SingletonMonoBehaviour<ResourceManager>.inst.LoadEffect(string.Format("Fx_BI_PinMark_{0:D2}",
					teamSlot));
			}

			return SingletonMonoBehaviour<ResourceManager>.inst.LoadEffect("Fx_BI_PinMark_01");
		}


		public void UpdateMarkTarget(BlisVector[] updateMarks)
		{
			if (updateMarks == null)
			{
				return;
			}

			for (int i = 0; i < markInfos.Count; i++)
			{
				if (markInfos[i] != null && updateMarks.Length > i &&
				    (updateMarks[i] == null || updateMarks[i].ToVector3() != markInfos[i].keyVector))
				{
					markInfos[i].Delete();
					markInfos[i] = null;
				}
			}

			for (int j = 0; j < markInfos.Count; j++)
			{
				if (markInfos[j] == null && updateMarks.Length > j && updateMarks[j] != null)
				{
					GameObject obj = Instantiate<GameObject>(GetMarkObject(j), updateMarks[j].ToVector3(),
						Quaternion.identity);
					GameObject minimapObj =
						MonoBehaviourInstance<GameUI>.inst.Minimap.UIMap.AddMark(j, updateMarks[j].ToVector3());
					GameObject mapwindowObj =
						MonoBehaviourInstance<GameUI>.inst.MapWindow.UIMap.AddMark(j, updateMarks[j].ToVector3());
					markInfos[j] = new LocalMarkInfo(obj, minimapObj, mapwindowObj, updateMarks[j].ToVector3(), j);
				}
			}
		}


		public void AddPingTarget(PingType type, int pingObjectId, Vector3 pingPosition, int senderObjectId)
		{
			GameObject gameObject = Instantiate<GameObject>(GetPingObject(type), pingPosition, Quaternion.identity);
			GameObject minimapObj =
				MonoBehaviourInstance<GameUI>.inst.Minimap.UIMap.AddPing(type, pingPosition, senderObjectId);
			GameObject mapwindowObj =
				MonoBehaviourInstance<GameUI>.inst.MapWindow.UIMap.AddPing(type, pingPosition, senderObjectId);
			if (type == PingType.Target)
			{
				LocalObject localObject =
					MonoBehaviourInstance<ClientService>.inst.World.Find<LocalObject>(pingObjectId);
				if (localObject is LocalMonster)
				{
					LocalMonster localMonster = localObject as LocalMonster;
					if (localMonster.FloatingUi != null && localMonster.FloatingUi.StatusBar != null)
					{
						gameObject.transform.SetParent(localMonster.FloatingUi.StatusBar.PingAnchor);
						gameObject.transform.localScale = Vector3.one;
						gameObject.transform.localPosition = Vector3.zero;
						gameObject.GetComponent<Image>().SetNativeSize();
					}
				}
				else if (localObject is LocalPlayerCharacter)
				{
					LocalPlayerCharacter localPlayerCharacter = localObject as LocalPlayerCharacter;
					if (localPlayerCharacter.FloatingUi != null && localPlayerCharacter.FloatingUi.StatusBar != null)
					{
						gameObject.transform.SetParent(localPlayerCharacter.FloatingUi.StatusBar.PingAnchor);
						gameObject.transform.localScale = Vector3.one;
						gameObject.transform.localPosition = Vector3.zero;
						gameObject.GetComponent<Image>().SetNativeSize();
					}
				}
			}

			LocalPingInfo localPingInfo =
				new LocalPingInfo(gameObject, minimapObj, mapwindowObj, pingPosition, type, senderObjectId);
			pingInfos.Add(localPingInfo);
			this.StartThrowingCoroutine(PingOff(localPingInfo),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][PingOff] Message:" + exception.Message + ", StackTrace:" + exception.StackTrace);
				});
		}


		private IEnumerator PingOff(LocalPingInfo info)
		{
			yield return waitPingOff;
			info.Delete();
			pingInfos.Remove(info);
		}


		public void UpdatePing()
		{
			markInfos.ForEach(delegate(LocalMarkInfo x)
			{
				if (x != null)
				{
					x.OnSwitch();
				}
			});
			pingInfos.ForEach(delegate(LocalPingInfo x)
			{
				if (x != null)
				{
					x.OnSwitch();
				}
			});
		}


		public class LocalMarkInfo
		{
			public GameObject fieldMark;


			public Vector3 keyVector;


			public GameObject mapwindowPing;


			public GameObject minimapPing;


			public int ownerObjectId;


			public LocalMarkInfo(GameObject obj, GameObject minimapObj, GameObject mapwindowObj, Vector3 pos,
				int teamSlotIndex)
			{
				fieldMark = obj;
				minimapPing = minimapObj;
				mapwindowPing = mapwindowObj;
				keyVector = pos;
				List<PlayerContext> teamMember =
					MonoBehaviourInstance<ClientService>.inst.GetTeamMember(MonoBehaviourInstance<ClientService>.inst
						.MyTeamNumber);
				for (int i = 0; i < teamMember.Count; i++)
				{
					if (teamMember[i].Character.TeamSlot == teamSlotIndex)
					{
						ownerObjectId = teamMember[i].Character.ObjectId;
						break;
					}
				}

				OnSwitch(true);
			}

			public void Delete()
			{
				if (fieldMark != null)
				{
					Destroy(fieldMark);
				}

				if (minimapPing != null)
				{
					Destroy(minimapPing);
				}

				if (mapwindowPing != null)
				{
					Destroy(mapwindowPing);
				}
			}


			public void OnSwitch(bool isInit = false)
			{
				bool flag = !MonoBehaviourInstance<ClientService>.inst.IgnoreTargetService.IsIgnoreUser(IgnoreType.Ping,
					ownerObjectId);
				if (fieldMark != null)
				{
					fieldMark.SetActive(flag);
				}

				if (minimapPing != null)
				{
					minimapPing.SetActive(flag);
				}

				if (mapwindowPing != null)
				{
					mapwindowPing.SetActive(flag);
				}

				if (flag && isInit)
				{
					Singleton<SoundControl>.inst.PlayUISound("Mark_Select");
				}
			}
		}


		public class LocalPingInfo
		{
			public GameObject fieldMark;


			public GameObject mapwindowMark;


			public GameObject minimapMark;


			public int ownerObjectId;


			public Vector3 position;


			public PingType type;

			public LocalPingInfo(GameObject obj, GameObject minimapObj, GameObject mapwindowObj, Vector3 pos,
				PingType type, int senderObjectId)
			{
				fieldMark = obj;
				minimapMark = minimapObj;
				mapwindowMark = mapwindowObj;
				position = pos;
				this.type = type;
				ownerObjectId = senderObjectId;
				OnSwitch(true);
			}


			public void Delete()
			{
				if (fieldMark)
				{
					Destroy(fieldMark);
				}

				if (minimapMark)
				{
					Destroy(minimapMark);
				}

				if (mapwindowMark)
				{
					Destroy(mapwindowMark);
				}
			}


			public void OnSwitch(bool isInit = false)
			{
				bool flag = !MonoBehaviourInstance<ClientService>.inst.IgnoreTargetService.IsIgnoreUser(IgnoreType.Ping,
					ownerObjectId);
				if (fieldMark != null)
				{
					fieldMark.SetActive(flag);
				}

				if (minimapMark != null)
				{
					minimapMark.SetActive(flag);
				}

				if (mapwindowMark != null)
				{
					mapwindowMark.SetActive(flag);
				}

				if (flag && isInit)
				{
					Singleton<SoundControl>.inst.PlayUISound("Ping" + type);
				}
			}
		}
	}
}