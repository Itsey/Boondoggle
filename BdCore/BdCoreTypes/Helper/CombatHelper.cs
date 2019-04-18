namespace Plisky.Boondoggle2 {

    using System;
    using System.Drawing;

    public class CombatHelper {

        public static bool CanMountPointHitTarget(double sourceHeading, MountPoint mountPoint, Point sourceLoc, Point destLoc) {
            if (mountPoint == MountPoint.Turret) { return true; }
            if (mountPoint == MountPoint.Internal) { return false; }

            Point relpt = new Point(destLoc.X - sourceLoc.X, destLoc.Y - sourceLoc.Y);
            double head = CalculateDirectonToRelativePoint(relpt);

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

        internal static double CalculateDirectonToRelativePoint(Point point) {
            double calcVal = 0;
            double heading = 0;
            double mod = 0;

            if ((point.X == 0) && (point.Y == 0)) {
                throw new BdBaseException("Dont be silly");
            }

            if (point.X == 0) {
                if (point.Y > 0) {
                    return 0;
                } else {
                    return 180;
                }
            }

            if (point.Y == 0) {
                if (point.X < 0) {
                    return 270;
                } else {
                    return 90;
                }
            }

            double xPart = Math.Abs(point.X);
            double yPart = Math.Abs(point.Y);

            if ((point.X > 0) && (point.Y > 0)) {
                // X/Y * Tan -1
                calcVal = xPart / yPart;
            } else if ((point.X > 0) && (point.Y < 0)) {
                // Y/X * tan-1 + 90
                calcVal = yPart / xPart;
                mod = 90;
            } else if ((point.X < 0) && (point.Y < 0)) {
                // X/Y * tan-1 + 270
                calcVal = xPart / yPart;
                mod = 180;
            } else {
                // 360 - X/Y * tan -1
                calcVal = xPart / yPart;
                mod = 361; // frig
            }

            heading = Math.Atan(calcVal);
            heading = heading * 180;
            heading /= Math.PI;
            if (mod == 361) {
                heading = 360 - heading;
            } else {
                heading += mod;
            }
            //if (point.X < 0) { heading += 180;  }
            heading = Math.Round(heading, 2);
            return heading;
        }
    }
}