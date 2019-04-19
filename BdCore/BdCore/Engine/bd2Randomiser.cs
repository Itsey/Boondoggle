namespace Plisky.Boondoggle2 {

    using System;
    using System.Drawing;

    public abstract class Bd2CombatCalculator {

        protected abstract int ActualGetD10();

        protected abstract int ActualGetD100();

        protected abstract bool ActualCanMountPointHitTarget(double sourceHeading, MountPoint mountPoint, Point sourceLoc, Point destLoc);
        

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


        public  bool CanMountPointHitTarget(double sourceHeading, MountPoint mountPoint, Point sourceLoc, Point destLoc) {
            if (mountPoint == MountPoint.Turret) { return true; }
            if (mountPoint == MountPoint.Internal) { return false; }

            return ActualCanMountPointHitTarget(sourceHeading,mountPoint,sourceLoc,destLoc);

          
        }



        
    }
}