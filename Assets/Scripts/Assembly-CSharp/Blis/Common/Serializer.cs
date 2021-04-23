using MessagePack.Resolvers;
using MessagePack.Unity;
using MessagePack.Unity.Extension;

namespace Blis.Common
{
	public static class Serializer
	{
		static Serializer()
		{
			CompositeResolver.Create(BlisFormatResolver.Instance, UnityResolver.Instance,
				UnityBlitWithPrimitiveArrayResolver.Instance, StandardResolver.Instance);
			Compression = new LZ4MsgPackSerializer();
			Default = new MsgPackSerializer();
		}


		public static ISerializer Compression { get; }


		public static ISerializer Default { get; }
	}
}