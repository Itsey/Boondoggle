namespace Plisky.Boondoggle2.Reference {
    using Plisky.Boondoggle2;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class BorisBot : BoonBotBase {

        //// Holds the results of the last scan operation
        //private ScanEquipmentUseResult lastScan;

        //// This is implementation specific, used by the logic in RTB to detemrin when to fire.
        //private bool disableWeapons = false;
        //// This is implementation specifc, use 
        //private const int DISTANCETOCHECK = 10;

        public BorisBot()
            : base() {

                base.InitialiseDetails("Boris", "0.00001");
        }

        /// <summary>
        /// There are two primary interface methods that are called by the engine for your BOT.  The first element is the ActualPrepareForBattle
        /// which occurs prior to the battle start.
        /// </summary>
        protected override void BotPrepareForBattle() {
            this.FanfareMessage = "Bollocks, lets have it!!!!!.......";
            InstallEquipment(KnownEquipmentIds.DEFAULTSCANNER, "MyScanner", MountPoint.Internal);
            InstallEquipment(KnownEquipmentIds.DEFAULTPOWERPACK, "PowerPack", MountPoint.Internal);
            InstallEquipment(KnownEquipmentIds.WEAPONTYPE_RIFLE_INSTANCE_1, "Rifle", MountPoint.Turret);
            //headingsToTry.Add(0); headingsToTry.Add(90); headingsToTry.Add(270); headingsToTry.Add(180); headingsToTry.Add(45);


            //// Prepare for battle is called prior to the battle start.

            //// Setting a Fanfare message displays your own personal message
            //this.FanfareMessage = "Geronimo.......";

            //// The minimum equipment you would need is a Scanner, A PowerPack and Weapon, check the documentation
            //// for all of the possible types of weaponry, powerpacks and scanners that are available.
            //InstallEquipment(KnownEquipmentIds.DefaultScanner, "MyScanner", MountPoint.Internal);
            //InstallEquipment(KnownEquipmentIds.DefaultPowerpack, "PowerPack", MountPoint.Internal);
            //InstallEquipment(KnownEquipmentIds.WeaponType_Rifle_Instance_1, "Rifle", MountPoint.Turret);
        }

        /// <summary>
        /// The main interface point called from the egine is ActualTakeAction, which is called for each tick.  Each tick is called passing a LastTickRecord
        /// whcih describes what happened to the bot during the last tick.  During the first tick no events will be returned in the last tick record.  
        /// </summary>
        /// <param name="turn">The integer turn number - increments from 1 thorugh to the end of the battle</param>
        /// <param name="tick">The integer tick number 1 through to 10</param>
        /// <param name="tdr">A description of the events that occured last turn.</param>
        protected override void BotTakeAction(int turn, int tick, LastTickRecord ltr) {
            Accelerate();

            if (this.CurrentSpeed < 5) {
                Accelerate();
            }


            //lastScan.ScanResultEach((ab, cd) => {
            //    if (cd == ScanTileResult.Bot) {
            //        Bilge.WarningLog("EH");
            //    }
            //});
            //if (lastScan.NumberOfPOI > 0) {
            //    BotWriteMessage("Point of interest found");

            //    foreach (var v in lastScan.GetPointsOfInterest()) {
            //        BotWriteMessage("FIRING : " + v.POIIdentity.ToString());
            //        FireWeapon(v.POIIdentity, "Rifle");
            //    }
            //}

            //double nd = CurrentHeading;

            //if (!IsThisDirectionClearn(nd)) {
            //    TestUtils.DumpScanResult(lastScan, turn, tick, this.Name);

            //    double newHeading = GetNewHeading(nd);

            //    if (newHeading != nd) {
            //        ChangeHeading(newHeading);
            //    } else {
            //        BotWriteMessage("No new heading found, stopping.");
            //        Decelerate();
            //    }
            //}


//################################

            //// Reference Bot Always Accellerates to 5 Speed and drives arounda t that speed
            //if (this.CurrentSpeed < 5) {
            //    Accelerate();
            //}

            //// A Point of interest indicates something that is not a wall or floor in the scan results.  This 
            //// will be an object such as a BOT - in this instance if right turn bot finds another bot it will open fire.
            //if (!disableWeapons) {

            //    // This bot disables its weapons when it runs out of ammunition.
            //    if (lastScan.NumberOfPOI > 0) {


            //        BotWriteMessage("Point of interest found");

            //        // Check Each Of the points of interest
            //        foreach (var v in lastScan.GetPointsOfInterest()) {
            //            BotWriteMessage("FIRING : " + v.POIIdentity.ToString());

            //            // If it is a BOT then open fire
            //            if (lastScan.GetResultAtPosition(v.ScanLocation) == ScanTileResult.Bot) {
            //                var result = FireWeapon(v.POIIdentity, "Rifle");

            //                // N.B. This will also fail for other reasons, for example if the weapon is on cooldown.
            //                if (result.State == UsageEndState.Fail_NoAmmo) {
            //                    disableWeapons = true;
            //                }
            //            }
            //        }
            //    }
            //}

            //// Find which direction you are heading in, from the bots Current Heading
            //double nd = CurrentHeading;

            //// This is a helper method that checks the scan results and sees whether there is a wall 
            //// or blocker in the way.
            //if (!IsThisDirectionClearn(nd)) {

            //    // DO NOT USE THIS METHOD
            //    TestUtils.DumpScanResult(lastScan, turn, tick, this.Name);

            //    // This is where your navigation logic goes, this is where you write the code to determine
            //    // how your bot should navigate around.
            //    double newHeading = GetNewHeading(nd);

            //    if (newHeading != nd) {
            //        ChangeHeading(newHeading);
            //    } else {
            //        BotWriteMessage("No new heading found, stopping.");
            //        Decelerate();
            //    }
            //}


        }

        protected override void BotTurnStartAction(int turn) {
            //throw new NotImplementedException();


            //lastScan = (ScanEquipmentUseResult)UseEquipment("MyScanner");

            //if (lastScan.NumberOfPOI > 0) {
            //    string poiFound = turn.ToString() + " POI : ";
            //    foreach (var v in lastScan.GetPointsOfInterest()) {
            //        poiFound += v.POIIdentity.ToString() + ",";
            //    }
            //    BotWriteMessage(poiFound);
            //}
        }



        ///// <summary>
        ///// This should be replaced by your own implementation
        ///// </summary>        
        //private double GetNewHeading(double nd) {
        //    int loopCount = 0;

        //    while (!IsThisDirectionClearn(nd)) {
        //        nd = nd + 90;
        //        nd = nd % 360;
        //        loopCount++;
        //    }
        //    return nd;
        //}

        ///// <summary>
        ///// NOT SURE WHAT TO DO WTIH THIS
        ///// </summary>        
        //private void BotWriteMessage(string p) {
        //    Bilge.Warning("BOT - " + this.Name + " : " + p);
        //}

        ///// <summary>
        ///// This should be replaced by your own implementation
        ///// </summary>        
        //private bool IsThisDirectionClearn(Double direction) {
        //    if (direction == 0) {
        //        for (int i = 1; i <= DISTANCETOCHECK; i++) {
        //            if (lastScan.GetResultAtPosition(new Point(0, i)) != ScanTileResult.Unoccupied) {
        //                return false;
        //            }
        //        }
        //        return true;
        //    }
        //    if (direction == 90) {
        //        for (int i = 1; i <= DISTANCETOCHECK; i++) {
        //            if (lastScan.GetResultAtPosition(new Point(i, 0)) != ScanTileResult.Unoccupied) {
        //                return false;
        //            }
        //        }
        //        return true;
        //    }

        //    if (direction == 180) {
        //        for (int i = -1; i >= -DISTANCETOCHECK; i--) {
        //            if (lastScan.GetResultAtPosition(new Point(i, i)) != ScanTileResult.Unoccupied) {
        //                return false;
        //            }
        //        }
        //        return true;
        //    }
        //    if (direction == 270) {
        //        for (int i = -1; i >= -DISTANCETOCHECK; i--) {
        //            if (lastScan.GetResultAtPosition(new Point(i, 0)) != ScanTileResult.Unoccupied) {
        //                return false;
        //            }
        //        }
        //        return true;
        //    }

        //    return true;
        //}


    }
}
