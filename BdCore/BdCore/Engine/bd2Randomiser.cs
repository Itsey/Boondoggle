namespace Plisky.Boondoggle2 {

    using System;

    public abstract class bd2Randomiser {

        protected abstract int ActualGetD10();

        protected abstract int ActualGetD100();

        public bool DidAchievePercentage(double hC) {
            int targetPercent = (int)Math.Round(hC);
            return ActualGetD100() <= targetPercent;
        }

        public int D10Rolls(int p) {
            int result = 0;
            for (; p > 0; p--) {
                result += ActualGetD10();
            }
            return result;
        }
    }
}