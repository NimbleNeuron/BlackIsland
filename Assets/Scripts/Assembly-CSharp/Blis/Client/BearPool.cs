using Blis.Common;

namespace Blis.Client
{
	public class BearPool : ObjectPool
	{
		public override void InitPool()
		{
			MonsterData monsterData = GameDB.monster.GetMonsterData(MonsterType.Bear);
			AllocPool(15, SingletonMonoBehaviour<ResourceManager>.inst.LoadMonster(monsterData.resource));
		}
	}
}