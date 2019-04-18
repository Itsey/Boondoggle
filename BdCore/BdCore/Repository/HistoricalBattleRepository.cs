using System.Collections.Generic;

namespace Plisky.Boondoggle2 {

    public abstract class HistoricalBattleRepository {
        private bool hasInitialised = false;

        private void DoInitialise() {
            if (!hasInitialised) {
                hasInitialised = true;
                ActualInitialise();
            }
        }

        protected abstract void ActualInitialise();

        protected abstract IEnumerable<BattleSummary> ActualListAllBattles();

        public IEnumerable<BattleSummary> GetBattles() {
            DoInitialise();
            return ActualListAllBattles();
        }
    }
}