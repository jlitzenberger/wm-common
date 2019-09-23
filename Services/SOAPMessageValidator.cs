﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.Xml;
using System.IO;
using System.Xml.Schema;

namespace WWM.Common.Services
{
    public class SOAPMessageValidator {
        XmlSchemaSet schemaSet;
        bool validateRequest;
        bool validateReply;
        bool isClientSide;
        [ThreadStatic]
        bool isRequest;

        public SOAPMessageValidator(XmlSchemaSet schemaSet, bool validateRequest, bool validateReply, bool isClientSide) {
            this.schemaSet = schemaSet;
            this.validateReply = validateReply;
            this.validateRequest = validateRequest;
            this.isClientSide = isClientSide;
        }

        public void ValidateMessageBody(ref System.ServiceModel.Channels.Message message, bool isRequest) {
            if (!message.IsFault) {
                XmlDictionaryReaderQuotas quotas = new XmlDictionaryReaderQuotas();
                XmlReader bodyReader = message.GetReaderAtBodyContents().ReadSubtree();
                XmlReaderSettings wrapperSettings = new XmlReaderSettings();
                wrapperSettings.CloseInput = true;
                wrapperSettings.Schemas = schemaSet;
                wrapperSettings.ValidationFlags = XmlSchemaValidationFlags.None;
                wrapperSettings.ValidationType = ValidationType.Schema;
                wrapperSettings.ValidationEventHandler += new ValidationEventHandler(InspectionValidationHandler);
                XmlReader wrappedReader = XmlReader.Create(bodyReader, wrapperSettings);

                // pull body into a memory backed writer to validate
                this.isRequest = isRequest;
                MemoryStream memStream = new MemoryStream();
                XmlDictionaryWriter xdw = XmlDictionaryWriter.CreateBinaryWriter(memStream);
                xdw.WriteNode(wrappedReader, false);
                xdw.Flush(); memStream.Position = 0;
                XmlDictionaryReader xdr = XmlDictionaryReader.CreateBinaryReader(memStream, quotas);

                // reconstruct the message with the validated body
                Message replacedMessage = Message.CreateMessage(message.Version, null, xdr);
                replacedMessage.Headers.CopyHeadersFrom(message.Headers);
                replacedMessage.Properties.CopyProperties(message.Properties);
                message = replacedMessage;
            }
        }


        public void InspectionValidationHandler(object sender, ValidationEventArgs e) {
            if (e.Severity == XmlSeverityType.Error) {
                // We are treating client and service side validation errors
                // differently here. Client side errors cause exceptions
                // and are thrown straight up to the user code. Service side
                // validations cause faults.
                if (isClientSide) {
                    if (isRequest) {
                        //throw new RequestClientValidationException(e.Message);
                    } else {
                        //throw new ReplyClientValidationException(e.Message);
                    }
                } else {
                    if (isRequest) {
                        //// this fault is caught by the ServiceModel 
                        //// infrastructure and turned into a fault reply.
                        //throw new RequestValidationFault(e.Message);

                        throw new FaultException<string>(e.Message);
                    } else {
                        //// this fault is caught and turned into a fault message
                        //// in BeforeSendReply in this class
                        //throw new ReplyValidationFault(e.Message);
                    }
                }
            }
        }
    }
}
