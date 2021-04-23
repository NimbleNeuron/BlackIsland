using MessagePack;
using UnityEngine;

namespace Blis.Common
{
	[MessagePackObject]
	public class SnapshotWrapper
	{
		[IgnoreMember] protected const int LAST_KEY_IDX = 4;


		[Key(1)] public int objectId;


		[Key(0)] public ObjectType objectType;


		[Key(2)] public Vector3 position;


		[Key(3)] public Quaternion rotation;


		[Key(4)] public byte[] snapshot;
	}
}