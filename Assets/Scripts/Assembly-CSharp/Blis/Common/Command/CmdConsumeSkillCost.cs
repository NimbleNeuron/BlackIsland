using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.CmdConsumeSkillCost, false)]
	public class CmdConsumeSkillCost : LocalPlayerCharacterCommandPacket
	{
		
		[Key(1)] public int consumeValue;

		
		[Key(2)] public SkillCostType costType;

		
		public override void Action(ClientService service, LocalPlayerCharacter self)
		{
			if (costType == SkillCostType.Hp)
			{
				self.ConsumeHp(consumeValue);
				return;
			}

			if (costType == SkillCostType.Sp)
			{
				self.ConsumeSp(consumeValue);
				return;
			}

			if (costType == SkillCostType.Ep)
			{
				self.ConsumeEp(consumeValue);
			}
		}
	}
}