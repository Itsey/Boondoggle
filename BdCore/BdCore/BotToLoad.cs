using System.Runtime.Serialization;

namespace Plisky.Boondoggle2 {

    [DataContract]
    public class BotToLoad {

        [DataMember(Name = "typ", Order = 0)]
        public string TypeName { get; set; }

        [DataMember(Name = "bin", Order = 1)]
        public string BinaryName { get; set; }
    }
}