using System.IO;
using System.Text;

namespace SimpleJSON
{
	
	public class JSONString : JSONNode
	{
		
		private string m_Data;

		
		public JSONString(string aData)
		{
			m_Data = aData;
		}

		
		
		public override JSONNodeType Tag => JSONNodeType.String;

		
		
		public override bool IsString => true;

		
		
		
		public override string Value {
			get => m_Data;
			set => m_Data = value;
		}

		
		public override void Serialize(BinaryWriter aWriter)
		{
			aWriter.Write(3);
			aWriter.Write(m_Data);
		}

		
		internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
		{
			aSB.Append('"').Append(Escape(m_Data)).Append('"');
		}

		
		public override bool Equals(object obj)
		{
			if (base.Equals(obj))
			{
				return true;
			}

			string text = obj as string;
			if (text != null)
			{
				return m_Data == text;
			}

			JSONString jsonstring = obj as JSONString;
			return jsonstring != null && m_Data == jsonstring.m_Data;
		}

		
		public override int GetHashCode()
		{
			return m_Data.GetHashCode();
		}
	}
}