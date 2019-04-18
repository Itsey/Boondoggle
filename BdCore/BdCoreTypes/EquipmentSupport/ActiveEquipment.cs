namespace Plisky.Boondoggle2 {

    using System;

    public class ActiveEquipment {
        public EquipmentItem UnderlyingItem { get; private set; }
        public Guid InstanceId { get; set; }

        public Guid OwningBotId { get; set; }

        public int EquipmentId {
            get;
            set;
        }

        public ItemClassification Classification { get; set; }

        public MountPoint MountPoint { get; set; }
        public int UseCount { get ; set; }

        public int RoundsRemaining { get; set; }

        public int CooldownTicksRemaining { get; set; }

        public ActiveEquipment(EquipmentItem basicItem) {
            UseCount = 0;
            UnderlyingItem = basicItem;
        }
    }
}