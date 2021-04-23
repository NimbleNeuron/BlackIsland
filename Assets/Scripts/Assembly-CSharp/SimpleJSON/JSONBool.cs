using System.IO;
using System.Text;

namespace SimpleJSON
{
	
	public class JSONBool : JSONNode
	{
		
		private bool m_Data;

		
		public JSONBool(bool aData)
		{
			m_Data = aData;
		}

		
		public JSONBool(string aData)
		{
			Value = aData;
		}

		
		
		public override JSONNodeType Tag => JSONNodeType.Boolean;

		
		
		public override bool IsBoolean => true;

		
		
		
		public override string Value {
			get => m_Data.ToString();
			set
			{
				bool data;
				if (bool.TryParse(value, out data))
				{
					m_Data = data;
				}
			}
		}

		
		
		
		public override bool AsBool {
			get => m_Data;
			set => m_Data = value;
		}

		
		public override void Serialize(BinaryWriter aWriter)
		{
			aWriter.Write(6);
			aWriter.Write(m_Data);
		}

		
		internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
		{
			aSB.Append(m_Data ? "true" : "false");
		}

		
		public override bool Equals(object obj)
		{
			return obj != null && obj is bool && m_Data == (bool) obj;
		}

		
		public override int GetHashCode()
		{
			return m_Data.GetHashCode();
		}
	}
}