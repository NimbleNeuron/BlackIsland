using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace SimpleJSON
{
	
	public abstract class JSONNode
	{
		
		internal static StringBuilder m_EscapeBuilder = new StringBuilder();

		
		public virtual JSONNode this[int aIndex] {
			get => null;
			set { }
		}

		
		public virtual JSONNode this[string aKey] {
			get => null;
			set { }
		}

		
		
		
		public virtual string Value {
			get => "";
			set { }
		}

		
		
		public virtual int Count => 0;

		
		
		public virtual bool IsNumber => false;

		
		
		public virtual bool IsString => false;

		
		
		public virtual bool IsBoolean => false;

		
		
		public virtual bool IsNull => false;

		
		
		public virtual bool IsArray => false;

		
		
		public virtual bool IsObject => false;

		
		
		public virtual IEnumerable<JSONNode> Children {
			get { yield break; }
		}

		
		
		public IEnumerable<JSONNode> DeepChildren {
			get
			{
				foreach (JSONNode jsonnode in Children)
				{
					foreach (JSONNode jsonnode2 in jsonnode.DeepChildren)
					{
						yield return jsonnode2;
					}
				}
			}
		}

		
		
		public abstract JSONNodeType Tag { get; }

		
		
		
		public virtual double AsDouble {
			get
			{
				double result = 0.0;
				if (double.TryParse(Value, out result))
				{
					return result;
				}

				return 0.0;
			}
			set => Value = value.ToString();
		}

		
		
		
		public virtual int AsInt {
			get => (int) AsDouble;
			set => AsDouble = value;
		}

		
		
		
		public virtual float AsFloat {
			get => (float) AsDouble;
			set => AsDouble = value;
		}

		
		
		
		public virtual bool AsBool {
			get
			{
				bool result = false;
				if (bool.TryParse(Value, out result))
				{
					return result;
				}

				return !string.IsNullOrEmpty(Value);
			}
			set => Value = value ? "true" : "false";
		}

		
		
		public virtual JSONArray AsArray => this as JSONArray;

		
		
		public virtual JSONObject AsObject => this as JSONObject;

		
		public virtual void Add(string aKey, JSONNode aItem) { }

		
		public virtual void Add(JSONNode aItem)
		{
			Add("", aItem);
		}

		
		public virtual JSONNode Remove(string aKey)
		{
			return null;
		}

		
		public virtual JSONNode Remove(int aIndex)
		{
			return null;
		}

		
		public virtual JSONNode Remove(JSONNode aNode)
		{
			return aNode;
		}

		
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			WriteToStringBuilder(stringBuilder, 0, 0, JSONTextMode.Compact);
			return stringBuilder.ToString();
		}

		
		public virtual string ToString(int aIndent)
		{
			StringBuilder stringBuilder = new StringBuilder();
			WriteToStringBuilder(stringBuilder, 0, aIndent, JSONTextMode.Indent);
			return stringBuilder.ToString();
		}

		
		internal abstract void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode);

		
		public static implicit operator JSONNode(string s)
		{
			return new JSONString(s);
		}

		
		public static implicit operator string(JSONNode d)
		{
			if (!(d == null))
			{
				return d.Value;
			}

			return null;
		}

		
		public static implicit operator JSONNode(double n)
		{
			return new JSONNumber(n);
		}

		
		public static implicit operator double(JSONNode d)
		{
			if (!(d == null))
			{
				return d.AsDouble;
			}

			return 0.0;
		}

		
		public static implicit operator JSONNode(float n)
		{
			return new JSONNumber(n);
		}

		
		public static implicit operator float(JSONNode d)
		{
			if (!(d == null))
			{
				return d.AsFloat;
			}

			return 0f;
		}

		
		public static implicit operator JSONNode(int n)
		{
			return new JSONNumber(n);
		}

		
		public static implicit operator int(JSONNode d)
		{
			if (!(d == null))
			{
				return d.AsInt;
			}

			return 0;
		}

		
		public static implicit operator JSONNode(bool b)
		{
			return new JSONBool(b);
		}

		
		public static implicit operator bool(JSONNode d)
		{
			return !(d == null) && d.AsBool;
		}

		
		public static bool operator ==(JSONNode a, object b)
		{
			if (a == b)
			{
				return true;
			}

			bool flag = a is JSONNull || a == null || a is JSONLazyCreator;
			bool flag2 = b is JSONNull || b == null || b is JSONLazyCreator;
			return flag && flag2 || a.Equals(b);
		}

		
		public static bool operator !=(JSONNode a, object b)
		{
			return !(a == b);
		}

		
		public override bool Equals(object obj)
		{
			return this == obj;
		}

		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		
		internal static string Escape(string aText)
		{
			m_EscapeBuilder.Length = 0;
			if (m_EscapeBuilder.Capacity < aText.Length + aText.Length / 10)
			{
				m_EscapeBuilder.Capacity = aText.Length + aText.Length / 10;
			}

			int i = 0;
			while (i < aText.Length)
			{
				char c = aText[i];
				switch (c)
				{
					case '\b':
						m_EscapeBuilder.Append("\\b");
						break;
					case '\t':
						m_EscapeBuilder.Append("\\t");
						break;
					case '\n':
						m_EscapeBuilder.Append("\\n");
						break;
					case '\v':
						goto IL_FA;
					case '\f':
						m_EscapeBuilder.Append("\\f");
						break;
					case '\r':
						m_EscapeBuilder.Append("\\r");
						break;
					default:
						if (c != '"')
						{
							if (c != '\\')
							{
								goto IL_FA;
							}

							m_EscapeBuilder.Append("\\\\");
						}
						else
						{
							m_EscapeBuilder.Append("\\\"");
						}

						break;
				}

				IL_106:
				i++;
				continue;
				IL_FA:
				m_EscapeBuilder.Append(c);
				goto IL_106;
			}

			string result = m_EscapeBuilder.ToString();
			m_EscapeBuilder.Length = 0;
			return result;
		}

		
		private static void ParseElement(JSONNode ctx, string token, string tokenName, bool quoted)
		{
			if (quoted)
			{
				ctx.Add(tokenName, token);
				return;
			}

			string a = token.ToLower();
			if (a == "false" || a == "true")
			{
				ctx.Add(tokenName, a == "true");
				return;
			}

			if (a == "null")
			{
				ctx.Add(tokenName, null);
				return;
			}

			double n;
			if (double.TryParse(token, out n))
			{
				ctx.Add(tokenName, n);
				return;
			}

			ctx.Add(tokenName, token);
		}

		
		public static JSONNode Parse(string aJSON)
		{
			Stack<JSONNode> stack = new Stack<JSONNode>();
			JSONNode jsonnode = null;
			int i = 0;
			StringBuilder stringBuilder = new StringBuilder();
			string text = "";
			bool flag = false;
			bool flag2 = false;
			while (i < aJSON.Length)
			{
				char c = aJSON[i];
				if (c <= ',')
				{
					if (c <= ' ')
					{
						switch (c)
						{
							case '\t':
								break;
							case '\n':
							case '\r':
								goto IL_33E;
							case '\v':
							case '\f':
								goto IL_330;
							default:
								if (c != ' ')
								{
									goto IL_330;
								}

								break;
						}

						if (flag)
						{
							stringBuilder.Append(aJSON[i]);
						}
					}
					else if (c != '"')
					{
						if (c != ',')
						{
							goto IL_330;
						}

						if (flag)
						{
							stringBuilder.Append(aJSON[i]);
						}
						else
						{
							if (stringBuilder.Length > 0 || flag2)
							{
								ParseElement(jsonnode, stringBuilder.ToString(), text, flag2);
							}

							text = "";
							stringBuilder.Length = 0;
							flag2 = false;
						}
					}
					else
					{
						flag = !flag;
						flag2 = flag2 || flag;
					}
				}
				else
				{
					if (c <= ']')
					{
						if (c != ':')
						{
							switch (c)
							{
								case '[':
									if (flag)
									{
										stringBuilder.Append(aJSON[i]);
										goto IL_33E;
									}

									stack.Push(new JSONArray());
									if (jsonnode != null)
									{
										jsonnode.Add(text, stack.Peek());
									}

									text = "";
									stringBuilder.Length = 0;
									jsonnode = stack.Peek();
									goto IL_33E;
								case '\\':
									i++;
									if (flag)
									{
										char c2 = aJSON[i];
										if (c2 <= 'f')
										{
											if (c2 == 'b')
											{
												stringBuilder.Append('\b');
												goto IL_33E;
											}

											if (c2 == 'f')
											{
												stringBuilder.Append('\f');
												goto IL_33E;
											}
										}
										else
										{
											if (c2 == 'n')
											{
												stringBuilder.Append('\n');
												goto IL_33E;
											}

											switch (c2)
											{
												case 'r':
													stringBuilder.Append('\r');
													goto IL_33E;
												case 't':
													stringBuilder.Append('\t');
													goto IL_33E;
												case 'u':
												{
													string s = aJSON.Substring(i + 1, 4);
													stringBuilder.Append((char) int.Parse(s,
														NumberStyles.AllowHexSpecifier));
													i += 4;
													goto IL_33E;
												}
											}
										}

										stringBuilder.Append(c2);
									}

									goto IL_33E;
								case ']':
									break;
								default:
									goto IL_330;
							}
						}
						else
						{
							if (flag)
							{
								stringBuilder.Append(aJSON[i]);
								goto IL_33E;
							}

							text = stringBuilder.ToString();
							stringBuilder.Length = 0;
							flag2 = false;
							goto IL_33E;
						}
					}
					else if (c != '{')
					{
						if (c != '}')
						{
							goto IL_330;
						}
					}
					else
					{
						if (flag)
						{
							stringBuilder.Append(aJSON[i]);
							goto IL_33E;
						}

						stack.Push(new JSONObject());
						if (jsonnode != null)
						{
							jsonnode.Add(text, stack.Peek());
						}

						text = "";
						stringBuilder.Length = 0;
						jsonnode = stack.Peek();
						goto IL_33E;
					}

					if (flag)
					{
						stringBuilder.Append(aJSON[i]);
					}
					else
					{
						if (stack.Count == 0)
						{
							throw new Exception("JSON Parse: Too many closing brackets");
						}

						stack.Pop();
						if (stringBuilder.Length > 0 || flag2)
						{
							ParseElement(jsonnode, stringBuilder.ToString(), text, flag2);
							flag2 = false;
						}

						text = "";
						stringBuilder.Length = 0;
						if (stack.Count > 0)
						{
							jsonnode = stack.Peek();
						}
					}
				}

				IL_33E:
				i++;
				continue;
				IL_330:
				stringBuilder.Append(aJSON[i]);
				goto IL_33E;
			}

			if (flag)
			{
				throw new Exception("JSON Parse: Quotation marks seems to be messed up.");
			}

			return jsonnode;
		}

		
		public virtual void Serialize(BinaryWriter aWriter) { }

		
		public void SaveToStream(Stream aData)
		{
			BinaryWriter aWriter = new BinaryWriter(aData);
			Serialize(aWriter);
		}

		
		public void SaveToCompressedStream(Stream aData)
		{
			throw new Exception(
				"Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		
		public void SaveToCompressedFile(string aFileName)
		{
			throw new Exception(
				"Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		
		public string SaveToCompressedBase64()
		{
			throw new Exception(
				"Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		
		public void SaveToFile(string aFileName)
		{
			Directory.CreateDirectory(new FileInfo(aFileName).Directory.FullName);
			using (FileStream fileStream = File.OpenWrite(aFileName))
			{
				SaveToStream(fileStream);
			}
		}

		
		public string SaveToBase64()
		{
			string result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				SaveToStream(memoryStream);
				memoryStream.Position = 0L;
				result = Convert.ToBase64String(memoryStream.ToArray());
			}

			return result;
		}

		
		public static JSONNode Deserialize(BinaryReader aReader)
		{
			JSONNodeType jsonnodeType = (JSONNodeType) aReader.ReadByte();
			switch (jsonnodeType)
			{
				case JSONNodeType.Array:
				{
					int num = aReader.ReadInt32();
					JSONArray jsonarray = new JSONArray();
					for (int i = 0; i < num; i++)
					{
						jsonarray.Add(Deserialize(aReader));
					}

					return jsonarray;
				}
				case JSONNodeType.Object:
				{
					int num2 = aReader.ReadInt32();
					JSONObject jsonobject = new JSONObject();
					for (int j = 0; j < num2; j++)
					{
						string aKey = aReader.ReadString();
						JSONNode aItem = Deserialize(aReader);
						jsonobject.Add(aKey, aItem);
					}

					return jsonobject;
				}
				case JSONNodeType.String:
					return new JSONString(aReader.ReadString());
				case JSONNodeType.Number:
					return new JSONNumber(aReader.ReadDouble());
				case JSONNodeType.NullValue:
					return new JSONNull();
				case JSONNodeType.Boolean:
					return new JSONBool(aReader.ReadBoolean());
				default:
					throw new Exception("Error deserializing JSON. Unknown tag: " + jsonnodeType);
			}
		}

		
		public static JSONNode LoadFromCompressedFile(string aFileName)
		{
			throw new Exception(
				"Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		
		public static JSONNode LoadFromCompressedStream(Stream aData)
		{
			throw new Exception(
				"Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		
		public static JSONNode LoadFromCompressedBase64(string aBase64)
		{
			throw new Exception(
				"Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		
		public static JSONNode LoadFromStream(Stream aData)
		{
			JSONNode result;
			using (BinaryReader binaryReader = new BinaryReader(aData))
			{
				result = Deserialize(binaryReader);
			}

			return result;
		}

		
		public static JSONNode LoadFromFile(string aFileName)
		{
			JSONNode result;
			using (FileStream fileStream = File.OpenRead(aFileName))
			{
				result = LoadFromStream(fileStream);
			}

			return result;
		}

		
		public static JSONNode LoadFromBase64(string aBase64)
		{
			return LoadFromStream(new MemoryStream(Convert.FromBase64String(aBase64))
			{
				Position = 0L
			});
		}
	}
}