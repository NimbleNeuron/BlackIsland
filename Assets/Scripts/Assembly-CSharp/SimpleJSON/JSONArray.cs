using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SimpleJSON
{
	
	public class JSONArray : JSONNode, IEnumerable
	{
		
		public bool inline;

		
		private readonly List<JSONNode> m_List = new List<JSONNode>();

		
		
		public override JSONNodeType Tag => JSONNodeType.Array;

		
		
		public override bool IsArray => true;

		
		public override JSONNode this[int aIndex] {
			get
			{
				if (aIndex < 0 || aIndex >= m_List.Count)
				{
					return new JSONLazyCreator(this);
				}

				return m_List[aIndex];
			}
			set
			{
				if (value == null)
				{
					value = new JSONNull();
				}

				if (aIndex < 0 || aIndex >= m_List.Count)
				{
					m_List.Add(value);
					return;
				}

				m_List[aIndex] = value;
			}
		}

		
		public override JSONNode this[string aKey] {
			get => new JSONLazyCreator(this);
			set
			{
				if (value == null)
				{
					value = new JSONNull();
				}

				m_List.Add(value);
			}
		}

		
		
		public override int Count => m_List.Count;

		
		
		public override IEnumerable<JSONNode> Children {
			get
			{
				foreach (JSONNode jsonnode in m_List)
				{
					yield return jsonnode;
				}
			}
		}

		
		public IEnumerator GetEnumerator()
		{
			foreach (JSONNode jsonnode in m_List)
			{
				yield return jsonnode;
			}
		}

		
		public override void Add(string aKey, JSONNode aItem)
		{
			if (aItem == null)
			{
				aItem = new JSONNull();
			}

			m_List.Add(aItem);
		}

		
		public override JSONNode Remove(int aIndex)
		{
			if (aIndex < 0 || aIndex >= m_List.Count)
			{
				return null;
			}

			JSONNode result = m_List[aIndex];
			m_List.RemoveAt(aIndex);
			return result;
		}

		
		public override JSONNode Remove(JSONNode aNode)
		{
			m_List.Remove(aNode);
			return aNode;
		}

		
		public override void Serialize(BinaryWriter aWriter)
		{
			aWriter.Write(1);
			aWriter.Write(m_List.Count);
			for (int i = 0; i < m_List.Count; i++)
			{
				m_List[i].Serialize(aWriter);
			}
		}

		
		internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
		{
			aSB.Append('[');
			int count = m_List.Count;
			if (inline)
			{
				aMode = JSONTextMode.Compact;
			}

			for (int i = 0; i < count; i++)
			{
				if (i > 0)
				{
					aSB.Append(',');
				}

				if (aMode == JSONTextMode.Indent)
				{
					aSB.AppendLine();
				}

				if (aMode == JSONTextMode.Indent)
				{
					aSB.Append(' ', aIndent + aIndentInc);
				}

				m_List[i].WriteToStringBuilder(aSB, aIndent + aIndentInc, aIndentInc, aMode);
			}

			if (aMode == JSONTextMode.Indent)
			{
				aSB.AppendLine().Append(' ', aIndent);
			}

			aSB.Append(']');
		}
	}
}