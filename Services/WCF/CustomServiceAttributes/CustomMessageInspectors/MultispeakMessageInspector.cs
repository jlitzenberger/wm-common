using System;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.Xml;
using System.IO;
using System.Xml.Schema;
using WWM.Common.Services;

namespace WM.Common.Services.WCF.CustomServiceAttributes.CustomMessageInspectors
{
    public class MultispeakMessageInspector : IDispatchMessageInspector, IClientMessageInspector {
        XmlSchemaSet schemaSet;
        bool validateRequest;
        bool validateReply;
        bool isClientSide;

        public MultispeakMessageInspector(XmlSchemaSet schemaSet, bool validateRequest, bool validateReply, bool isClientSide) {
            this.schemaSet = schemaSet;
            this.validateReply = validateReply;
            this.validateRequest = validateRequest;
            this.isClientSide = isClientSide;
        }

        public MultispeakMessageInspector() { }

        ///////////////////////////////////////////////////////////////////////////////////
        //These 2 implementations for IDispatchMessageInspector get called for every call
        object IDispatchMessageInspector.AfterReceiveRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel, System.ServiceModel.InstanceContext instanceContext) {
            //log http request
            string msgName = "SOAPRequestMultispeak" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xml";
            LogHttpMessageRequest(msgName, ref request);

            //log original message
            LogMessage(msgName, ref request);

            LogEvent("MultispeakMessageInspector", "AfterReceiveRequest()", "Original Message: " + msgName);

            if (SOAPUtility.GetOperationName() == "StakedWorkOrderNotification") {
                //transform malformed Stakeout message
                request = MultispeakTransformer.Transform(ref request);

                //log transformed message
                msgName = "SOAPRequestMultispeakTransFormed" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xml";
                LogMessage(msgName, ref request);
                LogEvent("MultispeakMessageInspector", "AfterReceiveRequest()", "Transformed Malformed Message: " + msgName);
            }

            if (validateRequest) {
                // inspect the message. If a validation error occurs, the thrown fault exception bubbles up.
                SOAPMessageValidator smv = new SOAPMessageValidator(schemaSet, validateRequest, validateReply, isClientSide);
                smv.ValidateMessageBody(ref request, true);
                LogEvent("MultispeakMessageInspector", "AfterReceiveRequest()", "WSDL Schema Validated");
            }

            LogEvent("MultispeakMessageInspector", "AfterReceiveRequest()", "Completed");

            return "MyCustomToken";
        }

        void IDispatchMessageInspector.BeforeSendReply(ref System.ServiceModel.Channels.Message reply, object correlationState) {
            string msgName = "SOAPResponseMultispeak" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xml";

            if (correlationState is string && "MyCustomToken" == (string)correlationState) {
                LogMessage(msgName, ref reply);
            }

            LogEvent("MultispeakMessageInspector", "BeforeSendReply()", "Response Message: " + msgName);
            LogText("----------------------------------------------------------------------------------------------------------------------------");
        }

        private static void LogText(string text) {
            //Logger.logDirectory = WMCommon.Properties.Settings.Default.MultispeakLogDirectory_DEV;
            //Logger.logFileName = WMCommon.Properties.Settings.Default.MultispeakLogFileName_DEV;
            //Logger.Log(text);
        }
        private static void LogEvent(string className, string methodName, string parameter) {
            //Logger.logDirectory = WMCommon.Properties.Settings.Default.MultispeakLogDirectory_DEV;
            //Logger.logFileName = WMCommon.Properties.Settings.Default.MultispeakLogFileName_DEV;
            //Logger.Log(className, methodName, parameter);
        }
        private static void LogMessage(string fileName, ref System.ServiceModel.Channels.Message request) {
            //Logger.logDirectory = WMCommon.Properties.Settings.Default.MultispeakLogDirectory_DEV;
            //Logger.logFileName = fileName;
            //Logger.LogMessage(ref request);
        }
        private static void LogHttpMessageRequest(string fileName, ref System.ServiceModel.Channels.Message request) {
            //Logger.logDirectory = WMCommon.Properties.Settings.Default.MultispeakLogDirectory_DEV;
            //Logger.logFileName = "MultispeakHttpMessageRequestLog.txt";
            //Logger.messageFileName = fileName;
            //Logger.LogHttpMessageRequest(ref request);
        }

        //IClientMessageInspector
        public void AfterReceiveReply(ref Message reply, object correlationState) {
            throw new NotImplementedException();
        }
        //IClientMessageInspector
        public object BeforeSendRequest(ref Message request, IClientChannel channel) {
            throw new NotImplementedException();
        }
    }
}

