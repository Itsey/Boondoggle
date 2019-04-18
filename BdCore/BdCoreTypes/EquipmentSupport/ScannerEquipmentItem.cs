using System.Collections.Generic;
using System.Drawing;

namespace Plisky.Boondoggle2 {

    public class ScannerEquipmentItem : EquipmentItem {
        private List<Point> allScanPoints = new List<Point>();

        private void RecalculateScanRanges() {
            int lowestXRange = int.MaxValue;
            int lowestYrange = int.MaxValue;
            int highestXRange = int.MinValue;
            int highestYRange = int.MinValue;

            foreach (var v in allScanPoints) {
                if (v.X < lowestXRange) {
                    lowestXRange = v.X;
                }
                if (v.Y < lowestYrange) {
                    lowestYrange = v.Y;
                }
                if (v.X > highestXRange) {
                    highestXRange = v.X;
                }
                if (v.Y > highestYRange) {
                    highestYRange = v.Y;
                }
            }
            TotalHeightScanned = highestYRange - lowestYrange;
            TotalWidthScanned = highestXRange - lowestXRange;
            MinimumYScanned = lowestYrange;
            MinimumXScanned = lowestXRange;
        }

        public int TotalWidthScanned { get; private set; }

        public int TotalHeightScanned { get; private set; }

        public void AddScannedOffsetPoint(Point pt) {
            if (allScanPoints.Contains(pt)) {
                throw new BdBaseException("That point has already been added to the scan range");
            }
            allScanPoints.Add(pt);
            RecalculateScanRanges();
            // TODO : How to secure this?
        }

        public IEnumerable<Point> GetAllScanPoints() {
            foreach (var v in allScanPoints) {
                yield return v;
            }
        }

        public int MinimumYScanned { get; set; }

        public int MinimumXScanned { get; set; }

        public int ChargeConsumed { get; set; }

        public int Cooldown { get; set; }
    }
}