using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace WM.Common.Security.Encryption {
    public class Miscellaneous {


        private static void EncryptConfigSection(string sectionKey) {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            ConfigurationSection section = config.GetSection(sectionKey);
            if (section != null) {
                if (!section.SectionInformation.IsProtected) {
                    if (!section.ElementInformation.IsLocked) {
                        section.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");
                        section.SectionInformation.ForceSave = true;
                        config.Save(ConfigurationSaveMode.Full);
                    }
                }
            }
        }
    }
}
