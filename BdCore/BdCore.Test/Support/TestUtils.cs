using Plisky.Diagnostics;
using Plisky.Plumbing;
using System.Drawing;
using System.Text;

namespace Plisky.Boondoggle2.Test {

    public static class TestUtils {
        

        public static void DumpScanResult(ScanEquipmentUseResult sut, int turn, int tick, string botName) {
            Bilge b = new Bilge(tl:System.Diagnostics.TraceLevel.Verbose);

            b.Info.Log(string.Format("Scan Result for {2} turn {0}, tick {1}..... ", turn, tick, botName) + sut.LowestXValue.ToString() + "," + sut.LowestYValue.ToString() + " --> W:" + sut.Width.ToString() + " H:" + sut.Height.ToString());
            b.Info.Log("Unscanned-0,Unoccupied-1,SolidWall-2,You-3,Bot-4");
            b.Info.Log("________________________________________________");
            for (int y = sut.LowestYValue + sut.Height; y >= sut.LowestYValue; y--) {
                StringBuilder sb = new StringBuilder();
                string spacer = string.Empty;
                if (y < 0) { spacer = " "; }

                sb.Append(string.Format("Y [{1}{0:D2}] |", y, spacer));
                for (int x = sut.LowestXValue; x < sut.LowestXValue + sut.Width; x++) {
                    var res = sut.GetResultAtPosition(new Point(x, y));
                    sb.Append(string.Format("{0},", (int)res));
                }
                b.Info.Log(sb.ToString());
            }

            b.Info.Log("Scan Done");
        }

        public static TestPreparedWorld CreateWorldWithBotsReadyForCombat(int withThisScanId = -1, MockEquipmentRepository mer = null) {
            Hub testHub = new Hub(true);
            TestPreparedWorld result = new TestPreparedWorld();
            if (mer == null) {
                result.MockRepos = new MockEquipmentRepository();
            } else {
                result.MockRepos = mer;
            }
            result.Bot1 = (BasicMockBot)new MockBotFactory().CreateBasicMockBot().WithMockPack().WithEquipmentCallback(ab => {
                EquipmentInstallationResult ae = ab.InstallEquipment(KnownEquipmentIds.MOCKPROJECTILEWEAPON, "Gun_Turret", MountPoint.Turret);
                ae = ab.InstallEquipment(KnownEquipmentIds.MOCKPROJECTILEWEAPON, "Gun_Front", MountPoint.Forward);
                ae = ab.InstallEquipment(KnownEquipmentIds.MOCKPROJECTILEWEAPON, "Gun_Near", MountPoint.Nearside);
                ae = ab.InstallEquipment(KnownEquipmentIds.MOCKPROJECTILEWEAPON, "Gun_Offs", MountPoint.Offside);
                ae = ab.InstallEquipment(KnownEquipmentIds.MOCKPROJECTILEWEAPON, "Gun_Rear", MountPoint.Backward);
                ae = ab.InstallEquipment(KnownEquipmentIds.MOCKSCANNER, "Scanner", MountPoint.Internal);
            }).ToBot();
            result.Bot2 = (BasicMockBot)new MockBotFactory().CreateBasicMockBot().WithMockPack().WithEquipmentCallback(ab => {
                EquipmentInstallationResult ae = ab.InstallEquipment(KnownEquipmentIds.MOCKPROJECTILEWEAPON, "Gun_Turret", MountPoint.Turret);
                ae = ab.InstallEquipment(KnownEquipmentIds.MOCKPROJECTILEWEAPON, "Gun_Front", MountPoint.Forward);
                ae = ab.InstallEquipment(KnownEquipmentIds.MOCKPROJECTILEWEAPON, "Gun_Near", MountPoint.Nearside);
                ae = ab.InstallEquipment(KnownEquipmentIds.MOCKPROJECTILEWEAPON, "Gun_Offs", MountPoint.Offside);
                ae = ab.InstallEquipment(KnownEquipmentIds.MOCKPROJECTILEWEAPON, "Gun_Rear", MountPoint.Backward);
                ae = ab.InstallEquipment(KnownEquipmentIds.MOCKSCANNER, "Scanner", MountPoint.Internal);
            }).ToBot();
            result.Engine = new TestEngineFactory().WithHub(testHub).WithDuellingWorld().WithEquipmentSupport(result.MockRepos).WithBot(result.Bot1).WithBot(result.Bot2).WithPrepare().ToMockEngine();
            result.Engine.PerformNextTick();
            result.HubUsed = testHub;

            if (withThisScanId >= 0) {
                int i1 = result.GetBot1EngineId();
                int i2 = result.GetBot2EngineId();

                result.Engine.Mock_CreateArtificalScanId(withThisScanId, i1, i2);
            }
            return result;
        }


        public static mockBd2Engine SetupForPowerTests() {
            var bot = new MockBotFactory().CreateBasicMockBot().WithEquipmentCallback(ab => {
                ab.InstallEquipment(KnownEquipmentIds.MOCKPOWERPACK, "PP", MountPoint.Internal);
            }).ToBot();
            var tef = new TestEngineFactory().WithDefaultWorld().WithBot(bot).WithBotSupport().WithEquipmentSupport();
            var sut = tef.ToMockEngine();
            sut.RegisterForMessages();
            PowerPackEquipmentItem ppi = (PowerPackEquipmentItem)tef.EngineSuppliedEquipmentSupport.GetEquipmentTypeById(KnownEquipmentIds.MOCKPOWERPACK);
            sut.StartBattle();
            return sut;
        }
    }
}