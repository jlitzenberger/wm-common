using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;

namespace WM.Common.Utils
{
    public static class XmlHelper
    {
        public static T DeserializeXmlToObject<T>(XmlDocument doc)
        {
            var ser = new XmlSerializer(typeof(T));
            var wrapper = (T)ser.Deserialize(new XmlNodeReader(doc));

            return wrapper;
        }
        public static XmlDocument TransformXmlDocument(XmlDocument doc, string xsltLocation)
        {
            XslCompiledTransform xslTransform = new XslCompiledTransform();
            XmlWriterSettings writerSettings = new XmlWriterSettings();
            StringBuilder writer = new StringBuilder();

            xslTransform.Load(xsltLocation);

            writerSettings.OmitXmlDeclaration = true;
            XmlWriter transformedData = XmlWriter.Create(writer, writerSettings);

            //xslTransform.Transform(doc.CreateNavigator(), null, writer);
            xslTransform.Transform(doc.CreateNavigator(), transformedData);

            if (!string.IsNullOrEmpty(writer.ToString()))
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(writer.ToString());

                return xmlDocument;
            }

            return null;
        }
        public static XmlDocument SerializeToXmlDocument(object obj)
        {
            ///////////////////////
            //load showJob into xml string
            var xml = SerializeObjectToXml(obj);

            //////////////////
            //load into Xml Document 
            XmlDocument doc = SerializeToXmlDocument(xml);

            return doc;
        }
        public static XmlDocument SerializeToXmlDocument(string xml)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            return xmlDoc;
        }
        public static string SerializeObjectToXml(object obj)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
            string xml;
            using (StringWriter sw = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sw))
                {
                    xmlSerializer.Serialize(writer, obj);
                    xml = sw.ToString(); // Your XML
                }
            }
            return xml;
        }
    }
}
