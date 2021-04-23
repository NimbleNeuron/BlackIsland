using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdMasteryLevelUp, false)]
	public class CmdMasteryLevelUp : LocalPlayerCharacterCommandPacket
	{
		[Key(2)] public int level;


		[Key(1)] public MasteryType masteryType;


		public override void Action(ClientService service, LocalPlayerCharacter self)
		{
			self.MasteryLevelUp(masteryType, level);
			if (service.MyObjectId == self.ObjectId)
			{
				service.MyPlayer.OnMasteryLevelUp(masteryType);
			}
		}
	}
}