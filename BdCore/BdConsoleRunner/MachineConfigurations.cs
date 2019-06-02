namespace Plisky.Boondoggle2.Runner {

    using Plisky.Plumbing;
    using System;
    using System.IO;

    public class MachineConfigurations {

        public static void PerformMachineConfig() {
            string pathToUse = Environment.ExpandEnvironmentVariables("%PLISKYAPPROOT%");
            if (!Directory.Exists(pathToUse)) {
                ConfigHub.Current.RegisterProvider("MapPathName", () => {
                    return Environment.CurrentDirectory;
                });
            } else {
                pathToUse = Path.Combine(pathToUse, "bdConfig");
                ConfigHub.Current.AddDirectoryFallbackProvider(pathToUse, "generic.chcfg");
            }
            //ConfigHub.Current.AddDefaultAppConfigFallback();
        }
    }
}