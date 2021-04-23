using Blis.Common;

namespace Blis.Server
{
	
	[ObjectAttr(ObjectType.Dummy)]
	public class WorldDummy : WorldMovableCharacter
	{
		
		protected override ObjectType GetObjectType()
		{
			return ObjectType.Dummy;
		}

		
		protected override int GetTeamNumber()
		{
			return 0;
		}

		
		protected override int GetCharacterCode()
		{
			return 0;
		}

		
		protected override HostileAgent GetHostileAgent()
		{
			return this.hostileAgent;
		}

		
		public virtual void Init(int prefabNo)
		{
			this.prefabNo = prefabNo;
			CharacterStat characterStat = new CharacterStat();
			characterStat.InitDummyStat();
			base.Init(characterStat);
			base.name = "Dummy";
			this.hostileAgent = new DummyHostileAgent(this);
			this.mySkillAgent = new WorldMovableCharacterSkillAgent(this);
			base.InitCharacterSkill(1, SpecialSkillId.None);
		}

		
		public override byte[] CreateSnapshot()
		{
			return WorldObject.serializer.Serialize<DummySnapshot>(new DummySnapshot
			{
				prefabNo = this.prefabNo,
				statusSnapshot = WorldObject.serializer.Serialize<CharacterStatusSnapshot>(new CharacterStatusSnapshot(base.Status)),
				initialStat = base.Stat.CreateSnapshot(),
				initialStateEffect = base.StateEffector.CreateSnapshot(),
				skillController = base.SkillController.CreateSnapshot(),
				moveAgentSnapshot = this.moveAgent.CreateSnapshot(),
				isInCombat = this.IsInCombat,
				isInvisible = base.IsInvisible
			});
		}

		
		private DummyHostileAgent hostileAgent;

		
		private int prefabNo;
	}
}
