namespace Plisky.Boondoggle2 {

    internal class CombatAttack {
        public MappedBot Victim { get; set; }

        public MappedBot Attacker { get; set; }

        public OffensiveWeaponEquipmentItem Weapon { get; set; }

        public MountPoint WeaponMount { get; set; }

        public ActiveEquipment WeaponInstance { get; set; }
    }
}