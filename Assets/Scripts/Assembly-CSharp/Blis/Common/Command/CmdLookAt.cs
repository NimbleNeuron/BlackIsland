using Blis.Client;
using MessagePack;
using UnityEngine;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdLookAt, false)]
	public class CmdLookAt : LocalObjectCommandPacket
	{
		[Key(3)] public BlisFixedPoint duration;


		[Key(1)] public BlisFixedPoint lookAtFromY;


		[Key(2)] public BlisFixedPoint lookAtToY;


		public override void Action(ClientService service, LocalObject self)
		{
			self.LookAt(Quaternion.Euler(new Vector3(0f, lookAtFromY.Value, 0f)),
				Quaternion.Euler(new Vector3(0f, lookAtToY.Value, 0f)), duration.Value);
		}
	}
}