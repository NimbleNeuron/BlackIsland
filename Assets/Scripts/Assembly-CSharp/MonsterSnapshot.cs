using MessagePack;


[MessagePackObject()]
public class MonsterSnapshot : CharacterSnapshot
{
	
	[Key(7)] public int monsterCode;
}