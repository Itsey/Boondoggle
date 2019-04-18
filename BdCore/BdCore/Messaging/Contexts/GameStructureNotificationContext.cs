using System.Runtime.Serialization;

namespace Plisky.Boondoggle2 {

    [DataContract]
    public class GameStructureNotificationContext : BattleContextBase {

        [DataMember]
        public int Turn { get; set; }

        [DataMember]
        public int Tick { get; set; }
    }
}