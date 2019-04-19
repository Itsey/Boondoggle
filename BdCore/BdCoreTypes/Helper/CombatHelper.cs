namespace Plisky.Boondoggle2 {

    using System;
    using System.Drawing;

    public class CombatHelper {
        public static double CalculateDirectonToRelativePoint(Point point) {

            if ((point.X == 0) && (point.Y == 0)) {
                throw new BdBaseException("Dont be silly");
            }


            double calcVal;
            double heading;
            double mod = 0;

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