using Plisky.Plumbing;

namespace Plisky.Boondoggle2.Test {

    public class TestPreparedWorld {

        public BasicMockBot Bot2 { get; set; }

        public BasicMockBot Bot1 { get; set; }

        public mockBd2Engine Engine { get; set; }

        public int GetBot1EngineId() {
            return Engine.GetMappedBotByPublicId(Engine.Mock_GetFirstBotPublicId()).EngineId;
        }

        public int GetBot2EngineId() {
            return Engine.GetMappedBotByPublicId(Engine.Mock_GetSecondBotPublicId()).EngineId;
        }

        public MockEquipmentRepository MockRepos { get; set; }

        public Hub HubUsed { get; set; }
    }
}