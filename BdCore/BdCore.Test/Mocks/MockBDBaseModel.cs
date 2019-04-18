namespace Plisky.Boondoggle.Test {

    using Plisky.Boondoggle2;

    public class MockBDBaseModel : bd2BaseModel {

        public void LaunchTestMessage() {
            hub.Launch<MockBDMessage>(new MockBDMessage(MainMessageKind.TestMessage, KnownSubkinds.MockSubkind));
        }
    }
}