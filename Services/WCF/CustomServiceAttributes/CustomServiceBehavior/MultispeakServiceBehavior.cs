using System;
using System.ServiceModel.Description;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.Xml.Schema;
using System.IO;

using WM.Common.XML;
using System.Xml;

namespace WM.Common.Services.WCF.CustomServiceAttributes.CustomServiceBehavior
{
    /// <summary>
    /// This Class provide an attribute that need to be applied on 
    /// data service class in order to enable custom handler
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class MultispeakServiceBehavior : Attribute, IServiceBehavior {
        /////////////////////////////////////////////////////////////////////////////////
        //These 3 implementations for IServiceBehavior get called on the first call only
        void IServiceBehavior.AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters) {
            //Nothing TO DO
            Log("MultispeakServiceBehavior", "AddBindingParameters()", serviceDescription.Name);
        }

        void IServiceBehavior.ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase) {
            foreach (ChannelDispatcher cd in serviceHostBase.ChannelDispatchers) {
                foreach (EndpointDispatcher ed in cd.Endpoints) {
                    if (serviceDescription.ServiceType.Name == "MultiSpeak") {
                        string xmlschema = XmlUtility.CreateXmlSchemaFromWSDL(AppDomain.CurrentDomain.BaseDirectory + "lib\\11_FA_Staking_MODIFIED.wsdl");
                        XmlSchemaSet schemaSet = XmlUtility.CreateXmlSchemaSet(xmlschema);

                        ed.DispatchRuntime.MessageInspectors.Add(new CustomMessageInspectors.MultispeakMessageInspector(schemaSet, true, false, false));

                        Log("MultispeakServiceBehavior", "ApplyDispatchBehavior()", serviceDescription.Name);
                    }
                }
            }
        }

        void IServiceBehavior.Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase) {
            //Nothing TO DO
            Log("MultispeakServiceBehavior", "Validate()", serviceDescription.Name);
        }

        private static void Log(string className, string functionName, string description) {
            //WrapperImpersonationContext context = new WrapperImpersonationContext("WE", "xxpc70490", "70490@We");
            //context.Enter();

            //Logger.logDirectory = WMCommon.Properties.Settings.Default.MultispeakLogDirectory_DEV;
            //Logger.logFileName = WMCommon.Properties.Settings.Default.MultispeakLogFileName_DEV;
            //Logger.Log(className, functionName, description);

            //context.Leave();
        }

    }
}

