using System.Runtime.Serialization;

namespace Plisky.Boondoggle2 {

    [DataContract]
    public class BattleRequest {

        [DataMember]
        public string MapIdentifier { get; set; }

        [DataMember]
        public string BattleDisplayName { get; set; }

        [DataMember]
        public string BattleUniqueId { get; set; }

        [DataMember]
        public string[] ContestantRequests { get; set; }
    }
}