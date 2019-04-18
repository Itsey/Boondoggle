using System.Runtime.Serialization;

namespace Plisky.Boondoggle2 {

    [DataContract]
    public class BotEndContext : BattleContextBase {

        [DataMember]
        public int BotId { get; set; }

        [DataMember]
        public BotEndReason Reason { get; set; }
    }
}