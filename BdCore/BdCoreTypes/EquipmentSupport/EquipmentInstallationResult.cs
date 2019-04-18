using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Plisky.Boondoggle2 {
    public class EquipmentInstallationResult {

        public int EquipmentId { get; set; } 
        public Guid InstanceId { get; set; }
        public InstallationResult Result { get; set; }
    }
}
