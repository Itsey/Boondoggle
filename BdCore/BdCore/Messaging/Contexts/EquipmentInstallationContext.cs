using System;

namespace Plisky.Boondoggle2 {

    public class EquipmentInstallationContext {
        public Guid OwningBotIdentity { get; set; }

        public int EquipmentIdentifier { get; set; }

        public MountPoint MountPoint { get; set; }
    }
}