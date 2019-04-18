namespace Plisky.Boondoggle2.Test {

    using Plisky.Boondoggle2.Repository;
    using System;

    public class MockBotFactory {
        private BasicMockBot result;
        private IKnowWhatBotsDo botQueryProvider = null;
        private IProvideBotInteractivity botInteractivityProvider = null;
        public MockBotActionProvider MockActionProviderUsed = null;

        private bool PrepareWithActionProvider = false;
        public EquipmentSupport EquipmentSupportUsed = null;
        public MockEquipmentRepository EquipmentRepositoryUsed = null;
        public bd2MessageBasedBotSupport MessageBasedProvider = null;

        public MockBotFactory CreateBasicMockBot() {
            result = new BasicMockBot();
            return this;
        }

        public MockBotFactory CreateMockWithName(string name, string version) {
            result = new BasicMockBot();
            return this;
        }

        public MockBotFactory WithItemSupport(EquipmentRepository thisOne = null) {
    
            if (thisOne == null) {
                EquipmentRepositoryUsed = new MockEquipmentRepository();

                EquipmentSupportUsed = new EquipmentSupport(EquipmentRepositoryUsed);
            } else {
                EquipmentSupportUsed = new EquipmentSupport(thisOne);
            }
            return this;
        }

        public MockBotFactory WithThisActionProvider(IKnowWhatBotsDo ikwbd, IProvideBotInteractivity ipba) {
            PrepareWithActionProvider = true;

            botQueryProvider = ikwbd;
            botInteractivityProvider = ipba;

            return this;
        }

        public MockBotFactory WithMockActionProvider() {
            PrepareWithActionProvider = true;

            var mbap = new MockBotActionProvider();

            botQueryProvider = mbap;
            botInteractivityProvider = mbap;

            MockActionProviderUsed = mbap;

            return this;
        }

        private void CheckCreate() {
            if (result == null) {
                throw new InvalidOperationException("Test logic fault, call create before with default");
            }
        }

        public BoonBotBase ToBot() {
            result.EquipmentCallback = installEquipment;

            if ((EquipmentSupportUsed != null) && (MockActionProviderUsed != null)) {
                MockActionProviderUsed.InjectEquipmentSupport = EquipmentSupportUsed;
            }

            if (UseMockPackOption) {
                result.AddMockPowerpack = true;
            }

            if (PrepareWithActionProvider) {
                result.PrepareForBattle(botInteractivityProvider, botQueryProvider, Guid.NewGuid());
            }

            return result;
        }

        private Action<BoonBotBase> installEquipment;

        public MockBotFactory WithEquipmentCallback(Action<BoonBotBase> bbb) {
            installEquipment = bbb;
            return this;
        }

        public bool UseMockPackOption { get; set; }

        public MockBotFactory WithMockPack() {
            UseMockPackOption = true;
            return this;
        }
    }
}