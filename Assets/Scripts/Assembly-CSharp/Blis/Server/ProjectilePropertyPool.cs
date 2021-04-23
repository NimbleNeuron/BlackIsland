namespace Blis.Server
{
	
	public class ProjectilePropertyPool : WorldObjectDataPool<ProjectileProperty>
	{
		
		public ProjectilePropertyPool()
		{
			base.AllocPool(100);
		}

		
		public ProjectileProperty Pop(SkillAgent owner, int code, SkillUseInfo skillUseInfo)
		{
			ProjectileProperty projectileProperty = base.Pop();
			if (projectileProperty == null)
			{
				return projectileProperty;
			}
			projectileProperty.Set(owner, code, skillUseInfo);
			return projectileProperty;
		}

		
		public override void Push(ProjectileProperty property)
		{
			property.Clear();
			base.Push(property);
		}
	}
}
