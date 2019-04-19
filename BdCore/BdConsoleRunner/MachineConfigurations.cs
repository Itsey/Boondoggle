namespace Plisky.Boondoggle2.Runner {

    using Plisky.Plumbing;
    using System;
    using System.IO;

    public class MachineConfigurations {

        public static void PerformMachineConfig() {
            string pathToUse = Environment.ExpandEnvironmentVariables("%PLISKYAPPROOT%");
            pathToUse = Path.Combine(pathToUse, "bdConfig");
            ConfigHub.Current.AddDirectoryFallbackProvider(pathToUse,"generic.chcfg");
            //ConfigHub.Current.AddDefaultAppConfigFallback();
        }
    }
}