using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;

namespace WM.Common.Services.WCF
{
    public static class Logger {
        public static string logDirectory { set; get; }
        public static string logFileName { set; get; }
        public static string messageFileName { set; get; }

        public static void LogHttpMessageRequest(ref System.ServiceModel.Channels.Message request) {
            int messageLogFileIndex = 0;
            string tempMessageFileName = string.Format("{0}" + logFileName, logDirectory, System.Threading.Interlocked.Increment(ref messageLogFileIndex));
            Uri requestUri = request.Headers.To;

            using (StreamWriter sw = new StreamWriter(tempMessageFileName, true)) {
                HttpRequestMessageProperty httpReq = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];

                sw.WriteLine("-------------------------------------------------------------------------------------------");
                sw.WriteLine("--(HTTP REQUEST)---------------------------------------------------------------------------");
                sw.WriteLine("---DATE/TIME: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " --------------------------------------------------------------");
                sw.WriteLine("---SOAP MessageFileName: " + messageFileName + " --------------------------");

                sw.WriteLine("    {0} {1}", httpReq.Method, requestUri);
                foreach (var header in httpReq.Headers.AllKeys) {
                    sw.WriteLine("    {0}: {1}", header, httpReq.Headers[header]);
                }
            }
        }

        public static void LogHttpMessageResponse(ref System.ServiceModel.Channels.Message response) {
            int messageLogFileIndex = 0;
            string tempMessageFileName = string.Format("{0}" + logFileName, logDirectory, System.Threading.Interlocked.Increment(ref messageLogFileIndex));

            using (StreamWriter sw = new StreamWriter(tempMessageFileName, true)) {
                sw.WriteLine("--(HTTP RESPONSE)---------------------------------------------------------------------------");
                sw.WriteLine("---DATE/TIME: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ---------------------------------------------------------------");
                sw.WriteLine("---SOAP MessageFileName: " + messageFileName + " --------------------------");

                sw.WriteLine("    {0} {1}", "SOAPAction: ", response.Headers.Action);
            }
        }

        public static void LogMessage(ref System.ServiceModel.Channels.Message msg) {
            int messageLogFileIndex = 0;
            string messageFileName = string.Format("{0}{1:000}_" + logFileName, logDirectory, System.Threading.Interlocked.Increment(ref messageLogFileIndex));

            using (StreamWriter sw = File.CreateText(messageFileName)) {
                if (!msg.IsEmpty) {
                    sw.WriteLine(MessageUtility.MessageToString(ref msg));
                }
            }
        }

        public static void Log(string message) {
            using (var stream = new StreamWriter(logDirectory + logFileName, true)) {
                stream.WriteLine(message);

                stream.Flush();
                stream.Close();
            }
        }
        public static void Log(string className, string methodName, string parameter) {
            Log(string.Format("@ {3} - {0} called method {1} with parameter {2} ", className, methodName, parameter, DateTime.Now));
        }

    }
}

