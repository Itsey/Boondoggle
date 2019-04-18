namespace Plisky.Boondoggle.Test {

    using Plisky.Boondoggle2;

    public class MockBDMessage : MessageBase {

        public MockBDMessage(MainMessageKind kind, KnownSubkinds subKind)
            : base(kind,KnownSubkinds.MockMessage) {
        }
    }
}