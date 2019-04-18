using System.Runtime.Serialization;

namespace Plisky.Boondoggle2 {

    [DataContract]
    public class SystemMessageContext : BattleContextBase {

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public int BotId { get; set; }
    }
}