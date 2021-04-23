using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdUpdateMoveSpeed, false)]
	public class CmdUpdateMoveSpeed : LocalMovableCharacterCommandPacket
	{
		[Key(1)] public BlisFixedPoint moveSpeed;


		public override void Action(ClientService service, LocalMovableCharacter self)
		{
			self.UpdateMoveSpeed(moveSpeed.Value);
			if (service.MyObjectId == self.ObjectId)
			{
				service.MyPlayer.OnUpdateStat();
			}
		}
	}
}