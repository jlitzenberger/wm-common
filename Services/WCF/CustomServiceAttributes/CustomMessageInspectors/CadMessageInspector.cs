using System;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.Linq;
using System.Xml;
using System.IO;
using System.Xml.Schema;
using System.Configuration;
using System.Collections.Generic;
using WM.Common.Security.Impersonate;

namespace WM.Common.Services.WCF.CustomServiceAttributes.CustomMessageInspectors
{
    public class CadMessageInspector : DBManagerBase, IDispatchMessageInspector, IParameterInspector
    {
        public string Operation { get; set; }
        public List<string> lstOperations { get; set; }

        public CadMessageInspector(OriginType iOrigin, EnvironmentType iEnvironment)
            : base(iOrigin, iEnvironment) { }

        ///////////////////////////////////////////////////////////////////////////////////
        //These 2 implementations for IDispatchMessageInspector get called for every call
        object IDispatchMessageInspector.AfterReceiveRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel, System.ServiceModel.InstanceContext instanceContext)
        {
            //get current operation
            var action = OperationContext.Current.IncomingMessageHeaders.Action;
            var operationName = action.Substring(action.LastIndexOf("/") + 1);

            //only log listed operations
            if (lstOperations.Any(s => s == operationName))
            {
                LogText("----------------------------------------------------------------------------------------------------------------------------");

                //log http request
                string msgName = "SOAPRequestCadService" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xml";
                LogHttpMessageRequest(msgName, ref request);

                //log message
                LogMessage(msgName, ref request);
                LogEvent("CadMessageInspector", "IDispatchMessageInspector.AfterReceiveRequest()", "Request Message: " + msgName);
            }

            return "MyCustomToken";
        }

        void IDispatchMessageInspector.BeforeSendReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
            //get current operation
            var action = OperationContext.Current.IncomingMessageHeaders.Action;
            var operationName = action.Substring(action.LastIndexOf("/") + 1);

            if (correlationState is string && "MyCustomToken" == (string)correlationState)
            {
                //only log listed operations
                if (lstOperations.Any(s => s == operationName))
                {
                    //log http response
                    string msgName = "SOAPResponseCadService" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xml";
                    LogHttpMessageResponse(msgName, ref reply);

                    //log message
                    LogMessage(msgName, ref reply);
                    LogEvent("CadMessageInspector", "IDispatchMessageInspector.BeforeSendReply()", "Response Message: " + msgName);
                }

            }
        }

        /////////////////////////////////////////////////////////////////////////////
        //These 2 implementations for IParameterInspector get called for every call
        void IParameterInspector.AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
        {
            LogEvent("CadMessageInspector", "IParameterInspector.AfterCall()", "OperationName: " + Operation + " - " + "Returned: " + returnValue.ToString());
        }
        object IParameterInspector.BeforeCall(string operationName, object[] inputs)
        {
            LogEvent("CadMessageInspector", "IParameterInspector.BeforeCall()", "OperationName: " + Operation + " - " + "Input: " + inputs[0].ToString());

            return null;
        }

        private static void LogText(string text)
        {
            try
            {
                using (new WrapperImpersonationContext("WE", "", "").Context)
                {
                    Logger.logDirectory = new DBManagerBase(OriginType.CAD, (EnvironmentType)Enum.Parse(typeof(EnvironmentType), ConfigurationManager.AppSettings.Get("Env"))).CadServiceLogDirectoryName;
                    Logger.logFileName = new DBManagerBase(OriginType.CAD, (EnvironmentType)Enum.Parse(typeof(EnvironmentType), ConfigurationManager.AppSettings.Get("Env"))).CadServiceLogFileName;
                    Logger.Log(text);
                }
            }
            catch { }
        }
        private static void LogEvent(string className, string methodName, string parameter)
        {
            try
            {
                using (new WrapperImpersonationContext("WE", "", "").Context)
                {
                    Logger.logDirectory = new DBManagerBase(OriginType.CAD, (EnvironmentType)Enum.Parse(typeof(EnvironmentType), ConfigurationManager.AppSettings.Get("Env"))).CadServiceLogDirectoryName;
                    Logger.logFileName = new DBManagerBase(OriginType.CAD, (EnvironmentType)Enum.Parse(typeof(EnvironmentType), ConfigurationManager.AppSettings.Get("Env"))).CadServiceLogFileName;
                    Logger.Log(className, methodName, parameter);
                }
            }
            catch { }
        }
        private static void LogMessage(string fileName, ref System.ServiceModel.Channels.Message request)
        {
            try
            {
                using (new WrapperImpersonationContext("WE", "", "").Context)
                {
                    Logger.logDirectory = new DBManagerBase(OriginType.CAD, (EnvironmentType)Enum.Parse(typeof(EnvironmentType), ConfigurationManager.AppSettings.Get("Env"))).CadServiceLogDirectoryName;
                    Logger.logFileName = fileName;
                    Logger.LogMessage(ref request);
                }
            }
            catch { }
        }
        private static void LogHttpMessageRequest(string fileName, ref System.ServiceModel.Channels.Message request)
        {
            try
            {
                using (new WrapperImpersonationContext("WE", "", "").Context)
                {
                    Logger.logDirectory = new DBManagerBase(OriginType.CAD, (EnvironmentType)Enum.Parse(typeof(EnvironmentType), ConfigurationManager.AppSettings.Get("Env"))).CadServiceLogDirectoryName;
                    Logger.logFileName = new DBManagerBase(OriginType.CAD, (EnvironmentType)Enum.Parse(typeof(EnvironmentType), ConfigurationManager.AppSettings.Get("Env"))).CadServiceLogHttpMessageFileName;
                    Logger.messageFileName = fileName;
                    Logger.LogHttpMessageRequest(ref request);
                }
            }
            catch { }
        }
        private static void LogHttpMessageResponse(string fileName, ref System.ServiceModel.Channels.Message response)
        {
            try
            {
                using (new WrapperImpersonationContext("WE", "", "").Context)
                {
                    Logger.logDirectory = new DBManagerBase(OriginType.CAD, (EnvironmentType)Enum.Parse(typeof(EnvironmentType), ConfigurationManager.AppSettings.Get("Env"))).CadServiceLogDirectoryName;
                    Logger.logFileName = new DBManagerBase(OriginType.CAD, (EnvironmentType)Enum.Parse(typeof(EnvironmentType), ConfigurationManager.AppSettings.Get("Env"))).CadServiceLogHttpMessageFileName;
                    Logger.messageFileName = fileName;
                    Logger.LogHttpMessageResponse(ref response);
                }
            }
            catch { }
        }

    }
}

