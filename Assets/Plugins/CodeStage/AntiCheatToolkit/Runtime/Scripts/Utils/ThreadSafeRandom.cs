#region copyright
// --------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [http://codestage.net]
// --------------------------------------------------------------
#endregion

namespace CodeStage.AntiCheat.Utils
{
	using System;

	internal class ThreadSafeRandom
	{
		private static readonly Random Global = new Random();

		[ThreadStatic]
		private static Random local;

		public static int Next(int minInclusive, int maxExclusive)
		{
			var inst = local;

			if (inst != null)
			{
				return inst.Next(minInclusive, maxExclusive);
			}

			int seed;

			lock (Global)
			{
				seed = Global.Next();
			}

			local = inst = new Random(seed);
			return inst.Next(minInclusive, maxExclusive);
		}
		
		public static long NextLong(long minInclusive, long maxExclusive)
		{
			var inst = local;

			if (inst != null)
			{
				return NextLong(inst, minInclusive, maxExclusive);
			}

			int seed;

			lock (Global)
			{
				seed = Global.Next();
			}

			local = inst = new Random(seed);
			return NextLong(inst, minInclusive, maxExclusive);
		}
		
		public static void NextBytes(byte[] buffer)
		{
			var inst = local;

			if (inst != null)
			{
				inst.NextBytes(buffer);
				return;
			}

			int seed;

			lock (Global)
			{
				seed = Global.Next();
			}

			local = inst = new Random(seed);
			inst.NextBytes(buffer);
		}

		public static void NextChars(char[] buffer)
		{
			var inst = local;

			if (inst != null)
			{
				NextChars(inst, buffer);
				return;
			}

			int seed;

			lock (Global)
			{
				seed = Global.Next();
			}

			local = inst = new Random(seed);
			NextChars(inst, buffer);
		}

		public static int Next()
		{
			return Next(1, int.MaxValue);
		}

		public static int Next(int maxExclusive)
		{
			return Next(1, maxExclusive);
		}

		private static long NextLong(Random random, long minInclusive, long maxExclusive)
		{
			var result = (long)random.Next((int)(minInclusive >> 32), (int)(maxExclusive >> 32));
			result <<= 32;
			result |= (uint)random.Next((int)minInclusive, (int)maxExclusive);
			return result;
		}
		
		private static void NextChars(Random random, char[] buffer)
		{
			for (var i = 0; i < buffer.Length; ++i)
			{
				// capping to byte value here to not exceed
				// 56 bit crypto keys length requirement by
				// Apple to avoid cryptography declaration
				buffer[i] = (char) (random.Next() % 256);
			}
		}
	}
}