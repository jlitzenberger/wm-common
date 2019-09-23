using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel.Description;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.Text;
using WM.Common.Services.WCF.CustomServiceAttributes.CustomMessageInspectors;
using WM.Common.Security.Impersonate;

namespace WM.Common.Services.WCF.CustomServiceAttributes.CustomEndpointBehavior
{
    /// <summary>
    /// This Class provide an attribute that need to be applied on 
    /// data service class in order to enable custom handler
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CadEndPointBehavior : Attribute, IEndpointBehavior {
        public static string CadServiceLogFileName { get; set; }
        public static string CadServiceLogDirectoryName { get; set; }
        public static string CadServiceLogHttpMessageFileName { get; set; }
        public List<string> lstOperations { get; set; }

        public CadEndPointBehavior(List<string> _lstOperations) {
            DBManagerBase dbm = new DBManagerBase(OriginType.CAD, (EnvironmentType)Enum.Parse(typeof(EnvironmentType), ConfigurationManager.AppSettings.Get("Env")));

            CadServiceLogFileName = dbm.CadServiceLogFileName;
            CadServiceLogDirectoryName = dbm.CadServiceLogDirectoryName;
            CadServiceLogHttpMessageFileName = dbm.CadServiceLogHttpMessageFileName;
            lstOperations = _lstOperations;
        }

        void IEndpointBehavior.AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) {
            LogText("----------------------------------------------------------------------------------------------------------------------------");
            CustomOperationBehavior.CadOperationBehavior cob = new CustomOperationBehavior.CadOperationBehavior();
            foreach (OperationDescription item in endpoint.Contract.Operations) {
                if (lstOperations.Any(s => s == item.Name)) {
                    item.Behaviors.Add(cob);
                    Log("CadEndPointBehavior", "IEndpointBehavior.AddBindingParameters()", "ContractName: " + endpoint.Contract.ContractType.FullName + "  OperationMethod: " + item.SyncMethod.ToString());
                }
            }
        }
        void IEndpointBehavior.ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime) {
            //throw new NotImplementedException();
        }
        void IEndpointBehavior.ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher) {
            CadMessageInspector cmi = new CustomMessageInspectors.CadMessageInspector(OriginType.CAD, (EnvironmentType)Enum.Parse(typeof(EnvironmentType), ConfigurationManager.AppSettings.Get("Env")));
            cmi.lstOperations = lstOperations;
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(cmi);
            Log("CadEndPointBehavior", "IEndpointBehavior.ApplyDispatchBehavior()", "ContractName: " + endpoint.Contract.ContractType.FullName + "  EndpointAddress: " + endpointDispatcher.EndpointAddress);
        }
        void IEndpointBehavior.Validate(ServiceEndpoint endpoint) {
            //throw new NotImplementedException();
        }

        private static void LogText(string text) {
            try {
                using (new WrapperImpersonationContext("WE", "", "").Context) {
                    Logger.logDirectory = new DBManagerBase(OriginType.CAD, (EnvironmentType)Enum.Parse(typeof(EnvironmentType), ConfigurationManager.AppSettings.Get("Env"))).CadServiceLogDirectoryName;
                    Logger.logFileName = new DBManagerBase(OriginType.CAD, (EnvironmentType)Enum.Parse(typeof(EnvironmentType), ConfigurationManager.AppSettings.Get("Env"))).CadServiceLogFileName;
                    Logger.Log(text);
                }
            } catch { }
        }
        private static void Log(string className, string functionName, string description) {
            try {
                using (new WrapperImpersonationContext("WE", "", "").Context) {
                    //Could do this too
                    //Logger.logDirectory = new DBManagerBase(OriginType.CADWEB, (EnvironmentType)Enum.Parse(typeof(EnvironmentType), ConfigurationManager.AppSettings.Get("Env"))).CadServiceLogDirectoryName;
                    Logger.logDirectory = CadServiceLogDirectoryName;
                    Logger.logFileName = CadServiceLogFileName;
                    Logger.Log(className, functionName, description);
                }
            } catch { }
        }
    }
}
