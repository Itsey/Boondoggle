using System.Drawing;
namespace Plisky.Boondoggle2 {

    public class ScanResultPOI {
        // TODO : ADD A TEST - Is the Point returned correctly from the  POI
        public Point ScanLocation { get; set; }

        public ScanResultPOI(Point v, int temporaryScanId) {
            // TODO: Complete member initialization
            this.ScanLocation = v;
            this.POIIdentity = temporaryScanId;
        }

        public int POIIdentity { get; set; }
    }
}