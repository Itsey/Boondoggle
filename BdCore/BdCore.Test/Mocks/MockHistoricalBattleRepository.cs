using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Plisky.Boondoggle2;

namespace Plisky.Boondoggle2.Test {
    public class MockHistoricalBattleRepository : HistoricalBattleRepository {
        public void AddBattles(int p) {
            throw new NotImplementedException();
        }

        public void AddBattle(string p1, string p2, int p3, DateTime dateTime) {
            throw new NotImplementedException();
        }

        protected override void ActualInitialise() {
            throw new NotImplementedException();
        }

        protected override IEnumerable<BattleSummary> ActualListAllBattles() {
            throw new NotImplementedException();
        }
    }
}
