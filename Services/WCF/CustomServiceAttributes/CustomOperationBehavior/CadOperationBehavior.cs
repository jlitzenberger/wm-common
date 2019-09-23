using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using WM.Common.Security.Impersonate;
using WM.Common.Services.WCF.CustomServiceAttributes.CustomMessageInspectors;

namespace WM.Common.Services.WCF.CustomServiceAttributes.CustomOperationBehavior
{
    public class CadOperationBehavior : Attribute, IOperationBehavior
    {
        public static string CadServiceLogFileName { get; set; }
        public static string CadServiceLogDirectoryName { get; set; }
        public static string CadServiceLogHttpMessageFileName { get; set; }

        public CadOperationBehavior()
        {
            DBManagerBase dbm = new DBManagerBase(OriginType.CAD, (EnvironmentType)Enum.Parse(typeof(EnvironmentType), ConfigurationManager.AppSettings.Get("Env")));

            CadServiceLogFileName = dbm.CadServiceLogFileName;
            CadServiceLogDirectoryName = dbm.CadServiceLogDirectoryName;
            CadServiceLogHttpMessageFileName = dbm.CadServiceLogHttpMessageFileName;
        }

        ////////////////////////////////////////////////////////////////////////////////////
        //These 4 implementations for IOperationBehavior get called on the first call only
        void IOperationBehavior.AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
        {
            Log("CadOperationBehavior", "IOperationBehavior.AddBindingParameters()", "OperationName: " + operationDescription.SyncMethod.GetBaseDefinition().GetBaseDefinition().ToString());
        }

        void IOperationBehavior.ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {
            CadMessageInspector inspector = new CadMessageInspector(OriginType.CAD, (EnvironmentType)Enum.Parse(typeof(EnvironmentType), ConfigurationManager.AppSettings.Get("Env")));
            inspector.Operation = operationDescription.SyncMethod.GetBaseDefinition().GetBaseDefinition().ToString();
            clientOperation.ParameterInspectors.Add(inspector);

            Log("CadOperationBehavior", "IOperationBehavior.ApplyClientBehavior()", "OperationName: " + operationDescription.SyncMethod.GetBaseDefinition().GetBaseDefinition().ToString());
        }

        void IOperationBehavior.ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            CadMessageInspector inspector = new CadMessageInspector(OriginType.CAD, (EnvironmentType)Enum.Parse(typeof(EnvironmentType), ConfigurationManager.AppSettings.Get("Env")));
            inspector.Operation = operationDescription.SyncMethod.GetBaseDefinition().GetBaseDefinition().ToString();
            dispatchOperation.ParameterInspectors.Add(inspector);

            Log("CadOperationBehavior", "IOperationBehavior.ApplyDispatchBehavior()", "OperationName: " + operationDescription.SyncMethod.GetBaseDefinition().GetBaseDefinition().ToString());
        }

        void IOperationBehavior.Validate(OperationDescription operationDescription)
        {
            Log("CadOperationBehavior", "IOperationBehavior.Validate()", "OperationName: " + operationDescription.SyncMethod.GetBaseDefinition().GetBaseDefinition().ToString());
        }

        private static void Log(string className, string functionName, string description)
        {
            try
            {
                using (new WrapperImpersonationContext("WE", "", "").Context)
                {
                    //Could do this too
                    //Logger.logDirectory = new DBManagerBase(OriginType.CADWEB, (EnvironmentType)Enum.Parse(typeof(EnvironmentType), ConfigurationManager.AppSettings.Get("Env"))).CadServiceLogDirectoryName;

                    Logger.logDirectory = CadServiceLogDirectoryName;
                    Logger.logFileName = CadServiceLogFileName;
                    Logger.Log(className, functionName, description);
                }
            }
            catch { }
        }
    }
}

