using Newtonsoft.Json;

namespace Blis.Common
{
	public class BulletCapacity
	{
		public readonly int capacity;


		public readonly int itemCode;


		public readonly ReloadType loadType;


		public int count;


		public int initCount;


		public float time;

		[JsonConstructor]
		public BulletCapacity(int itemCode, int capacity, ReloadType loadType, float time, int initCount, int count)
		{
			this.itemCode = itemCode;
			this.capacity = capacity;
			this.loadType = loadType;
			this.time = time;
			this.initCount = initCount;
			this.count = count;
		}
	}
}