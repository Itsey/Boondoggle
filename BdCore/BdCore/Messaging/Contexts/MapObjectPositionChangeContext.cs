using System.Drawing;
using System.Runtime.Serialization;

namespace Plisky.Boondoggle2 {
    // TODO -- > MOVE! To Contexts

    [DataContract]
    public class MapObjectPositionChangeContext : BattleContextBase {

        [DataMember]
        public Point Destination { get; set; }

        [DataMember]
        public int ObjectIdentity { get; set; }
    }
}