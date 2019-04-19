namespace Plisky.Boondoggle2.Test {

    using Plisky.Plumbing;
    using System;
    using System.Collections.Generic;

    public class TestEngineFactory {
        public EquipmentSupport EngineSuppliedEquipmentSupport;

        public BoonBotBase GetBotAdded(int index) {
            return addTheseBots[index];
        }

        private bool needSupport = true;
        public static int TestingMaxSpeed = 10;

        private bd2World worldToUse;

        public TestEngineFactory WithDuellingWorld() {
            shouldIncludeWorld = true;
            var mp = new bd2MapRepository().GetMapByName("default");
            mp.MapType = MapConditionType.LastBotStanding;
            worldToUse = new bd2World(mp);
            return this;
        }

        public TestEngineFactory WithDefaultWorld() {
            shouldIncludeWorld = true;
            worldToUse = new bd2World(new bd2MapRepository().GetMapByName("default"));
            return this;
        }

        public TestEngineFactory WithMap(Bd2Map mp) {
            shouldIncludeWorld = true;
            worldToUse = new bd2World(mp);
            return this;
        }

        public TestEngineFactory() {
        }

        public TestEngineFactory WithBot(BoonBotBase thisOne = null) {
            if (thisOne == null) {
                var bmb = new BasicMockBot();
                bmb.AddMockPowerpack = true;
                thisOne = bmb;
                WithEquipmentSupport();
            }
            addTheseBots.Add(thisOne);
            return this;
        }

        private List<BoonBotBase> addTheseBots = new List<BoonBotBase>();

        private bool needPrepare = false;

        public TestEngineFactory WithPrepare() {
            needPrepare = true;
            return this;
        }

        private bool shouldIncludeWorld = false;

        public bd2Engine ToLiveEngine() {
            bd2Engine mainEngine = new bd2Engine();
            if (requiresCustomHub) {
                mainEngine.InjectHub(this.hubToUse);
            }
            //mainEngine.InjectBotSupport();
            //mainEngine.InjectEquipmentSupport(new EquipmentSupport(new HardcodedEquipmentRepository()));
            mainEngine.RegisterForMessages();
            //xmloutput.RegisterForMessages();
            return mainEngine;
        }

        public mockBd2Engine ToMockEngine() {
            mockBd2Engine result = new mockBd2Engine();
            if (requiresCustomHub) {
                result.InjectHub(hubToUse);
            }

            if (shouldIncludeWorld) {
                result.AddWorld(worldToUse);
            }

            if (shouldIncludeEqipmentSupport) {
                result.InjectEquipmentSupport(equipmentSupportTouse);
                EngineSuppliedEquipmentSupport = equipmentSupportTouse;
            }

            if (needSupport) {
                result.InjectBotSupport();
            }

            foreach (var v in addTheseBots) {
                result.AddBot(v);
            }

            if (needPrepare) {
                result.RegisterForMessages();
                result.StartBattle();
            }
            return result;
        }

        private Hub hubToUse;
        private bool requiresCustomHub = false;

        public TestEngineFactory WithHub(Hub testHub) {
            requiresCustomHub = true;
            hubToUse = testHub;
            return this;
        }

        private IKnowWhatBotsDo inj_ikwbd = null;
        private IProvideBotInteractivity inj_ipba = null;
        private bool injectBotSupport = false;

        public TestEngineFactory WithBotSupport(IKnowWhatBotsDo ikwbd = null, IProvideBotInteractivity ipba = null) {
            injectBotSupport = true;
            if (ikwbd == null) {
                if (ipba != null) {
                    throw new InvalidOperationException("Need both null");
                }
                bd2MessageBasedBotSupport mbSupport = new bd2MessageBasedBotSupport();
                inj_ikwbd = mbSupport;
                inj_ipba = mbSupport;
            } else {
                inj_ikwbd = ikwbd;
                inj_ipba = ipba;
            }

            return this;
        }

        private bool shouldIncludeEqipmentSupport = false;
        private EquipmentSupport equipmentSupportTouse;

        public TestEngineFactory WithEquipmentSupport(EquipmentRepository equipRepos = null) {
            this.shouldIncludeEqipmentSupport = true;
            if (equipRepos == null) {
                equipRepos = new MockEquipmentRepository();
            }
            this.equipmentSupportTouse = new EquipmentSupport(equipRepos);
            return this;
        }
    }
}