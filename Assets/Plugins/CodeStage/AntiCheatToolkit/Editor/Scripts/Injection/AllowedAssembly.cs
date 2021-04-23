#region copyright
// --------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [http://codestage.net]
// --------------------------------------------------------------
#endregion

namespace CodeStage.AntiCheat.EditorCode
{
	using System;

	[Serializable]
	public class AllowedAssembly
	{
		public string name;
		public int[] hashes;

		public AllowedAssembly(string name, int[] hashes)
		{
			this.name = name;
			this.hashes = hashes;
		}

		public bool AddHash(int hash)
		{
			if (Array.IndexOf(hashes, hash) != -1) return false;

			var oldLen = hashes.Length;
			var newLen = oldLen + 1;

			var newHashesArray = new int[newLen];
			Array.Copy(hashes, newHashesArray, oldLen);

			hashes = newHashesArray;
			hashes[oldLen] = hash;

			return true;
		}

		public override string ToString()
		{
			return name + " (hashes: " + hashes.Length + ")";
		}
	}
}