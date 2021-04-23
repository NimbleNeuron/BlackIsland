namespace Blis.Server
{
	
	public class DeployProperty
	{
		
		public readonly BuildPhase buildPhase;

		
		public readonly DeploySetting deploySetting;

		
		public readonly MatchingRegion matchingRegion;

		
		public readonly string redisHost;

		
		public readonly string redisReadOnlyHost;

		
		public readonly string restRootUrl;

		
		public DeployProperty(DeploySetting deploySetting, BuildPhase buildPhase, MatchingRegion region)
		{
			this.deploySetting = deploySetting;
			this.buildPhase = buildPhase;
			matchingRegion = region;
			redisHost = buildPhase.getRedisHost();
			redisReadOnlyHost = buildPhase.getRedisReadOnlyHost();
			restRootUrl = buildPhase.getRestRootUrl();
		}
	}
}