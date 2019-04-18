using System.Runtime.Serialization;

namespace Plisky.Boondoggle2 {

    [DataContract]
    public class BotEnterWorldContext : BattleContextBase {

        [DataMember]
        public string BotName { get; set; }

        [DataMember]
        public string BotVersion { get; set; }

        [DataMember]
        public int ObjectId { get; set; }
    }
}