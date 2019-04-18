using System;
using System.Runtime.Serialization;

namespace Plisky.Boondoggle2 {

    [DataContract]
    public class NavigationInfoContext : BattleContextBase {

        public void SetBot(MappedBot bt) {
            this.BotId = bt.EngineId;
            this.PublicBotId = bt.Bot.PublicId;
        }

        [DataMember]
        public int SpeedDelta { get; set; }

        [DataMember]
        public double NewHeading { get; set; }

        [DataMember]
        public int BotId { get; set; }

        [DataMember]
        public Guid PublicBotId { get; set; }
    }
}