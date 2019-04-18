namespace Plisky.Boondoggle2.Test {

    using Plisky.Boondoggle2;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Simple action provider that implements both interfaces but should be used only for the simple scenarios -
    /// for more complex ones the message based action provider should be used and the important messages hooked.
    /// </summary>
    public class MockBotActionProvider : bd2BaseBotEngineSupport {

        public static int PredefinedSimpleScan = 1;
        private ActiveLoadout activeBotLoadout1;
        private Dictionary<Guid, ActiveEquipment> EngineKitList = new Dictionary<Guid, ActiveEquipment>();        
        private Stack<EquipmentUseDetails> lastUsages = new Stack<EquipmentUseDetails>();

        public EquipmentSupport InjectEquipmentSupport { get; set; }

        public int LastSpeedChangeValue { get; set; }

        public double LastHeadingChangeValue { get; set; }

        public MockBotActionProvider() {
            var mbf = new MockBotFrame();
            activeBotLoadout1 = new ActiveLoadout(this, mbf);
        }

        protected override int ActualGetCurrentSpeed(BoonBotBase publicId) {
            return LastSpeedChangeValue;
        }

        protected override void ActualChangeSpeed(BoonBotBase targetBot, int byThisMuch) {
            LastSpeedChangeValue = byThisMuch;
        }

        protected override void ActualChangeHeading(BoonBotBase targetBot, double byThisMuch) {
            LastHeadingChangeValue = byThisMuch;
        }

        protected override EquipmentInstallationResult ActualMountEquipment(BoonBotBase targetBot, int equipmentIdentifier, MountPoint mp) {
            // TODO : Shouldnt this be in the base?
            //EquipmentInstallationResult result = new EquipmentInstallationResult();
            
            
            EquipmentInstallationResult result = activeBotLoadout1.AddEquipment(equipmentIdentifier, mp);
           
            return result;
        }

        protected override EquipmentUseResult ActualUseEquipmentItem(BoonBotBase targetBot, EquipmentUseDetails eud) {

            lastUsages.Push(eud);
            ActiveEquipment ae = EngineKitList[eud.InstanceIdentity];

            if (ae == null) {
                throw new BdBaseException("That kit is not installed");
            }

            switch (ae.EquipmentId) {
                case KnownEquipmentIds.MOCKSCANNER: return PerformMockScan();
                case KnownEquipmentIds.MOCKINSTALLEVERYWHERE: return PerformDefaultEquipmentUse();
                case KnownEquipmentIds.MOCKINSTALLINTERNALONLY: return PerformDefaultEquipmentUse();
                case KnownEquipmentIds.MOCKPROJECTILEWEAPON: return PerformWeaponUsage(eud);
                default: throw new BdBaseException("That equipment is not known - internal error ");
            }
        }

        private EquipmentUseResult PerformWeaponUsage(EquipmentUseDetails eud) {
            return new EquipmentUseResult() {
                State = UsageEndState.Success
            };
        }

        private static EquipmentUseResult PerformDefaultEquipmentUse() {
            return new EquipmentUseResult() {
                State = UsageEndState.Success
            };
        }

        private EquipmentUseResult PerformMockScan() {
            ScanEquipmentUseResult result = new ScanEquipmentUseResult();
            result.State = UsageEndState.Success;

           
            return result;
        }
        protected override double ActualGetCurrentHeading(BoonBotBase boonBotBase) {
            return LastHeadingChangeValue;
        }

        public EquipmentInstallationResult Mock_GetEquipemntByGuid(Guid g) {
            var kt = EngineKitList[g];
            return new EquipmentInstallationResult() {
                InstanceId = kt.InstanceId,
                EquipmentId = kt.EquipmentId,
                Result = InstallationResult.Installed
            };
            
        }
        public int Mock_GetEquipmentUseCount ( Guid g) {
            int result = 0;
            foreach (var v in lastUsages) {
                if (v.InstanceIdentity == g) {
                    result++;
                }
            }
            return result;
        }
        public EquipmentUseDetails Mock_GetMostRecentEUD() {
            return lastUsages.Pop();
        }

        public int Mock_GetEquipmentCountAtPosition(MountPoint mp) {
            
            int result = activeBotLoadout1.GetEquipmentInMountPoint(mp).Count();
            
            return result;
        }

        protected override ActiveEquipment ActualCreateActiveEquipmentInstance(int equipmentIdentifier) {
            var item = InjectEquipmentSupport.GetEquipmentTypeById(equipmentIdentifier);
     
            var result = new ActiveEquipment(item) {
                InstanceId = Guid.NewGuid(),
                EquipmentId = equipmentIdentifier
            };
            EngineKitList.Add(result.InstanceId, result);

            return result;

           
        }

         protected override bool ActualIsValidEquipmentLocation(int equipmentIdentifier, MountPoint mp) {
            return InjectEquipmentSupport.CanMountEquipment(equipmentIdentifier, mp);
        }
    } 
}