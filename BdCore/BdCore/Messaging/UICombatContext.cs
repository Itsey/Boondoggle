namespace Plisky.Boondoggle2 {

    public class UICombatContext : BattleContextBase {
        public int AggressorId { get; set; }

        public int VictimId { get; set; }

        public int WeaponTypeId { get; set; }

        public int Damage { get; set; }

        public bool DidHit { get; set; }
    }
}