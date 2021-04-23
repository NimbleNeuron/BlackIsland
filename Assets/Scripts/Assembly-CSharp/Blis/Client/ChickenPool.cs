using Blis.Common;

namespace Blis.Client
{
	public class ChickenPool : ObjectPool
	{
		public override void InitPool()
		{
			MonsterData monsterData = GameDB.monster.GetMonsterData(MonsterType.Chicken);
			AllocPool(47, SingletonMonoBehaviour<ResourceManager>.inst.LoadMonster(monsterData.resource));
		}
	}
}