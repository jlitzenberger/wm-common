using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Schema;

namespace WM.Common.XML
{
    public class XmlUtility
    {
        public static void RemoveAttributeFromXmlDocument(XmlDocument xdoc, string RemoveAttribute, string RemoveAttributeValue)
        {
            foreach (XmlNode node in xdoc.SelectNodes("//*"))
            {
                RemoveNodeAttributeByValueFromXmlDocument(xdoc, node, RemoveAttribute, RemoveAttributeValue);
            }
        }

        public static void RemoveNodeChildrenFromXmlDocument(XmlDocument xmlDoc, string Node)
        {
            List<XmlNode> nodes = new List<XmlNode>();
            foreach (XmlNode node in xmlDoc.GetElementsByTagName(Node))
            {
                nodes.Add(node);
            }

            foreach (XmlNode node in nodes)
            {
                node.ParentNode.RemoveChild(node);
            }
        }

        private static void RemoveNodeAttributeByValueFromXmlDocument(XmlDocument xdoc, XmlNode node, string RemoveAttribute, string RemoveAttributeValue)
        {
            XmlElement ElemCopy = CopyNodeWithoutSpecificAttribute(xdoc, node, RemoveAttribute, RemoveAttributeValue);

            if (node.Clone().Attributes.GetNamedItem(RemoveAttribute) != null)
            {
                if (node.Clone().Attributes.GetNamedItem(RemoveAttribute).Value == RemoveAttributeValue)
                {
                    ReplaceNodeInXmlDocument(ElemCopy, node);
                }
            }
        }

        private static void RemoveNodeByValueFromXmlDocument(XmlDocument xdoc, XmlNode node)
        {
            node.ParentNode.RemoveChild(node);
        }

        private static XmlElement CopyNodeWithoutSpecificAttribute(XmlDocument xdoc, XmlNode node, string attribute, string attributeValue)
        {
            XmlElement elem = CreateElement(xdoc, node);

            foreach (XmlAttribute att in node.Clone().Attributes)
            {
                if ((att.Name != attribute && att.Value != attributeValue))
                {
                    elem.Attributes.Append(CreateAttribute(xdoc, att));
                }
            }
            return elem;
        }

        private static XmlAttribute CreateAttribute(XmlDocument xdoc, XmlAttribute att)
        {
            XmlAttribute xmlatt = xdoc.CreateAttribute(att.Prefix, att.LocalName, att.NamespaceURI);
            xmlatt.Value = att.Value;

            return xmlatt;
        }

        private static XmlElement CreateElement(XmlDocument xdoc, XmlNode node)
        {
            XmlElement elem = xdoc.CreateElement(string.Empty, node.Name, xdoc.DocumentElement.NamespaceURI);
            elem.InnerText = node.InnerText;

            return elem;
        }

        private static void ReplaceNodeInXmlDocument(XmlElement NewElement, XmlNode OldNode)
        {
            OldNode.ParentNode.InsertAfter(NewElement, OldNode.PreviousSibling);
            OldNode.ParentNode.RemoveChild(OldNode);
        }

        public static XmlDictionaryReader LoadXmlReader(XmlDocument xdoc)
        {
            MemoryStream ms = new MemoryStream();
            XmlWriter xw = XmlWriter.Create(ms);
            xdoc.Save(xw);
            xw.Flush();
            xw.Close();
            ms.Position = 0;
            XmlReader xr = XmlReader.Create(ms);
            XmlDictionaryReader xdr = XmlDictionaryReader.CreateDictionaryReader(xr);

            return xdr;
        }

        public static XmlSchemaSet CreateXmlSchemaSet(string xmlschema)
        {
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(xmlschema);
            XmlReader xr = LoadXmlReader(xd);
            XmlSchema schema = XmlSchema.Read(xr, null);

            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.Add(schema);

            return schemaSet;
        }

        public static string CreateXmlSchemaFromWSDL(string pathWSDL)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(pathWSDL);

            XmlNamespaceManager nsmgr = CreateXmlNamespaceManagerFromXmlDocument(xmldoc);

            XmlElement root = xmldoc.DocumentElement;
            XmlNodeList xnl = root.SelectNodes("//*", nsmgr);
            string xmlschema = string.Empty;

            foreach (XmlElement item in xnl)
            {
                if (item.LocalName == "schema")
                {
                    foreach (XmlAttribute at in root.OwnerDocument.DocumentElement.Attributes)
                    {
                        item.SetAttribute("xmlns:" + at.LocalName + "", at.Value);
                    }

                    xmlschema = item.OuterXml;
                }

            }
            return xmlschema;
        }

        public static XmlNamespaceManager CreateXmlNamespaceManagerFromXmlDocument(XmlDocument xmldoc)
        {
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmldoc.NameTable);
            foreach (XmlAttribute at in xmldoc.DocumentElement.Attributes)
            {
                nsmgr.AddNamespace(at.LocalName, at.Value);
            }
            return nsmgr;
        }
    }
}