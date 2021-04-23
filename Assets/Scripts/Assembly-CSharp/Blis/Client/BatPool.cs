using Blis.Common;

namespace Blis.Client
{
	public class BatPool : ObjectPool
	{
		public override void InitPool()
		{
			MonsterData monsterData = GameDB.monster.GetMonsterData(MonsterType.Bat);
			AllocPool(29, SingletonMonoBehaviour<ResourceManager>.inst.LoadMonster(monsterData.resource));
		}
	}
}