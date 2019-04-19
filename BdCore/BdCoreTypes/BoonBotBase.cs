using Plisky.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace Plisky.Boondoggle2 {
    public abstract class BoonBotBase {

        protected Bilge b = new Bilge(tl: TraceLevel.Off);
        

        /// <summary>
        /// Inject a new instance of bilge, or change the trace level of the current instance. To set the trace level ensure that
        /// the first parameter is null.  To set bilge simply pass a new instance of bilge.
        /// </summary>
        /// <param name="blg">An instance of Bilge to use inside this Hub</param>
        /// <param name="tl">If specified and blg==null then will alter the tracelevel of the current Bilge</param>
        public void InjectBilge(Bilge blg, TraceLevel tl = TraceLevel.Off) {
            if (blg != null) {
                b = blg;
            } else {
                b.CurrentTraceLevel = tl;
            }
        }


        protected List<EquipmentInstallationResult> allInstalledEquipment = new List<EquipmentInstallationResult>();
        protected Dictionary<string, EquipmentInstallationResult> namedEquipment = new Dictionary<string, EquipmentInstallationResult>();

        protected IKnowWhatBotsDo engineQueryProvider;
        protected IProvideBotInteractivity engineActionProvider;

        protected int LastTurnReceived = 0;
        protected int LastTickReceived = 0;
        protected LastTickRecord WhatHappened;

        public string FanfareMessage { get; set; }

        public string Name { get; private set; }

        public string Version { get; private set; }

        public Guid PublicId { get; private set; }

        protected EquipmentInstallationResult PowerPack { get; private set; }

        protected abstract void BotPrepareForBattle();

        protected abstract void BotTakeAction(int turn, int tick, LastTickRecord ltr);

        protected abstract void BotTurnStartAction(int turn);

        protected void InitialiseDetails(string name, string version) {
            if (string.IsNullOrEmpty(name)) {
                throw new BdBaseException("The name must be specified");
            }
            if (string.IsNullOrEmpty(version)) {
                throw new BdBaseException("The version must be specified");
            }
            Name = name;
            Version = version;
        }

        protected BoonBotBase() {
            b = new Bilge(tl: TraceLevel.Off);
            InitialiseDetails("Name Undefined", "0");
            FanfareMessage = Name + " prepares for battle.";
        }

        public void PrepareForBattle(IProvideBotInteractivity botActionSupport, IKnowWhatBotsDo botInfoSupport, Guid publicIdentity) {
            b.Assert.True(botActionSupport != null, "BotBase - PrepareForBattle - Initialise called with Null for BotActionSupport ");
            b.Assert.True(botInfoSupport != null, "BotBase - PrepareForBattle - Initialise called with Null for BotInfoSupport");

            PublicId = publicIdentity;
            engineActionProvider = botActionSupport;
            engineQueryProvider = botInfoSupport;
            BotPrepareForBattle();
        }

        public void TakeAction(int turn, int tick, LastTickRecord ltr) {
            this.WhatHappened = ltr;
            if (turn != LastTurnReceived) {
                BotTurnStartAction(turn);
                LastTurnReceived = turn;
            }

            BotTakeAction(turn, tick, ltr);
        }

        protected int CurrentSpeed { get; set; }

        protected Double CurrentHeading { get; set; }

        public int Accelerate(int howMuch = 1) {
            b.Verbose.Log("Accellerate Requested");
            CheckPreparation();

            engineActionProvider.ChangeSpeed(this, howMuch);
            CurrentSpeed = engineQueryProvider.GetCurrentSpeed(this);
            b.Verbose.Log(string.Format("Accelerate request by bot ({0}) - actual speed now [{1}]", PublicId, CurrentSpeed));
            return CurrentSpeed;
        }

        public int Decelerate(int howMuch = 1) {
            engineActionProvider.ChangeSpeed(this, -1 * howMuch);
            CurrentSpeed = engineQueryProvider.GetCurrentSpeed(this);
            return CurrentSpeed;
        }

        public double ChangeHeading(double requestedChange) {
            b.Verbose.Log("Change Heading requested - " + requestedChange.ToString());
            CheckPreparation();
            engineActionProvider.ChangeHeading(this, requestedChange);
            CurrentHeading = engineQueryProvider.GetCurrentHeading(this);
            return CurrentHeading;
        }

        public double ChangeHeadingBy(double amountToTurnBy) {
            CurrentHeading = engineQueryProvider.GetCurrentHeading(this);
            amountToTurnBy += CurrentHeading;
            return ChangeHeading(amountToTurnBy);
        }

        

        public EquipmentInstallationResult GetPowerPack() {
            return this.PowerPack;
        }

        public EquipmentInstallationResult InstallEquipment(int equipmentIdentifier, string userReferenceName, MountPoint mountPoint) {
            b.Verbose.Log("InstallEquipment - selecting " + equipmentIdentifier + " as user - " + userReferenceName);
            CheckPreparation();
            EquipmentInstallationResult result = new EquipmentInstallationResult();

            ItemClassification clas = KnownEquipmentIds.GetClassificationFromId(equipmentIdentifier);
            if ((this.PowerPack != null) && (clas == ItemClassification.PowerPack)) {
                return null;
            }

            if (namedEquipment.ContainsKey(userReferenceName)) {
                throw new BdBaseException("The name is already in use");
            }

            EquipmentInstallationResult ae = engineActionProvider.MountEquipment(this, equipmentIdentifier, mountPoint);
            b.Assert.True(ae != null, "You have to return a result, if it failed use the status indicator.");

            if (ae.Result != InstallationResult.Installed) {
                b.Warning.Log("Failed to install equipment to " + mountPoint.ToString() + " reason " + ae.Result.ToString());
                return ae;
            } else {
                namedEquipment.Add(userReferenceName, ae);
                allInstalledEquipment.Add(ae);
                if (clas == ItemClassification.PowerPack) {
                    b.Verbose.Log("Powerpack Installed");
                    this.PowerPack = ae;
                }
            }
            return ae;
        }

        private void CheckPreparation() {
            b.Verbose.Log("Checking preparation for bot: " + this.Name + " Pid:" + this.PublicId.ToString());
            if (engineActionProvider == null) {
                throw new BdBaseException("Prepare has not been called on this bot.  Call prepare before this action");
            }
            if (engineQueryProvider == null) {
                throw new BdBaseException("Prepare has not been called on this bot.  Call prepare before this query.");
            }
        }

        public EquipmentUseResult UseEquipment(string p) {
            b.Assert.True(namedEquipment != null, "The named equipment store must be initialised in Bot initialisation");

            if (!namedEquipment.ContainsKey(p)) {
                throw new BdBaseException("The named equipment has not been installed.");
            }
            // TODO : Refactor this to have an internal use(ActiveEquipment) so that the next call doesnt search again
            return UseEquipment(namedEquipment[p].InstanceId);
        }

        public EquipmentUseResult FireWeapon(int target, string weapon) {
            if (!namedEquipment.ContainsKey(weapon)) {
                throw new BdBaseException("The named equipment has not been installed.");
            }
            var v = namedEquipment[weapon];
            EquipmentUseDetails eud = new EquipmentUseDetails();
            eud.InstanceIdentity = v.InstanceId;
            eud.IParam = target;
            var res = engineActionProvider.UseEquipmentItem(this, eud);
            if (res.State == UsageEndState.Success) {

            }
            return res;
        }

        public EquipmentInstallationResult GetEquipment(string p) {
            return namedEquipment[p];
        }

        public EquipmentUseResult UseEquipment(Guid g) {
            foreach (var v in allInstalledEquipment) {
                if (v.InstanceId == g) {
                    b.Verbose.Log("Usage activation requested for item " + g.ToString());
                    EquipmentUseDetails eud = new EquipmentUseDetails();
                    eud.InstanceIdentity = v.InstanceId;
                    var res = engineActionProvider.UseEquipmentItem(this, eud);

                    if (res.State == UsageEndState.Success) {

                    }
                    return res;
                }
            }
            throw new BdBaseException("Unable to find the equpment listed");
        }

        public void HeadToPoint(Point point) {
            double heading = CombatHelper.CalculateDirectonToRelativePoint(point);
            ChangeHeading(heading);
        }
    }
}