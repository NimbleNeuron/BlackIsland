using Blis.Common;

namespace Blis.Client
{
	public class WicklinePool : ObjectPool
	{
		public override void InitPool()
		{
			MonsterData monsterData = GameDB.monster.GetMonsterData(MonsterType.Wickline);
			AllocPool(1, SingletonMonoBehaviour<ResourceManager>.inst.LoadMonster(monsterData.resource));
		}
	}
}