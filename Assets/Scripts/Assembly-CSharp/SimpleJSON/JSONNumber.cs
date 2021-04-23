using System;
using System.IO;
using System.Text;

namespace SimpleJSON
{
	
	public class JSONNumber : JSONNode
	{
		
		private double m_Data;

		
		public JSONNumber(double aData)
		{
			m_Data = aData;
		}

		
		public JSONNumber(string aData)
		{
			Value = aData;
		}

		
		
		public override JSONNodeType Tag => JSONNodeType.Number;

		
		
		public override bool IsNumber => true;

		
		
		
		public override string Value {
			get => m_Data.ToString();
			set
			{
				double data;
				if (double.TryParse(value, out data))
				{
					m_Data = data;
				}
			}
		}

		
		
		
		public override double AsDouble {
			get => m_Data;
			set => m_Data = value;
		}

		
		public override void Serialize(BinaryWriter aWriter)
		{
			aWriter.Write(4);
			aWriter.Write(m_Data);
		}

		
		internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
		{
			aSB.Append(m_Data);
		}

		
		private static bool IsNumeric(object value)
		{
			return value is int || value is uint || value is float || value is double || value is decimal ||
			       value is long || value is ulong || value is short || value is ushort || value is sbyte ||
			       value is byte;
		}

		
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}

			if (base.Equals(obj))
			{
				return true;
			}

			JSONNumber jsonnumber = obj as JSONNumber;
			if (jsonnumber != null)
			{
				return m_Data == jsonnumber.m_Data;
			}

			return IsNumeric(obj) && Convert.ToDouble(obj) == m_Data;
		}

		
		public override int GetHashCode()
		{
			return m_Data.GetHashCode();
		}
	}
}