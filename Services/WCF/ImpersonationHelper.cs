using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WM.Common.Security.Impersonate;

namespace WM.Common.Services.WCF
{
    public class ImpersonationHelper : DBManagerBase {
        public ImpersonationHelper(OriginType iOrigin, EnvironmentType iEnvironment)
            : base(iOrigin, iEnvironment) { }

        public System.IO.FileStream load(string filename) {
            System.IO.FileStream stream = null;
            try {
                using (new WrapperImpersonationContext(this.ServiceLogDirectoryName, this.ServiceLoggerUser, this.ServiceLoggerPassword).Context) {
                    stream = new System.IO.FileStream(this.ServiceLogDirectoryName + filename, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);
                }
            } catch (System.IO.IOException) {
            }
            return stream;
        }
        public string loadFileName(string filename) {
            string pathfilename = string.Empty;
            try {
                using (new WrapperImpersonationContext(this.ServiceLoggerDomain, this.ServiceLoggerUser, this.ServiceLoggerPassword).Context) {
                    pathfilename = this.ServiceLogDirectoryName + filename;
                }
            } catch (System.IO.IOException) {
            }
            return pathfilename;
        }
    }
}
