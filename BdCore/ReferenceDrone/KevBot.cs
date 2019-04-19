using Plisky.Boondoggle2;
using Plisky.Plumbing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace Plisky.Boondoggle2.Reference {

    public class KevBot : BoonBotBase {

        private EquipmentInstallationResult scanner;  // ALTERED BY JIM
        private List<Heading> headings, headingsSouth;
        public static Point North = new Point(0, 1);
        public static Point South = new Point(0, -1);
        public static Point East = new Point(1, 0);
        public static Point West = new Point(-1, 0);
        public static Point Northwest = new Point(-1, 1);
        public static Point Southwest = new Point(-1, -1);
        public static Point Northeast = new Point(1, 1);
        public static Point Southeast = new Point(1, -1);
        public static Point ScanCentrePoint = new Point(0, 0);
        public static Heading ScanCentre = new Heading(ScanCentrePoint);
        private Heading currentHeading;
        private Heading currentPosition;
        private bool rescan;
        private ScanEquipmentUseResult grid;
        private int speed;
        private bool scannedThisTick;
        private int totalScans = 0;
        private int turnScans = 0;
        private int rifle = 0;
        protected override void BotPrepareForBattle() {
            this.FanfareMessage = "'Sup Bitches?";

            InstallEquipment(KnownEquipmentIds.DEFAULTPOWERPACK, "PowerPack", MountPoint.Internal);
            InstallEquipment(KnownEquipmentIds.DEFAULTPOWERPACK, "PowerPack2", MountPoint.Internal);

            scanner = InstallEquipment(KnownEquipmentIds.DEFAULTSCANNER, "Scanner", MountPoint.Internal);
            if (scanner == null) {
                b.Info.Log("Scanner registration failed");
            }

            Heading headingNorth = new Heading(North);
            InstallEquipment(KnownEquipmentIds.WEAPONTYPE_RIFLE_INSTANCE_1, "Rifle0", MountPoint.Turret);
            InstallEquipment(KnownEquipmentIds.WEAPONTYPE_RIFLE_INSTANCE_1, "Rifle1", MountPoint.Turret);
            InstallEquipment(KnownEquipmentIds.WEAPONTYPE_RIFLE_INSTANCE_1, "Rifle2", MountPoint.Turret);
            InstallEquipment(KnownEquipmentIds.WEAPONTYPE_RIFLE_INSTANCE_1, "Rifle3", MountPoint.Turret);
            InstallEquipment(KnownEquipmentIds.WEAPONTYPE_RIFLE_INSTANCE_1, "Rifle4", MountPoint.Turret);
            InstallEquipment(KnownEquipmentIds.WEAPONTYPE_RIFLE_INSTANCE_1, "Rifle5", MountPoint.Turret);
            InstallEquipment(KnownEquipmentIds.WEAPONTYPE_RIFLE_INSTANCE_1, "Rifle6", MountPoint.Turret);
            InstallEquipment(KnownEquipmentIds.WEAPONTYPE_RIFLE_INSTANCE_1, "Rifle7", MountPoint.Turret);
            InstallEquipment(KnownEquipmentIds.WEAPONTYPE_RIFLE_INSTANCE_1, "Rifle8", MountPoint.Turret);
            InstallEquipment(KnownEquipmentIds.WEAPONTYPE_RIFLE_INSTANCE_1, "Rifle9", MountPoint.Turret);
            InstallEquipment(KnownEquipmentIds.WEAPONTYPE_RIFLE_INSTANCE_1, "Rifle10", MountPoint.Turret);

            headings = new List<Heading>() { headingNorth, new Heading(South), new Heading(East), new Heading(West), new Heading(Northwest), new Heading(Southwest), new Heading(Northeast), new Heading(Southeast) };

            headingsSouth = new List<Heading>() { new Heading(South), new Heading(Southwest), new Heading(Southeast) };

            currentHeading = headingNorth;
            currentPosition = ScanCentre;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks>
        /// [-1, 1][ 0, 1][ 1, 1]
        /// [-1, 0][ kev ][ 1, 0]
        /// [-1,-1][ 0,-1][ 1,-1]
        /// </remarks>
        /// <param name="turn"></param>
        /// <param name="tick"></param>

        protected override void BotTakeAction(int turn, int tick, LastTickRecord ltr) {

            //reset the scan 
            scannedThisTick = false;

            //if (turn == 4 & tick == 10) {
            //    Plisky.Boondoggle2.Test.TestUtils.DumpScanResult(grid, turn, tick, this.Name);
            //}

            int things = this.Scan(turn, tick);

            if (things == 0) {
                if (speed > 1) {
                    for (int i = 0; i < 11; i++) {
                        b.Info.Log($"Nothing around!  Speed is {speed} Decelerating to {speed + 1}");
                        this.speed = Decelerate();
                    }
                }
            } else {
                if (rifle < 11) {
                    foreach (var poi in grid.GetPointsOfInterest()) {
                        b.Info.Log("KevBot FIRE!!!  - TargetId {" + poi.POIIdentity.ToString() + "}");
                        var eur = FireWeapon(poi.POIIdentity, "Rifle" + rifle);
                        b.Info.Log($"KevBot fire result was : {eur.State}" );
                        if (eur.State == UsageEndState.Fail_NoAmmo) {
                            rifle++;
                        }
                    }
                } else {
                    if (speed < 10) {
                        for (int i = 0; i < 11; i++) {
                            b.Info.Log($"Bitches in the hood!  Speed is {speed} Accelerating to {speed + 1}");
                            this.speed = Accelerate();
                        }
                    }
                }
            }

            var nextHeading = GetNextPositionFromDirection();

            //The position we are about to occupy is unscanned
            if (grid.GetResultAtPosition(nextHeading.Point) == ScanTileResult.Unscanned) {
                b.Info.Log("Found our next heading is unscanned, rescan!");
                rescan = true;
                things = this.Scan(turn, tick);
                nextHeading = GetNextPositionFromDirection();
            }

            //The position we are about to occupy is unoccupied
            if (grid.GetResultAtPosition(nextHeading.Point) == ScanTileResult.Unoccupied) {
                // Nothing of interest was returned on the map
                //if (things == 0)
                //{
                //    return;
                //}

                bool currentHeadingIsSafe = true;
                int bots = 0;
                //There is something on the map that we might bump into, check whether
                //they can potentially move into the position we are about to occupy
                foreach (var rp in currentPosition.RelatedPoints) {
                    if (grid.GetResultAtPosition(rp) == ScanTileResult.Bot) {
                        currentHeadingIsSafe = false;
                        b.Info.Log($"Found a bot at {rp.X}:{rp.Y} - can't go to {currentPosition.Point.X}:{currentPosition.Point.Y}, in case we collied");
                        bots++;
                    }
                    ////TODO : change this to walls straight ahead of us only
                    //if (grid.GetResultAtPosition(rp) == ScanTileResult.SolidWall)
                    //{
                    //    currentHeadingIsSafe = false;
                    //    b.Info.Log("Found a wall at {0}:{1} - can't go to {2}:{3}, in case we collied", rp.X, rp.Y, currentPosition.Point.X, currentPosition.Point.Y);
                    //    break;
                    //}
                }

                if (currentHeadingIsSafe) {
                    return;
                }

                b.Info.Log($"Current heading is not safe, there are {bots} bots about, change direction!");

                rescan = true;
                Scan(turn, tick);
            } else if (grid.GetResultAtPosition(nextHeading.Point) == ScanTileResult.SolidWall) {
                b.Info.Log($"Found a wall at {nextHeading.Point.X}:{nextHeading.Point.Y}");
                rescan = true;
                Scan(turn, tick);
                currentHeading.InUse = false;
                SetHeadingAttributes(currentHeading);
                nextHeading.Used = true;
                nextHeading.InUse = false;
                SetHeadingAttributes(nextHeading);
            }

            //foreach (var rp in currentHeading.RelatedPoints) {
            //    if (grid.GetResultAtPosition(rp) == ScanTileResult.Bot) {
            //        b.Info.Log("Found a bot at {0}:{1} - can't go to {2}:{3}, in case we collied", rp.X, rp.Y, currentHeading.Point.X, currentHeading.Point.Y);
            //        currentHeading.InUse = false;
            //        SetHeadingAttributes(currentHeading);
            //        break;
            //    }
            //}

            //if (currentHeading.InUse)
            //    return;

            List<Heading> freeheadings = FindFreeHeadings(grid);

            if (freeheadings.Count() == 0) {
                b.Info.Log("No free headings left, start again");
                headings.ForEach(h => h.Used = false);
                freeheadings = FindFreeHeadings(grid);
            }

            foreach (var heading in freeheadings) {
                //if (heading == currentHeading)
                //    continue;
                if (heading == nextHeading)
                    continue;
                if (heading.Used)
                    continue;

                if (grid.GetResultAtPosition(heading.Point) == ScanTileResult.Unoccupied) {
                    currentHeading.InUse = false;
                    currentHeading.Used = true;
                    SetHeadingAttributes(currentHeading);
                    b.Info.Log(currentHeading + " Used set to true, InUse set to false");
                    nextHeading.InUse = false;
                    nextHeading.Used = true;
                    SetHeadingAttributes(nextHeading);
                    b.Info.Log(nextHeading + " Used set to true, InUse set to false");

                    heading.InUse = true;
                    SetHeadingAttributes(heading);
                    b.Info.Log(heading + " InUse set to true");
                    currentHeading = heading;
                    b.Info.Log($"Found a clear path at {currentHeading.Point.X}:{currentHeading.Point.Y} {Heading.HeadingText(currentHeading.Point)} - Let's go there!" );
                    HeadToPoint(heading.Point);
                    return;
                }
            }
            b.Info.Log("No where to go, HALT!");
            this.speed = 0;
        }

        private void SetCurrentPositionFromDirection() {
            this.currentPosition = this.GetNextPositionFromDirection();
        }

        private void SetHeadingAttributes(Heading heading) {
            for (int i = 0; i < this.headings.Count; i++) {
                if (this.headings[i].Point == heading.Point) {
                    this.headings[i].InUse = heading.InUse;
                    this.headings[i].Used = heading.Used;
                    break;
                }
            }
        }

        private Heading GetNextPositionFromDirection() {
            Point nextHeading = currentPosition.Point;

            switch ((int)this.CurrentHeading) {
                case 0:
                    nextHeading.Y = nextHeading.Y + 1;
                    break;

                case 45:
                    nextHeading.Y = nextHeading.Y + 1;
                    nextHeading.X = nextHeading.X + 1;

                    break;

                case 180:
                    nextHeading.Y = nextHeading.Y - 1;

                    break;

                case 225:
                    nextHeading.Y = nextHeading.Y - 1;
                    nextHeading.X = nextHeading.X - 1;
                    break;

                case 90:
                    nextHeading.X = nextHeading.X + 1;
                    break;

                case 135:
                    nextHeading.Y = nextHeading.Y - 1;
                    nextHeading.X = nextHeading.X + 1;
                    break;

                case 270:
                    nextHeading.X = nextHeading.X - 1;
                    break;

                case 315:
                    nextHeading.Y = nextHeading.Y + 1;
                    nextHeading.X = nextHeading.X - 1;
                    break;

                default:
                    throw new InvalidOperationException("Where am I?");
            }

            return new Heading(nextHeading);
        }

        private List<Heading> FindFreeHeadings(ScanEquipmentUseResult grid) {

            var freeheadings = (from h in headings where h.Used == false && h.Point != currentPosition.Point && grid.GetResultAtPosition(h.Point) == ScanTileResult.Unoccupied select h).ToList();

            List<Point> potentialBotLocations = new List<Point>(grid.GetPointsOfInterest().Count() * 9);

            foreach (var poi in grid.GetPointsOfInterest()) {
                if (grid.GetResultAtPosition(poi.ScanLocation) == ScanTileResult.Bot) {
                    potentialBotLocations.Add(poi.ScanLocation);
                    var poiHeading = new Heading(poi.ScanLocation);
                    potentialBotLocations.AddRange(poiHeading.RelatedPoints);
                }
            }

            for (int i = freeheadings.Count() - 1; i >= 0; i--) {
                bool headingRemoved = false;
                foreach (var rp in freeheadings[i].RelatedPoints) {
                    //if (grid.GetResultAtPosition(rp) == ScanTileResult.SolidWall) {
                    //    headingRemoved = true;
                    //}
                    foreach (var potentialBotLocation in potentialBotLocations) {
                        if (potentialBotLocation == rp) {
                            b.Info.Log($"Found a (potential) bot location at {rp.X}:{rp.Y} - can't go to {freeheadings[i].Point.X}:{freeheadings[i].Point.Y}, in case we collied");
                            headingRemoved = true;
                            break;
                        }
                    }
                    if (headingRemoved) {
                        freeheadings.RemoveAt(i);
                        break;
                    }
                    //if (grid.GetResultAtPosition(rp) == ScanTileResult.Bot) {
                    //    b.Info.Log("Found a bot at {0}:{1} - can't go to {2}:{3}, in case we collied", rp.X, rp.Y, freeheadings[i].Point.X, freeheadings[i].Point.Y);
                    //    freeheadings.RemoveAt(i);
                    //    break;
                    //} else {
                    //    var rph = new Heading(rp);
                    //    foreach (var rp2 in rph.RelatedPoints) {
                    //        if (grid.GetResultAtPosition(rp2) == ScanTileResult.Bot) {
                    //            b.Info.Log("Found a bot at {0}:{1} - can't go to {2}:{3}, in case we collied", rp2.X, rp2.Y, freeheadings[i].Point.X, freeheadings[i].Point.Y);
                    //            freeheadings.RemoveAt(i);
                    //            headingRemoved = true;
                    //            break;
                    //        } 
                    //    }
                    //    if (headingRemoved)
                    //    {
                    //        break;
                    //    }
                    //}
                }
            }
            return freeheadings;
        }

        protected override void BotTurnStartAction(int turn) {
            b.Info.Log($"Turn {turn}  -  SCANS by KevBot = {totalScans}");
            turnScans = 0;
        }

        private int Scan(int turn, int tick) {
            int things = grid != null ? grid.NumberOfPOI : 0;

            if (tick == 1 | rescan || (tick > 1 & things > 0)) {

                if (!scannedThisTick) {
                    if (things > 0) {
                        b.Info.Log($"RESCAN as found {things} things on the scan during tick {tick}" );
                    } else if (rescan) {
                        b.Info.Log($"RESCAN set to true {tick}");
                    } else if (tick == 1) {
                        b.Info.Log($"RESCAN as this is the first tick of the turn {tick}");
                    }
                    grid = UseEquipment("Scanner") as ScanEquipmentUseResult;
                    totalScans++;
                    turnScans++;
                    this.currentPosition = ScanCentre;
                    this.currentHeading = ScanCentre;
                    this.currentHeading.InUse = false;
                    SetHeadingAttributes(currentHeading);
                    rescan = false;
                    scannedThisTick = true;
                    //TestUtils.DumpScanResult(grid, turn, tick, this.Name);
                }
            } else {
                SetCurrentPositionFromDirection();
            }

            return grid.NumberOfPOI;
        }

        public KevBot()
            : base() {
            this.InitialiseDetails("Kev Bot1", "0.1");
        }
    }

    [DebuggerDisplay("{ToString()}")]
    public class Heading {
        public Point Point;
        public bool Used;
        public bool InUse;

        public readonly List<Point> RelatedPoints;

        public Heading(Point point) {
            this.Point = point;
            RelatedPoints = new List<Point>(7);

            /// [x-1, y+1][x, y+1][ x+1, y+1]
            /// [x-1, y  ][x, y  ][ X+1, y  ]
            /// [x-1, y-1][x, y-1][ x+1, y-1]

            /// [-2, 2][-1, 2][ 0, 2][ 1, 2][ 2, 2]
            /// [-2, 1][-1, 1][ 0, 1][ 1, 1][ 2, 1]
            /// [-2, 0][-1, 0][ kev ][ 1, 0][ 2, 0]
            /// [-2,-1][-1,-1][ 0,-1][ 1,-1][ 2,-1]
            /// [-2,-2][-1,-2][ 0,-2][ 1,-2][ 2,-2]
            RelatedPoints.Add(new Point(point.X, point.Y + 1));
            RelatedPoints.Add(new Point(point.X, point.Y - 1));

            RelatedPoints.Add(new Point(point.X - 1, point.Y));
            RelatedPoints.Add(new Point(point.X - 1, point.Y - 1));
            RelatedPoints.Add(new Point(point.X - 1, point.Y + 1));

            RelatedPoints.Add(new Point(point.X + 1, point.Y - 1));
            RelatedPoints.Add(new Point(point.X + 1, point.Y + 1));

            RelatedPoints.Add(new Point(point.X + 1, point.Y));

            for (int i = RelatedPoints.Count() - 1; i >= 0; i--) {
                if (RelatedPoints[i].X == 0 && RelatedPoints[i].Y == 0) {
                    RelatedPoints.RemoveAt(i);
                    break;
                }
            }
        }

        public bool Equals(Heading obj) {
            return this.Point.Equals(obj.Point);
        }

        public override string ToString() {
            return HeadingText(this.Point);
        }

        public static string HeadingText(Point currentPosition) {
            if (currentPosition == KevBot.North) return "North";
            if (currentPosition == KevBot.South) return "South";
            if (currentPosition == KevBot.East) return "East";
            if (currentPosition == KevBot.West) return "West";
            if (currentPosition == KevBot.Southwest) return "South West";
            if (currentPosition == KevBot.Southeast) return "South East";
            if (currentPosition == KevBot.Northwest) return "North West";
            if (currentPosition == KevBot.Northeast) return "North East";
            if (currentPosition == KevBot.ScanCentrePoint) return "Scan Centre";

            return "Unknown Description";
        }
    }

    public static class Extentions {

        public static string Description(this Point p) {
            return Heading.HeadingText(p);
        }

        /// [-2, 2][-1, 2][ 0, 2][ 1, 2][ 2, 2]
        /// [-2, 1][-1, 1][ 0, 1][ 1, 1][ 2, 1]
        /// [-2, 0][-1, 0][ kev ][ 1, 0][ 2, 0]
        /// [-2,-1][-1,-1][ 0,-1][ 1,-1][ 2,-1]
        /// [-2,-2][-1,-2][ 0,-2][ 1,-2][ 2,-2]
    }
}