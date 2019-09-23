using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Web;
using System.Xml;
using System.Xml.Schema;
using WM.Common.XML;

namespace WM.Common.Services
{
    public class SOAPUtility {
        public static XmlDocument LoadMessageBodyBufferToXmlDocument(ref Message message) {
            Message bufferMessage = CreateMessageBuffer(ref message);

            XmlReader xdr = bufferMessage.GetReaderAtBodyContents().ReadSubtree();
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(xdr);
            xdr.Close();

            return xdoc;
        }

        public static Message CreateMessageBuffer(ref Message message) {
            MessageBuffer buffer = message.CreateBufferedCopy(int.MaxValue);
            Message bufferMessage = buffer.CreateMessage();

            return bufferMessage;
        }

        public static Message CreateNewMessageFromBodyContents(XmlDictionaryReader xdrMessageBodyContents, ref Message OldMessage) {
            //create new message from modified XML document
            Message newMessage = Message.CreateMessage(OldMessage.Version, null, xdrMessageBodyContents);
            newMessage.Headers.CopyHeadersFrom(OldMessage);
            newMessage.Properties.CopyProperties(OldMessage.Properties);

            return newMessage;
        }

        public static Message CreateNewMessage(XmlDictionaryReader xdrMessageContents, ref Message OldMessage) {
            //create new message from modified XML document
            Message newMessage = Message.CreateMessage(xdrMessageContents, int.MaxValue, OldMessage.Version);
            newMessage.Headers.CopyHeadersFrom(OldMessage);
            newMessage.Properties.CopyProperties(OldMessage.Properties);

            return newMessage;
        }

        public static string GetOperationName() {
            var action = OperationContext.Current.IncomingMessageHeaders.Action;
            var operationName = action.Substring(action.LastIndexOf("/", StringComparison.OrdinalIgnoreCase) + 1);

            return operationName;
        }
    }
}