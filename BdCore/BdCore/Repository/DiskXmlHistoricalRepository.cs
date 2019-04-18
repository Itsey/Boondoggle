using Plisky.Plumbing;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace Plisky.Boondoggle2 {

    public class DiskXmlHistoricalRepository : HistoricalBattleRepository {
        private List<string> battleControlFiles = new List<string>();

        protected override void ActualInitialise() {
            string battlePath = ConfigHub.Current.GetSetting("bdbattlearchive",true);

            foreach (var v in Directory.GetFiles(battlePath, "*.control")) {
                battleControlFiles.Add(v);
            }
        }

        protected override IEnumerable<BattleSummary> ActualListAllBattles() {
            foreach (var v in battleControlFiles) {
                DataContractSerializer dcs = new DataContractSerializer(typeof(BattleStatusFile));
                using (FileStream fs = new FileStream(v, FileMode.Open, FileAccess.Read)) {
                    BattleStatusFile bsc = (BattleStatusFile)dcs.ReadObject(fs);
                    BattleSummary bs = new BattleSummary();
                    bs.BattleName = bsc.UniqueName;
                    bs.DisplayName = bsc.DescriptiveName;
                    yield return bs;
                }
            }
        }
    }
}