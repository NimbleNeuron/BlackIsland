using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Server
{
	
	[ObjectAttr(ObjectType.SecurityCamera)]
	public class WorldSecurityCamera : WorldObject
	{
		
		protected override ObjectType GetObjectType()
		{
			return ObjectType.SecurityCamera;
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
			this.colliderAgent.Init(0.5f);
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

		
		private CubeColliderAgent colliderAgent;

		
		private int areaCode;
	}
}
