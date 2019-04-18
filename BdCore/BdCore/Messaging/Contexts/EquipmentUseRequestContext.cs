using System;

namespace Plisky.Boondoggle2 {

    public class EquipmentUseRequestContext {
        public Guid OwningBotIdentity { get; set; }

        public Guid RequestedEquipmentInstance { get; set; }

        public int IParam { get; set; }
    }
}