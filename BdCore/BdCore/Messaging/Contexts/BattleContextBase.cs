using System.Runtime.Serialization;

namespace Plisky.Boondoggle2 {

    [DataContract]
    public class BattleContextBase {
        public int EventIndex { get; set; }

        [DataMember]
        public MainMessageKind Kind { get; set; }

        [DataMember]
        public KnownSubkinds SubKind { get; set; }
    }
}