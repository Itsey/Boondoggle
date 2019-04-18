namespace Plisky.Boondoggle2 {

    public class OffensiveWeaponEquipmentItem : EquipmentItem {
        public int InitialAmmunition { get; set; }

        public int D10DamageRolls { get; set; }

        public int DamageModifier { get; set; }

        public int BaseHitChance { get; set; }

        public int BaseChargeCost { get; set; }

        public int BaseWeight { get; set; }
    }
}