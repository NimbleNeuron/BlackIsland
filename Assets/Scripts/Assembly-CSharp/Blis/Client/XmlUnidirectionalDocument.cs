using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Blis.Client
{
	public class XmlUnidirectionalDocument
	{
		private readonly XmlReader xmlReader;


		private XmlUnidirectionalDocument(XmlReader xmlReader)
		{
			this.xmlReader = xmlReader;
		}


		public IEnumerable<XmlUnidirectionalAttribute> Attributes {
			get
			{
				xmlReader.MoveToFirstAttribute();
				do
				{
					yield return new XmlUnidirectionalAttribute(xmlReader.Name, xmlReader.Value);
				} while (xmlReader.MoveToNextAttribute());
			}
		}


		public IEnumerable<XmlUnidirectionalDocument> Childs {
			get
			{
				for (;;)
				{
					xmlReader.Read();
					if (xmlReader.EOF)
					{
						break;
					}

					if (xmlReader.NodeType == XmlNodeType.EndElement)
					{
						goto Block_1;
					}

					if (xmlReader.NodeType != XmlNodeType.Whitespace)
					{
						yield return new XmlUnidirectionalDocument(xmlReader);
						while (xmlReader.NodeType != XmlNodeType.EndElement)
						{
							xmlReader.Read();
						}
					}
				}

				yield break;
				Block_1:
				if (xmlReader.IsEmptyElement)
				{
					yield return new XmlUnidirectionalDocument(xmlReader);
				}
			}
		}


		public string Name => xmlReader.Name;


		public static XmlUnidirectionalDocument Create(TextReader textReader)
		{
			XmlTextReader xmlTextReader = new XmlTextReader(textReader);
			xmlTextReader.MoveToContent();
			return new XmlUnidirectionalDocument(xmlTextReader);
		}


		public object DeserializeFromXmlDefaultSerializer(Type type)
		{
			do
			{
				xmlReader.Read();
			} while (xmlReader.NodeType == XmlNodeType.Whitespace);

			return TextDataUnidirectionalDocumentSerializerStorage.DeserializeFromXmlDefaultSerializer(xmlReader, type);
		}


		public object DeserializeFromNetSerializer(Type type)
		{
			do
			{
				xmlReader.Read();
			} while (xmlReader.NodeType == XmlNodeType.Whitespace);

			return TextDataUnidirectionalDocumentSerializerStorage.DeserializeFromNetSerializer(xmlReader, type);
		}
	}
}