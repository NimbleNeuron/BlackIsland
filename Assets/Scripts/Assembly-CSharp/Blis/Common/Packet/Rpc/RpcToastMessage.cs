using Blis.Client;
using Blis.Common.Utils;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.RpcToastMessage, false)]
	public class RpcToastMessage : RpcPacket
	{
		[Key(0)] public ToastMessageType toastMessageType;


		public override void Action(ClientService service)
		{
			string toastMessage = LnUtil.GetToastMessage(toastMessageType);
			MonoBehaviourInstance<GameUI>.inst.ToastMessage.ShowMessage(toastMessage);
		}
	}
}