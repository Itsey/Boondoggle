namespace Plisky.Boondoggle2.Test {

    public class mockBd2Randomiser : bd2Randomiser {
        private int d10result;
        private int d100result;

        public void Mock_SetD10Result(int toThis) {
            d10result = toThis;
        }

        public void Mock_SetD100Result(int toThis) {
            d100result = toThis;
        }

        protected override int ActualGetD10() {
            return d10result;
        }

        protected override int ActualGetD100() {
            return d100result;
        }

        public mockBd2Randomiser() {
            d10result = d100result = 0;
        }
    }
}