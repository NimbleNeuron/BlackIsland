using System.Collections.Generic;

namespace Blis.Server
{
	
	public class DeployPropertyManager : Singleton<DeployPropertyManager>
	{
		
		private readonly Dictionary<DeploySetting, DeployProperty> properties =
			new Dictionary<DeploySetting, DeployProperty>();

		
		public DeployPropertyManager()
		{
			Add(DeploySetting.Local, BuildPhase.Local, MatchingRegion.Dev);
			Add(DeploySetting.Dev, BuildPhase.Dev, MatchingRegion.Dev);
			Add(DeploySetting.Alpha, BuildPhase.Alpha, MatchingRegion.Dev);
			Add(DeploySetting.QA, BuildPhase.QA, MatchingRegion.QA);
			Add(DeploySetting.Docker, BuildPhase.Local, MatchingRegion.Dev);
			Add(DeploySetting.Staging, BuildPhase.Staging, MatchingRegion.Staging);
			Add(DeploySetting.Seoul, BuildPhase.Release, MatchingRegion.Seoul);
			Add(DeploySetting.Ohio, BuildPhase.Release, MatchingRegion.Ohio);
			Add(DeploySetting.Frankfurt, BuildPhase.Release, MatchingRegion.Frankfurt);
			Add(DeploySetting.SaoPaulo, BuildPhase.Release, MatchingRegion.SaoPaulo);
			Add(DeploySetting.Singapore, BuildPhase.Release, MatchingRegion.Singapore);
			Add(DeploySetting.HongKong, BuildPhase.Release, MatchingRegion.HongKong);
		}

		
		private void Add(DeploySetting setting, BuildPhase phase, MatchingRegion region)
		{
			properties.Add(setting, new DeployProperty(setting, phase, region));
		}

		
		public DeployProperty Get(DeploySetting deploySetting)
		{
			DeployProperty result;
			if (properties.TryGetValue(deploySetting, out result))
			{
				return result;
			}

			return new DeployProperty(DeploySetting.Local, BuildPhase.Local, MatchingRegion.Dev);
		}
	}
}