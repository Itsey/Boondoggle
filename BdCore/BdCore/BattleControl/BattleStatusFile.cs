using System.Runtime.Serialization;

namespace Plisky.Boondoggle2 {

    [DataContract]
    public class BattleStatusFile {

        [DataMember(Name ="uq",Order =0)]
        public string UniqueName { get; set; }

        [DataMember(Name = "pth", Order = 1)]
        public string PathToBinaries { get; set; }

        [DataMember(Name = "out", Order = 2)]
        public string OutputPath { get; set; }

        [DataMember(Name = "bts", Order = 3)]
        public BotToLoad[] AllBots { get; set; }

        [DataMember(Name = "dsc", Order = 4)]
        public string DescriptiveName { get; set; }
    }
}