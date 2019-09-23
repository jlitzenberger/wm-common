using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using WM.Common.Services.WCF.CustomServiceAttributes.CustomMessageInspectors;

namespace WM.Common.Services.WCF.CustomServiceAttributes.CustomOperationBehavior
{
    public class MultispeakOperationBehavior : Attribute, IOperationBehavior, IParameterInspector {
        public MultispeakOperationBehavior() { }

        /////////////////////////////////////////////////////////////////////////////
        //These 2 implementations for IParameterInspector get called for every call
        void IParameterInspector.AfterCall(string operationName, object[] outputs, object returnValue, object correlationState) {
            Log("MultispeakOperationBehavior", "AfterCall()", operationName);
        }
        object IParameterInspector.BeforeCall(string operationName, object[] inputs) {
            Log("MultispeakOperationBehavior", "BeforeCall()", operationName);

            //some validation logic

            return null;
        }

        ////////////////////////////////////////////////////////////////////////////////////
        //These 4 implementations for IOperationBehavior get called on the first call only
        void IOperationBehavior.AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters) {
            Log("MultispeakOperationBehavior", "AddBindingParameters()", operationDescription.Name);
        }

        void IOperationBehavior.ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation) {
            MultispeakMessageInspector inspector = new MultispeakMessageInspector();
            clientOperation.ParameterInspectors.Add(this);

            Log("MultispeakOperationBehavior", "ApplyClientBehavior()", operationDescription.Name);
        }

        void IOperationBehavior.ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation) {
            MultispeakMessageInspector inspector = new MultispeakMessageInspector();
            dispatchOperation.ParameterInspectors.Add(this);

            Log("MultispeakOperationBehavior", "ApplyDispatchBehavior()", operationDescription.Name);
        }

        void IOperationBehavior.Validate(OperationDescription operationDescription) {
            Log("MultispeakOperationBehavior", "Validate()", operationDescription.Name);
        }

        private static void Log(string className, string functionName, string description) {
            //Logger.logDirectory = WMCommon.Properties.Settings.Default.MultispeakLogDirectory_DEV;
            //Logger.logFileName = WMCommon.Properties.Settings.Default.MultispeakLogFileName_DEV;
            //Logger.Log(className, functionName, description);
        }
    }
}
