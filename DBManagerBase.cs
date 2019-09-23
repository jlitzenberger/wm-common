using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Win32;

namespace WM.Common
{
    public enum OriginType
    {
        STORMS,
        CAD,
        ADS,
        CO
    }
    public enum EnvironmentType
    {
        PRD,
        STG,
        DEV
    }

    public partial class DBManagerBase
    {
        public OriginType iOrigin { get; set; }
        public EnvironmentType iEnvironment { get; set; }

        public string iDatabase { get; set; }

        public Uri iInboundJobUri { get; set; }
        public Uri iInboundDataItemUri { get; set; }
        public Uri iInboundAssignmentUri { get; set; }
        public Uri iInboundPersonnelUri { get; set; }
        public Uri iInboundAvailabilityUri { get; set; }
        
        public string OLEDB_ConnectStr { get; set; }

        public string CadServiceLogFileName { get; set; }
        public string CadServiceLogDirectoryName { get; set; }
        public string CadServiceLogHttpMessageFileName { get; set; }

        public string ServiceLogFileName { get; set; }
        public string ServiceLogDirectoryName { get; set; }
        public string ServiceLogHttpMessageFileName { get; set; }

        public string ServiceLoggerDomain { get; set; }
        public string ServiceLoggerUser { get; set; }
        public string ServiceLoggerPassword { get; set; }

        public string CadApplicationServerName { get; set; }

        public string CadAttachmentRepository { get; set; }
        public string CadLocalAttachmentRepository { get; set; }

        public string Oracle_ConnectStr;


        public Uri AdsApiUri { get; set; }
        private Uri StormsApiUri { get; set; }
        private Uri FieldOrderApiUri { get; set; }
        public Uri CoApiUri { get; set; }
        public System.Net.WebRequest StormsWebRequest { get; set; }
        public System.Net.WebRequest FieldOrderWebRequest { get; set; }
        private string StormsApiCredentials { get; set; }
        private string FieldOrderApiCredentials { get; set; }


        public DBManagerBase() { }

        public DBManagerBase(OriginType objOrigin, EnvironmentType objEnvironment)
        {
            iOrigin = objOrigin;
            iEnvironment = objEnvironment;

            switch (iOrigin)
            {
                case OriginType.STORMS:
                    switch (iEnvironment)
                    {
                        case EnvironmentType.PRD:
                            OLEDB_ConnectStr = WM.Common.Properties.Settings.Default.dbStormsOLE_Prod;
                            Oracle_ConnectStr = WM.Common.Properties.Settings.Default.OracleConnection_STORMS_PRD;

                            ServiceLoggerDomain = WM.Common.Properties.Settings.Default.LoggerDomain_PRD;
                            ServiceLoggerUser = WM.Common.Properties.Settings.Default.LoggerUser_PRD;
                            ServiceLoggerPassword = WM.Common.Properties.Settings.Default.LoggerPassword_PRD;

                            CadApplicationServerName = WM.Common.Properties.Settings.Default.CadServer_PRD;
                            
                            StormsApiUri = new Uri(WM.Common.Properties.Settings.Default.StormsWebApiUri_PRD.ToString());
                            StormsApiCredentials = WM.Common.Properties.Settings.Default.STORMS_Api_PRD.ToString();

                            break;
                        case EnvironmentType.STG:
                            OLEDB_ConnectStr = WM.Common.Properties.Settings.Default.dbStormsOLE_Test;
                            Oracle_ConnectStr = WM.Common.Properties.Settings.Default.OracleConnection_STORMS_STG;

                            ServiceLoggerDomain = WM.Common.Properties.Settings.Default.LoggerDomain_STG;
                            ServiceLoggerUser = WM.Common.Properties.Settings.Default.LoggerUser_STG;
                            ServiceLoggerPassword = WM.Common.Properties.Settings.Default.LoggerPassword_STG;

                            CadApplicationServerName = WM.Common.Properties.Settings.Default.CadServer_STG;
                            
                            StormsApiUri = new Uri(WM.Common.Properties.Settings.Default.StormsWebApiUri_STG.ToString());
                            StormsApiCredentials = WM.Common.Properties.Settings.Default.STORMS_Api_STG.ToString();

                            break;
                        case EnvironmentType.DEV:
                            OLEDB_ConnectStr = WM.Common.Properties.Settings.Default.dbStormsOLE_Dev;
                            Oracle_ConnectStr = WM.Common.Properties.Settings.Default.OracleConnection_STORMS_DEV;

                            ServiceLoggerDomain = WM.Common.Properties.Settings.Default.LoggerDomain_DEV;
                            ServiceLoggerUser = WM.Common.Properties.Settings.Default.LoggerUser_DEV;
                            ServiceLoggerPassword = WM.Common.Properties.Settings.Default.LoggerPassword_DEV;

                            CadApplicationServerName = WM.Common.Properties.Settings.Default.CadServer_DEV;
                            



                            StormsApiUri = new Uri(WM.Common.Properties.Settings.Default.StormsWebApiUri_DEV.ToString());
                            StormsApiCredentials = WM.Common.Properties.Settings.Default.STORMS_Api_DEV.ToString();
                            System.Net.NetworkCredential myCred = new System.Net.NetworkCredential("xxxxx", "xxxxx", "WE"); 
                            System.Net.CredentialCache myCache = new System.Net.CredentialCache();
                            myCache.Add(new Uri(StormsApiUri.AbsoluteUri), "Basic", myCred);
                            StormsWebRequest = System.Net.WebRequest.Create(new Uri(StormsApiUri.AbsoluteUri));
                            StormsWebRequest.Credentials = myCache;





                            break;

                        default:
                                                        
                            break;
                    }
                    break;
                case OriginType.CAD:
                    switch (iEnvironment)
                    {
                        case EnvironmentType.PRD:
                            OLEDB_ConnectStr = WM.Common.Properties.Settings.Default.dbCadOLE_Prod;
                            Oracle_ConnectStr = WM.Common.Properties.Settings.Default.OracleConnection_CAD_PRD;

                            iInboundJobUri = new Uri(WM.Common.Properties.Settings.Default.WM_Common_InboundJobService_InboundJobService.ToString());
                            iInboundDataItemUri = new Uri(WM.Common.Properties.Settings.Default.WM_Common_InboundDataItemService_InboundDataItemService.ToString());
                            iInboundAssignmentUri = new Uri(WM.Common.Properties.Settings.Default.WM_Common_InboundAssignmentService_InboundAssignmentService.ToString());
                            iInboundPersonnelUri = new Uri(WM.Common.Properties.Settings.Default.WM_Common_InboundPersonnelService_InboundPersonnelService.ToString());
                            iInboundAvailabilityUri = new Uri(WM.Common.Properties.Settings.Default.WM_Common_InboundAvailabilityService_InboundAvailabilityService.ToString());

                            ServiceLogFileName = WM.Common.Properties.Settings.Default.CadServiceLogFileName_PRD;
                            ServiceLogDirectoryName = WM.Common.Properties.Settings.Default.CadServiceLogDirectoryName_PRD;
                            ServiceLogHttpMessageFileName = WM.Common.Properties.Settings.Default.CadServiceLogHttpMessageFileName_PRD;

                            ServiceLoggerDomain = WM.Common.Properties.Settings.Default.LoggerDomain_PRD;
                            ServiceLoggerUser = WM.Common.Properties.Settings.Default.LoggerUser_PRD;
                            ServiceLoggerPassword = WM.Common.Properties.Settings.Default.LoggerPassword_PRD;

                            CadApplicationServerName = WM.Common.Properties.Settings.Default.CadServer_PRD;

                            break;
                        case EnvironmentType.STG:
                            OLEDB_ConnectStr = WM.Common.Properties.Settings.Default.dbCadOLE_Stage;
                            Oracle_ConnectStr = WM.Common.Properties.Settings.Default.OracleConnection_CAD_STG;

                            iInboundJobUri = new Uri(WM.Common.Properties.Settings.Default.WM_Common_InboundJobService_InboundJobService_STG.ToString());
                            iInboundDataItemUri = new Uri(WM.Common.Properties.Settings.Default.WM_Common_InboundDataItemService_InboundDataItemService_STG.ToString());
                            iInboundAssignmentUri = new Uri(WM.Common.Properties.Settings.Default.WM_Common_InboundAssignmentService_InboundAssignmentService_STG.ToString());
                            iInboundPersonnelUri = new Uri(WM.Common.Properties.Settings.Default.WM_Common_InboundPersonnelService_InboundPersonnelService_STG.ToString());
                            iInboundAvailabilityUri = new Uri(WM.Common.Properties.Settings.Default.WM_Common_InboundAvailabilityService_InboundAvailabilityService_STG.ToString());

                            ServiceLogFileName = WM.Common.Properties.Settings.Default.CadServiceLogFileName_STG;
                            ServiceLogDirectoryName = WM.Common.Properties.Settings.Default.CadServiceLogDirectoryName_STG;
                            ServiceLogHttpMessageFileName = WM.Common.Properties.Settings.Default.CadServiceLogHttpMessageFileName_STG;

                            ServiceLoggerDomain = WM.Common.Properties.Settings.Default.LoggerDomain_STG;
                            ServiceLoggerUser = WM.Common.Properties.Settings.Default.LoggerUser_STG;
                            ServiceLoggerPassword = WM.Common.Properties.Settings.Default.LoggerPassword_STG;

                            CadApplicationServerName = WM.Common.Properties.Settings.Default.CadServer_STG;

                            break;
                        case EnvironmentType.DEV:
                            OLEDB_ConnectStr = WM.Common.Properties.Settings.Default.dbCadOLE_Dev;
                            Oracle_ConnectStr = WM.Common.Properties.Settings.Default.OracleConnection_CAD_DEV;

                            iInboundJobUri = new Uri(WM.Common.Properties.Settings.Default.WM_Common_InboundJobService_InboundJobService_DEV.ToString());
                            iInboundDataItemUri = new Uri(WM.Common.Properties.Settings.Default.WM_Common_InboundDataItemService_InboundDataItemService_DEV.ToString());
                            iInboundAssignmentUri = new Uri(WM.Common.Properties.Settings.Default.WM_Common_InboundAssignmentService_InboundAssignmentService_DEV.ToString());
                            iInboundPersonnelUri = new Uri(WM.Common.Properties.Settings.Default.WM_Common_InboundPersonnelService_InboundPersonnelService_DEV.ToString());
                            iInboundAvailabilityUri = new Uri(WM.Common.Properties.Settings.Default.WM_Common_InboundAvailabilityService_InboundAvailabilityService_DEV.ToString());

                            ServiceLogFileName = WM.Common.Properties.Settings.Default.CadServiceLogFileName_DEV;
                            ServiceLogDirectoryName = WM.Common.Properties.Settings.Default.CadServiceLogDirectoryName_DEV;
                            ServiceLogHttpMessageFileName = WM.Common.Properties.Settings.Default.CadServiceLogHttpMessageFileName_DEV;

                            ServiceLoggerDomain = WM.Common.Properties.Settings.Default.LoggerDomain_DEV;
                            ServiceLoggerUser = WM.Common.Properties.Settings.Default.LoggerUser_DEV;
                            ServiceLoggerPassword = WM.Common.Properties.Settings.Default.LoggerPassword_DEV;

                            CadApplicationServerName = WM.Common.Properties.Settings.Default.CadServer_DEV;

                            CadAttachmentRepository = WM.Common.Properties.Settings.Default.CadAttachmentRepository_DEV;
                            CadLocalAttachmentRepository = WM.Common.Properties.Settings.Default.CadLocalAttachmentRepository_DEV;



                            FieldOrderApiUri = new Uri(WM.Common.Properties.Settings.Default.StormsWebApiUri_DEV.ToString());
                            FieldOrderApiCredentials = WM.Common.Properties.Settings.Default.FieldOrderApi64EncodedCredentials_DEV.ToString();
                            System.Net.NetworkCredential myCred = new System.Net.NetworkCredential("xxxxx", "xxxxx", "WE");
                            System.Net.CredentialCache myCache = new System.Net.CredentialCache();
                            myCache.Add(new Uri(FieldOrderApiUri.AbsoluteUri), "Basic", myCred);
                            FieldOrderWebRequest = System.Net.WebRequest.Create(new Uri(FieldOrderApiUri.AbsoluteUri));
                            FieldOrderWebRequest.Credentials = myCache;

                            break;
                        default:
                            OLEDB_ConnectStr = WM.Common.Properties.Settings.Default.dbCadOLE_Prod;
                            Oracle_ConnectStr = WM.Common.Properties.Settings.Default.OracleConnection_CAD_PRD;

                            iInboundJobUri = new Uri(WM.Common.Properties.Settings.Default.WM_Common_InboundJobService_InboundJobService.ToString());
                            iInboundDataItemUri = new Uri(WM.Common.Properties.Settings.Default.WM_Common_InboundDataItemService_InboundDataItemService.ToString());
                            iInboundAssignmentUri = new Uri(WM.Common.Properties.Settings.Default.WM_Common_InboundAssignmentService_InboundAssignmentService.ToString());
                            iInboundPersonnelUri = new Uri(WM.Common.Properties.Settings.Default.WM_Common_InboundPersonnelService_InboundPersonnelService.ToString());
                            iInboundAvailabilityUri = new Uri(WM.Common.Properties.Settings.Default.WM_Common_InboundAvailabilityService_InboundAvailabilityService.ToString());

                            break;
                    }

                    break;
                case OriginType.ADS:
                    switch (iEnvironment)
                    {
                        case EnvironmentType.DEV:
                            AdsApiUri = new Uri(WM.Common.Properties.Settings.Default.AdsWebApiUri_DEV.ToString());
                            //AdsApiUri = new Uri("http://localhost/WMServices.ADSService/api/");

                            break;
                        case EnvironmentType.STG:
                            AdsApiUri = new Uri(WM.Common.Properties.Settings.Default.AdsWebApiUri_STG.ToString());

                            break;
                        case EnvironmentType.PRD:
                            AdsApiUri = new Uri(WM.Common.Properties.Settings.Default.AdsWebApiUri_PRD.ToString());

                            break;

                        default:
                            break;
                    }

                    break;

                case OriginType.CO:
                    switch (iEnvironment) {
                        case EnvironmentType.DEV:
                            CoApiUri = new Uri(WM.Common.Properties.Settings.Default.CoWebApiUri_DEV.ToString());

                            break;
                        case EnvironmentType.STG:
                            CoApiUri = new Uri(WM.Common.Properties.Settings.Default.CoWebApiUri_STG.ToString());

                            break;
                        case EnvironmentType.PRD:
                            CoApiUri = new Uri(WM.Common.Properties.Settings.Default.CoWebApiUri_PRD.ToString());

                            break;

                        default:
                            break;
                    }

                    break;
                default:
                    break;
            }
        }
    }
}