using System;
using System.Collections.Generic;

namespace Plisky.Boondoggle2 {

    public class bdBattleManager {
        private HistoricalBattleRepository mhr;
        public int TotalBattles { get; set; }

        public bdBattleManager(HistoricalBattleRepository mhr) {
            this.mhr = mhr;
        }

        public BattleSummary GetBattleSummary(string p) {
            throw new NotImplementedException();
        }

        public IEnumerable<BattleSummary> ListRecentBattles() {
            foreach (var q in mhr.GetBattles()) {
                yield return q;
            }
        }
    }
}