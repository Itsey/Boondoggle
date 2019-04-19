namespace Plisky.Boondoggle2 {

    using Plisky.Plumbing;
    using System;
    using System.Drawing;
    using System.IO;
    using System.Xml.Linq;

    public class bd2MapRepository {

        public Bd2Map GetMapByName(string name) {
            if (name.ToLower() == "default") {
                return CreateDefaultMap();
            }
            string mpn = ConfigHub.Current.GetSetting("MapPathName", true);
            mpn = Path.Combine(mpn, name + ".tmx");
            if (File.Exists(mpn)) {
                return LoadMapFromFilename(name, mpn);
            }

            throw new BdBaseException("The map filename can not be found.  Is application config set up?");
        }

        private Bd2Map LoadMapFromFilename(string name, string mpn) {
            XDocument xd = XDocument.Load(mpn);
            var dimsEl = xd.Element("map").Element("layer");
            var srcElements = dimsEl.Element("data");
            int xOffset = Convert.ToInt32(dimsEl.Attribute("width").Value);
            int yOffset = Convert.ToInt32(dimsEl.Attribute("height").Value);
            Bd2Map result = new Bd2Map(name, xOffset, yOffset);

            int x = 0; int y = yOffset;
            foreach (var nextPointElement in srcElements.Elements("tile")) {
                x++;
                if (x > xOffset) { x = 1; y--; }
                if (y <= 0) { throw new BdBaseException("Invalid XML"); }

                int tleVal = Convert.ToInt32(nextPointElement.Attribute("gid").Value);
                Point where = new Point(x, y);
                switch (tleVal) {
                    case 2: continue;
                    case 1: result.SetTileAtPosition(where, MapTile.BoundaryWall1); break;
                    case 5:
                    case 3: result.SetStartPosition(where); break;
                    default: throw new BdBaseException("Tile Loaded From Map file is not understood");
                }
            }
            return result;
        }

        private static Bd2Map CreateDefaultMap() {
            Bd2Map result = new Bd2Map("DefaultMap", 100, 100);

            for (int x = 1; x <= 100; x++) {
                result.SetTileAtPosition(new Point(x, 1), MapTile.BoundaryWall1);
                result.SetTileAtPosition(new Point(x, 100), MapTile.BoundaryWall1);
                result.SetTileAtPosition(new Point(1, x), MapTile.BoundaryWall1);
                result.SetTileAtPosition(new Point(100, x), MapTile.BoundaryWall1);
            }

            result.SetStartPosition(new Point(2, 2));
            result.SetStartPosition(new Point(99, 99));
            return result;
        }
    }
}