namespace Plisky.Boondoggle2 {

    using System;
    using System.Collections.Generic;
    using System.Drawing;

    public class EquipmentUseResult {

        public UsageEndState State { get; set; }
    }

    public class ScanEquipmentUseResult : EquipmentUseResult {
        private Dictionary<Point, ScanTileResult> occupiedTiles = new Dictionary<Point, ScanTileResult>();
        private List<ScanResultPOI> pointsOfInterest = new List<ScanResultPOI>();

        public int LowestYValue { get; private set; }

        public int LowestXValue { get; private set; }

        public int Height { get; private set; }

        public int Width { get; private set; }

        public void SetDimensions(int lowX, int lowY, int width, int height) {
            LowestXValue = lowX; LowestYValue = lowY;
            Width = width;
            Height = height;
        }

        public ScanTileResult GetResultAtPosition(Point pt) {
            if (occupiedTiles.ContainsKey(pt)) {
                return occupiedTiles[pt];
            }
            return ScanTileResult.Unscanned;
        }

        public void SetScanResultAtPosition(Point pt, ScanTileResult res) {
            occupiedTiles.Add(pt, res);
        }

        public void ScanResultEach(Action<Point, ScanTileResult> tile) {
            for (int xOffs = 0; xOffs < Width; xOffs++) {
                for (int yOffs = 0; yOffs < Height; yOffs++) {
                    Point pt = new Point(LowestXValue + xOffs, LowestYValue + yOffs);
                    tile(pt, GetResultAtPosition(pt));
                }
            }
        }

        public void AddPointOfInterest(Point v, int id) {
            var nxt = new ScanResultPOI(v, id);
            pointsOfInterest.Add(nxt);
        }

        public int NumberOfPOI {
            get {
                return pointsOfInterest.Count;
            }
        }

        public IEnumerable<ScanResultPOI> GetPointsOfInterest() {
            foreach (var v in pointsOfInterest) {
                yield return v;
            }
        }
    }
}