namespace Plisky.Boondoggle2 {

    public class PowerPackEquipmentItem : EquipmentItem {
        private int lowSpeedPowerDrain = 1;
        private int medSpeedPowerDrain = 3;
        private int highSpeedPowerDrain = 5;

        public int ChargePerTurn { get; set; }

        public int TotalPower { get; set; }

        public int Acceleration { get; set; }

        internal int GetPowerDrain(SpeedRating speedRating) {
            switch (speedRating) {
                case SpeedRating.Slow: return lowSpeedPowerDrain;
                case SpeedRating.Medium: return medSpeedPowerDrain;
                case SpeedRating.Fast: return highSpeedPowerDrain;
                default:
                    throw new BdBaseException("Speed level not mapped for powerpack");
            }
        }

        public void SetPowerDrainLevels(int low, int medium, int high) {
            lowSpeedPowerDrain = low;
            medSpeedPowerDrain = medium;
            highSpeedPowerDrain = high;
        }

        public int MaxSpeed { get; set; }
    }
}