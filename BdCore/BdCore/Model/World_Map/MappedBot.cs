namespace Plisky.Boondoggle2 {

    public class MappedBot : MappedObject {
        public BoonBotBase Bot;

        public MappedBot(BoonBotBase desiredBot)
            : base() {
            Bot = desiredBot;
        }

        public int LifeRemaining { get; set; }

        public bool IsActive { get; set; }

        public int ChargeRemaining { get; set; }

        public int PowerRemaining { get; set; }

        public bool DeathNotificationOccured { get; set; }

        public int TurnsAccelerationActionsRemaining { get; set; }

        internal bool ConsumeCharge(int p) {
            if (this.ChargeRemaining > p) {
                this.ChargeRemaining -= p;
                return true;
            }
            ChargeRemaining = 0;
            return false;
        }

        public bool IsAlive() {
            return this.LifeRemaining > 0;
        }
    }
}