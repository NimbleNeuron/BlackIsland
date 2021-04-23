using System.Collections.Generic;
using Blis.Common;
using MessagePack;


[MessagePackObject()]
public class CharacterSnapshot
{
	
	[IgnoreMember] protected const int LAST_KEY_IDX = 6;

	
	[Key(1)] public List<CharacterStatValue> initialStat;

	
	[Key(2)] public List<CharacterStateValue> initialStateEffect;

	
	[Key(5)] public bool isInCombat;

	
	[Key(6)] public bool isInvisible;

	
	[Key(4)] public MoveAgentSnapshot moveAgentSnapshot;

	
	[Key(3)] public SkillControllerSnapshot skillController;

	
	[Key(0)] public byte[] statusSnapshot;
}