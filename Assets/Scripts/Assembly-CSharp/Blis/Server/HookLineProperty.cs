using System;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	public class HookLineProperty : ProjectileProperty
	{
		
		private readonly HookLineProjectileData hookLineProjectileData;

		
		private bool isImmediatelyLinkHookLine;

		
		private bool isLinkPoint;

		
		private WorldCharacter linkFromCharacter;

		
		private WorldCharacter linkToCharacter;

		
		private Vector3? linkToPoint;

		
		private Action<WorldCharacter, WorldHookLineProjectile> onDisconnectionRangeOut;

		
		private Action<WorldCharacter, WorldHookLineProjectile> onDisconnectionTimeOut;

		
		public HookLineProperty(SkillAgent owner, int projectileCode, int hookLineCode, SkillUseInfo skillUseInfo) :
			base(owner, projectileCode, skillUseInfo)
		{
			hookLineProjectileData = GameDB.projectile.GetHookLineData(hookLineCode);
		}

		
		
		public int HookLineCode => hookLineProjectileData.code;

		
		
		public float LinkedDistance => hookLineProjectileData.connectionMaxRange;

		
		
		public float LinkedDuration => hookLineProjectileData.connectionDuration;

		
		
		public WorldCharacter LinkFromCharacter => linkFromCharacter;

		
		
		public WorldCharacter LinkToCharacter => linkToCharacter;

		
		
		public Vector3? LinkToPoint => linkToPoint;

		
		
		public bool IsLinkPoint => isLinkPoint;

		
		
		public bool IsImmediatelyLinkHookLine => isImmediatelyLinkHookLine;

		
		
		public Action<WorldCharacter, WorldHookLineProjectile> OnDisconnectionRangeOut => onDisconnectionRangeOut;

		
		
		public Action<WorldCharacter, WorldHookLineProjectile> OnDisconnectionTimeOut => onDisconnectionTimeOut;

		
		public void SetDisconnectionRangeOutAction(Action<WorldCharacter, WorldHookLineProjectile> action)
		{
			onDisconnectionRangeOut = action;
		}

		
		public void SetDisconnectionTimeOutAction(Action<WorldCharacter, WorldHookLineProjectile> action)
		{
			onDisconnectionTimeOut = action;
		}

		
		public void SetLinkFromCharacter(WorldCharacter linkFromCharacter)
		{
			this.linkFromCharacter = linkFromCharacter;
		}

		
		public void SetLinkToCharacter(WorldCharacter linkToCharacter)
		{
			this.linkToCharacter = linkToCharacter;
			isImmediatelyLinkHookLine = true;
		}

		
		public void SetLinkToPosition(Vector3 linkToPoint)
		{
			this.linkToPoint = linkToPoint;
			isLinkPoint = true;
			isImmediatelyLinkHookLine = true;
		}
	}
}