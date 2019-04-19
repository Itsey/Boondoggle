using System;

namespace Plisky.Boondoggle2 {

    public class CombatAttack {
        public MappedBot Victim { get; set; }

        public MappedBot Attacker { get; set; }

        public OffensiveWeaponEquipmentItem Weapon { get; set; }

        public MountPoint WeaponMount { get; set; }

        public ActiveEquipment WeaponInstance { get; set; }

        internal void Validate() {
            if (Victim==null) {
                throw new BdBaseException("victim must be set for a valid combatAttack");
            }
            if (Attacker == null) {
                throw new BdBaseException("attacker must be set for a valid combatAttack");
            }
            if (WeaponInstance == null) {
                throw new BdBaseException("weapon instance missing");
            }
            if (Weapon == null) {
                throw new BdBaseException("attacker must use a weapon");
            }
        }
    }
}