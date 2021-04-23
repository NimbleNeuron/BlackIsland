using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[ObjectAttr(ObjectType.SecurityConsole)]
	public class WorldSecurityConsole : WorldObject
	{
		
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
			return this.colliderAgent;
		}

		
		
		public int AreaCode
		{
			get
			{
				return this.areaCode;
			}
		}

		
		public void Init()
		{
			GameUtil.BindOrAdd<CubeColliderAgent>(base.gameObject, ref this.colliderAgent);
			this.colliderAgent.Init(1f);
			AreaData currentAreaData = AreaUtil.GetCurrentAreaData(MonoBehaviourInstance<GameService>.inst.CurrentLevel, base.transform.position, 2147483640);
			this.areaCode = currentAreaData.code;
		}

		
		protected override SkillAgent GetSkillAgent()
		{
			return null;
		}

		
		public override byte[] CreateSnapshot()
		{
			return null;
		}

		
		public void CheckoutConsole(int characterId)
		{
			this.checkoutTime[characterId] = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
			base.EnqueueCommand(new CmdConsoleCheckOut
			{
				playerCharacterId = characterId
			});
		}

		
		public void ActiveSafeArea(bool enable)
		{
			base.EnqueueCommand(new CmdActiveConsoleSafeArea
			{
				enable = enable
			});
		}

		
		public void CastingConsoleAction(SecurityConsoleEvent eventType)
		{
			base.EnqueueCommand(new CmdConsoleAction
			{
				eventType = eventType
			});
		}

		
		public void ExecuteConsoleAction(WorldPlayerCharacter user, SecurityConsoleEvent eventType)
		{
			GameDB.effectAndSound.GetSecurityConsoleEventData(eventType);
			if (eventType == SecurityConsoleEvent.AreaSecurityCameraSight)
			{
				this.CheckoutConsole(user.ObjectId);
				using (IEnumerator<WorldSecurityCamera> enumerator = MonoBehaviourInstance<GameService>.inst.Area.GetSecurityCameras(this.areaCode).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						WorldSecurityCamera target = enumerator.Current;
						user.AttachSight(target, 10f, 180f, true, false);
					}
					return;
				}
			}
			throw new GameException(ErrorType.InvalidAction);
		}

		
		public void CancelConsoleAction()
		{
			base.EnqueueCommand(new CmdCancelConsoleAction());
		}

		
		public bool IsNear(Vector3 worldPosition)
		{
			return new Bounds(base.GetPosition(), GameConstants.InGame.LAST_SAFE_ZONE_SIZE).Contains(worldPosition);
		}

		
		private CubeColliderAgent colliderAgent;

		
		private int areaCode;

		
		private Dictionary<int, float> checkoutTime = new Dictionary<int, float>();
	}
}
