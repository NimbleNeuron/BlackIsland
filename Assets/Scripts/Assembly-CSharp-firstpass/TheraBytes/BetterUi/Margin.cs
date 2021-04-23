using System;
using UnityEngine;

namespace TheraBytes.BetterUi
{
	[Serializable]
	public class Margin
	{
		[SerializeField] private int left;


		[SerializeField] private int right;


		[SerializeField] private int top;


		[SerializeField] private int bottom;


		public Margin() : this(0, 0, 0, 0) { }


		public Margin(Vector4 source) : this((int) source.x, (int) source.z, (int) source.y, (int) source.w) { }


		public Margin(RectOffset source) : this(source.left, source.right, source.top, source.bottom) { }


		public Margin(int left, int right, int top, int bottom)
		{
			this.left = left;
			this.right = right;
			this.top = top;
			this.bottom = bottom;
		}


		public float Horizontal => left + right;


		public float Vertical => top + bottom;


		
		public int Left {
			get => left;
			set => left = value;
		}


		
		public int Right {
			get => right;
			set => right = value;
		}


		
		public int Top {
			get => top;
			set => top = value;
		}


		
		public int Bottom {
			get => bottom;
			set => bottom = value;
		}


		public int this[int idx] {
			get
			{
				switch (idx)
				{
					case 0:
						return left;
					case 1:
						return right;
					case 2:
						return top;
					default:
						return bottom;
				}
			}
			set
			{
				switch (idx)
				{
					case 0:
						left = value;
						return;
					case 1:
						right = value;
						return;
					case 2:
						top = value;
						return;
					default:
						bottom = value;
						return;
				}
			}
		}


		public Margin Clone()
		{
			return new Margin(left, right, top, bottom);
		}


		public void CopyValuesTo(RectOffset target)
		{
			target.left = left;
			target.right = right;
			target.top = top;
			target.bottom = bottom;
		}


		public Vector4 ToVector4()
		{
			return new Vector4(left, top, right, bottom);
		}


		public override string ToString()
		{
			return string.Format("(left: {0}, right: {1}, top: {2}, bottom: {3})", left, right, top, bottom);
		}
	}
}