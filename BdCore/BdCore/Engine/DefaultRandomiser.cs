namespace Plisky.Boondoggle2 {

    using System;

    public class DefaultRandomiser : bd2Randomiser {
        private Random r = new Random();

        protected override int ActualGetD10() {
            return r.Next(10) + 1;
        }

        protected override int ActualGetD100() {
            return r.Next(100) + 1;
        }
    }
}