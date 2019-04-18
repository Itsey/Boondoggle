namespace Plisky.Boondoggle2.Test {

    using System;
    using System.Collections.Generic;

    public class BasicMockBot : BoonBotBase {
        private List<string> turnTicksRecieved = new List<string>();

        public bool HasPrepareOccured { get; set; }

        public Action<BoonBotBase> EquipmentCallback = null;

        public BasicMockBot() : base() {
            InitialiseDetails("MockBot", "0");
            this.FanfareMessage = "Test Bot Online.";
        }

        public string GetFanfareMessage() {
            return this.FanfareMessage;
        }

        public bool AddMockPowerpack { get; set; }

        protected override void BotPrepareForBattle() {
            if (EquipmentCallback != null) {
                EquipmentCallback(this);
            }
            if (AddMockPowerpack) {
                this.InstallEquipment(KnownEquipmentIds.MOCKPOWERPACK, "PP", MountPoint.Internal);
            }

            HasPrepareOccured = true;
        }

        public bool HasThisTurnOccured(int p1, int p2) {
            string match = p1.ToString() + "," + p2.ToString();
            return turnTicksRecieved.Contains(match);
        }

        protected override void BotTakeAction(int turn, int tick, LastTickRecord ltr) {
            WhatHappened = ltr;
            string match = turn.ToString() + "," + tick.ToString();
            turnTicksRecieved.Add(match);
        }

        protected override void BotTurnStartAction(int turn) {
            string match = turn.ToString() + ",0";
            turnTicksRecieved.Add(match);
        }

        public void ChangeHeading(int p) {
            throw new System.NotImplementedException();
        }

        public int GetEquipmentCount() {
            return allInstalledEquipment.Count;
        }



        public EquipmentInstallationResult Mock_GetWeaponEquipmentInstanceId(string p) {
            return GetEquipment(p);
        }

        public LastTickRecord Mock_GetLTR() {
            return WhatHappened;
        }

        public void Mock_SetNameVer(string p1, string p2) {
            this.InitialiseDetails(p1, p2);
        }
    }
}