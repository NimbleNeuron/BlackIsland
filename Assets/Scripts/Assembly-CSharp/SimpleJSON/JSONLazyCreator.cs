using System.Text;

namespace SimpleJSON
{
	
	internal class JSONLazyCreator : JSONNode
	{
		
		private readonly string m_Key;

		
		private JSONNode m_Node;

		
		public JSONLazyCreator(JSONNode aNode)
		{
			m_Node = aNode;
			m_Key = null;
		}

		
		public JSONLazyCreator(JSONNode aNode, string aKey)
		{
			m_Node = aNode;
			m_Key = aKey;
		}

		
		
		public override JSONNodeType Tag => JSONNodeType.None;

		
		public override JSONNode this[int aIndex] {
			get => new JSONLazyCreator(this);
			set =>
				Set(new JSONArray
				{
					value
				});
		}

		
		public override JSONNode this[string aKey] {
			get => new JSONLazyCreator(this, aKey);
			set =>
				Set(new JSONObject
				{
					{
						aKey,
						value
					}
				});
		}

		
		
		
		public override int AsInt {
			get
			{
				JSONNumber aVal = new JSONNumber(0.0);
				Set(aVal);
				return 0;
			}
			set
			{
				JSONNumber aVal = new JSONNumber(value);
				Set(aVal);
			}
		}

		
		
		
		public override float AsFloat {
			get
			{
				JSONNumber aVal = new JSONNumber(0.0);
				Set(aVal);
				return 0f;
			}
			set
			{
				JSONNumber aVal = new JSONNumber(value);
				Set(aVal);
			}
		}

		
		
		
		public override double AsDouble {
			get
			{
				JSONNumber aVal = new JSONNumber(0.0);
				Set(aVal);
				return 0.0;
			}
			set
			{
				JSONNumber aVal = new JSONNumber(value);
				Set(aVal);
			}
		}

		
		
		
		public override bool AsBool {
			get
			{
				JSONBool aVal = new JSONBool(false);
				Set(aVal);
				return false;
			}
			set
			{
				JSONBool aVal = new JSONBool(value);
				Set(aVal);
			}
		}

		
		
		public override JSONArray AsArray {
			get
			{
				JSONArray jsonarray = new JSONArray();
				Set(jsonarray);
				return jsonarray;
			}
		}

		
		
		public override JSONObject AsObject {
			get
			{
				JSONObject jsonobject = new JSONObject();
				Set(jsonobject);
				return jsonobject;
			}
		}

		
		private void Set(JSONNode aVal)
		{
			if (m_Key == null)
			{
				m_Node.Add(aVal);
			}
			else
			{
				m_Node.Add(m_Key, aVal);
			}

			m_Node = null;
		}

		
		public override void Add(JSONNode aItem)
		{
			Set(new JSONArray
			{
				aItem
			});
		}

		
		public override void Add(string aKey, JSONNode aItem)
		{
			Set(new JSONObject
			{
				{
					aKey,
					aItem
				}
			});
		}

		
		public static bool operator ==(JSONLazyCreator a, object b)
		{
			return b == null || a == b;
		}

		
		public static bool operator !=(JSONLazyCreator a, object b)
		{
			return !(a == b);
		}

		
		public override bool Equals(object obj)
		{
			return obj == null || this == obj;
		}

		
		public override int GetHashCode()
		{
			return 0;
		}

		
		internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
		{
			aSB.Append("null");
		}
	}
}