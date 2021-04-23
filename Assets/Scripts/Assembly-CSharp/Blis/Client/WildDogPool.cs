using Blis.Common;

namespace Blis.Client
{
	public class WildDogPool : ObjectPool
	{
		public override void InitPool()
		{
			MonsterData monsterData = GameDB.monster.GetMonsterData(MonsterType.WildDog);
			AllocPool(25, SingletonMonoBehaviour<ResourceManager>.inst.LoadMonster(monsterData.resource));
		}
	}
}