namespace Plisky.Boondoggle2 {
    using Plisky.Diagnostics;
    using Plisky.Plumbing;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;

    public class bd2Map {

        protected Bilge b = new Bilge(tl: TraceLevel.Off);
        
        /// <summary>
        /// Inject a new instance of bilge, or change the trace level of the current instance. To set the trace level ensure that
        /// the first parameter is null.  To set bilge simply pass a new instance of bilge.
        /// </summary>
        /// <param name="blg">An instance of Bilge to use inside this Hub</param>
        /// <param name="tl">If specified and blg==null then will alter the tracelevel of the current Bilge</param>
        public void InjectBilge(Bilge blg, TraceLevel tl = TraceLevel.Off) {
            if (blg != null) {
                b = blg;
            } else {
                b.CurrentTraceLevel = tl;
            }
        }



        protected Dictionary<Point, MapTile> mapLocations = new Dictionary<Point, MapTile>();
        protected List<Point> validStartLocations = new List<Point>();

        public MapConditionType MapType { get; set; }

        public int Height { get; set; }

        public int Width { get; set; }

        public string Name { get; set; }

        public MapTile GetTileAtPosition(Point position) {
            if ((position.X <= 0) || (position.X > Height) || (position.Y <= 0) || (position.Y > Height)) {
                b.Info.Log("Tile request out of range, Engine should range check,  Asked for " + position.ToString() + "Map W:" + Width.ToString() + " H: " + Height.ToString());
                throw new BdBaseException("Tile out of map range");
            }

            if (!mapLocations.ContainsKey(position)) {
                return MapTile.DefaultGround;
            } else {
                return mapLocations[position];
            }
        }

        public bd2Map() {
            Height = Width = 0;
            Name = null;
            MapType = MapConditionType.ZeroBotsRemain;
        }

        public bd2Map(string desiredName, int desiredWidth, int desiredHeight) {
            Height = desiredHeight;
            Width = desiredWidth;
            Name = desiredName;
        }

        internal void SetTileAtPosition(Point insertLocation, MapTile desiredTile) {
            if (!mapLocations.ContainsKey(insertLocation)) {
                mapLocations.Add(insertLocation, desiredTile);
            } else {
                mapLocations[insertLocation] = desiredTile;
            }
        }

        public int MaxSupportedBots {
            get {
                return validStartLocations.Count;
            }
        }

        public void SetStartPosition(Point point) {
            foreach (var v in validStartLocations) {
                if (v == point) {
                    throw new BdBaseException("Cant add the same start position twice");
                }
            }
            validStartLocations.Add(point);
        }

        public Point GetStartPosition(int count) {
            b.Verbose.Log( "Request start position for point number : " + count.ToString());

            if ((count < 1) || (count > validStartLocations.Count)) {
                throw new BdBaseException(string.Format("The start position request is out of range. Requested [{0}] Max [{1}]", count, validStartLocations.Count));
            }
            count--;  // Array of start locations is zero based, we use 1 based
            return validStartLocations[count];
        }

        public string DisplayName {
            get {
                return Name;
            }
        }
    }
}