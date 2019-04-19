namespace Plisky.Boondoggle2 {

    using System;
    using System.Drawing;

    public class DefaultRandomiser : Bd2CombatCalculator {
        private Random r = new Random();

      

        protected override bool ActualCanMountPointHitTarget(double sourceHeading, MountPoint mountPoint, Point sourceLoc, Point destLoc) {
            Point relpt = new Point(destLoc.X - sourceLoc.X, destLoc.Y - sourceLoc.Y);
            double head = CombatHelper.CalculateDirectonToRelativePoint(relpt);

            if ((head < 0) || (head > 360)) {
                throw new BdBaseException("Validation fail, incorrect heading");
            }

            if ((sourceHeading > 315) || (sourceHeading <= 45)) {
                switch (mountPoint) {
                    case MountPoint.Forward: return (head >= 315) || (head <= 45);
                    case MountPoint.Backward: return (head >= 135) && (head <= 225);
                    case MountPoint.Nearside: return (head >= 45) && (head <= 135);
                    case MountPoint.Offside: return (head >= 225) & (head <= 315);
                    default: throw new BdBaseException("Not mapped mount point, invalid test");
                }
            } else if ((sourceHeading > 45) && (sourceHeading <= 135)) {
                switch (mountPoint) {
                    case MountPoint.Offside: return (head >= 315) || (head <= 45);
                    case MountPoint.Nearside: return (head >= 135) && (head <= 225);
                    case MountPoint.Forward: return (head >= 45) && (head <= 135);
                    case MountPoint.Backward: return (head >= 225) & (head <= 315);
                    default: throw new BdBaseException("Not mapped mount point, invalid test");
                }
            } else if ((sourceHeading > 135) && (sourceHeading <= 225)) {
                switch (mountPoint) {
                    case MountPoint.Backward: return (head >= 315) || (head <= 45);
                    case MountPoint.Forward: return (head >= 135) && (head <= 225);
                    case MountPoint.Offside: return (head >= 45) && (head <= 135);
                    case MountPoint.Nearside: return (head >= 225) & (head <= 315);
                    default: throw new BdBaseException("Not mapped mount point, invalid test");
                }
            } else {
                switch (mountPoint) {
                    case MountPoint.Nearside: return (head >= 315) || (head <= 45);
                    case MountPoint.Offside: return (head >= 135) && (head <= 225);
                    case MountPoint.Backward: return (head >= 45) && (head <= 135);
                    case MountPoint.Forward: return (head >= 225) & (head <= 315);
                    default: throw new BdBaseException("Not mapped mount point, invalid test");
                }
            }
        }

        protected override int ActualGetD10() {
            return r.Next(10) + 1;
        }

        protected override int ActualGetD100() {
            return r.Next(100) + 1;
        }
    }
}