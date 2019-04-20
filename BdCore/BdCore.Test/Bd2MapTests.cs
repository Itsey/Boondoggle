namespace Plisky.Boondoggle2.Test {
    using Plisky.Boondoggle2;
    using Plisky.Diagnostics;
    using Plisky.Test;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Text;
    using Xunit;

    public class Bd2MapTests {

        protected Bilge b = new Bilge(tl: TraceLevel.Off);



        [Fact(DisplayName = nameof(EveryTile_RetursnDefault))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]

        public void EveryTile_RetursnDefault() {
            b.Info.Flow();

            var mp = new MockBd2Map();
            mp.Test_SetDimensions(100, 100);

            for(int x=1; x<=100; x++) {
                for(int y=1; y<=100; y++) {

                    var mt = mp.GetTileAtPosition(new Point(x, y));
                    Assert.Equal<MapTile>(MapTile.DefaultGround, mt);
                }
            }
            
            
        }

        [Theory(DisplayName = nameof(TileOutOfRange))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        [InlineData(-1,1)]
        [InlineData(11, 1)]
        [InlineData(1, -1)]
        [InlineData(1, 101)]
        [InlineData(-1, 101)]
        [InlineData(0, 1)]
        [InlineData(1, 0)]
        public void TileOutOfRange(int x, int y) {
            b.Info.Flow();

            var mp = new MockBd2Map();
            mp.Test_SetDimensions(10, 100);


            var excpt = Assert.Throws<BdBaseException>(() => {
                mp.GetTileAtPosition(new Point(x,y));
            });

            Assert.Contains("range", excpt.Message);
        }



        [Fact(DisplayName = nameof(SetStartPositionTwice_Blows))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void SetStartPositionTwice_Blows() {
            b.Info.Flow();
            Bd2Map mp = new Bd2Map("mock",10,10);

            var excpt = Assert.Throws<BdBaseException>(() => {
                mp.SetStartPosition(new Point(1, 1));
                mp.SetStartPosition(new Point(1, 1));
            });

            Assert.Contains("start", excpt.Message);
        }

        [Fact(DisplayName = nameof(SetStartPositionOutOfRange_Blows))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void SetStartPositionOutOfRange_Blows() {
            b.Info.Flow();
            Bd2Map mp = new Bd2Map("mock", 10, 10);

            var excpt = Assert.Throws<BdBaseException>(() => {
                mp.SetStartPosition(new Point(11, 1));
            });

            Assert.Contains("range", excpt.Message);

            excpt = Assert.Throws<BdBaseException>(() => {
                mp.SetStartPosition(new Point(1,11));
            });

            Assert.Contains("range", excpt.Message);
        }

        [Fact(DisplayName = nameof(RequestTooManyStartPositionsBlows))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void RequestTooManyStartPositionsBlows() {
            b.Info.Flow();
            Bd2Map mp = new Bd2Map("mock", 10, 10);
            mp.SetStartPosition(new Point(1, 1));

            var excpt = Assert.Throws<BdBaseException>(() => {
                mp.GetStartPosition(2);
            });

            Assert.Contains("start", excpt.Message);
        }


        [Fact(DisplayName = nameof(Constructor_SetsName))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Constructor_SetsName() {
            b.Info.Flow();

            const string mapName = "myMockMapName";
            Bd2Map mp = new Bd2Map(mapName, 10, 10);
            Assert.Equal(mapName, mp.Name);
        }

        [Fact(DisplayName = nameof(Constructor_SetsDimensions))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Constructor_SetsDimensions() {
            b.Info.Flow();

            const string mapName = "myMockMapName";
            Bd2Map mp = new Bd2Map(mapName, 11, 10);
            Assert.Equal(11, mp.Width);
            Assert.Equal(10,mp.Height);
        }

    }
}
