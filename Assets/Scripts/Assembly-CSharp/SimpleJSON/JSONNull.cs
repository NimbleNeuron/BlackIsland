using System.IO;
using System.Text;

namespace SimpleJSON
{
	
	public class JSONNull : JSONNode
	{
		
		
		public override JSONNodeType Tag => JSONNodeType.NullValue;

		
		
		public override bool IsNull => true;

		
		
		
		public override string Value {
			get => "null";
			set { }
		}

		
		
		
		public override bool AsBool {
			get => false;
			set { }
		}

		
		public override bool Equals(object obj)
		{
			return this == obj || obj is JSONNull;
		}

		
		public override int GetHashCode()
		{
			return 0;
		}

		
		public override void Serialize(BinaryWriter aWriter)
		{
			aWriter.Write(5);
		}

		
		internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
		{
			aSB.Append("null");
		}
	}
}