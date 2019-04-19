namespace Plisky.Boondoggle2.Test {
    using Plisky.Plumbing;
    using System;
    using System.Drawing;
    using Xunit;

    public class BotEquipnmentScannerTests {

        [Fact]
        public void Scanner_MockScannerIsInternalInstallOnly() {
            var sut = new MockBotFactory().CreateBasicMockBot().WithMockActionProvider().WithItemSupport().ToBot();
            EquipmentInstallationResult ae = sut.InstallEquipment(KnownEquipmentIds.MOCKSCANNER, "FriendlyName", MountPoint.Internal);
            Assert.True(ae.InstanceId != Guid.Empty, "The scanner must be installed successfully");
        }

        [Fact]
        public void Scanner_UseEquipmentReturnsScanResult() {
            var result = GetDefinedScanResult(MockBotActionProvider.PredefinedSimpleScan);
            Assert.True(result is ScanEquipmentUseResult, "The result type returned from using  a scanner should be a scanresult");
        }

        private ScanEquipmentUseResult GetDefinedScanResult(int definedResult, bool includeSecondBot = false) {
            Hub testHub = new Hub();

            var scanningBot = new MockBotFactory().CreateBasicMockBot().WithMockPack().ToBot();

            var scannedBot = new MockBotFactory().CreateBasicMockBot().WithMockPack().ToBot();

            TestEngineFactory tef = new TestEngineFactory().WithHub(testHub).WithDefaultWorld().WithEquipmentSupport().WithBotSupport().WithBot(scanningBot);

            if (includeSecondBot) {
                tef = tef.WithBot(scannedBot);
            }

            var engx = tef.WithPrepare().ToMockEngine();

            engx.PerformNextTick();
            engx.Mock_DirectSetBotLocation(scanningBot.PublicId, new Point(99, 99));  // Set bot to top right of the map.

            if (includeSecondBot) {
                engx.Mock_DirectSetBotLocation(scannedBot.PublicId, new Point(97, 99));  // Set bot to top right of the map.
            }

            bd2Engine ee = (bd2Engine)engx;
            

            var ae = scanningBot.InstallEquipment(KnownEquipmentIds.MOCKSCANNER, "FriendlyName", MountPoint.Internal);
            engx.Mock_DirectSetBotCharge(scanningBot.PublicId, 100);
            var result = scanningBot.UseEquipment(ae.InstanceId);
            ScanEquipmentUseResult resASR = (ScanEquipmentUseResult)result;

            Assert.Equal<UsageEndState>(UsageEndState.Success, resASR.State); 

            return resASR;
        }

        [Fact]
        public void Scanner_DefaultScan_Is20x20() {
            var sut = GetDefinedScanResult(MockBotActionProvider.PredefinedSimpleScan);

            Assert.Equal(20, sut.Height);
            Assert.Equal(20, sut.Width); 
        }

        [Fact]
        public void Scanner_DefaultScanBotNextToWall_Correct() {
            var sut = GetDefinedScanResult(MockBotActionProvider.PredefinedSimpleScan);

            for (int x = -1; x < 2; x++) {
                var pt = new Point(x, 1);
                var res = sut.GetResultAtPosition(pt);
                Assert.True(res == ScanTileResult.SolidWall, "The bot should be just south of a bounding wall");
            }
            for (int y = -1; y < 2; y++) {
                var pt = new Point(1, y);
                var res = sut.GetResultAtPosition(pt);
                Assert.True(res == ScanTileResult.SolidWall, "The bot should be just west of a bounding wall");
            }
        }

        [Fact]
        public void Scanner_DefaultSecondaryBot_CountsAsPOI() {
            var sut = GetDefinedScanResult(MockBotActionProvider.PredefinedSimpleScan, true);
            Assert.Equal(1, sut.NumberOfPOI);
        }

        [Fact]
        public void Scanner_DefauultScan_NoPOI() {
            var sut = GetDefinedScanResult(MockBotActionProvider.PredefinedSimpleScan);
            Assert.Equal(0, sut.NumberOfPOI); 
        }

        [Fact]
        public void Scanner_PoiHasUniqueId() {
            var sut = GetDefinedScanResult(MockBotActionProvider.PredefinedSimpleScan, true);

            foreach (ScanResultPOI v in sut.GetPointsOfInterest()) {
                foreach (ScanResultPOI p in sut.GetPointsOfInterest()) {
                    if (object.ReferenceEquals(v, p)) {
                        continue;
                    }
                    Assert.NotEqual(v.POIIdentity, p.POIIdentity); 
                }
            }
        }

        [Fact]
        public void Scanner_DefaultScan_FindsSecondaryBot() {
            var sut = GetDefinedScanResult(MockBotActionProvider.PredefinedSimpleScan, true);
            bool wasBotPresent = false;
            sut.ScanResultEach((tpt, res) => {
                if (res == ScanTileResult.Bot) {
                    wasBotPresent = true;
                }
            });
            Assert.True(wasBotPresent, "There should be another bot in the scan result");
        }

        [Fact]
        public void Scaner_DefaultScan_ContainsMeOnceAtCorrectLoc() {
            var sut = GetDefinedScanResult(MockBotActionProvider.PredefinedSimpleScan);
            int numMatches = 0;
            bool wasResultAtZero = false;
            sut.ScanResultEach((tpt, res) => {
                if (res == ScanTileResult.You) {
                    numMatches++;
                    if ((tpt.X == 0) && (tpt.Y == 0)) {
                        wasResultAtZero = true;
                    }
                }
            });

            Assert.Equal(1, numMatches); 
            Assert.True(wasResultAtZero, "The scanning bot should be at 0,0 in the scan result");
        }

        [Fact]
        public void Scanner_DefaultScan_ReturnsUnscannedForOutOfRange() {
            var resASR = GetDefinedScanResult(MockBotActionProvider.PredefinedSimpleScan);

            Assert.Equal(ScanTileResult.Unscanned, resASR.GetResultAtPosition(new Point(11, 11)));
            Assert.Equal(ScanTileResult.Unscanned, resASR.GetResultAtPosition(new Point(-11, 5)));
            Assert.Equal(ScanTileResult.Unscanned, resASR.GetResultAtPosition(new Point(5, -11)));
        }
    }
}