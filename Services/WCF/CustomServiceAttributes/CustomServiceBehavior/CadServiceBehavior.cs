using System;
using System.ServiceModel.Description;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.Xml.Schema;
using System.IO;
using System.Linq;

using WM.Common.XML;
using System.Xml;
using System.Configuration;
using WM.Common.Services.WCF.CustomServiceAttributes;
using System.Collections.Generic;
using WM.Common.Security.Impersonate;

namespace WM.Common.Services.WCF.CustomServiceAttributes.CustomServiceBehavior
{
    /// <summary>
    /// This Class provide an attribute that need to be applied on 
    /// data service class in order to enable custom handler
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CadServiceBehavior : Attribute, IServiceBehavior
    {
        public static string CadServiceLogFileName { get; set; }
        public static string CadServiceLogDirectoryName { get; set; }
        public static string CadServiceLogHttpMessageFileName { get; set; }

        public CadServiceBehavior()
        {
            DBManagerBase dbm = new DBManagerBase(OriginType.CAD, (EnvironmentType)Enum.Parse(typeof(EnvironmentType), ConfigurationManager.AppSettings.Get("Env")));

            CadServiceLogFileName = dbm.CadServiceLogFileName;
            CadServiceLogDirectoryName = dbm.CadServiceLogDirectoryName;
            CadServiceLogHttpMessageFileName = dbm.CadServiceLogHttpMessageFileName;
        }

        /////////////////////////////////////////////////////////////////////////////////
        //These 3 implementations for IServiceBehavior get called on the first call only
        void IServiceBehavior.AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
            //if (endpoints.Count > 0)
            //{
            //    if (endpoints[0].Name == "BasicHttpBinding_ICADService")
            //    {
            //        WMCommon.Utils.Services.WCF.CustomServiceAttributes.CustomOperationBehavior.CadOperationBehavior cob = new CustomOperationBehavior.CadOperationBehavior();
            //        foreach (var item in serviceDescription.Endpoints[0].Contract.Operations)
            //        {
            //            if (lstOperations.Any(s => s == item.Name))
            //            {
            //                item.Behaviors.Add(cob);
            //                Log("CadServiceBehavior", "IServiceBehavior.AddBindingParameters()", "ServiceName: " + serviceDescription.ServiceType.FullName);
            //            }
            //        }
            //    }
            //}
        }
        void IServiceBehavior.ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            //foreach (ChannelDispatcher cd in serviceHostBase.ChannelDispatchers)
            //{
            //    foreach (EndpointDispatcher ed in cd.Endpoints)
            //    {
            //        if (serviceDescription.ServiceType.Name == "CADService")
            //        {
            //            ed.DispatchRuntime.MessageInspectors.Add(new CustomMessageInspectors.CadMessageInspector(OriginType.CADWEB, (EnvironmentType)Enum.Parse(typeof(EnvironmentType), ConfigurationManager.AppSettings.Get("Env"))));

            //            Log("CadServiceBehavior", "IServiceBehavior.ApplyDispatchBehavior()", "ContractName: " + ed.ContractName + " EndpointAddress: " + ed.EndpointAddress);
            //        }
            //    }
            //}
        }
        void IServiceBehavior.Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            //Log("CadServiceBehavior", "IServiceBehavior.Validate()", "ServiceName: " + serviceDescription.ServiceType.FullName);
        }

        private static void Log(string className, string functionName, string description)
        {
            try
            {
                using (new WrapperImpersonationContext("WE", "", "").Context)
                {
                    Logger.logDirectory = CadServiceLogDirectoryName;
                    Logger.logFileName = CadServiceLogFileName;
                    Logger.Log(className, functionName, description);
                }
            }
            catch { }
        }
    }
}