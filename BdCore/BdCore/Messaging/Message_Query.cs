namespace Plisky.Boondoggle2 {

    using System;

    public class Message_Query : MessageBase {
    
        public Guid PublicBotId { get; set; }

        public Message_Query(MainMessageKind topType, KnownSubkinds subtype)
            : base(topType, subtype) {
            
        }
    }
}