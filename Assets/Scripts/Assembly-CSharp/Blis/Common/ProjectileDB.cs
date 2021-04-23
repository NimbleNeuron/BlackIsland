using System;
using System.Collections.Generic;
using System.Linq;

namespace Blis.Common
{
	public class ProjectileDB
	{
		private List<HookLineProjectileData> hookLineProjectileList = new List<HookLineProjectileData>();


		private List<ProjectileData> projectileList = new List<ProjectileData>();

		public void SetData<T>(List<T> data)
		{
			Type typeFromHandle = typeof(T);
			if (typeFromHandle == typeof(ProjectileData))
			{
				projectileList = data.Cast<ProjectileData>().ToList<ProjectileData>();
				return;
			}

			if (typeFromHandle == typeof(HookLineProjectileData))
			{
				hookLineProjectileList = data.Cast<HookLineProjectileData>().ToList<HookLineProjectileData>();
			}
		}


		public ProjectileData GetData(int code)
		{
			ProjectileData projectileData = projectileList.Find(x => x.code == code);
			if (projectileData == null)
			{
				Log.E(string.Format("Not Found ProjectileData ({0})", code));
			}

			return projectileData;
		}


		public HookLineProjectileData GetHookLineData(int code)
		{
			HookLineProjectileData hookLineProjectileData = hookLineProjectileList.Find(x => x.code == code);
			if (hookLineProjectileData == null)
			{
				Log.E(string.Format("Not Found HookLineProjectileData ({0})", code));
			}

			return hookLineProjectileData;
		}
	}
}