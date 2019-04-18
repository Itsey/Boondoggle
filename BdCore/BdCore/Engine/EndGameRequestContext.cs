using System.Runtime.Serialization;

namespace Plisky.Boondoggle2 {

    [DataContract]
    public class EndGameRequestContext {

        [DataMember]
        public string EndGameDataDump { get; set; }
    }
}