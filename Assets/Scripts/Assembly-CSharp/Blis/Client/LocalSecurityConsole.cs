using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BIOutline;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	[ObjectAttr(ObjectType.SecurityConsole)]
	public class LocalSecurityConsole : LocalObject
	{
		private readonly Dictionary<int, float> checkoutTime = new Dictionary<int, float>();
		private Animator animator;


		private int areaCode;


		private CubeColliderAgent colliderAgent;


		private bool isLastSafeConsole;


		private SelectionRenderer selectionRenderer;


		public int AreaCode => areaCode;

		protected override ObjectType GetObjectType()
		{
			return ObjectType.SecurityConsole;
		}


		protected override int GetTeamNumber()
		{
			return 0;
		}


		protected override ColliderAgent GetColliderAgent()
		{
			return colliderAgent;
		}


		public void Init(int areaCode)
		{
			this.areaCode = areaCode;
			GameUtil.Bind<SelectionRenderer>(gameObject, ref selectionRenderer);
			selectionRenderer.SetColor(SingletonMonoBehaviour<PlayerController>.inst.MouseOverManager.InSightColor);
			selectionRenderer.SetUntouched(true);
			GameUtil.BindOrAdd<CubeColliderAgent>(gameObject, ref colliderAgent);
			colliderAgent.Init(1f);
			animator = GetComponentInChildren<Animator>();
			foreach (LocalSecurityCamera localSecurityCamera in MonoBehaviourInstance<ClientService>.inst.World
				.FindAll<LocalSecurityCamera>(x => x.AreaCode == this.areaCode))
			{
				localSecurityCamera.ConnectConsole(this);
			}

			AttachPickable(gameObject).Init(this);
			MonoBehaviourInstance<GameUI>.inst.Minimap.UIMap.CreateStaticObject(objectId, GetPosition(),
				SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Ico_Map_SecurityConsole"),
				MiniMapIconType.System);
			this.StartThrowingCoroutine(UpdateCheckOutDic(),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][UpdateCheckOutDic] Message:" + exception.Message + ", StackTrace:" +
					      exception.StackTrace);
				});
		}


		public override string GetLocalizedName(bool includeColor)
		{
			return Ln.Get("보안콘솔");
		}


		protected override bool GetIsOutSight()
		{
			return false;
		}


		public override void Init(byte[] snapshotData) { }


		public bool IsCheckoutPlayer(int playerId)
		{
			return checkoutTime.ContainsKey(playerId);
		}


		public int GetCheckoutCount()
		{
			return checkoutTime.Count;
		}


		private IEnumerator UpdateCheckOutDic()
		{
			for (;;)
			{
				foreach (int key in checkoutTime.Keys.ToList<int>())
				{
					if (Time.time - checkoutTime[key] >= 180f)
					{
						checkoutTime.Remove(key);
					}
				}

				yield return new WaitForSeconds(1f);
			}
		}


		public void CheckoutConsole(int playerId)
		{
			checkoutTime[playerId] = Time.time;
		}


		public void ConsoleAnimation(SecurityConsoleEvent eventType)
		{
			SecurityConsoleEventData securityConsoleEventData =
				GameDB.effectAndSound.GetSecurityConsoleEventData(eventType);
			animator.SetTrigger(securityConsoleEventData.objectAnimation);
		}


		public void ActiveLastSafeConsole(bool enable)
		{
			if (enable)
			{
				isLastSafeConsole = true;
				MonoBehaviourInstance<EnvironmentEffectManager>.inst.InvokeEvent<LastSafeZoneEffect>(GetPosition(), 1f,
					"show");
				MonoBehaviourInstance<GameUI>.inst.Minimap.UIMap.CreateStaticObject(-1, GetPosition(),
					SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Ico_Map_SafeArea"),
					MiniMapIconType.System);
				return;
			}

			isLastSafeConsole = false;
			MonoBehaviourInstance<EnvironmentEffectManager>.inst.InvokeEvent<LastSafeZoneEffect>(GetPosition(), 1f,
				"hide");
			MonoBehaviourInstance<GameUI>.inst.Minimap.UIMap.CreateStaticObject(objectId, GetPosition(),
				SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Ico_Map_SecurityConsole"),
				MiniMapIconType.System);
		}


		public bool IsLastSafeConsole()
		{
			return isLastSafeConsole;
		}


		public bool IsNear(Vector3 worldPosition)
		{
			return new Bounds(GetPosition(), GameConstants.InGame.LAST_SAFE_ZONE_SIZE).Contains(worldPosition);
		}


		public override bool IsMouseHitPossible(LocalSightAgent targetSightAgent, bool isInvisible)
		{
			return true;
		}


		public override bool SetCursor(LocalPlayerCharacter myPlayer)
		{
			MonoBehaviourInstance<GameInput>.inst.GameCursor.SetCursorTarget(CursorTarget.SecurityConsole);
			return true;
		}
	}
}