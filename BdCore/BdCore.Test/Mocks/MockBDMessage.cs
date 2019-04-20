namespace Plisky.Boondoggle2.Test {


    public class MockBDMessage : MessageBase {

        public MockBDMessage(MainMessageKind kind, KnownSubkinds subKind)
            : base(kind,KnownSubkinds.MockMessage) {
        }
    }
}