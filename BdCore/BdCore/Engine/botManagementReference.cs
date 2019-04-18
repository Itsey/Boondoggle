namespace Plisky.Boondoggle2 {

    using System;
    using System.Diagnostics;

    [DebuggerDisplay("{DirectReference.Name} S{DirectReference.CurrentSpeed} D{DirectReference.CurrentHeading}")]
    public class BotManagementReference {
        public int EngineIdentity { get; set; }

        public Guid PublicIdentity { get; set; }

        public BoonBotBase DirectReference { get; set; }

        public LastTickResults TickInfo { get; set; }
    }
}