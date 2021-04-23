using Newtonsoft.Json;

namespace Blis.Common
{
	
	public class HookLineProjectileData
	{
		
		[JsonConstructor]
		public HookLineProjectileData(int code, string fireLinePrefab, string connectionLinePrefab, string casterLineStickObjectName, string targetLineStickObjectName, float connectionDuration, float connectionMaxRange, string connectionEffectPrefab, string connectionSound, string StretchObjectName)
		{
			this.code = code;
			this.fireLinePrefab = fireLinePrefab;
			this.connectionLinePrefab = connectionLinePrefab;
			this.casterLineStickObjectName = casterLineStickObjectName;
			this.targetLineStickObjectName = targetLineStickObjectName;
			this.connectionDuration = connectionDuration;
			this.connectionMaxRange = connectionMaxRange;
			this.connectionEffectPrefab = connectionEffectPrefab;
			this.connectionSound = connectionSound;
			this.StretchObjectName = StretchObjectName;
		}

		
		public readonly int code;

		
		public readonly string fireLinePrefab;

		
		public readonly string connectionLinePrefab;

		
		public readonly string casterLineStickObjectName;

		
		public readonly string targetLineStickObjectName;

		
		public readonly float connectionDuration;

		
		public readonly float connectionMaxRange;

		
		public readonly string connectionEffectPrefab;

		
		public readonly string connectionSound;

		
		public readonly string StretchObjectName;
	}
}
