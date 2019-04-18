namespace Plisky.Boondoggle2 {

    using System;

    public class Message_BotPerformAction : MessageBase {
        //public ActionSubMessageKind SubKind { get; private set; }

        public Guid PublicBotId { get; set; }

        public Message_BotPerformAction(MainMessageKind topType, KnownSubkinds subtype)
            : base(topType, subtype) {
           // SubKind = subtype;
        }
    }
}