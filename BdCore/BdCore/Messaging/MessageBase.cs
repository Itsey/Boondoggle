namespace Plisky.Boondoggle2 {

    using System.Threading;

    public abstract class MessageBase {
        private static int lastUsedMessageReference = 1;

        public double DParameter { get; set; }

        public object RequestContext { get; set; }

        public object ResponseContext { get; set; }

        public int MessageReference { get; private set; }

        public MainMessageKind MessageKind { get; private set; }
        public KnownSubkinds SubKind { get; private set; }

        protected MessageBase(MainMessageKind kind, KnownSubkinds subKind) {
            MessageReference = Interlocked.Increment(ref lastUsedMessageReference);
            MessageKind = kind;
            SubKind = subKind;
        }
    }
}