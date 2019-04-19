namespace Plisky.Boondoggle2 {
    using Plisky.Diagnostics;
    using Plisky.Plumbing;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Linq;

    public class bd2World : Bd2GeneralBase {


        protected Bd2Map activeMap;
        private int nextStartLoc = 0;

        public Bd2Map Map {
            get {
                return activeMap;
            }
        }

        public bd2World(Bd2Map desiredMap) {
            this.activeMap = desiredMap;
        }

        public Point ReturnNextStartLocation() {
            nextStartLoc++;
            Point result = activeMap.GetStartPosition(nextStartLoc);

            return result;
        }

        private const int QUAD_NE1 = 45;
        private const int QUAD_NE2 = 90;
        private const int QUAD_SE1 = 135;
        private const int QUAD_SE2 = 180;
        private const int QUAD_SW1 = 225;
        private const int QUAD_SW2 = 270;
        private const int QUAD_NW1 = 315;
        private const int QUAD_NW2 = 360;
        private const int VAL_RADSTODEGRESSDIV = 180;

        public virtual Point CalculateTargetPointFromSourceAndHeading(Point source, double heading) {
            b.Verbose.Log("Calcuating Target from(" + source.ToString() + ") Dir:" + heading.ToString());

            //Helpers.ValidateHeading(heading);

            double opposite = 0;
            double adjacent;
            double angle;
            int finalX;
            int finalY;

            if (heading <= QUAD_NE1) {
                angle = heading;
                angle = angle * Math.PI / VAL_RADSTODEGRESSDIV;
                adjacent = activeMap.Height - source.Y;
                opposite = adjacent * Math.Tan(angle);
                finalX = source.X + (int)Math.Round(opposite);
                finalY = activeMap.Height + 1;
            } else if (heading <= QUAD_NE2) {
                angle = QUAD_NE2 - heading;
                angle = angle * Math.PI / VAL_RADSTODEGRESSDIV;
                adjacent = activeMap.Width - source.X;
                opposite = adjacent * Math.Tan(angle);
                finalX = activeMap.Width + 1;
                finalY = source.Y + (int)Math.Round(opposite);
            } else if (heading <= QUAD_SE1) {
                angle = heading - QUAD_NE2;
                angle = angle * Math.PI / VAL_RADSTODEGRESSDIV;
                adjacent = activeMap.Width - source.X;
                finalX = activeMap.Width + 1;
                opposite = adjacent * Math.Tan(angle);
                finalY = source.Y - (int)Math.Round(opposite);
            } else if (heading <= QUAD_SE2) {
                angle = QUAD_SE2 - heading;
                angle = angle * Math.PI / VAL_RADSTODEGRESSDIV;
                adjacent = source.Y;
                opposite = adjacent * Math.Tan(angle);
                finalX = source.X + (int)Math.Round(opposite);
                finalY = 0;
            } else if (heading <= QUAD_SW1) {
                angle = heading - QUAD_SE2;
                angle = angle * Math.PI / VAL_RADSTODEGRESSDIV;
                adjacent = source.Y;
                opposite = adjacent * Math.Tan(angle);
                finalX = source.X - (int)Math.Round(opposite);
                finalY = 0;
            } else if (heading <= QUAD_SW2) {
                angle = QUAD_SW2 - heading;
                angle = angle * Math.PI / VAL_RADSTODEGRESSDIV;
                adjacent = source.X;
                opposite = adjacent * Math.Tan(angle);
                finalX = 0;
                finalY = source.Y - (int)Math.Round(opposite);
            } else if (heading <= QUAD_NW1) {
                angle = heading - 270;
                angle = angle * Math.PI / VAL_RADSTODEGRESSDIV;
                adjacent = source.X;
                opposite = adjacent * Math.Tan(angle);
                finalY = source.Y + (int)Math.Round(opposite);
                finalX = 0;
            } else {
                angle = QUAD_NW2 - heading;
                angle = angle * Math.PI / VAL_RADSTODEGRESSDIV;
                adjacent = activeMap.Height - source.Y;
                opposite = adjacent * Math.Tan(angle);
                finalX = source.X - (int)Math.Round(opposite);
                finalY = activeMap.Height + 1;
            }

            return (new Point(finalX, finalY));
        }

        public virtual IEnumerable<Point> EnumerateAllPointsOnRoute_Bresnam(Point initial, Point destination, int cutoff) {
            int returnedPoints = 0;
            int dx, dy, err, e2;
            int sx, sy;

            dx = Math.Abs(initial.X - destination.X);
            dy = Math.Abs(initial.Y - destination.Y);

            if (initial.X < destination.X) { sx = 1; } else { sx = -1; }
            if (initial.Y < destination.Y) { sy = 1; } else { sy = -1; }

            err = dx - dy;
            while (true) {
                if (returnedPoints >= cutoff) { yield break; }
                returnedPoints++;
                yield return new Point(initial.X, initial.Y);

                if ((initial.X == destination.X) && (initial.Y == destination.Y)) { break; }

                e2 = 2 * err;

                if (e2 > -dy) {
                    err = err - dy;
                    initial.X = initial.X + sx;
                }
                if (e2 < dx) {
                    err = err + dx;
                    initial.Y = initial.Y + sy;
                }
            }
        }

        public virtual Point CalculateNextPositionForObject(Point point, double heading) {
            if ((point.X >= Map.Width) || (point.Y >= Map.Height)) {
                // TODO : Unit test
                throw new InvalidOperationException("Requested point is out of range");
            }
            var pt = CalculateTargetPointFromSourceAndHeading(point, heading);
            
            b.Verbose.Log("Calculating next point starting " + point.ToString() + " heading: " + heading.ToString());
            b.Assert.True(pt != point, "The next point is the same as the current point, thats a fault.");
            

            return EnumerateAllPointsOnRoute_Bresnam(point, pt, 2).Skip(1).First();
        }

        public virtual bool IsFreeWorldSpace(Point desiredLocation) {
            return IsValidSpace(desiredLocation) && activeMap.GetTileAtPosition(desiredLocation) == MapTile.DefaultGround;
        }

        public virtual bool IsValidSpace(Point desiredLocation) {
            if ((desiredLocation.X <= 0) || (desiredLocation.X > activeMap.Width)) {
                return false;
            }
            if ((desiredLocation.Y <= 0) || (desiredLocation.Y > activeMap.Height)) {
                return false;
            }
            return true;
        }

        public virtual  bool IsLOSBetween(Point sourceLoc, Point destLoc) {
            foreach (var v in EnumerateAllPointsOnRoute_Bresnam(sourceLoc, destLoc, 200)) {
                if (!IsFreeWorldSpace(v)) {
                    return false;
                }
            }
            return true;
        }

        internal int GetDistanceBetween(Point point1, Point point2) {
            return 1;
        }
    }
}