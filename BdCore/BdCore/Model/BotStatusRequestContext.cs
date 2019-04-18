using System.Runtime.Serialization;

namespace Plisky.Boondoggle2 {
    // TODO -- > MOVE! To Contexts

    [DataContract]
    public class BotStatusRequestContext : BattleContextBase {

        [DataMember]
        public int CurrentCharge { get; set; }

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int Power { get; set; }
    }
}