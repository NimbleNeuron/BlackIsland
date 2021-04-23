using Blis.Common;

namespace BIFog
{
	
	public class FogProjectileHider : FogHiderOnCenter
	{
		
		public void SetProjectileType(ProjectileType projectileType)
		{
			this.projectileType = projectileType;
		}

		
		protected override void OnOutSight()
		{
			if (this.projectileType == ProjectileType.Around)
			{
				base.OnOutSight();
			}
		}

		
		private ProjectileType projectileType;
	}
}
