using Plisky.Boondoggle2;
using Plisky.Plumbing;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Plisky.Boondoggle2.Reference {

    public class VictimLock {
        Point lastKnownLoc;
        Point nextPredictedLoc;

        int predictedHeading;
 
    }
    public enum PBMode {
        Explore,Hunt,Run,Powersave
    }

    public class PBBehaviour {
        public PBMode ActiveBehaviour { get; set; }

        public void SetBehaviour(PBMode mode) {
            ActiveBehaviour = mode;
        }

        public void DetermineTargetStats() {
            
        }
    }


    public class PirateBot : BoonBotBase {
        private Queue<ScanEquipmentUseResult> previousScans = new Queue<ScanEquipmentUseResult>();

        private PBBehaviour behaviour;

        private ScanEquipmentUseResult scanMinusOne;
        private ScanEquipmentUseResult lastScan;
        private const int DISTANCETOCHECK = 10;
        private bool weaponsActive = true;
        private int scanAge = 0;
        
        private Dictionary<Point, ScanTileResult> internalMap = new Dictionary<Point, ScanTileResult>();
        private List<int> headingsToTry = new List<int>();
        private Point myLocation = new Point(0, 0);

        protected override void BotPrepareForBattle() {
            this.FanfareMessage = "Arrrrr... and Avast!";
            InstallEquipment(KnownEquipmentIds.DEFAULTSCANNER, "MyScanner", MountPoint.Internal);
            InstallEquipment(KnownEquipmentIds.DEFAULTPOWERPACK, "PowerPack", MountPoint.Internal);
            InstallEquipment(KnownEquipmentIds.WEAPONTYPE_RIFLE_INSTANCE_1, "Rifle", MountPoint.Turret);
            headingsToTry.Add(0); headingsToTry.Add(90); headingsToTry.Add(270); headingsToTry.Add(180); headingsToTry.Add(45);


            
        }

        protected override void BotTurnStartAction(int turn) {
              
        }

        protected override void BotTakeAction(int turn, int tick, LastTickRecord tdr) {

            ProcessLastTickRecord(tdr);

            scanAge++;

            if ((lastScan == null) || (tdr.Events.Count>0) || (scanAge>10)) {
                PerformScan();
                AnalyseScanForTargetLock();
            } 
            

            if (this.CurrentSpeed < 5) {
                Accelerate();
            }
            

       
            double nd = CurrentHeading;

            if (!IsThisDirectionClearn(nd)) {
                //TestUtils.DumpScanResult(lastScan, turn, tick, this.Name);

                double newHeading = GetNewHeading(nd);

                if (newHeading != nd) {
                    ChangeHeading(newHeading);
                } else {
                    BotWriteMessage("No new heading found, stopping.");
                    Decelerate();
                }
            }
        }

        private void ProcessLastTickRecord(LastTickRecord tdr) {
            if (tdr.Events.Count > 0) {
                foreach (var v in tdr.Events) {
                    if (v.ActionType == LastTickEventType.Moved) {
                        myLocation.X += v.XDeltaMove;
                        myLocation.Y += v.YDeltaMove;
                    }
                }
            }
        }

        private void AnalyseScanForTargetLock() {


            if (lastScan.NumberOfPOI >0) {
                behaviour.SetBehaviour(PBMode.Hunt);
            }

            lastScan.ScanResultEach((ab, cd) => {
                if (cd == ScanTileResult.Bot) {
                    b.Warning.Log("EH");
                }
            });

            if (lastScan.NumberOfPOI > 0) {
                BotWriteMessage("Point of interest found");

                foreach (var v in lastScan.GetPointsOfInterest()) {

                    if ((weaponsActive)&&(lastScan.GetResultAtPosition(v.ScanLocation) == ScanTileResult.Bot)) {
                        if (CurrentSpeed > 5) {
                            Decelerate();
                        }
                        BotWriteMessage("FIRING : " + v.POIIdentity.ToString());
                        var res = FireWeapon(v.POIIdentity, "Rifle");
                        if (res.State == UsageEndState.Fail_NoAmmo) {
                            weaponsActive = false;
                        }
                    } else {
                        // RAMMING SPEED!
                        HeadToPoint(v.ScanLocation);
                        Accelerate();
                    }
                }
            }

        }


        private bool scan = true;
        private void PerformScan() {

            if (scan) {
                scanMinusOne = lastScan;
                lastScan = (ScanEquipmentUseResult)UseEquipment("MyScanner");
                previousScans.Enqueue(lastScan);
                if (previousScans.Count > 10) {
                    previousScans.Dequeue();
                }
                scanAge = 0;
            }
            scan = !scan;
        }

        private double GetNewHeading(double nd) {
            int loopCount = 0;

            while (!IsThisDirectionClearn(nd)) {
                BotWriteMessage("Looking for new direction " + nd.ToString());
                nd = headingsToTry[loopCount];
                nd = nd % 360;
                loopCount++;
                if (loopCount > headingsToTry.Count) {
                    // Cant handle it - crash
                    return CurrentHeading;
                }
            }
            return nd;
        }

        private void BotWriteMessage(string p) {
            b.Warning.Log("BOT - " + this.Name + " : " + p);
        }

        private bool IsThisDirectionClearn(Double direction) {
            if (direction == 0) {
                return IsStraightUpClear();
            }
            if (direction == 90) {
                return IsRightClear();
            }

            if (direction == 180) {
                return IsDownClear();
            }
            if (direction == 270) {
                return IsLeftClear();
            }

            return true;
        }

        private bool IsRightClear() {
            for (int i = 1; i <= DISTANCETOCHECK; i++) {
                if (lastScan.GetResultAtPosition(new Point(i, 0)) != ScanTileResult.Unoccupied) {
                    return false;
                }
            }
            return true;
        }

        private bool IsLeftClear() {
            for (int i = -1; i >= -DISTANCETOCHECK; i--) {
                if (lastScan.GetResultAtPosition(new Point(i, 0)) != ScanTileResult.Unoccupied) {
                    return false;
                }
            }
            return true;
        }

        private bool IsDownClear() {
            for (int i = -1; i >= -DISTANCETOCHECK; i--) {
                if (lastScan.GetResultAtPosition(new Point(i, i)) != ScanTileResult.Unoccupied) {
                    return false;
                }
            }
            return true;
        }

        private bool IsStraightUpClear() {
            b.Info.Log("Checking ahead ... ");
            for (int i = 1; i <= DISTANCETOCHECK; i++) {
                if (lastScan.GetResultAtPosition(new Point(0, i)) != ScanTileResult.Unoccupied) {
                    return false;
                }
            }
            return true;
        }

        

        
        public PirateBot() :base() {
            string name = "Pirate Bot";
            string ver = "0.0.0.1";            
            InitialiseDetails(name, ver);            
        }

        public static BoonBotBase CreateBot() {
            return new PirateBot();
        }
    }
}