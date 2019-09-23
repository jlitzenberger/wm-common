using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Web;
using System.Xml;
using WM.Common.XML;

namespace WM.Common.Services.WCF.CustomServiceAttributes.CustomMessageInspectors
{
    public class MultispeakTransformer
    {
        public static Message Transform(ref Message OldMessage)
        {
            XmlDocument xmldoc = SOAPUtility.LoadMessageBodyBufferToXmlDocument(ref OldMessage);

            XmlUtility.RemoveAttributeFromXmlDocument(xmldoc, "xmlns", string.Empty);
            XmlUtility.RemoveNodeChildrenFromXmlDocument(xmldoc, "mapLocation");

            XmlDictionaryReader xr = XmlUtility.LoadXmlReader(xmldoc);

            Message msg = SOAPUtility.CreateNewMessageFromBodyContents(xr, ref OldMessage);

            return msg;

        }
    }
}
